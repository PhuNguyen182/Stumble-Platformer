using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Gameplay.PlayRules;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using Cysharp.Threading.Tasks;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class PlayGroundManager : MonoBehaviour
    {
        [SerializeField] private bool isTesting;
        
        [Space(10)]
        [SerializeField] private InputReceiver inputReceiver;
        [SerializeField] private CameraHandler cameraHandler;
        [SerializeField] private PlayerHandler playerHandler;
        [SerializeField] private EnvironmentHandler environmentHandler;

        private GameStateController _gameStateController;
        
        private IPlayRule _playeRule;
        private IDisposable _initLevelDisposable;

        private ISubscriber<RespawnMessage> _respawnSubscriber;
        private ISubscriber<SetupLevelMessage> _initLevelSubscriber;

        private void Start()
        {
            SetupGameplay();
            InitializeMessage();
            GenerateLevel().Forget();
        }

        private void SetupGameplay()
        {
            _gameStateController = new();
            var builder = Disposable.CreateBuilder();
            _gameStateController.AddTo(ref builder);
            builder.RegisterTo(this.destroyCancellationToken);
        }

        private void InitializeMessage()
        {
            _respawnSubscriber = GlobalMessagePipe.GetSubscriber<RespawnMessage>();
            _initLevelSubscriber = GlobalMessagePipe.GetSubscriber<SetupLevelMessage>();

            var builder = MessagePipe.DisposableBag.CreateBuilder();
            _initLevelSubscriber.Subscribe(SetupLevel).AddTo(builder);
            _respawnSubscriber.Subscribe(RespawnPlayer).AddTo(builder);
            _initLevelDisposable = builder.Build();
        }

        private async UniTask GenerateLevel()
        {
            if (isTesting)
                return;

            if (PlayGameConfig.Current != null)
            {
                string levelName = PlayGameConfig.Current.PlayLevelName;
                await environmentHandler.GenerateLevel(levelName);
            }
        }

        private void SetupLevel(SetupLevelMessage message)
        {
            SetupPlayLevel(message.EnvironmentIdentifier).Forget();
        }

        private async UniTask SetupPlayLevel(EnvironmentIdentifier environmentIdentifier)
        {
            inputReceiver.IsActive = false;
            environmentHandler.SetEnvironmentIdentifier(environmentIdentifier);
            await StartGame();
        }

        private async UniTask StartGame()
        {
            await environmentHandler.WaitForTeaser();

            _playeRule = environmentHandler.EnvironmentIdentifier.PlayRule;
            _playeRule.SetStateController(_gameStateController);

            playerHandler.SpawnPlayer();
            cameraHandler.SetFollowTarget(playerHandler.CurrentPlayer.transform);
            _playeRule.CurrentPlayerID = playerHandler.CurrentPlayer.PlayerID;

            playerHandler.SetPlayerActive(true);
            inputReceiver.IsActive = true;
        }

        private void RespawnPlayer(RespawnMessage message)
        {
            if (playerHandler.PlayerInstanceID == message.ID)
            {
                playerHandler.RespawnPlayer();
            }
        }

        private void OnDestroy()
        {
            _initLevelDisposable.Dispose();
        }
    }
}