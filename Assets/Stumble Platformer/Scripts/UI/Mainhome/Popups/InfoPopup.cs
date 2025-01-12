using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.UI.Mainhome.Popups
{
    public class InfoPopup : BasePopup<InfoPopup>
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button confirmButton;

        private const string ClosePopupTrigger = "Close";
        private readonly int _closePopupHash = Animator.StringToHash(ClosePopupTrigger);

        protected override void OnAwake()
        {
            closeButton.onClick.AddListener(Close);
            confirmButton.onClick.AddListener(Close);
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
