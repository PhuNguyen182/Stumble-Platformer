using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using StumblePlatformer.Scripts.Multiplayers;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Gameplay.PlayRules;
using StumblePlatformer.Scripts.UI.Gameplay.MainPanels;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using GlobalScripts.SceneUtils;
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

        private string _levelName = "";
        private GameStateController _gameStateController;
        
        public BasePlayRule PlayRule { get; private set; }
        public CameraHandler CameraHandler => cameraHandler;
        public ulong ServerClientID { get; private set; }
        public static PlayGroundManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            SetupGameStateMachine();
        }

        private void SetupGameStateMachine()
        {
            DisposableBuilder builder = Disposable.CreateBuilder();
            _gameStateController = new(cameraHandler, environmentHandler, endGamePanel, finalePanel);

            _gameStateController.AddTo(ref builder);
            builder.RegisterTo(destroyCancellationToken);
        }

        private void GetLevelEntry()
        {
            if (isTesting)
                return;

            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
            {
                if (PlayGameConfig.Current != null)
                    _levelName = PlayGameConfig.Current.PlayLevelName;
            }

            else if(GameplaySetup.PlayMode == GameMode.Multiplayer)
                _levelName = MultiplayerManager.Instance.CarrierCollection.PlayEntryCarrier.NetworkData.Value.PlayLevelName.Value;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += HandleSceneLoad;
        }

        private void HandleSceneLoad(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            GetLevelEntry();

            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
            {
                if (string.CompareOrdinal(sceneName, SceneConstants.Gameplay) == 0)
                    environmentHandler.GenerateLevel(_levelName);
            }

            else
            {
                if (string.CompareOrdinal(sceneName, SceneConstants.Gameplay) == 0)
                {
                    environmentHandler.GenerateLevel(_levelName);
                    MultiplayerManager.Instance.SetupConnectedIDsRpc();
                }
            }
        }

        public void SetupLevel(EnvironmentIdentifier environmentIdentifier) => SetupPlayLevel(environmentIdentifier).Forget();

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

            WaitingPopup.Setup().HideWaiting();
            await environmentHandler.WaitForTeaser();
            playGamePanel?.SetLevelNameActive(false);
            playGamePanel?.SetPlayObjectActive(true);
            PlayRule.StartGame();

            await playGamePanel.CountDown();
            cameraHandler.SetFollowCameraActive(true);
            PlayRule.IsActive = true;

            playerHandler.SetPlayerActive(true);
            playerHandler.SetPlayerPhysicsActive(true);
            playerHandler.ActivateAllPlayer();

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
    }
}
