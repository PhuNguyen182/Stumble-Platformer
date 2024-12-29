using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using StumblePlatformer.Scripts.Gameplay.PlayRules;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.UI.Gameplay.MainPanels;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using MessagePipe;
using GlobalScripts.SceneUtils;

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

        [Header("UIs")]
        [SerializeField] private PlayGamePanel playGamePanel;
        [SerializeField] private EndGamePanel endGamePanel;
        [SerializeField] private FinalePanel finalePanel;

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
            _gameStateController = new(cameraHandler, environmentHandler, endGamePanel, finalePanel);
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
                WaitingPopup.Setup().HideWaiting();
            }
        }

        private void SetupLevel(SetupLevelMessage message)
        {
            SetupPlayLevel(message.EnvironmentIdentifier).Forget();
        }

        private async UniTask SetupPlayLevel(EnvironmentIdentifier environmentIdentifier)
        {
            inputReceiver.IsActive = false;
            playGamePanel?.ResetCountdown();
            cameraHandler.SetupVirtualCameraBody(environmentIdentifier);
            environmentHandler.SetEnvironmentIdentifier(environmentIdentifier);
            await StartGame();
        }

        private async UniTask StartGame()
        {
            PlayRule = environmentHandler.EnvironmentIdentifier.PlayRule;
            PlayRule.SetStateController(_gameStateController);

            playGamePanel?.SetLevelNameActive(true);
            playGamePanel?.SetPlayObjectActive(false);
            playGamePanel?.SetLevelObjective(PlayRule.ObjectiveTitle);
            playGamePanel?.SetLevelName(environmentHandler.EnvironmentIdentifier.LevelName);
            SetupPlayRule(PlayRule);

            await environmentHandler.WaitForTeaser();
            playGamePanel?.SetLevelNameActive(false);
            playGamePanel?.SetPlayObjectActive(true);
            playerHandler.SpawnPlayer();
            cameraHandler.SetFollowTarget(playerHandler.CurrentPlayer.transform);
            PlayRule.StartGame();

            cameraHandler.ResetCurrentCameraFollow();
            cameraHandler.SetFollowCameraActive(true);
            await UniTask.NextFrame(destroyCancellationToken);
            cameraHandler.SetFollowCameraActive(false);

            if (playGamePanel)
                await playGamePanel.CountDown();
            
            cameraHandler.SetFollowCameraActive(true);
            PlayRule.CurrentPlayerID = playerHandler.CurrentPlayer.PlayerID;
            PlayRule.IsActive = true;

            playerHandler.SetPlayerActive(true);
            playerHandler.SetPlayerPhysicsActive(true);
            environmentHandler.SetLevelSecondaryComponentActive(true);
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

            if (playRule is SurvivalRule survivalRule)
            {
                playGamePanel.UpdateTimeRule(survivalRule.PlayDuration);
                survivalRule.SetPlayRuleTimer(playGamePanel.PlayRuleTimer);
            }
        }

        private void OnDestroy()
        {
            _initLevelDisposable.Dispose();
        }
    }
}
