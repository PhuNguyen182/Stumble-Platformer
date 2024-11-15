using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.GameHandlers;
using StumblePlatformer.Scripts.Common.Enums;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public abstract class BasePlayRule : MonoBehaviour, IPlayRule
    {
        protected IDisposable messageDisposable;
        protected ISubscriber<ReportPlayerHealthMessage> playerHealthSubscriber;
        protected GameStateController gameStateController;

        public int PlayerHealth { get; private set; }

        private void Start()
        {
            RegisterMessage();
        }

        public abstract void OnPlayerWin();
        public abstract void OnPlayerFinish();
        public abstract void OnPlayerLose();
        
        public void Finish()
        {
            gameStateController.FinishLevel();
            OnPlayerFinish();
        }

        public void Win()
        {
            gameStateController.EndGame(EndResult.Win);
            OnPlayerWin();
        }

        public void Lose()
        {
            gameStateController.EndGame(EndResult.Lose);
            OnPlayerLose();
        }

        protected void RegisterMessage()
        {
            var builder = DisposableBag.CreateBuilder();
            playerHealthSubscriber = GlobalMessagePipe.GetSubscriber<ReportPlayerHealthMessage>();
            playerHealthSubscriber.Subscribe(UpdateHealth)
                                   .AddTo(builder);
            messageDisposable = builder.Build();
        }

        protected void UpdateHealth(ReportPlayerHealthMessage message)
        {
            PlayerHealth = message.Health;
        }

        public void SetStateController(GameStateController gameStateController)
        {
            this.gameStateController = gameStateController;
        }

        private void OnDestroy()
        {
            messageDisposable.Dispose();
        }
    }
}
