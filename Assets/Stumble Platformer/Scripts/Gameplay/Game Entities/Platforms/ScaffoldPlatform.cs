using R3;
using R3.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Miscs;
using Sirenix.OdinInspector;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public class ScaffoldPlatform : BasePlatform
    {
        [SerializeField] private DummyPlatform[] dummyPlatforms;

        protected override void OnAwake()
        {
            RegisterPlatform();
        }

        public override void OnPlatformCollide(Collision collision) { }

        public override void OnPlatformExit(Collision collision) { }

        public override void OnPlatformStay(Collision collision) { }

        public override void PlatformAction() { }

        [Button]
        public void GetDummyPlatforms()
        {
            dummyPlatforms = transform.GetComponentsInChildren<DummyPlatform>();
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

                dummyPlatforms[i].OnTriggerExitAsObservable()
                                 .Subscribe(OnPlatformTriggerExit)
                                 .AddTo(ref builder);
            }

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
    }
}
