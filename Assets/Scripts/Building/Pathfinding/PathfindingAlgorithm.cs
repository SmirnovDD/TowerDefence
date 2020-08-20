using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pathfinding
{
    [RequireComponent(typeof(PathfindingMultithreading))]
    [RequireComponent(typeof(PathfindingManager))]

    public class PathfindingAlgorithm : MonoBehaviour
    {
        private PathfindingManager _pathfindingManager;

        void Awake()
        {
            _pathfindingManager = GetComponent<PathfindingManager>();
        }


        public void FindPath(PathRequest request, Action<PathSolution> callback)
        {

            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            List<Node> waypoints = new List<Node>();
            bool pathSuccess = false;

            Node startNode = _pathfindingManager.NodeFromWorldPoint(request.PathStart, request.Grid);
            Node targetNode = _pathfindingManager.NodeFromWorldPoint(request.PathEnd, request.Grid);
            startNode.Parent = startNode;

            if (startNode.Walkable && targetNode.Walkable)
            {
                Heap<Node> openSet = new Heap<Node>(request.Grid.Nodes.Length); //NODE COUNT !!!!!
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);
                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        //sw.Stop();
                        //print ("Path found: " + sw.ElapsedMilliseconds + " ms");
                        pathSuccess = true;
                        break;
                    }

                    foreach (Node neighbour in _pathfindingManager.GetNeighbours(currentNode, request.Grid))
                    {
                        if (!neighbour.Walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newMovementCostToNeighbour =
                            currentNode.GCost + GetDistance(currentNode, neighbour) + neighbour.MovementPenalty;
                        if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                        {
                            neighbour.GCost = newMovementCostToNeighbour;
                            neighbour.HCost = GetDistance(neighbour, targetNode);
                            neighbour.Parent = currentNode;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }

            if (pathSuccess)
            {
                float pathLength = -1;
                float timeToFinish = -1;
                waypoints = RetracePath(startNode, targetNode);
                waypoints.Insert(0, targetNode);
                Node[] waypointsArray = waypoints.ToArray();
                Array.Reverse(waypointsArray);

                if (request.GetLength || request.GetTimeToFinish)
                    pathLength = GetPathLength(request.PathStart, waypointsArray);
                if (request.GetTimeToFinish)
                    timeToFinish = GetTimeToFinish(request.MovementSpeed, pathLength);

                callback(new PathSolution(waypointsArray, pathSuccess, request.Callback, pathLength, timeToFinish));
                //pathSuccess = waypoints.Length > 0;
            }
            else
                callback(new PathSolution(new Node[0], pathSuccess, request.Callback, 999999, 999999));
        }

        private float GetPathLength(Vector3 startPoint, Node[] waypointsArray)
        {
            float length;
            if (waypointsArray.Length == 0)
                return 999999;

            length = Vector3.SqrMagnitude(startPoint - waypointsArray[0].Position);
            if (waypointsArray.Length == 1)
                return length;

            for (int i = waypointsArray.Length - 1; i >= 1; i--)
            {
                length += Vector3.SqrMagnitude(waypointsArray[i].Position - waypointsArray[i - 1].Position);
            }

            return length;
        }

        private float GetTimeToFinish(float movementSpeed, float pathLength)
        {
            return movementSpeed > 0 ? Mathf.Sqrt(pathLength) / movementSpeed : float.PositiveInfinity;
        }
        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            List<Node> waypoints = SimplifyPath(path);
            //Array.Reverse(waypoints);
            return waypoints;
        }

        private List<Node> SimplifyPath(List<Node> path)
        {
            List<Node> waypoints = new List<Node>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew =
                    new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i]);
                }

                directionOld = directionNew;
            }

            return waypoints;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}
