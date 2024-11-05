using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovingPlatform : BasePlatform
    {
        [Header("Movement")]
        [SerializeField] protected float movementSpeed = 3f;
        [SerializeField] protected float toleranceOffset = 0.1f;
        [SerializeField] protected float movementDelayAmount = 1f;
        [SerializeField] protected bool resetWaypoints;

        [Header("Pivots")]
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 endPosition;

        protected float usedSpeed = 0;
        protected float delayAmount = 0;
        protected Vector3 firstPosition;
        protected Vector3 lastPosition;

        public Vector3 StartPosition { get => startPosition; set => startPosition = value; }
        public Vector3 EndPosition { get => endPosition; set => endPosition = value; }

        protected override void OnAwake()
        {
            usedSpeed = movementSpeed;
            firstPosition = startPosition;
            lastPosition = endPosition;
        }

        public override void OnPlatformCollide(Collision collision)
        {

        }

        public override void OnPlatformExit(Collision collision)
        {

        }

        public override void PlatformAction()
        {
            if (transform.IsCloseTo(lastPosition, toleranceOffset))
            {
                SetPlatformActive(false);

                if (delayAmount < movementDelayAmount)
                {
                    delayAmount += Time.fixedDeltaTime;
                }

                else
                {
                    SwapPivotPositions();
                    SetPlatformActive(true);
                }
            }

            else
            {
                delayAmount = 0;
                MovePlatform();
            }
        }

        private void MovePlatform()
        {
            Vector3 dir = (lastPosition - firstPosition).normalized;
            Vector3 movement = platformBody.position + dir * usedSpeed * Time.fixedDeltaTime;
            platformBody.MovePosition(movement);
        }

        private void SwapPivotPositions()
        {
            firstPosition = firstPosition + lastPosition;
            lastPosition = firstPosition - lastPosition;
            firstPosition = firstPosition - lastPosition;
        }

        private void SetPlatformActive(bool active)
        {
            usedSpeed = active ? movementSpeed : 0;
        }

        protected virtual void ResetWaypoints()
        {
            startPosition = transform.position;
            endPosition = transform.position;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startPosition, 0.15f);
            Gizmos.DrawSphere(endPosition, 0.15f);
        }

        private void OnValidate()
        {
            platformBody ??= GetComponent<Rigidbody>();

            if (resetWaypoints)
            {
                resetWaypoints = false;
                ResetWaypoints();
            }
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MovingPlatform)), CanEditMultipleObjects]
    public class MovingPlatformPositionHandle : Editor
    {
        protected virtual void OnSceneGUI()
        {
            MovingPlatform movingPlatform = (MovingPlatform)target;

            EditorGUI.BeginChangeCheck();
            Vector3 newStartPosition = Handles.PositionHandle(movingPlatform.StartPosition, Quaternion.identity);
            Vector3 newEndPosition = Handles.PositionHandle(movingPlatform.EndPosition, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                movingPlatform.StartPosition = newStartPosition;
                movingPlatform.EndPosition = newEndPosition;
            }
        }
    }
#endif
}
