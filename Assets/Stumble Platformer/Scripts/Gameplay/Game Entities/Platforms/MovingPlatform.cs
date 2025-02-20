using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using GlobalScripts.Extensions;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class MovingPlatform : BasePlatform
    {
        [SerializeField] protected MovingType movingType = MovingType.PingPong;

        [Header("Movement")]
        [SerializeField] protected float movementSpeed = 3f;
        [SerializeField] protected float slowSpeedOnWayPoint = 0.5f;
        [SerializeField] protected float toleranceOffset = 0.1f;
        [SerializeField] protected float movementDelayAmount = 1f;
        [SerializeField] protected bool slowDownWhenCloseToStopPosition = true;

        [Header("Platform Tools")]
        [SerializeField] protected bool resetWaypoints;
        [SerializeField] protected float dummyPlatformHeight = 0.1f;

        [Header("Pivots")]
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 endPosition;

        protected float usedSpeed = 0;
        protected float delayTimer = 0;
        protected float slowDownDistance = 0;

        protected Vector3 firstPosition;
        protected Vector3 lastPosition;

        public Vector3 StartPosition { get => startPosition; set => startPosition = value; }
        public Vector3 EndPosition { get => endPosition; set => endPosition = value; }

        protected override void OnAwake()
        {
            base.OnAwake();
            usedSpeed = movementSpeed;
            firstPosition = startPosition;
            lastPosition = endPosition;
            CalculateSlowPlatformSpeed();
        }

        public override void OnPlatformCollide(Collision collision) { }
        public override void OnPlatformStay(Collision collision) { }
        public override void OnPlatformExit(Collision collision) { }

        public override void PlatformAction()
        {
            if (transform.IsCloseTo(lastPosition, toleranceOffset))
            {
                SetPlatformActive(false);

                if (delayTimer < movementDelayAmount)
                    delayTimer += Time.fixedDeltaTime;

                else
                {
                    OnPlatformTargeted();
                    SetPlatformActive(true);
                }
            }

            else if(slowDownWhenCloseToStopPosition && transform.IsCloseTo(lastPosition, slowDownDistance))
            {
                SmoothMovementSpeed(lastPosition);
                MovePlatform();
            }

            else if (slowDownWhenCloseToStopPosition && transform.IsCloseTo(firstPosition, slowDownDistance))
            {
                SmoothMovementSpeed(firstPosition);
                MovePlatform();
            }

            else
            {
                delayTimer = 0;
                usedSpeed = IsPlatformActive ? movementSpeed : 0;
                MovePlatform();
            }
        }

        protected virtual void CalculateSlowPlatformSpeed()
        {
            if (slowDownWhenCloseToStopPosition)
                slowDownDistance = Vector3.Distance(startPosition, endPosition) / 2f;
        }

        protected void OnPlatformTriggerEnter(Collider collider)
        {
        }

        protected void OnPlatformTriggerExit(Collider collider)
        {
        }

        protected virtual void ResetWaypoints()
        {
            startPosition = transform.position;
            endPosition = transform.position;
        }

        protected void MovePlatform()
        {
            Vector3 dir = (lastPosition - firstPosition).normalized;
            Vector3 movement = platformBody.position + dir * usedSpeed * Time.deltaTime;

            if (!usePhysics)
                platformBody.transform.position = movement;
            else
                platformBody.MovePosition(movement);
        }

        protected void SmoothMovementSpeed(Vector3 toPosition)
        {
            float speedInterpolate = transform.GetDistance(toPosition) / slowDownDistance;
            float smoothedSpeed = Mathf.Lerp(slowSpeedOnWayPoint, movementSpeed, speedInterpolate);
            usedSpeed = smoothedSpeed;
        }

        protected virtual void OnPlatformTargeted()
        {
            switch (movingType)
            {
                case MovingType.Restart:
                    SwitchToStartPosition();
                    break;
                case MovingType.PingPong:
                    SwapPivotPositions();
                    break;
            }
        }

        private void SwitchToStartPosition() => transform.position = firstPosition;

        private void SwapPivotPositions() => (firstPosition, lastPosition) = (lastPosition, firstPosition);

        private void FetchComponents()
        {
            platformBody ??= GetComponent<Rigidbody>();
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
            FetchComponents();

            if (resetWaypoints)
            {
                resetWaypoints = false;
                ResetWaypoints();
            }
        }
#endif
    }
}
