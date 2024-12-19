using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Gameplay.PlayRules;
using StumblePlatformer.Scripts.Gameplay.Inputs;
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
        
        private IDisposable _initLevelDisposable;
        private ISubscriber<SetupLevelMessage> _initLevelSubscriber;

        public BasePlayRule PlayRule { get; private set; }

        private void Awake()
        {
#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
#endif
        }

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
            _initLevelSubscriber = GlobalMessagePipe.GetSubscriber<SetupLevelMessage>();

            var builder = MessagePipe.DisposableBag.CreateBuilder();
            _initLevelSubscriber.Subscribe(SetupLevel).AddTo(builder);
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
            cameraHandler.SetupVirtualCameraBody(environmentIdentifier);
            environmentHandler.SetEnvironmentIdentifier(environmentIdentifier);
            await StartGame();
        }

        private async UniTask StartGame()
        {
            await environmentHandler.WaitForTeaser();

            PlayRule = environmentHandler.EnvironmentIdentifier.PlayRule;
            PlayRule.SetStateController(_gameStateController);
            SetupPlayRule(PlayRule);

            playerHandler.SpawnPlayer();
            cameraHandler.SetFollowCameraActive(true);
            cameraHandler.SetFollowTarget(playerHandler.CurrentPlayer.transform);
            PlayRule.CurrentPlayerID = playerHandler.CurrentPlayer.PlayerID;

            PlayRule.IsActive = true;
            playerHandler.SetPlayerActive(true);
            playerHandler.SetPlayerPhysicsActive(true);
            inputReceiver.IsActive = true;
        }

        private void SetupPlayRule(BasePlayRule playRule)
        {
            if (playRule is ISetPlayerHandler playerHandlerSetter)
                playerHandlerSetter.SetPlayerHandler(playerHandler);

            if (playRule is ISetCameraHandler cameraHandlerSetter)
                cameraHandlerSetter.SetCameraHandler(cameraHandler);

            if (playRule is ISetEnvironmentHandler environmentHandlerSetter)
                environmentHandlerSetter.SetEnvironmentHandler(environmentHandler);
        }

        private void OnDestroy()
        {
            _initLevelDisposable.Dispose();
        }
    }
}
