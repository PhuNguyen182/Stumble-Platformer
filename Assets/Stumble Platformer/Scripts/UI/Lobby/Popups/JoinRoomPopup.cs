using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using StumblePlatformer.Scripts.Multiplayers;
using Sirenix.OdinInspector;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Lobby.Popups
{
    public class JoinRoomPopup : BasePopup<JoinRoomPopup>
    {
        [DetailedInfoBox("Enter Room Code", "")]
        [SerializeField] private TMP_InputField roomCodeField;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button joinButton;

        private const string ClosePopupTrigger = "Close";
        private readonly int _closePopupHash = Animator.StringToHash(ClosePopupTrigger);

        protected override void OnAwake()
        {
            base.OnAwake();
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            closeButton.onClick.AddListener(Close);
            joinButton.onClick.AddListener(JoinRoom);
        }

        private void JoinRoom()
        {
            JoinRoomAsync().Forget();
        }

        private async UniTask JoinRoomAsync()
        {
            MessagePopup.Setup().ShowWaiting().SetMessage("Joining Room").ShowCloseButton(false);
            bool canJoin = await LobbyManager.Instance.JoinLobby(roomCodeField.text);

            if (canJoin)
            {
                MessagePopup.Setup().HideWaiting();
                // This is automatically enter the Waiting Scene if server has entered the Waiting Scene
            }

            else
            {
                Close();
                MessagePopup.Setup().ShowWaiting()
                            .SetMessage("Cannot find open room!")
                            .ShowCloseButton(true);
            }
        }

        protected override void DoDisappear()
        {
            roomCodeField.text = "";
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
