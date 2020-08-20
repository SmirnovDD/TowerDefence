using System;
using UnityEngine;
using System.Collections;

namespace Pathfinding
{
	[Serializable]
	public class Node : IHeapItem<Node>
	{
		public bool Walkable;
		public Vector3 Position;
		public int GridX;
		public int GridY;
		public int MovementPenalty = 10;

		public int GCost;
		public int HCost;
		public Node Parent;
		private int _heapIndex;
		//Debug
		//public DebugNodeView DebugView;

		public Node(int _gridX, int _gridY)
		{
			GridX = _gridX;
			GridY = _gridY;
		}
		public Node(Vector3 _worldPos, int _gridX, int _gridY, int _penalty, bool walkable = true)
		{
			Walkable = walkable;
			Position = _worldPos;
			GridX = _gridX;
			GridY = _gridY;
			MovementPenalty = _penalty;
		}

		public int fCost
		{
			get { return GCost + HCost; }
		}

		public int HeapIndex
		{
			get { return _heapIndex; }
			set { _heapIndex = value; }
		}

		public int CompareTo(Node nodeToCompare)
		{
			int compare = fCost.CompareTo(nodeToCompare.fCost);
			if (compare == 0)
			{
				compare = HCost.CompareTo(nodeToCompare.HCost);
			}

			return -compare;
		}
	}
}
