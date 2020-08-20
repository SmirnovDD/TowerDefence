using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace Pathfinding
{
	[RequireComponent(typeof(PathfindingAlgorithm))]
	[RequireComponent(typeof(PathfindingManager))]

	public class PathfindingMultithreading : MonoBehaviour
	{

		Queue<PathSolution> results = new Queue<PathSolution>();

		static PathfindingMultithreading instance;
		PathfindingAlgorithm pathfinding;

		void Awake()
		{
			instance = this;
			pathfinding = GetComponent<PathfindingAlgorithm>();
		}

		void Update()
		{
			if (results.Count > 0)
			{
				int itemsInQueue = results.Count;
				lock (results)
				{
					for (int i = 0; i < itemsInQueue; i++)
					{
						PathSolution result = results.Dequeue();
						result.Callback(result.Path, result.Success, result.Length, result.TimeToFinish);
					}
				}
			}
		}

		public static void RequestPath(PathRequest request)
		{
			ThreadStart threadStart = delegate {
				instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
			};
			threadStart.Invoke();
		}

		public void FinishedProcessingPath(PathSolution result)
		{
			lock (results)
			{
				results.Enqueue(result);
			}
		}
	}

	public struct PathSolution
	{
		public Node[] Path;
		public bool Success;
		public float Length;
		public float TimeToFinish;
		public Action<Node[], bool, float, float> Callback;

		public PathSolution(Node[] path, bool success, Action<Node[], bool, float, float> callback, float length, float timeToFinish)
		{
			Path = path;
			Success = success;
			Length = length;
			TimeToFinish = timeToFinish;
			Callback = callback;
		}

	}

	public struct PathRequest
	{
		public Vector3 PathStart;
		public Vector3 PathEnd;
		public PathfindingGrid Grid;
		public Action<Node[], bool, float, float> Callback;
		public bool GetLength;
		public bool GetTimeToFinish;
		public float MovementSpeed;
		public PathRequest(Vector3 start, Vector3 end, Action<Node[], bool, float, float> callback, PathfindingGrid targetGrid, bool getLength, bool getTimeToFinish, float movementSpeed)
		{
			PathStart = start;
			PathEnd = end;
			Callback = callback;
			Grid = targetGrid;
			GetLength = getLength;
			GetTimeToFinish = getTimeToFinish;
			MovementSpeed = movementSpeed;
		}
	}
}
