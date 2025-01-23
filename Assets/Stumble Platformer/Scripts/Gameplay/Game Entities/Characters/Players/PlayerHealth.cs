using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerHealth : MonoBehaviour, ICharacterHealth
    {
        [SerializeField] private float deadDelayAmount = 1f;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerGraphics playerGraphics;
        [SerializeField] private PlayerMessages playerMessages;

        private int _healthPoint = 0;
        private int _checkPointIndex = 0;

        private bool _hasFinishLevel = false;
        private bool _canTakeDamage = true;
        private bool _hasPlayerController;

        public int CheckPointIndex => _checkPointIndex;
        public int HealthPoint => _healthPoint;

        private void Awake()
        {
            _hasPlayerController = TryGetComponent(out playerController);
        }

        private void Start()
        {
            playerMessages.InitializeMessages();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!playerController.IsOwner)
                return;

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
                if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
                {
                    if (_healthPoint > 0)
                        OnDeadZone(deadZone);
                }

                else OnDeadZone(deadZone);
            }
        }

        private async UniTask OnDeadZoneDelay()
        {
            await UniTask.WaitForSeconds(deadDelayAmount, cancellationToken: destroyCancellationToken);

            if (_hasPlayerController)
                playerController.SetCharacterActive(false);

            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
            {
                if (_healthPoint > 0)
                    playerMessages.RespawnPlayer();
                else
                    Kill();
            }

            else playerMessages.RespawnPlayer();
        }

        public void Kill()
        {
            if (_hasFinishLevel) 
                return;

            if (_hasPlayerController)
                playerController.SetCharacterActive(false);

            playerGraphics.CharacterVisual.SetLose();
        }

        public void SetPlayerCompleteLevel(bool isCompleted)
        {
            _hasFinishLevel = isCompleted;
        }

        public void OnRespawn()
        {
            _canTakeDamage = true;
            playerGraphics.SetPlayerGraphicActive(true);
        }

        public void TakeDamage(HealthDamage damage)
        {
            if (!_canTakeDamage)
                return;

            _canTakeDamage = false;
            _healthPoint = _healthPoint - damage.DamageAmount;

            if (damage.DamageType == DamageType.Energy)
            {
                playerGraphics.PlayDeadEffect();
                playerGraphics.SetPlayerGraphicActive(false);
                KillOneLife(damage);
            }
        }

        public void SetHealth(int health)
        {
            _healthPoint = health;
        }

        private void OnFinishZone(FinishZone finishZone)
        {
            if (_hasPlayerController)
            {
                playerController.IsActive = false;
                finishZone.ReportFinish(playerController);
            }
        }

        private void OnRespawnArea(RespawnArea respawnArea)
        {
            if (respawnArea.AreaIndex > _checkPointIndex)
                _checkPointIndex = respawnArea.AreaIndex;
        }

        private void OnDeadZone(DeadZone deadZone)
        {
            if (_hasPlayerController && !playerController.IsActive)
                return;

            playerController.IsActive = false;
            deadZone.PlayDeathEffect(transform.position);

            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
            {
                HealthDamage damage = new HealthDamage
                {
                    DamageAmount = 1,
                    DamageType = deadZone.DamageType
                };
                KillOneLife(damage);
            }

            playerMessages.PlayerDamage();
            OnDeadZoneDelay().Forget();
        }

        private void KillOneLife(HealthDamage damageData)
        {
            TakeDamage(damageData);
            playerMessages.ReportHealth(HealthPoint);
        }
    }
}
