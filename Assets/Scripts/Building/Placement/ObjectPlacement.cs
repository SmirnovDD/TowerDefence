using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using TMPro;
using System.Linq;

public class ObjectPlacement : MonoBehaviour
{
    public GameObject TestObjectPrefab;
    public bool UnlimitedPlacement;

    public GridInfoSO GridInfoSORef;
    public Camera BuildCamera;

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
        _objectToPlace = Instantiate(TestObjectPrefab);
        _objectToPlace.transform.localRotation = Quaternion.Euler(0, _placementRotation, 0);
        _objectToPlaceScript = _objectToPlace.GetComponent<InteriorObject>();
        _mainCell = _objectToPlaceScript.ObjectCells.First(el => el.MainCell);
        _cellToObjDisplacement = -_mainCell.CellLocalPos;
        _objectOldPos = Vector3.down * 15; //can be anything that is not equal to mainCellPos
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

        ResetNodesHighlight();
        _checkedForCollisionNodes.Clear();
        _canPlaceObject = true;
        bool objOutsideOfBounds = false;

        foreach (var cell in _objectToPlaceScript.ObjectCells)
        {
            Node placemenNode = PathfindingManager.Me.NodeFromWorldPoint(_objectToPlace.transform.TransformPoint(_mainCell.CellLocalPos), BuildMode.Me.CurrentBuildGrid, true);
            if (objOutsideOfBounds == false)
            {
                if (Vector3.Magnitude(placemenNode.Position - _objectToPlace.transform.TransformPoint(_mainCell.CellLocalPos)) > GridInfoSORef.CellRadius)
                {
                    objOutsideOfBounds = true;
                    //placemenNode.DebugView.HighlightNode(DebugNodeView.HighlightMode.OccupiedNode);
                    _canPlaceObject = false;
                }
                else if (placemenNode.Walkable == false)
                {
                    //placemenNode.DebugView.HighlightNode(DebugNodeView.HighlightMode.OccupiedNode);
                    _canPlaceObject = false;
                }
                //else
                //    placemenNode.DebugView.HighlightNode(DebugNodeView.HighlightMode.TargetPoint);
            }                           
            
            _checkedForCollisionNodes.Add(placemenNode);
        }

        //if (objOutsideOfBounds)
        //    foreach (var c in _checkedForCollisionNodes)
        //        c.DebugView.HighlightNode(DebugNodeView.HighlightMode.OccupiedNode);

        _updateWalkableNodes = false;
    }

    private void ResetNodesHighlight()
    {
        foreach(var n in _checkedForCollisionNodes)
        {
            //if(n.Walkable)
            //    n.DebugView.HighlightNode(DebugNodeView.HighlightMode.Walkable);
            //else
            //    n.DebugView.HighlightNode(DebugNodeView.HighlightMode.NonWalkable);
        }
    }
    private void PlaceObject()
    {
        foreach (var c in _checkedForCollisionNodes)
            c.Walkable = false;

        ResetNodesHighlight();

        if (UnlimitedPlacement == false)
            _placingObject = false;
        else
            PickObjectToPlace();
    }

    private void DestroyPlacementObj()
    {
        ResetNodesHighlight();
        if (_objectToPlace)
            Destroy(_objectToPlace);
    }
}
