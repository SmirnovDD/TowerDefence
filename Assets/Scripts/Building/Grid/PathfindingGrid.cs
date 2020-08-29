using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public partial class PathfindingGrid : MonoBehaviour
    {
        public Transform PlaneTr;
        public GridInfoSO GridInfoSORef;
        public Vector2 GridSize;
        public Node[,] Nodes { protected set; get; }

        private float _nodeDiameter;
        protected virtual void Awake()
        {            
            Init();
        }

        public void Init()
        {
            CreateGrid();
        }

        public void CreateGrid()
        {
            InitReferencesForGridCreation();
            CreateGridNodes();
        }

        private void InitReferencesForGridCreation()
        {
            _nodeDiameter = GridInfoSORef.CellRadius * 2;
        }

        protected void CreateGridNodes()
        {
            var bottomLeftGridPoint = PlaneTr.position - PlaneTr.right * GridInfoSORef.CellRadius * GridSize.x - PlaneTr.forward * GridInfoSORef.CellRadius * GridSize.y;
            Nodes = new Node[(int)GridSize.x, (int)GridSize.y];

            for (int x = 0; x < GridSize[0]; x++)
            {
                for (int y = 0; y < GridSize[1]; y++)
                {
                    Nodes[x, y] = CreateNode(bottomLeftGridPoint, x, y);
                }
            }
        }

        private Node CreateNode(Vector3 gridBottomLeft, int x, int y)
        {
            Vector3 worldPoint = gridBottomLeft + PlaneTr.right * (x * _nodeDiameter + GridInfoSORef.CellRadius) +
                                         PlaneTr.forward * (y * _nodeDiameter + GridInfoSORef.CellRadius);
            Ray ray = new Ray(worldPoint + PlaneTr.up, -PlaneTr.up);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f, GridInfoSORef.WalkableLayer);
            foreach (var h in hits)
            {
                if (h.collider.gameObject != PlaneTr.gameObject)
                    continue;

                worldPoint.y = h.point.y;
                return new Node(worldPoint, x, y, 0);
            }
            return new Node(worldPoint, x, y, 10, false);
        }

        private void OnDrawGizmos()
        {
            if (Nodes == null)
                return;
            foreach(var n in Nodes)
            {
                Gizmos.color = n.Walkable ? Color.green : Color.red;
                Gizmos.DrawCube(n.Position, new Vector3(1,0.1f,1) * _nodeDiameter);
            }
        }
    }   
}
