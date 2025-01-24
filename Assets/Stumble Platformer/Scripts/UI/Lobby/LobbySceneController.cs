using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalScripts.SceneUtils;
using StumblePlatformer.Scripts.Gameplay;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Common.Constants;
using StumblePlatformer.Scripts.UI.Lobby.Popups;
using StumblePlatformer.Scripts.Multiplayers;
using Cysharp.Threading.Tasks;
using StumblePlatformer.Scripts.GameManagers;

namespace StumblePlatformer.Scripts.UI.Lobby
{
    public class LobbySceneController : MonoBehaviour
    {
        [SerializeField] private Button backHomeButton;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button joinPublicRoomButton;
        [SerializeField] private Button joinPrivateRoomButton;

        public static LobbySceneController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            PreloadPopups();
            RegisterButtons();
        }

        private void Start()
        {
            if (GameplaySetup.PlayMode == GameMode.Multiplayer)
                WaitingPopup.Setup().HideWaiting();
        }

        private void PreloadPopups()
        {
            JoinRoomPopup.PreloadFromAddress(CommonPopupPaths.JoinRoomPopupPath).Forget();
            CreateRoomPopup.PreloadFromAddress(CommonPopupPaths.CreateRoomPopupPath).Forget();
        }

        private void RegisterButtons()
        {
            backHomeButton.onClick.AddListener(BackToMainHome);
            createRoomButton.onClick.AddListener(() => OnOpenCreateRoom().Forget());
            joinPublicRoomButton.onClick.AddListener(() => OnJoinPublicRoom().Forget());
            joinPrivateRoomButton.onClick.AddListener(() => OnJoinPrivateRoom().Forget());
        }

        private void BackToMainHome()
        {
            WaitingPopup.Setup().ShowWaiting();
            SceneLoader.LoadScene(SceneConstants.Mainhome).Forget();
        }

        private async UniTask OnOpenCreateRoom()
        {
            bool isConnected = await GameManager.Instance.ConectionHandler.CheckConnection();
            if (isConnected)
                await CreateRoomPopup.CreateFromAddress(CommonPopupPaths.CreateRoomPopupPath);
        }

        private async UniTask OnJoinPublicRoom()
        {
            bool isConnected = await GameManager.Instance.ConectionHandler.CheckConnection();
            if (isConnected)
                await JoinPublicRoomAsync();
        }

        private async UniTask OnJoinPrivateRoom()
        {
            bool isConnected = await GameManager.Instance.ConectionHandler.CheckConnection();
            if (isConnected)
                await JoinRoomPopup.CreateFromAddress(CommonPopupPaths.JoinRoomPopupPath);
        }

        private async UniTask JoinPublicRoomAsync()
        {
            MessagePopup.Setup().ShowWaiting().SetMessage("Joining Room").ShowCloseButton(false);
            bool canJoin = await LobbyManager.Instance.JoinLobby();

            if (canJoin)
            {
                MessagePopup.Setup().HideWaiting();
                WaitingPopup.Setup().ShowWaiting();
                // This is automatically enter the Waiting Scene if server has entered the Waiting Scene
            }
            else
            {
                MessagePopup.Setup().ShowWaiting().SetMessage("Cannot find open room!").ShowCloseButton(true);
            }
        }

        public void LoadWaitingScene()
        {
            SceneLoader.LoadNetworkScene(SceneConstants.Waiting);
        }

        private void ReleasePopups()
        {
            JoinRoomPopup.Release();
            CreateRoomPopup.Release();
        }

        private void OnDestroy()
        {
            ReleasePopups();
        }
    }
}
