using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.PlayRules;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using Cysharp.Threading.Tasks;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    public class PlayGroundController : MonoBehaviour
    {
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private EnvironmentSetup environmentSetup;
        [SerializeField] private bool isTesting;

        private PlayerController _currentPlayer;
        private GameStateController _gameStateController;
        
        private IPlayeRule _playeRule;
        private ISubscriber<InitializeLevelMessage> _initLevelSubscriber;
        private IDisposable _initLevelDisposable;

        public PlayerController CurrentPlayer => _currentPlayer;
        public EnvironmentIdentifier EnvironmentIdentifier { get; private set; }

        private void Awake()
        {
            if (isTesting)
                _currentPlayer = playerPrefab;
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
            _initLevelSubscriber = GlobalMessagePipe.GetSubscriber<InitializeLevelMessage>();
            _initLevelSubscriber.Subscribe(message => SetEnvironmentIdentifier(message.EnvironmentIdentifier))
                                .AddTo(builder);
            _initLevelDisposable = builder.Build();
        }

        public void SpawnPlayer()
        {
            // Spawn player here, spawn player and disable it, then play teaser line for camera, then activate player and play
        }

        public async UniTask GenerateLevelAsync()
        {
            if (PlayGameConfig.Current != null)
            {
                string levelName = PlayGameConfig.Current.PlayLevelName;
                await environmentSetup.GenerateLevel(levelName);
            }
        }

        public void SetEnvironmentIdentifier(EnvironmentIdentifier environmentIdentifier)
        {
            EnvironmentIdentifier = environmentIdentifier;
            _playeRule = EnvironmentIdentifier.PlayRule;
            SetupEnvironment();
        }

        private void SetupEnvironment()
        {
            environmentSetup.SetupSky(EnvironmentIdentifier.Skybox);
        }

        private void OnDestroy()
        {
            _initLevelDisposable.Dispose();
        }
    }
}
