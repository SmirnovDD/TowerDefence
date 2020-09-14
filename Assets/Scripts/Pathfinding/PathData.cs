using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pathfinding
{
    [Serializable]
    public class PathData
    {

        public Node[] Nodes;
        public int finishLineIndex;
        public int slowDownIndex;

        public PathData(Node[] waypoints, Vector3 startPos)
        {
            Nodes = waypoints;
            //turnBoundaries = new Line[Nodes.Length];
            //finishLineIndex = turnBoundaries.Length - 1;

            //Vector2 previousPoint = startPos.GetXZVec2();
            //for (int i = 0; i < Nodes.Length; i++)
            //{
            //    Vector2 currentPoint = Nodes[i].LocalPosition.GetXZVec2();
            //    Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            //    Vector2 turnBoundaryPoint =
            //        (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;
            //    turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);
            //    previousPoint = turnBoundaryPoint;
            //}

            //float dstFromEndPoint = 0;
            //for (int i = Nodes.Length - 1; i > 0; i--)
            //{
            //    dstFromEndPoint += Vector3.Distance(Nodes[i].LocalPosition, Nodes[i - 1].LocalPosition);
            //    if (dstFromEndPoint > stoppingDst)
            //    {
            //        slowDownIndex = i;
            //        break;
            //    }
            //}
        }

        public void DrawWithGizmos()
        {
            //
            // Gizmos.color = Color.black;
            // foreach (Vector3 p in lookPoints)
            // {
            //     Gizmos.DrawCube(p + Vector3.up, Vector3.one);
            // }
            //
            // Gizmos.color = Color.white;
            // foreach (Line l in turnBoundaries)
            // {
            //     l.DrawWithGizmos(3);
            // }

        }
    }
}