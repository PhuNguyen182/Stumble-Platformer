using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalScripts.SceneUtils;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Multiplayers;
using StumblePlatformer.Scripts.UI.Lobby.Popups;
using StumblePlatformer.Scripts.Common.Constants;
using StumblePlatformer.Scripts.Gameplay;
using Cysharp.Threading.Tasks;

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
            createRoomButton.onClick.AddListener(OnOpenCreateRoom);
            joinPublicRoomButton.onClick.AddListener(OnJoinPublicRoom);
            joinPrivateRoomButton.onClick.AddListener(OnJoinPrivateRoom);
        }

        private void BackToMainHome()
        {
            WaitingPopup.Setup().ShowWaiting();
            SceneLoader.LoadScene(SceneConstants.Mainhome).Forget();
        }

        private void OnOpenCreateRoom()
        {
            CreateRoomPopup.CreateFromAddress(CommonPopupPaths.CreateRoomPopupPath).Forget();
        }

        private void OnJoinPublicRoom()
        {
            JoinPublicRoomAsync().Forget();
        }

        private void OnJoinPrivateRoom()
        {
            JoinRoomPopup.CreateFromAddress(CommonPopupPaths.JoinRoomPopupPath).Forget();
        }

        private async UniTask JoinPublicRoomAsync()
        {
            MessagePopup.Setup().ShowWaiting().SetMessage("Joining Room").ShowCloseButton(false);
            bool canJoin = await LobbyManager.Instance.JoinLobby();

            if (canJoin)
            {
                MessagePopup.Setup().HideWaiting();
                GameplaySetup.PlayerType = PlayerType.Client;
                WaitingPopup.Setup().ShowWaiting();
                LoadWaitingScene();
            }
            else
            {
                MessagePopup.Setup().ShowWaiting().SetMessage("Cannot find open room!").ShowCloseButton(true);
            }
        }

        public void LoadWaitingScene()
        {
            SceneLoader.LoadScene(SceneConstants.Waiting).Forget();
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
