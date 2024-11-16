using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Common.Constants;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Gameplay.PlayRules;
using Cysharp.Threading.Tasks;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    public class PlayGroundController : MonoBehaviour
    {
        [SerializeField] private CameraHandler cameraHandler;
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private EnvironmentSetup environmentSetup;
        [SerializeField] private bool isTesting;

        private PlayerController _currentPlayer;
        private GameStateController _gameStateController;
        
        private IPlayRule _playeRule;
        private IDisposable _initLevelDisposable;

        private ISubscriber<RespawnMessage> _respawnSubscriber;
        private ISubscriber<InitializeLevelMessage> _initLevelSubscriber;

        public PlayerController CurrentPlayer => _currentPlayer;
        public EnvironmentIdentifier EnvironmentIdentifier { get; private set; }

        private void Awake()
        {
            if (isTesting)
            {
                _currentPlayer = playerPrefab;
                _currentPlayer.PlayerHealth.SetHealth(CharacterConstants.MaxLife);
            }
        }

        private void Start()
        {
            InitLevel();
            SetupGameplay();
        }

        private void SetupGameplay()
        {
            _gameStateController = new();
        }

        private void InitLevel()
        {
            var builder = DisposableBag.CreateBuilder();
            _respawnSubscriber = GlobalMessagePipe.GetSubscriber<RespawnMessage>();
            _initLevelSubscriber = GlobalMessagePipe.GetSubscriber<InitializeLevelMessage>();

            _initLevelSubscriber.Subscribe(message => SetEnvironmentIdentifier(message.EnvironmentIdentifier).Forget())
                                .AddTo(builder);
            _respawnSubscriber.Subscribe(message => RespawnPlayer(message.ID))
                              .AddTo(builder);
            _initLevelDisposable = builder.Build();
        }

        public void SpawnPlayer()
        {
            // Spawn player here, spawn player and disable it, then play teaser line for camera, then activate player and play
        }

        public async UniTask GenerateLevel()
        {
            if (PlayGameConfig.Current != null)
            {
                string levelName = PlayGameConfig.Current.PlayLevelName;
                await environmentSetup.GenerateLevel(levelName);
            }
        }

        public async UniTask WaitForTeaser()
        {
            cameraHandler.SetTeaserCameraActive(true);
            EnvironmentIdentifier.SetTeaserActive(true);
            await EnvironmentIdentifier.WaitForEndOfTeaser();
            cameraHandler.SetTeaserCameraActive(false);
        }

        public async UniTask SetEnvironmentIdentifier(EnvironmentIdentifier environmentIdentifier)
        {
            EnvironmentIdentifier = environmentIdentifier;
            _playeRule = EnvironmentIdentifier.PlayRule;
            _playeRule.SetStateController(_gameStateController);
            _playeRule.CurrentPlayerID = _currentPlayer.PlayerID;

            SetupEnvironment();
            await WaitForTeaser();
        }

        public void RespawnPlayer(int respawnId)
        {
            if (_currentPlayer.gameObject.GetInstanceID() != respawnId)
                return;

            int checkPointIndex = _currentPlayer.GetCheckPointIndex();
            RespawnArea currentCheckPoint = EnvironmentIdentifier.PlayLevel
                                            .GetCheckPointByIndex(checkPointIndex);
            
            Vector3 respawnPosition = currentCheckPoint.GetRandomSpawnPosition();
            _currentPlayer.transform.position = respawnPosition;

            _currentPlayer.SetCharacterActive(true);
            _currentPlayer.IsActive = true;
        }

        private void SetupEnvironment()
        {
            environmentSetup.SetupSky(EnvironmentIdentifier.Skybox);
            cameraHandler.SetupTeaserCamera(EnvironmentIdentifier.TeaserFollower.transform, EnvironmentIdentifier.TeaserPath);
        }

        private void OnDestroy()
        {
            _initLevelDisposable.Dispose();
        }
    }
}
