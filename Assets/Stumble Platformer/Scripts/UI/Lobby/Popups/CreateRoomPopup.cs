using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.Gameplay;
using StumblePlatformer.Scripts.Multiplayers;
using StumblePlatformer.Scripts.Common.Enums;
using GlobalScripts.SceneUtils;
using Cysharp.Threading.Tasks;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Lobby.Popups
{
    public class CreateRoomPopup : BasePopup<CreateRoomPopup>
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button createPublicRoomButton;
        [SerializeField] private Button createPrivateRoomButton;
        [SerializeField] private TMP_InputField playerCountField;
        [SerializeField] private TMP_InputField roomName;

        private const int MinPlayerCount = 1;
        private const int MaxPlayerCount = 7;
        private const string ClosePopupTrigger = "Close";
        private readonly int _closePopupHash = Animator.StringToHash(ClosePopupTrigger);

        private int _playerCount = 0;

        protected override void OnAwake()
        {
            base.OnAwake();
            RegisterButtons();
        }

        protected override void DoAppear()
        {
            base.DoAppear();
            UpdatePlayerCount(playerCountField.text);
        }

        private void RegisterButtons()
        {
            closeButton.onClick.AddListener(Close);
            createPublicRoomButton.onClick.AddListener(CreatePublicRoom);
            createPrivateRoomButton.onClick.AddListener(CreatePrivateRoom);
            playerCountField.onValueChanged.AddListener(UpdatePlayerCount);
            roomName.text = $"PlayRoom{Random.Range(100000, 1000000)}";
        }

        private void UpdatePlayerCount(string value)
        {
            if (string.IsNullOrEmpty(value))
                _playerCount = int.Parse($"{MinPlayerCount}");
            
            else
            {
                _playerCount = int.Parse(value);
                _playerCount = Mathf.Clamp(_playerCount, MinPlayerCount, MaxPlayerCount);
            }

            playerCountField.text = $"{_playerCount}";
        }

        private void CreatePublicRoom()
        {
            GameplaySetup.PlayerType = PlayerType.Host;
            MultiplayerManager.Instance.PlayerAmount = _playerCount;
            CreateRoomAsync(false).Forget();
        }

        private void CreatePrivateRoom()
        {
            GameplaySetup.PlayerType = PlayerType.Host;
            MultiplayerManager.Instance.PlayerAmount = _playerCount;
            CreateRoomAsync(true).Forget();
        }

        private async UniTask CreateRoomAsync(bool isPrivate)
        {
            MessagePopup.Setup().ShowWaiting().SetMessage("Creating Room").ShowCloseButton(false);
            bool canCreateRoom = await LobbyManager.Instance.CreateLobby(roomName.text, isPrivate);

            if (canCreateRoom)
            {
                MessagePopup.Setup().HideWaiting();
                await SceneLoader.LoadScene(SceneConstants.Waiting);
            }

            else
            {
                MessagePopup.Setup().ShowWaiting()
                            .SetMessage("Cannot create play room now!")
                            .ShowCloseButton(true);
            }
        }

        protected override void DoClose()
        {
            CloseAsync().Forget();
        }

        private async UniTask CloseAsync()
        {
            if (PopupAnimator)
            {
                PopupAnimator.SetTrigger(_closePopupHash);
                await UniTask.WaitForSeconds(0.167f, cancellationToken: destroyCancellationToken);
            }
            base.DoClose();
        }
    }
}
