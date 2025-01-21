using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using GlobalScripts.Extensions;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public class MovingObjectMultiplePoints : MovingPlatform
    {
        [SerializeField] private bool isRevertible;

        [Header("Waypoints")]
        [SerializeField] private Vector3[] positions = new Vector3[2];

        private int maxStep = 0;
        private int stepCount = 0;
        private MovingPlatformOrder order = MovingPlatformOrder.None;

        public Vector3[] Positions
        {
            get => positions;
            set => positions = value;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            usedSpeed = movementSpeed;

            firstPosition = transform.position;
            lastPosition = positions[1];

            maxStep = positions.Length;
            order = MovingPlatformOrder.Ascending;
        }

        public override void PlatformAction()
        {
            if (transform.IsCloseTo(lastPosition, toleranceOffset))
            {
                SetPlatformActive(false);

                if (delayAmount < movementDelayAmount)
                    delayAmount += Time.fixedDeltaTime;

                else
                {
                    if (isRevertible)
                    {
                        if (order == MovingPlatformOrder.Ascending)
                        {
                            AscendingPositions();
                            if (stepCount == maxStep)
                                order = MovingPlatformOrder.Descending;
                        }

                        else
                        {
                            DescendingPositions();
                            if (stepCount == 0)
                                order = MovingPlatformOrder.Ascending;
                        }
                    }

                    else
                        AscendingPositions();

                    SetPlatformActive(true);
                }
            }

            else if (slowDownWhenCloseToStopPosition && transform.IsCloseTo(lastPosition, slowDownDistance))
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
                delayAmount = 0;
                MovePlatform();
            }
        }

        private void AscendingPositions()
        {
            firstPosition = positions[stepCount];
            lastPosition = positions[(stepCount + 1) % positions.Length];
            stepCount = (stepCount + 1) % positions.Length;
        }

        private void DescendingPositions()
        {
            firstPosition = positions[stepCount];
            lastPosition = positions[(stepCount - 1) % positions.Length];
            stepCount = (stepCount - 1) % positions.Length;
        }

        protected override void ResetWaypoints()
        {
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = transform.position;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if (positions.Length > 0)
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    Gizmos.DrawSphere(positions[i], 0.15f);
                }
            }
        }
#endif
    }
}
