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
        protected ISubscriber<PlayerFinishMessage> playerFinishSubscriber;
        protected ISubscriber<PlayerFallMessage> playerFallSubscriber;
        protected ISubscriber<PlayerLoseMessage> playerLoseSubscriber;
        protected GameStateController gameStateController;

        public int PlayerHealth { get; private set; }
        public int CurrentPlayerID { get; set; }

        private void Start()
        {
            RegisterMessage();
        }

        protected void RegisterMessage()
        {
            var builder = DisposableBag.CreateBuilder();

            playerHealthSubscriber = GlobalMessagePipe.GetSubscriber<ReportPlayerHealthMessage>();
            playerFinishSubscriber = GlobalMessagePipe.GetSubscriber<PlayerFinishMessage>();
            playerFallSubscriber = GlobalMessagePipe.GetSubscriber<PlayerFallMessage>();
            playerLoseSubscriber = GlobalMessagePipe.GetSubscriber<PlayerLoseMessage>();

            playerHealthSubscriber.Subscribe(UpdateHealth).AddTo(builder);
            playerFinishSubscriber.Subscribe(Finish).AddTo(builder);
            playerFallSubscriber.Subscribe(Fall).AddTo(builder);
            playerLoseSubscriber.Subscribe(Lose).AddTo(builder);

            messageDisposable = builder.Build();
        }

        protected void UpdateHealth(ReportPlayerHealthMessage message)
        {
            PlayerHealth = message.Health;
        }

        public abstract void OnPlayerWin();
        public abstract void OnPlayerFinish();
        public abstract void OnPlayerLose();
        public abstract void OnPlayerFall();

        public void Finish(PlayerFinishMessage message)
        {
            gameStateController.FinishLevel();
            OnPlayerFinish();
        }

        public void Win()
        {
            gameStateController.EndGame(EndResult.Win);
            OnPlayerWin();
        }

        public void Lose(PlayerLoseMessage message)
        {
            if (CurrentPlayerID != message.ID)
                return;

            gameStateController.EndGame(EndResult.Lose);
            OnPlayerLose();
        }

        public void Fall(PlayerFallMessage message)
        {
            if (CurrentPlayerID != message.ID)
                return;

            OnPlayerFall();
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
