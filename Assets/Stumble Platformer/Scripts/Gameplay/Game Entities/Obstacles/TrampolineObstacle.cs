using R3;
using R3.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Miscs;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class TrampolineObstacle : BaseObstacle
    {
        [SerializeField] private float pushForce = 8;
        [SerializeField] private DummyPlatform dummyPlatform;
        [SerializeField] private Animator platformAnimator;

        private readonly int _pushHash = Animator.StringToHash("Push");

        public override void OnAwake()
        {
            base.OnAwake();
            RegisterTrampoline();
        }

        public override void DamageCharacter(Collision collision) { }

        public override void ExitDamage(Collision collision) { }

        public override void ObstacleAction() { }

        private void RegisterTrampoline()
        {
            if (dummyPlatform == null)
                return;

            var builder = Disposable.CreateBuilder();
            
            dummyPlatform.OnTriggerEnterAsObservable()
                         .Subscribe(OnTrampolineCollide)
                         .AddTo(ref builder);
            
            builder.RegisterTo(this.destroyCancellationToken);
        }

        private void OnTrampolineCollide(Collider collider)
        {
            if (collider.attachedRigidbody != null)
            {
                if (collider.attachedRigidbody.isKinematic)
                    return;

                if (collider.TryGetComponent(out PlayerController characterMovement) && characterMovement.IsOwner)
                {
                    characterMovement.OnGrounded();
                    Jump(collider);
                }

                if (!characterMovement) // Prevent animate push twice
                    Jump(collider);
            }
        }

        private void Jump(Collider collider)
        {
            collider.attachedRigidbody.velocity = transform.up * pushForce;
            platformAnimator.SetTrigger(_pushHash);
        }
    }
}
