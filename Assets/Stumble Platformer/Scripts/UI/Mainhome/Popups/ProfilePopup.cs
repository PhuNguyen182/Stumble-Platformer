using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.GameDatas;
using StumblePlatformer.Scripts.UI.Mainhome.MainPanels;
using Cysharp.Threading.Tasks;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Mainhome.Popups
{
    public class ProfilePopup : BasePopup<ProfilePopup>
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button saveButtonButton;
        [SerializeField] private TMP_InputField playerNameHolder;

        private PlayerProfileBox _playerProfileBox;

        private const string ClosePopupTrigger = "Close";
        private readonly int _closePopupHash = Animator.StringToHash(ClosePopupTrigger);

        protected override void OnAwake()
        {
            RegisterButton();
        }

        protected override void DoAppear()
        {
            base.DoAppear();
            playerNameHolder.text = GameDataManager.Instance.PlayerProfile.PlayerName;
        }

        private void RegisterButton()
        {
            closeButton.onClick.AddListener(Close);
            saveButtonButton.onClick.AddListener(SaveProfile);
            playerNameHolder.onValueChanged.AddListener(OnPlayerNameChange);
        }

        private void OnPlayerNameChange(string value)
        {
            bool canClosePopup = !string.IsNullOrEmpty(value);
            closeButton.interactable = canClosePopup;
            saveButtonButton.interactable = canClosePopup;
        }

        private void SaveProfile()
        {
            GameDataManager.Instance.PlayerProfile.PlayerName = playerNameHolder.text;
            _playerProfileBox?.UpdatePlayerProfile();
            Close();
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

        public void SetProfileBox(PlayerProfileBox profileBox)
        {
            _playerProfileBox ??= profileBox;
        }
    }
}
