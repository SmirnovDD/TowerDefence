using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;

namespace Pathfinding
{
    //[RequireComponent(typeof(SailorModel))]
    public class PathfindingUnit : MonoBehaviour
    {
        public Transform TestTarget;
        [Header("Movement Parameters")]
        public float BaseSpeed;
        [ReadOnly] public float Speed;
        public float TurnSpeed;

        [Header("Status")]
        [ReadOnly] public State MoveState;
        [ReadOnly] public PathfindingGrid CurrentGrid;
        [Header("Debug Display")]
        [CanBeNull] public PathfindingManager.TargetPoint CurrentWaypoint;
        [CanBeNull] public Node TargetNode;

        private List<PathfindingManager.TargetPoint> Waypoints = new List<PathfindingManager.TargetPoint>();
        private Coroutine _lerpToBehaviourCoroutine;
        private Coroutine _connectionMovementCoroutine;
        private bool _newPathAfterConnectionFinishes;
        private PathfindingGrid newGrid;
        [NonSerialized] public float SqrWaypointReachDistance;
        private int _currentWaypointIndex;
        private PathData _pathToWaypoint = new PathData(new Node[0], Vector3.zero);
        private int _pathIndex = 0;

        private Vector3 _lastLocalTargetPos;
        private UnitAnimationController _animationController;

        private void Awake()
        {
            _animationController = GetComponent<UnitAnimationController>();
        }

        public void Start()
        {
            MoveState = State.Stationary;
            SqrWaypointReachDistance = PathfindingManager.Me.SqrWaypointReachDistance;
        }

        public void SetPath(Transform target)
        {
            if (Vector3.Distance(_lastLocalTargetPos, target.position)< 0.1f)
                return;

            _lastLocalTargetPos = target.position;

            SetWaypoints(target);
            RequestPathToWaypoint();            
        }
        
        private void SetWaypoints(Transform target)
        {
            Waypoints = PathfindingManager.Me.CalculatePathForSailor(this, target);
            CurrentWaypoint = Waypoints[0];
            _currentWaypointIndex = 0;
        }

        private void RequestPathToWaypoint()
        {
            PathfindingMultithreading.RequestPath(new PathRequest(transform.position,
                CurrentWaypoint.TargetPosition.position,
                OnPathFound, CurrentGrid, true, true, BaseSpeed));
        }

        private void OnPathFound(Node[] nodes, bool pathSuccessful, float pathLength, float timeToFinish)
        {
            if (pathSuccessful == false)
            {
                Debug.Log("UNSUCCESSFUL PATH " + gameObject.name + " at " + transform.localPosition);
                return;
            }
            InitPath(nodes, pathLength, timeToFinish);
        }

        private void InitPath(Node[] nodes, float pathLength, float timeToFinish)
        {
            if (nodes.Length == 0)
                Debug.Log("Should never happen");

            if (MoveState == State.LerpingIntoBehaviour)
            {
                StopCoroutine(_lerpToBehaviourCoroutine);
                _lerpToBehaviourCoroutine = null;
            }

            //_sailorModel.ToggleAnimation(State.Stationary, false);
            //_sailorModel.ToggleAnimation(State.Moving, true);
            MoveState = State.Moving;
            _pathToWaypoint = new PathData(nodes, transform.position);
            _pathIndex = 0;
        }

        private void Update()
        {
            if(TestTarget)
                SetPath(TestTarget);
            if (MoveState == State.Moving)
            {
                TargetNode = _pathToWaypoint.Nodes[_pathIndex];
                if (GetSqrDistanceToPoint(TargetNode.Position) > SqrWaypointReachDistance)
                {
                    transform.position = Vector3.MoveTowards(transform.position, TargetNode.Position, Speed * GameTimeManager.Me.DeltaTime);
                    if (TargetNode.Position - transform.position != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation (TargetNode.Position - transform.position);
                        transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, GameTimeManager.Me.DeltaTime * TurnSpeed);
                        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                    }
                }
                else
                {
                    _pathIndex++;
                    if (_pathIndex >= _pathToWaypoint.Nodes.Length)
                    {
                        OnWaypointReached();
                    }
                }
            }
        }
        public void OnWaypointReached()
        {
            SetNextWaypoint();
            if (CurrentWaypoint == null)
            {
                MoveState = State.LerpingIntoBehaviour;
                _lerpToBehaviourCoroutine =
                    StartCoroutine(LerpIntoBehaviour(_lastLocalTargetPos));
            }
            else
            {
                var previousWaypoint = Waypoints[_currentWaypointIndex - 1];
                if (Vector3.SqrMagnitude(CurrentWaypoint.TargetPosition.position - previousWaypoint.TargetPosition.position) < SqrWaypointReachDistance)
                {
                    OnWaypointReached(); //if SetNextWaypoint will make currentwaypoint equal to null, we want to finish movement, we also want to check for connection special movement
                }
                else
                {                                      
                    RequestPathToWaypoint();                    
                }
            }
        }

        private void SetNextWaypoint()
        {
            _currentWaypointIndex++;
            CurrentWaypoint = _currentWaypointIndex < Waypoints.Count ? Waypoints[_currentWaypointIndex] : null;
            if (CurrentWaypoint != null)
            {
                //_sailorModel.ToggleAnimation(State.Moving, false);
            }
        }

        private IEnumerator LerpIntoBehaviour(Vector3 pos)
        {
            float t = 0;
            Vector3 initialPos = transform.position;
            Quaternion initialRot = transform.rotation;
            while (t < 1)
            {
                t += 2 * GameTimeManager.Me.DeltaTime;
                transform.position = Vector3.Lerp(initialPos, pos, t);
                transform.rotation = Quaternion.Lerp(initialRot, Quaternion.identity, t);
                yield return null;
            }
            //_sailorModel.ToggleAnimation(State.Moving, false);
            //_sailorModel.ToggleAnimation(State.Stationary, true);
            MoveState = State.Stationary;
            _lerpToBehaviourCoroutine = null;
        }
        private float GetSqrDistanceToPoint(Vector3 point)
        {
            return Vector3.SqrMagnitude(transform.position - point);
        }
    }

    public enum State
    {
        Stationary,
        Moving,
        LerpingIntoBehaviour,
        MovingThroughConnection
    }
}
