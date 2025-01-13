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
        public virtual bool IsActive { get; set; }
        public string ObjectiveTitle => objectiveTitle;

        private void Start()
        {
            OnStart();
            RegisterCommonMessage();
            UpdateHandlerManager.Instance.AddUpdateBehaviour(this);

            if(TryGetComponent<NetworkObject>(out var networkObject))
            {
                networkObject.Spawn();
            }
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
            EndLevelServerRpc(message.ID, (int)message.Result, message.ClientID);
        }

        public void EndGame(EndGameMessage message)
        {
            if (CurrentPlayerID != message.ID)
                return;

            gameStateController.EndGame(message.Result);
            OnEndGame(message.Result);
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void EndLevelServerRpc(int playerId, int endResult, ulong clientId)
        {
            EndLevelClientRpc(playerId, endResult, clientId);
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void EndLevelClientRpc(int playerId, int endResult, ulong clientId)
        {
            if (CurrentPlayerID != playerId)
                return;

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
                ulong currentClientId = NetworkManager.Singleton.LocalClient.ClientId;
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
        }
    }
}
