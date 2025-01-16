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
using Cysharp.Threading.Tasks;
using GlobalScripts.SceneUtils;
using GlobalScripts;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class PlayGroundManager : NetworkBehaviour
    {
        [SerializeField] private bool isTesting;
        [SerializeField] private NetworkObject testPlayer;
        
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
        public static PlayGroundManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
#endif
            SetupGameplay();
            GetLevelEntry();
        }

        private void SetupGameplay()
        {
            _gameStateController = new(cameraHandler, environmentHandler, endGamePanel, finalePanel);
            var builder = Disposable.CreateBuilder();
            _gameStateController.AddTo(ref builder);
            builder.RegisterTo(this.destroyCancellationToken);
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
                NetworkManager.SceneManager.OnSceneEvent += HandleOnSceneEvent;
        }

        private void HandleOnSceneEvent(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadComplete:
                    {
                        DebugUtils.Log("Load Completed");
                        break;
                    }
                case SceneEventType.LoadEventCompleted:
                    {
                        if (string.CompareOrdinal(sceneEvent.SceneName, SceneConstants.Gameplay) == 0)
                            NetworkManager.Singleton.SceneManager.LoadScene(_levelName, LoadSceneMode.Additive);
                        
                        DebugUtils.Log("Load Event Completed" + sceneEvent.ClientId);
                        break;
                    }
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

            WaitingPopup.Setup().HideWaiting();
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

        private void SpawnPlayer()
        {
            playerHandler.SpawnPlayer();
            cameraHandler.SetFollowTarget(playerHandler.CurrentPlayer.transform);
            cameraHandler.ResetCurrentCameraFollow();

            PlayRule.CurrentPlayerID = playerHandler.CurrentPlayer.PlayerID;
            PlayRule.IsActive = true;

            playerHandler.SetPlayerActive(true);
            playerHandler.SetPlayerPhysicsActive(true);
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

        private void ShutdownNetwork()
        {
            if (NetworkManager == null)
                return;

            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
                NetworkManager?.Shutdown();

            else if (GameplaySetup.PlayMode == GameMode.Multiplayer)
                MultiplayerManager.Instance.Shutdown();
        }

        public override void OnDestroy()
        {
            ShutdownNetwork();
            base.OnDestroy();
        }
    }
}
