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
    public abstract class BasePlayRule : MonoBehaviour, IPlayRule, IUpdateHandler, ISetPlayerHandler, ISetCameraHandler, ISetEnvironmentHandler
    {
        [SerializeField] protected string objectiveTitle;

        protected IDisposable messageDisposable;
        protected DisposableBagBuilder bagBuilder;
        protected PlayerHandler playerHandler;
        protected EnvironmentHandler environmentHandler;
        protected CameraHandler cameraHandler;

        protected ISubscriber<ReportPlayerHealthMessage> playerHealthSubscriber;
        protected ISubscriber<EndGameMessage> endGameSubscriber;
        protected ISubscriber<LevelEndMessage> levelEndSubscriber;
        protected ISubscriber<PlayerDamageMessage> playerDamageSubscriber;
        protected GameStateController gameStateController;

        public int CurrentPlayerID { get; set; }
        public int PlayerHealth { get; set; }
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
            playerDamageSubscriber = GlobalMessagePipe.GetSubscriber<PlayerDamageMessage>();

            levelEndSubscriber.Subscribe(EndLevel).AddTo(bagBuilder);
            playerHealthSubscriber.Subscribe(UpdateHealth).AddTo(bagBuilder);
            playerDamageSubscriber.Subscribe(DamagePlayer).AddTo(bagBuilder);

            RegisterCustomMessages();
            messageDisposable = bagBuilder.Build();
        }

        protected virtual void RegisterCustomMessages() { }

        public void DamagePlayer(PlayerDamageMessage message)
        {
            if (CurrentPlayerID != message.ID)
                return;

            OnPlayerDamage();
        }

        protected void UpdateHealth(ReportPlayerHealthMessage message)
        {
            if (message.PlayerID != CurrentPlayerID)
                return;

            PlayerHealth = message.Health;
            OnPlayerHealthUpdate();
        }

        public abstract void OnEndGame(EndResult endResult);
        public abstract void OnLevelEnded(EndResult endResult);
        public abstract void OnPlayerDamage();

        public virtual void OnPlayerHealthUpdate() { }
        public virtual void OnUpdate(float deltaTime) { }

        #region Handler Setters
        public void SetPlayerHandler(PlayerHandler playerHandler)
        {
            this.playerHandler = playerHandler;
        }

        public void SetEnvironmentHandler(EnvironmentHandler environmentHandler)
        {
            this.environmentHandler = environmentHandler;
        }

        public void SetCameraHandler(CameraHandler cameraHandler)
        {
            this.cameraHandler = cameraHandler;
        }

        public void SetStateController(GameStateController gameStateController)
        {
            this.gameStateController = gameStateController;
        }
        #endregion

        public void EndLevel(LevelEndMessage message)
        {
            if (CurrentPlayerID != message.ID)
                return;

            environmentHandler.SetLevelActive(false);
            environmentHandler.SetLevelSecondaryComponentActive(false);
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

        private void OnDestroy()
        {
            messageDisposable.Dispose();
            UpdateHandlerManager.Instance.RemoveUpdateBehaviour(this);
        }
    }
}
