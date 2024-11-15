using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using Cysharp.Threading.Tasks;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerHealth : MonoBehaviour, IDamageable, ICharacterHealth
    {
        [SerializeField] private float deadDelayAmount = 1f;
        [SerializeField] private float respawnDelayAmount = 1f;
        [SerializeField] private PlayerController playerController;

        private int _healthPoint = 0;
        private int _checkPointIndex = 0;
        private IPublisher<RespawnMessage> _respawnPublisher;

        public int CheckPointIndex => _checkPointIndex;

        public int HealthPoint => throw new System.NotImplementedException();

        private void Start()
        {
            _respawnPublisher = GlobalMessagePipe.GetPublisher<RespawnMessage>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out RespawnArea respawnArea))
            {
                if (respawnArea.AreaIndex > _checkPointIndex)
                    _checkPointIndex = respawnArea.AreaIndex;
            }

            if(other.TryGetComponent(out DeadZone deadZone))
            {
                playerController.IsActive = false;
                deadZone.PlayDeathEffect(transform.position);
                
                playerController.TakeDamage(new DamageData
                {
                    DamageAmount = 1
                });

                OnDeadZoneDelay().Forget();
            }
        }

        private async UniTask OnDeadZoneDelay()
        {
            await UniTask.WaitForSeconds(deadDelayAmount, cancellationToken: destroyCancellationToken);
            playerController.SetCharacterActive(false);

            await UniTask.WaitForSeconds(respawnDelayAmount, cancellationToken: destroyCancellationToken);
            _respawnPublisher.Publish(new RespawnMessage
            {
                ID = gameObject.GetInstanceID()
            });
        }

        public void TakeDamage(DamageData damageData)
        {
            TakeDamage(damageData.DamageAmount);
        }

        public void TakeDamage(int damage)
        {
            _healthPoint = _healthPoint - damage;
        }
    }
}
