using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public abstract class BasePlayRule : MonoBehaviour, IPlayeRule
    {
        protected IDisposable _messageDisposable;
        protected ISubscriber<ReportPlayerHealthMessage> _playerHealthSubscriber;

        public int PlayerHealth { get; private set; }

        private void Start()
        {
            RegisterMessage();
        }

        public abstract void OnPlayerFinish();
        public abstract void OnPlayerFall();

        public void Win()
        {
            
        }

        public void Lose()
        {
            
        }

        protected void RegisterMessage()
        {
            var builder = DisposableBag.CreateBuilder();
            _playerHealthSubscriber = GlobalMessagePipe.GetSubscriber<ReportPlayerHealthMessage>();
            _playerHealthSubscriber.Subscribe(UpdateHealth)
                                   .AddTo(builder);
            _messageDisposable = builder.Build();
        }

        protected void UpdateHealth(ReportPlayerHealthMessage message)
        {
            PlayerHealth = message.Health;
        }

        private void OnDestroy()
        {
            _messageDisposable.Dispose();
        }
    }
}
