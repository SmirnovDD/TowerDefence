using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCellsGenerator : MonoBehaviour
{
    public GridInfoSO GridInfoSORef;
    public int[] CellsGridSize = new int[2];
    public float YOffset;
    public bool DrawGizmoz = true;

    private int _cellsAmmountX, _cellsAmmountY;
    private List<ObjectCell> _objectCells = new List<ObjectCell>();

    public enum CenterCellsOptions
    {
        CenterBoth,
        CenterX,
        CenterY,
        CenterNone
    }
    public enum SizeRounding
    {
        RoundToInt,
        CeilToInt
    }

    private void Awake()
    {
        CreateCells();
        GetComponentInParent<PlacementObject>().ObjectCells = _objectCells.ToArray();
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        _objectCells.Clear();
        CreateCells();
    }

    private void CreateCells()
    {
        _cellsAmmountX = CellsGridSize[0];
        _cellsAmmountY = CellsGridSize[1];
        bool mainCellInstantiated = false;
        float cellDiameter = GridInfoSORef.CellRadius * 2;
        
        Vector3 xPos;

        xPos = new Vector3(transform.position.x - _cellsAmmountX * GridInfoSORef.CellRadius + GridInfoSORef.CellRadius, transform.position.y - YOffset, transform.position.z - _cellsAmmountY * GridInfoSORef.CellRadius + GridInfoSORef.CellRadius);

        for (int i = 0; i < _cellsAmmountX; i++)
        {
            var pos = xPos;
            for (int j = 0; j < _cellsAmmountY; j++)
            {
                if (mainCellInstantiated == false)
                {
                    _objectCells.Add(new ObjectCell { CellLocalPos = transform.parent.InverseTransformPoint(pos), MainCell = true });
                    mainCellInstantiated = true;
                }
                else
                    _objectCells.Add(new ObjectCell { CellLocalPos = transform.parent.InverseTransformPoint(pos) });

                pos += transform.forward * cellDiameter;
            }

            xPos += transform.right * cellDiameter;
        }
    }

    private void OnDrawGizmos()
    {
        if (DrawGizmoz == false)
            return;

        Gizmos.color = Color.green;
        foreach (var c in _objectCells)
            Gizmos.DrawCube(transform.TransformPoint(c.CellLocalPos), new Vector3(GridInfoSORef.CellRadius * 2, 0.1f, GridInfoSORef.CellRadius * 2));
    }

    [Serializable]
    public class ObjectCell
    {
        public Vector3 CellLocalPos;
        public bool MainCell;
    }
}
