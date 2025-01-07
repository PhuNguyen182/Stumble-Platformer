using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StumblePlatformer.Scripts.Gameplay.PlayRules;
using StumblePlatformer.Scripts.UI.Gameplay.MainPanels;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class PlayGroundManager : NetworkBehaviour
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
        
        public BasePlayRule PlayRule { get; private set; }
        public CameraHandler CameraHandler => cameraHandler;
        public static PlayGroundManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
#endif
            SetupGameplay();
        }

        private void Start()
        {
            GenerateLevel().Forget();
        }

        private void SetupGameplay()
        {
            _gameStateController = new(cameraHandler, environmentHandler, endGamePanel, finalePanel);
            var builder = Disposable.CreateBuilder();
            _gameStateController.AddTo(ref builder);
            builder.RegisterTo(this.destroyCancellationToken);
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

        public void SetupLevel(EnvironmentIdentifier environmentIdentifier)
        {
            SetupPlayLevel(environmentIdentifier).Forget();
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

            playerHandler.SpawnPlayer();
            cameraHandler.SetFollowTarget(playerHandler.CurrentPlayer.transform);
            cameraHandler.ResetCurrentCameraFollow();

            cameraHandler.SetFollowCameraActive(true);
            await UniTask.NextFrame(destroyCancellationToken);
            cameraHandler.SetFollowCameraActive(false);
            SetupPlayRule(PlayRule);

            await environmentHandler.WaitForTeaser();
            playGamePanel?.SetLevelNameActive(false);
            playGamePanel?.SetPlayObjectActive(true);
            PlayRule.StartGame();

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

            if(playRule is RacingRule racingRule)
            {
                racingRule.PlayerHealth = playerHandler.OriginPlayerHealth;
                racingRule.SetLifeCounter(playGamePanel.LifeCounter);
            }

            if (playRule is SurvivalRule survivalRule)
            {
                playGamePanel.UpdateTimeRule(survivalRule.PlayDuration);
                survivalRule.SetPlayRuleTimer(playGamePanel.PlayRuleTimer);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
