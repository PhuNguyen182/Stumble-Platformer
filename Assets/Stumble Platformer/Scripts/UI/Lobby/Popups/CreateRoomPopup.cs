using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.Multiplayers;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Multiplayers.Datas;
using StumblePlatformer.Scripts.Gameplay;
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
            roomName.text = $"PlayRoom{Random.Range(100000, 1000000)}";
        }

        private void RegisterButtons()
        {
            closeButton.onClick.AddListener(Close);
            createPublicRoomButton.onClick.AddListener(CreatePublicRoom);
            createPrivateRoomButton.onClick.AddListener(CreatePrivateRoom);
            playerCountField.onValueChanged.AddListener(UpdatePlayerCount);
        }

        private void UpdatePlayerCount(string value)
        {
            if (string.IsNullOrEmpty(value))
                _playerCount = int.Parse($"{MultiplayerConstants.MinPlayerCount}");
            
            else
            {
                _playerCount = int.Parse(value);
                _playerCount = Mathf.Clamp(_playerCount, MultiplayerConstants.MinPlayerCount, MultiplayerConstants.MaxPlayerCount);
            }

            playerCountField.text = $"{_playerCount}";
        }

        private void CreatePublicRoom()
        {
            Close();
            GameplaySetup.PlayerType = PlayerType.Host;
            CreateRoomAsync(false).Forget();
        }

        private void CreatePrivateRoom()
        {
            Close();
            GameplaySetup.PlayerType = PlayerType.Host;
            MultiplayerManager.Instance.SetPlayerCountInRoom(_playerCount);
            CreateRoomAsync(true).Forget();
        }

        private async UniTask CreateRoomAsync(bool isPrivate)
        {
            MessagePopup.Setup().ShowWaiting()
                        .SetMessage("Creating Room")
                        .ShowCloseButton(false);
            bool canCreateRoom = await LobbyManager.Instance.CreateLobby(roomName.text, isPrivate);

            if (canCreateRoom)
            {
                MessagePopup.Setup().HideWaiting();
                WaitingPopup.Setup().ShowWaiting();
                LobbySceneController.Instance.LoadWaitingScene();
                MultiplayerManager.Instance.SetPlayerCountInRoom(_playerCount);
                InjectPlayEntry();
            }

            else
            {
                MessagePopup.Setup().ShowWaiting()
                            .SetMessage("Cannot create play room now!")
                            .ShowCloseButton(true);
            }
        }

        private void InjectPlayEntry()
        {
            PlayEntryData playEntry = new()
            {
                PlayLevelName = MultiplayerManager.Instance.CarrierCollection.PlayEntryCarrier.GetRandomLevelName()
            };

            MultiplayerManager.Instance.CarrierCollection.PlayEntryCarrier.Initialize(playEntry);
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
