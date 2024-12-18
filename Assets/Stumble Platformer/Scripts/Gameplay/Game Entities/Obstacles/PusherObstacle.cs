using R3;
using R3.Triggers;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Miscs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class PusherObstacle : BaseObstacle
    {
        [SerializeField] private DummyPlatform dummyPlatform;

        public override void OnAwake()
        {
            RegisterDummyPlatform();
        }

        private void RegisterDummyPlatform()
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

        private void OnPlatformTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out ICharacterParentSetter parentSetter))
            {
                parentSetter.SetParent(transform);
            }
        }

        private void OnPlatformTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent(out ICharacterParentSetter parentSetter))
            {
                parentSetter.SetParent(null);
            }
        }

        public override void DamageCharacter(Collision collision)
        {
            
        }

        public override void ExitDamage(Collision collision)
        {
            
        }

        public override void ObstacleAction()
        {
            
        }
    }
}
