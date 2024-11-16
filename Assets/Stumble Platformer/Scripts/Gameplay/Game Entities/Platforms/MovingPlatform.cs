using R3;
using R3.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Miscs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using GlobalScripts.Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class MovingPlatform : BasePlatform
    {
        [SerializeField] protected BoxCollider platformCollider;
        [SerializeField] protected DummyPlatform dummyPlatform;

        [Header("Movement")]
        [SerializeField] protected float movementSpeed = 3f;
        [SerializeField] protected float toleranceOffset = 0.1f;
        [SerializeField] protected float movementDelayAmount = 1f;

        [Header("Platform Tools")]
        [SerializeField] protected bool resetWaypoints;
        [SerializeField] protected bool checkDummyPlatform;
        [SerializeField] protected float dummyPlatformHeight = 0.1f;

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
            RegisterDummyPlatform();
        }

        public override void OnPlatformCollide(Collision collision)
        {
            
        }

        public override void OnPlatformStay(Collision collision)
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
                    delayAmount += Time.fixedDeltaTime;

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

        protected void OnPlatformTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out ICharacterParentSetter parentSetter))
            {
                parentSetter.SetParent(transform);
            }
        }

        protected void OnPlatformTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent(out ICharacterParentSetter parentSetter))
            {
                parentSetter.SetParent(null);
            }
        }


        protected virtual void ResetWaypoints()
        {
            startPosition = transform.position;
            endPosition = transform.position;
        }

        protected void RegisterDummyPlatform()
        {
            var builder = Disposable.CreateBuilder();
            
            dummyPlatform.OnTriggerEnterAsObservable()
                         .Subscribe(OnPlatformTriggerEnter)
                         .AddTo(ref builder);
            
            dummyPlatform.OnTriggerExitAsObservable()
                         .Subscribe(OnPlatformTriggerExit)
                         .AddTo(ref builder);
            
            builder.RegisterTo(this.destroyCancellationToken);
        }

        protected void MovePlatform()
        {
            Vector3 dir = (lastPosition - firstPosition).normalized;
            Vector3 movement = platformBody.position + dir * usedSpeed * Time.fixedDeltaTime;
            platformBody.transform.position = movement;
        }

        private void SwapPivotPositions()
        {
            (firstPosition, lastPosition) = (lastPosition, firstPosition);
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
            platformCollider ??= GetComponent<BoxCollider>();

            if (resetWaypoints)
            {
                resetWaypoints = false;
                ResetWaypoints();
            }

            if (checkDummyPlatform)
            {
                checkDummyPlatform = false;
                if (transform.childCount <= 0)
                    return;

                dummyPlatform = transform.GetChild(0).GetComponent<DummyPlatform>();

                if (dummyPlatform == null)
                    return;

                float height = platformCollider.size.y / 2 + dummyPlatformHeight * 0.5f / transform.localScale.y;
                Vector3 size = new Vector3(platformCollider.size.x, dummyPlatformHeight / transform.localScale.y, platformCollider.size.z);
                Vector3 center = new Vector3(platformCollider.center.x, height, platformCollider.center.z);
                dummyPlatform.SetSizeAndCenter(size, center);
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
