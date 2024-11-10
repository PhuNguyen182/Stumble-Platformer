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

        private bool _isSteppedOn = false;
        private float _platformDuration = 0;
        private MaterialPropertyBlock block;

        private readonly int _dropHash = Animator.StringToHash("Drop");
        private readonly int _steppedOnHash = Animator.StringToHash("StepOn");
        private readonly int _colorProperty = Shader.PropertyToID("_BaseColor");

        protected override void OnAwake()
        {
            block = new();
        }

        protected override void OnStart()
        {
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

        public override void OnPlatformCollide(Collision collision)
        {
            if (!_isSteppedOn)
            {
                _isSteppedOn = true;
                OnStepped().Forget();
            }
        }

        public override void OnPlatformStay(Collision collision) { }

        public override void OnPlatformExit(Collision collision) { }

        private void SetPlatformColor(bool isSteppedOn)
        {
            platformRenderer.GetPropertyBlock(block);
            block.SetColor(_colorProperty, isSteppedOn ? steppedOnColor : originalColor);
            platformRenderer.SetPropertyBlock(block);
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

        private void OnDestroy()
        {
            block.Clear();
            UpdateHandlerManager.Instance.RemoveUpdateBehaviour(this);
        }
    }
}
