using Pathfinding;
using Unity.Collections;
using UnityEngine;

public class InteriorObject : MonoBehaviour
{
    [ReadOnly] public ObjectCellsGenerator.ObjectCell[] ObjectCells;   

    public virtual PathfindingGrid RaycastTargetGrid(Transform connectionTr, LayerMask walkableLayer)
    {
        return null;
    }
}
