using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerMessages : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;

        private IPublisher<RespawnMessage> _respawnPublisher;
        private IPublisher<PlayerDamageMessage> _playerDamagePublisher;
        private IPublisher<ReportPlayerHealthMessage> _reportPlayerHealthPublisher;

        private ISubscriber<KillCharactersMessage> _killCharacterSubscriber;
        private IDisposable _messageDisposable;

        public void InitializeMessages()
        {
            _respawnPublisher = GlobalMessagePipe.GetPublisher<RespawnMessage>();
            _reportPlayerHealthPublisher = GlobalMessagePipe.GetPublisher<ReportPlayerHealthMessage>();
            _playerDamagePublisher = GlobalMessagePipe.GetPublisher<PlayerDamageMessage>();

            var builder = DisposableBag.CreateBuilder();
            _killCharacterSubscriber = GlobalMessagePipe.GetSubscriber<KillCharactersMessage>();
            _killCharacterSubscriber.Subscribe(message => Kill()).AddTo(builder);
            _messageDisposable = builder.Build();
        }

        public void RespawnPlayer()
        {
            _respawnPublisher.Publish(new RespawnMessage
            {
                ID = gameObject.GetInstanceID()
            });
        }

        public void ReportHealth(int hp)
        {
            _reportPlayerHealthPublisher.Publish(new ReportPlayerHealthMessage
            {
                Health = hp,
                PlayerID = gameObject.GetInstanceID()
            });
        }

        public void PlayerDamage()
        {
            _playerDamagePublisher.Publish(new PlayerDamageMessage
            {
                ID = gameObject.GetInstanceID()
            });
        }

        private void Kill()
        {
            playerHealth.Kill();
        }

        private void OnDestroy()
        {
            _messageDisposable.Dispose();
        }
    }
}
