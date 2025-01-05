using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.Multiplayers;
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

        private const int MinPlayerCount = 1;
        private const int MaxPlayerCount = 16;
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
            MultiplayerManager.Instance.PlayerAmount = _playerCount;
            // To do: create public room
        }

        private void CreatePrivateRoom()
        {
            MultiplayerManager.Instance.PlayerAmount = _playerCount;
            // To do: create private room
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
