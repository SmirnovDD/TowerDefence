using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;

public class ObjectPlacement : MonoBehaviour
{
    public bool UnlimitedPlacement;

    public GridInfoSO GridInfoSORef;
    public Camera BuildCamera;
    [SerializeField] private GameObject _objectToPlacePrefab;

    private GameObject _objectToPlace;
    private InteriorObject _objectToPlaceScript;
    private ObjectCellsGenerator.ObjectCell _mainCell;

    private Vector3 _objectOldPos;
    private Vector3 _cellToObjDisplacement;
    private float _placementRotation = 0;

    private bool _placingObject;
    private bool _canPlaceObject;
    private bool _mouseOverCurrentGrid;

    private List<Node> _checkedForCollisionNodes = new List<Node>();
    private bool _updateWalkableNodes;
    [SerializeField] private KeyCode _rotateKey;

    private void Start()
    {
        BuildMode.Me.BuildModeActivated.AddListener(PickObjectToPlace);
        BuildMode.Me.DestroyPlacementObj.AddListener(DestroyPlacementObj);
    }
    private void OnDisable()
    {
        BuildMode.Me.BuildModeActivated.RemoveListener(PickObjectToPlace);
        BuildMode.Me.DestroyPlacementObj.RemoveListener(DestroyPlacementObj);
    }
    public void PickObjectToPlace()
    {
        _placingObject = true;
        _objectToPlace = Instantiate(_objectToPlacePrefab, Vector3.up * 100, Quaternion.identity);
        _objectToPlace.transform.localRotation = Quaternion.Euler(0, _placementRotation, 0);
        _objectToPlaceScript = _objectToPlace.GetComponent<InteriorObject>();
        _mainCell = _objectToPlaceScript.ObjectCells.First(el => el.MainCell);
        _cellToObjDisplacement = -_mainCell.CellLocalPos;
        _objectOldPos = Vector3.positiveInfinity;
    }

    void Update()
    {
        if (BuildMode.Me.modeActive == false)
            return;

        if (_placingObject == false)
            return;

        MoveObjectInGrid();
        RotateObjectInGrid();
        CheckForCollision();
        
        if (Input.GetMouseButtonDown(0) && _canPlaceObject && _mouseOverCurrentGrid)
            PlaceObject();
    }

    private void MoveObjectInGrid()
    {
        Ray ray = BuildCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        _mouseOverCurrentGrid = false;

        if(Physics.Raycast(ray, out hit, 100f, GridInfoSORef.WalkableLayer))
        {
            _mouseOverCurrentGrid = true;
            _objectToPlace.transform.position = hit.point + _cellToObjDisplacement;
        }

        if (_mouseOverCurrentGrid == false)
            return;

        var mainCellWorldPos = _objectToPlace.transform.position + _mainCell.CellLocalPos;
        var objectSnappedPos = PathfindingManager.Me.NodeFromWorldPoint(mainCellWorldPos, BuildMode.Me.CurrentBuildGrid, true).Position + _cellToObjDisplacement;
        _objectToPlace.transform.position = objectSnappedPos;

        if (_objectOldPos != _objectToPlace.transform.position)
        {
            _updateWalkableNodes = true;
            _objectOldPos = _objectToPlace.transform.position;
        }
    }
    private void RotateObjectInGrid()
    {
        if (Input.GetKeyDown(_rotateKey))
            ChangeObjectRotation();
    }
    private void ChangeObjectRotation()
    {
        _placementRotation += 90;
        _objectToPlace.transform.localRotation = Quaternion.Euler(0, _placementRotation, 0);
        _updateWalkableNodes = true;
    }

    private void CheckForCollision()
    {
        if (_updateWalkableNodes == false)        
            return;

        _checkedForCollisionNodes.Clear();
        _canPlaceObject = true;
        bool objOutsideOfBounds = false;

        foreach (var cell in _objectToPlaceScript.ObjectCells)
        {
            Node placemenNode = PathfindingManager.Me.NodeFromWorldPoint(_objectToPlace.transform.TransformPoint(cell.CellLocalPos), BuildMode.Me.CurrentBuildGrid, true);
            if (objOutsideOfBounds == false)
            {
                if (GetDistanceFromNodeToObjectCellPos(placemenNode.Position, _objectToPlace.transform.TransformPoint(cell.CellLocalPos)) > GridInfoSORef.CellRadius)
                {
                    objOutsideOfBounds = true;
                    _canPlaceObject = false;
                }
                else if (placemenNode.Walkable == false)
                {
                    _canPlaceObject = false;
                }
            }                           
            
            _checkedForCollisionNodes.Add(placemenNode);
        }
        
        _updateWalkableNodes = false;
    }

    private float GetDistanceFromNodeToObjectCellPos(Vector3 placementNodePos, Vector3 cellPos)
    {
        return new Vector2(placementNodePos.x - cellPos.x, placementNodePos.z - cellPos.z).magnitude;
    }
    private void PlaceObject()
    {
        foreach (var c in _checkedForCollisionNodes)
            c.Walkable = false;
        
        if (UnlimitedPlacement == false)
            _placingObject = false;
        else
            PickObjectToPlace();
    }

    private void DestroyPlacementObj()
    {
        if (_objectToPlace)
            Destroy(_objectToPlace);
    }

    public void ChangePlacementObject(GameObject placementObject)
    {
        DestroyPlacementObj();
        _objectToPlacePrefab = placementObject;
        PickObjectToPlace();
    }
}
