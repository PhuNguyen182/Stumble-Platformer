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
    public class PlayerHealth : MonoBehaviour, ICharacterHealth
    {
        [SerializeField] private float deadDelayAmount = 1f;
        [SerializeField] private float respawnDelayAmount = 1f;
        [SerializeField] private PlayerController playerController;

        private int _healthPoint = 0;
        private int _checkPointIndex = 0;

        private IPublisher<RespawnMessage> _respawnPublisher;
        private IPublisher<ReportPlayerHealthMessage> _reportPlayerHealthPublisher;

        public int CheckPointIndex => _checkPointIndex;

        public int HealthPoint => _healthPoint;

        private void Start()
        {
            _respawnPublisher = GlobalMessagePipe.GetPublisher<RespawnMessage>();
            _reportPlayerHealthPublisher = GlobalMessagePipe.GetPublisher<ReportPlayerHealthMessage>();
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

                _reportPlayerHealthPublisher.Publish(new ReportPlayerHealthMessage
                {
                    Health = HealthPoint
                });

                OnDeadZoneDelay().Forget();
            }
        }

        private async UniTask OnDeadZoneDelay()
        {
            await UniTask.WaitForSeconds(deadDelayAmount, cancellationToken: destroyCancellationToken);
            playerController.SetCharacterActive(false);

            //if (_healthPoint > 0)
            {
                await UniTask.WaitForSeconds(respawnDelayAmount, cancellationToken: destroyCancellationToken);
                _respawnPublisher.Publish(new RespawnMessage
                {
                    ID = gameObject.GetInstanceID()
                });
            }
        }

        public void TakeDamage(int damage)
        {
            _healthPoint = _healthPoint - damage;
        }

        public void SetHealth(int health)
        {
            _healthPoint = health;
        }
    }
}
