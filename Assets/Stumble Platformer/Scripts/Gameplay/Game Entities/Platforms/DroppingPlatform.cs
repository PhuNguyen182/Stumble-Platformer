using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public class DroppingPlatform : BasePlatform, IUpdateHandler
    {
        [SerializeField] private float delayOnStepped = 0.2f;
        [SerializeField] private float droppingDuration = 6f;
        [SerializeField] private Animator platformAnimator;
        [SerializeField] private AnimationClip dropClip;

        [Header("Visuals")]
        [SerializeField] private Color originalColor;
        [SerializeField] private Color steppedOnColor;
        [SerializeField] private Renderer platformRenderer;
        [SerializeField] private Collider platformCollider;

        private const string DropProperty = "Drop";
        private const string StepOnProperty = "StepOn";
        private const string ColorProperty = "_BaseColor";

        private bool _isSteppedOn = false;
        private float _platformDuration = 0;
        private MaterialPropertyBlock _block;

        private readonly int _dropHash = Animator.StringToHash(DropProperty);
        private readonly int _steppedOnHash = Animator.StringToHash(StepOnProperty);
        private readonly int _colorProperty = Shader.PropertyToID(ColorProperty);

        protected override void OnAwake()
        {
            _block = new();
            SetPlatformColor(false);
        }

        protected override void OnStart()
        {
            base.OnStart();
            UpdateHandlerManager.Instance.AddUpdateBehaviour(this);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isSteppedOn && _platformDuration < droppingDuration)
            {
                _platformDuration += deltaTime;

                if(_platformDuration >= droppingDuration)
                {
                    DropPlatform().Forget();
                }
            }
        }

        public override void PlatformAction() { }

        public override void SetPlatformActive(bool active)
        {
            base.SetPlatformActive(active);
            IsActive = active;
        }

        public override void OnPlatformCollide(Collision collision)
        {
            if (!_isSteppedOn && IsActive)
            {
                _isSteppedOn = true;
                OnStepped().Forget();
            }
        }

        public override void OnPlatformStay(Collision collision) { }

        public override void OnPlatformExit(Collision collision) { }

        private void SetPlatformColor(bool isSteppedOn)
        {
            platformRenderer.GetPropertyBlock(_block);
            _block.SetColor(_colorProperty, isSteppedOn ? steppedOnColor : originalColor);
            platformRenderer.SetPropertyBlock(_block);
        }

        private async UniTask OnStepped()
        {
            platformAnimator.SetTrigger(_steppedOnHash);
            await UniTask.Delay(TimeSpan.FromSeconds(delayOnStepped)
                                , cancellationToken: destroyCancellationToken);
            SetPlatformColor(true);
        }

        private async UniTask DropPlatform()
        {
            platformCollider.isTrigger = true;
            platformAnimator.SetTrigger(_dropHash);
            await UniTask.Delay(TimeSpan.FromSeconds(dropClip.length)
                                , cancellationToken: destroyCancellationToken);
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = originalColor;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.15f, 0.15f);
        }
#endif

        public override void OnDestroy()
        {
            base.OnDestroy();
            _block.Clear();
            UpdateHandlerManager.Instance.RemoveUpdateBehaviour(this);
        }
    }
}
