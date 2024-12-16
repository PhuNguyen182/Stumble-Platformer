using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.GameManagers;
using GlobalScripts.UpdateHandlerPattern;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public abstract class BasePlayRule : MonoBehaviour, IPlayRule, IUpdateHandler
    {
        [SerializeField] protected string objectiveTitle;

        protected IDisposable messageDisposable;
        protected DisposableBagBuilder bagBuilder;

        protected ISubscriber<ReportPlayerHealthMessage> playerHealthSubscriber;
        protected ISubscriber<EndGameMessage> endGameSubscriber;
        protected ISubscriber<LevelEndMessage> levelEndSubscriber;
        protected ISubscriber<PlayerFallMessage> playerFallSubscriber;
        protected GameStateController gameStateController;

        public int CurrentPlayerID { get; set; }
        public int PlayerHealth { get; protected set; }
        public bool IsActive { get; set; }
        public string ObjectiveTitle => objectiveTitle;

        private void Start()
        {
            OnStart();
            RegisterCommonMessage();
            UpdateHandlerManager.Instance.AddUpdateBehaviour(this);
        }

        protected virtual void OnStart() { }

        protected void RegisterCommonMessage()
        {
            bagBuilder = DisposableBag.CreateBuilder();

            endGameSubscriber = GlobalMessagePipe.GetSubscriber<EndGameMessage>();
            levelEndSubscriber = GlobalMessagePipe.GetSubscriber<LevelEndMessage>();
            playerHealthSubscriber = GlobalMessagePipe.GetSubscriber<ReportPlayerHealthMessage>();
            playerFallSubscriber = GlobalMessagePipe.GetSubscriber<PlayerFallMessage>();

            levelEndSubscriber.Subscribe(EndLevel).AddTo(bagBuilder);
            playerHealthSubscriber.Subscribe(UpdateHealth).AddTo(bagBuilder);
            playerFallSubscriber.Subscribe(Fall).AddTo(bagBuilder);

            RegisterCustomMessages();
            messageDisposable = bagBuilder.Build();
        }

        protected virtual void RegisterCustomMessages() { }

        protected void UpdateHealth(ReportPlayerHealthMessage message)
        {
            if (message.PlayerID != CurrentPlayerID)
                return;

            PlayerHealth = message.Health;
            OnPlayerHealthUpdate();
        }

        public abstract void OnUpdate(float deltaTime);
        public abstract void OnEndGame(EndResult endResult);
        public abstract void OnLevelEnded(EndResult endResult);
        public abstract void OnPlayerFall();
        public abstract void OnPlayerHealthUpdate();

        public void EndLevel(LevelEndMessage message)
        {
            if (CurrentPlayerID != message.ID)
                return;

            gameStateController.EndLevel(message.Result);
            OnLevelEnded(message.Result);
        }

        public void EndGame(EndGameMessage message)
        {
            if (CurrentPlayerID != message.ID)
                return;

            gameStateController.EndGame(message.Result);
            OnEndGame(message.Result);
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
            UpdateHandlerManager.Instance.RemoveUpdateBehaviour(this);
        }
    }
}
