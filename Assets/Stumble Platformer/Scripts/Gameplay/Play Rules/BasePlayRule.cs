using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.GameManagers;
using GlobalScripts.UpdateHandlerPattern;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public abstract class BasePlayRule : NetworkBehaviour, IPlayRule, IUpdateHandler, ISetPlayerHandler, ISetCameraHandler, ISetEnvironmentHandler
    {
        [SerializeField] protected string objectiveTitle;

        protected PlayerHandler playerHandler;
        protected CameraHandler cameraHandler;
        protected EnvironmentHandler environmentHandler;
        protected GameStateController gameStateController;

        protected DisposableBagBuilder bagBuilder;
        protected IDisposable messageDisposable;

        protected ISubscriber<EndGameMessage> endGameSubscriber;
        protected ISubscriber<LevelEndMessage> levelEndSubscriber;
        protected ISubscriber<ReportPlayerHealthMessage> playerHealthSubscriber;
        protected ISubscriber<PlayerDamageMessage> playerDamageSubscriber;

        public bool IsActive { get; set; }
        public bool IsEndGame { get; set; }
        public int PlayerHealth { get; set; }
        public string ObjectiveTitle => objectiveTitle;

        public override void OnNetworkSpawn()
        {
            OnStart();
            RegisterCommonMessage();

            UpdateHandlerManager.Instance.AddUpdateBehaviour(this);
            if (TryGetComponent(out NetworkObject networkObject))
            {
                if (NetworkManager.Singleton.IsServer && !networkObject.IsSpawned)
                    networkObject.Spawn(true);
            }
        }

        protected virtual void OnStart()
        {
            IsEndGame = false;
        }

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
            OnPlayerDamage();
        }

        protected void UpdateHealth(ReportPlayerHealthMessage message)
        {
            PlayerHealth = message.Health;
            OnPlayerHealthUpdate();
        }

        public abstract void StartGame();
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
            IsEndGame = true;
            EndLevelRpc((int)message.Result, message.ClientID);
        }

        public void EndGame(EndGameMessage message)
        {
            gameStateController.EndGame(message.Result);
            OnEndGame(message.Result);
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void EndLevelRpc(int endResult, ulong clientId)
        {
            environmentHandler.SetLevelActive(false);
            environmentHandler.SetLevelSecondaryComponentActive(false);

            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
            {
                EndResult result = (EndResult)endResult;
                gameStateController.EndLevel(result);
                OnLevelEnded(result);
            }

            else
            {
                ulong currentClientId = NetworkManager.LocalClient.ClientId;
                EndResult networkResult = clientId == currentClientId 
                                          ? EndResult.Win : EndResult.Lose;
                gameStateController.EndLevel(networkResult);
                OnLevelEnded(networkResult);
            }
        }

        public override void OnDestroy()
        {
            messageDisposable.Dispose();
            UpdateHandlerManager.Instance.RemoveUpdateBehaviour(this);
            base.OnDestroy();
        }
    }
}
