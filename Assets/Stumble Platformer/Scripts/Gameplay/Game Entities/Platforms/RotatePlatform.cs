using R3;
using R3.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Miscs;
using GlobalScripts.Extensions;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public class RotatePlatform : BasePlatform
    {
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private DummyPlatform[] dummyPlatforms;

        [Space(10)]
        [SerializeField] private bool checkLocalPosition;
        [SerializeField] private bool followLocalPosition;
        [SerializeField] private Vector3 localPosition;
        [SerializeField] private Transform parent;

        [Header("Rotation")]
        [SerializeField] private bool usePhysics;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private RotateAxis rotateAxis;

        private Vector3 _rotateAxis;
        private Quaternion _rotation;

        protected override void OnAwake()
        {
            _rotateAxis = rotateAxis switch
            {
                RotateAxis.X => Vector3.right,
                RotateAxis.Y => Vector3.up,
                RotateAxis.Z => Vector3.forward,
                _ => Vector3.zero
            };

            RegisterPlatform();
        }

        public override void SetPlatformActive(bool active)
        {
            base.SetPlatformActive(active);
            IsActive = active;
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
            if (followLocalPosition)
                platformBody.position = parent.TransformPoint(localPosition);

            if (rotateSpeed != 0)
            {
                _rotation = Quaternion.Euler(_rotateAxis * rotateSpeed * Time.fixedDeltaTime);
                _rotation = platformBody.rotation * _rotation;
                
                if (usePhysics)
                    platformBody.MoveRotation(_rotation);
                else
                    platformBody.transform.rotation = _rotation;
            }
        }

        private void RegisterPlatform()
        {
            var builder = Disposable.CreateBuilder();

            for (int i = 0; i < dummyPlatforms.Length; i++)
            {
                if (dummyPlatforms[i] == null)
                    continue;

                dummyPlatforms[i].OnTriggerEnterAsObservable()
                                 .Subscribe(OnPlatformTriggerEnter)
                                 .AddTo(ref builder);

                dummyPlatforms[i].OnTriggerStayAsObservable()
                                 .Subscribe(OnPlatformTriggerStay)
                                 .AddTo(ref builder);

                dummyPlatforms[i].OnTriggerExitAsObservable()
                                 .Subscribe(OnPlatformTriggerExit)
                                 .AddTo(ref builder);
            }
            
            builder.RegisterTo(this.destroyCancellationToken);
        }

        private void OnPlatformTriggerEnter(Collider collider)
        {
            if (!collider.HasLayer(playerMask))
            {
                if (collider.TryGetComponent(out ICharacterParentSetter parentSetter))
                {
                    parentSetter.SetParent(platformBody.transform);
                }
            }
        }

        private void OnPlatformTriggerStay(Collider collider)
        {
            if (collider.transform.parent != null && collider.transform.parent.GetInstanceID() != platformBody.transform.GetInstanceID())
                return;

            if (collider.HasLayer(playerMask))
            {
                if (collider.TryGetComponent(out ICharacterParentSetter parentSetter))
                {
                    parentSetter.SetParent(platformBody.transform);
                }
            }
        }

        private void OnPlatformTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent(out ICharacterParentSetter parentSetter))
            {
                parentSetter.SetParent(null);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (checkLocalPosition)
            {
                checkLocalPosition = false;
                localPosition = transform.localPosition;
            }
        }
#endif
    }
}
