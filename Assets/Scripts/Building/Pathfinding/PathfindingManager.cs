using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding
{
    [RequireComponent(typeof(PathfindingAlgorithm))]
    [RequireComponent(typeof(PathfindingMultithreading))]

    public class PathfindingManager : MonoSingleton<PathfindingManager>
    {
        public LayerMask unwalkableMask;
        public int obstacleProximityPenalty = 10;
        public Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
        public GridInfoSO GridsInfoSORef;
        public PathfindingGrid Grid;
        //GRIDS PATHFINDING
        public float SqrWaypointReachDistance = 0.03f;
        //!GRIDS PATHFINDING

        //DEBUG
        public Material DebugNodeWalkableMaterial;
        public Material DebugNodeNonWalkableMaterial;
        public Material DebugNodeTargetPointMaterial;
        public Material DebugNodePathPointMaterial;
        public Material DebugNodeOccupiedNodeMaterial;

        public void RegenerateGrid()
        {
            Grid.CreateGrid();
            Debug.Log("Regenerated");
        }
        public List<Node> GetNeighbours(Node node, PathfindingGrid grid)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.GridX + x;
                    int checkY = node.GridY + y;

                    if (checkX >= 0 && checkX < grid.GridSize[0] && checkY >= 0 && checkY < grid.GridSize[1])
                    {
                        neighbours.Add(grid.Nodes[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }
        public Node NodeFromWorldPoint(Vector3 worldPoint, PathfindingGrid grid, bool allowNonWalkable = false)
        {
            Node targetNode = new Node(0, 0);
            float minimumDistance = 9999999;
            foreach (Node n in grid.Nodes)
            {
                if (n.Walkable == false && allowNonWalkable == false)
                    continue;

                float distance = Vector3.SqrMagnitude(n.Position - worldPoint);
                if (distance < minimumDistance)
                {
                    targetNode = n;
                    minimumDistance = distance;
                }
            }

            return targetNode;
        }      

        public List<TargetPoint> CalculatePathForSailor(PathfindingUnit sailor, Transform target)
        {
            List<TargetPoint> sailorsPath = new List<TargetPoint>();

            sailorsPath.Add(new TargetPoint(target));
            return sailorsPath;
        }

        [Serializable]
        public class TargetPoint
        {
            public Transform TargetPosition;

            public TargetPoint(Transform targetTr)
            {
                TargetPosition = targetTr;
            }
        }              
    }
}