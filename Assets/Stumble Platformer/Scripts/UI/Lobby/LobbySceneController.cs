using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.UI.Lobby.Popups;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.UI.Lobby
{
    public class LobbySceneController : MonoBehaviour
    {
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
            createRoomButton.onClick.AddListener(OnOpenCreateRoom);
            joinPublicRoomButton.onClick.AddListener(OnJoinPublicRoom);
            joinPrivateRoomButton.onClick.AddListener(OnJoinPrivateRoom);
        }

        private void OnOpenCreateRoom()
        {
            CreateRoomPopup.CreateFromAddress(CreateRoomPopupPath).Forget();
        }

        private void OnJoinPublicRoom()
        {

        }

        private void OnJoinPrivateRoom()
        {
            JoinRoomPopup.CreateFromAddress(JoinRoomPopupPath).Forget();
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
