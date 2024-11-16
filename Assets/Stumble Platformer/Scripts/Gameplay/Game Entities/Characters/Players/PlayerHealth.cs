using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Common.Messages;
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
        private IPublisher<PlayerFinishMessage> _playerFinishPublisher;
        private IPublisher<PlayerFallMessage> _playerFallPublisher;
        private IPublisher<PlayerLoseMessage> _playerLosePublisher;

        public int CheckPointIndex => _checkPointIndex;

        public int HealthPoint => _healthPoint;

        private void Start()
        {
            _respawnPublisher = GlobalMessagePipe.GetPublisher<RespawnMessage>();
            _reportPlayerHealthPublisher = GlobalMessagePipe.GetPublisher<ReportPlayerHealthMessage>();
            _playerFinishPublisher = GlobalMessagePipe.GetPublisher<PlayerFinishMessage>();
            _playerFallPublisher = GlobalMessagePipe.GetPublisher<PlayerFallMessage>();
            _playerLosePublisher = GlobalMessagePipe.GetPublisher<PlayerLoseMessage>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out FinishZone finishZone))
            {
                OnFinishZone(finishZone);
            }

            if(other.TryGetComponent(out RespawnArea respawnArea))
            {
                OnRespawnArea(respawnArea);
            }

            if(other.TryGetComponent(out DeadZone deadZone))
            {
                OnDeadZone(deadZone);
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

        private void OnFinishZone(FinishZone finishZone)
        {
            finishZone.ReportFinish(playerController);

            _playerFinishPublisher.Publish(new PlayerFinishMessage
            {
                ID = gameObject.GetInstanceID()
            });
        }

        private void OnRespawnArea(RespawnArea respawnArea)
        {
            if (respawnArea.AreaIndex > _checkPointIndex)
                _checkPointIndex = respawnArea.AreaIndex;
        }

        private void OnDeadZone(DeadZone deadZone)
        {
            playerController.IsActive = false;
            deadZone.PlayDeathEffect(transform.position);

            playerController.TakeDamage(new DamageData
            {
                DamageAmount = DeadZone.GamePlayMode == GamePlayMode.SinglePlayer ? 1 : 0
            });

            _playerFallPublisher.Publish(new PlayerFallMessage
            {
                ID = gameObject.GetInstanceID()
            });

            _reportPlayerHealthPublisher.Publish(new ReportPlayerHealthMessage
            {
                Health = HealthPoint
            });

            OnDeadZoneDelay().Forget();
        }
    }
}
