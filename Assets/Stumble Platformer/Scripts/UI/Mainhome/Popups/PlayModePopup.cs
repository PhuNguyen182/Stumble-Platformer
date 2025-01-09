using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using StumblePlatformer.Scripts.Gameplay.Databases;
using StumblePlatformer.Scripts.Multiplayers;
using StumblePlatformer.Scripts.Gameplay;
using GlobalScripts.SceneUtils;

namespace StumblePlatformer.Scripts.UI.Mainhome.Popups
{
    public class PlayModePopup : BasePopup<PlayModePopup>
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button singlePlayButton;
        [SerializeField] private Button multiplayerButton;
        [SerializeField] private LevelNameCollection levelNameCollection;

        private const string ClosePopupTrigger = "Close";
        private readonly int _closePopupHash = Animator.StringToHash(ClosePopupTrigger);

        protected override void OnAwake()
        {
            base.OnAwake();
            RegisterButton();
        }

        private void RegisterButton()
        {
            closeButton.onClick.AddListener(Close);
            singlePlayButton.onClick.AddListener(SelectSinglePlayMode);
            multiplayerButton.onClick.AddListener(SelectMultiPlayMode);
        }

        private void SelectSinglePlayMode()
        {
            GameplaySetup.PlayMode = GameMode.SinglePlayer;
            string levelName = levelNameCollection.GetRandomName();

            PlayGameConfig.Current = new()
            {
                PlayLevelName = levelName
            };

            WaitingPopup.Setup().ShowWaiting();
            MultiplayerManager.Instance.StartSingleMode();
            SceneLoader.LoadNetworkScene(SceneConstants.Gameplay);
        }

        private void SelectMultiPlayMode()
        {
            GameplaySetup.PlayMode = GameMode.Multiplayer;
            WaitingPopup.Setup().ShowWaiting();
            SceneLoader.LoadScene(SceneConstants.Lobby).Forget();
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
