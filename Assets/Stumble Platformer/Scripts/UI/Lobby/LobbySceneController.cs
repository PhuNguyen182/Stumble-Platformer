using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalScripts.SceneUtils;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Multiplayers;
using StumblePlatformer.Scripts.UI.Lobby.Popups;
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

        private const string JoinRoomPopupPath = "Popups/Lobby/Join Room Popup.prefab";
        private const string CreateRoomPopupPath = "Popups/Lobby/Create Room Popup.prefab";

        private void Awake()
        {
            PreloadPopups();
            RegisterButtons();
        }

        private void Start()
        {
            WaitingPopup.Setup().HideWaiting();
        }

        private void PreloadPopups()
        {
            JoinRoomPopup.PreloadFromAddress(JoinRoomPopupPath).Forget();
            CreateRoomPopup.PreloadFromAddress(CreateRoomPopupPath).Forget();
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
            CreateRoomPopup.CreateFromAddress(CreateRoomPopupPath).Forget();
        }

        private void OnJoinPublicRoom()
        {
            JoinPublicRoomAsync().Forget();
        }

        private void OnJoinPrivateRoom()
        {
            JoinRoomPopup.CreateFromAddress(JoinRoomPopupPath).Forget();
        }

        private async UniTask JoinPublicRoomAsync()
        {
            MessagePopup.Setup().ShowWaiting().SetMessage("Joining Room").ShowCloseButton(false);
            bool canJoin = await LobbyManager.Instance.JoinLobby();

            if (canJoin)
            {
                MessagePopup.Setup().HideWaiting();
                GameplaySetup.PlayerType = PlayerType.Client;
                SceneLoader.LoadNetworkScene(SceneConstants.Waiting);
            }
            else
            {
                MessagePopup.Setup().ShowWaiting().SetMessage("Cannot find open room!").ShowCloseButton(true);
            }
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
