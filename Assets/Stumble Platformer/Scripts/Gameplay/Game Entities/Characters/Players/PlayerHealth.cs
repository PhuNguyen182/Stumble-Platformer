using System;
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
        [SerializeField] private PlayerController playerController;

        private int _healthPoint = 0;
        private int _checkPointIndex = 0;
        private bool _hasFinishLevel;

        private IPublisher<RespawnMessage> _respawnPublisher;
        private IPublisher<LevelEndMessage> _playerFinishPublisher;
        private IPublisher<PlayerFallMessage> _playerFallPublisher;
        private IPublisher<ReportPlayerHealthMessage> _reportPlayerHealthPublisher;
        private ISubscriber<KillCharactersMessage> _killCharacterSubscriber;
        private IDisposable _messageDisposable;

        public int CheckPointIndex => _checkPointIndex;
        public int HealthPoint => _healthPoint;

        private void Start()
        {
            _hasFinishLevel = false;
            _respawnPublisher = GlobalMessagePipe.GetPublisher<RespawnMessage>();
            _reportPlayerHealthPublisher = GlobalMessagePipe.GetPublisher<ReportPlayerHealthMessage>();
            _playerFinishPublisher = GlobalMessagePipe.GetPublisher<LevelEndMessage>();
            _playerFallPublisher = GlobalMessagePipe.GetPublisher<PlayerFallMessage>();

            var builder = DisposableBag.CreateBuilder();
            _killCharacterSubscriber = GlobalMessagePipe.GetSubscriber<KillCharactersMessage>();
            _killCharacterSubscriber.Subscribe(message => Kill()).AddTo(builder);
            _messageDisposable = builder.Build();
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
                if (_healthPoint > 0)
                {
                    OnDeadZone(deadZone);
                }
            }
        }

        private async UniTask OnDeadZoneDelay()
        {
            await UniTask.WaitForSeconds(deadDelayAmount, cancellationToken: destroyCancellationToken);
            playerController.SetCharacterActive(false);

            if (_healthPoint > 0)
            {
                _respawnPublisher.Publish(new RespawnMessage
                {
                    ID = gameObject.GetInstanceID()
                });
            }

            else Kill();
        }

        private void Kill()
        {
            if (_hasFinishLevel) 
                return;

            playerController.SetCharacterActive(false);
            playerController.PlayerGraphics.CharacterVisual.CharacterAnimator.SetTrigger(CharacterAnimationKeys.LoseKey);
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
            _hasFinishLevel = true;
            finishZone.ReportFinish(playerController);
            playerController.IsActive = false;

            _playerFinishPublisher.Publish(new LevelEndMessage
            {
                ID = gameObject.GetInstanceID(),
                Result = EndResult.Win
            });
        }

        private void OnRespawnArea(RespawnArea respawnArea)
        {
            if (respawnArea.AreaIndex > _checkPointIndex)
                _checkPointIndex = respawnArea.AreaIndex;
        }

        private void OnDeadZone(DeadZone deadZone)
        {
            if (!playerController.IsActive)
                return;

            playerController.IsActive = false;
            deadZone.PlayDeathEffect(transform.position);

            playerController.TakeDamage(new DamageData
            {
                DamageAmount = DeadZone.GamePlayMode == GamePlayMode.SinglePlayer ? 1 : 0
            });

            _reportPlayerHealthPublisher.Publish(new ReportPlayerHealthMessage
            {
                Health = HealthPoint,
                PlayerID = gameObject.GetInstanceID()
            });

            _playerFallPublisher.Publish(new PlayerFallMessage
            {
                ID = gameObject.GetInstanceID()
            });

            OnDeadZoneDelay().Forget();
        }

        private void OnDestroy()
        {
            _messageDisposable.Dispose();
        }
    }
}
