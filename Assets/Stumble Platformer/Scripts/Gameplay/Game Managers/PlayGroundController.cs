using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Common.Constants;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;
using StumblePlatformer.Scripts.Gameplay.PlayRules;
using Cysharp.Threading.Tasks;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class PlayGroundController : MonoBehaviour
    {
        [SerializeField] private CameraHandler cameraHandler;
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private EnvironmentSetup environmentSetup;
        [SerializeField] private PlayDataCollectionInitializer playDataCollectionInitializer;
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
            }
        }

        private void Start()
        {
            InitLevel();
            SetupGameplay();
        }

        private void SetupGameplay()
        {
            var builder = Disposable.CreateBuilder();
            _gameStateController = new();
            _gameStateController.AddTo(ref builder);
            builder.RegisterTo(this.destroyCancellationToken);
        }

        private void InitLevel()
        {
            var builder = MessagePipe.DisposableBag.CreateBuilder();
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
            // Spawn player prefab, then update its appearance via skin menu
            Vector3 playerPosition = EnvironmentIdentifier.SpawnCharacterArea.MainCharacterSpawnPosition;
            _currentPlayer = Instantiate(playerPrefab, playerPosition, Quaternion.identity);

            CharacterSkin characterSkin; // Get a temp skin
            bool hasSkin = playDataCollectionInitializer.CharacterVisualDatabase.TryGetCharacterSkin("21", out characterSkin);
            
            if (hasSkin)
                _currentPlayer.PlayerGraphics.SetCharacterVisual(characterSkin);

            _currentPlayer.PlayerHealth.SetHealth(CharacterConstants.MaxLife);
            SetPlayerActive(false);
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

            SetupEnvironment();
            await GameplayManager.Instance.InitGameplay();
            _playeRule.CurrentPlayerID = _currentPlayer.PlayerID;

            await WaitForTeaser();
            SetPlayerActive(true);
        }

        public void SetPlayerActive(bool active)
        {
            _currentPlayer.IsActive = active;
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
            SetPlayerActive(true);
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
