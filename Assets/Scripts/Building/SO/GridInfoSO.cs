using UnityEngine;

[CreateAssetMenu(menuName = "Grid Info SO")]
public class GridInfoSO : ScriptableObject
{
    public float CellRadius;
    public LayerMask WalkableLayer;
}
