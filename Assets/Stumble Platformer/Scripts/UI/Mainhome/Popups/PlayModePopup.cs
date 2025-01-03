using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.Gameplay;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using StumblePlatformer.Scripts.Gameplay.Databases;
using GlobalScripts.SceneUtils;
using Cysharp.Threading.Tasks;

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
            GameplayMode.PlayMode = Common.Enums.PlayMode.SinglePlayer;
            string levelName = levelNameCollection.GetRandomName();
            PlayGameConfig.Current = new PlayGameConfig
            {
                PlayLevelName = levelName
            };

            SceneBridge.LoadNextScene(SceneConstants.Gameplay).Forget();
        }

        private void SelectMultiPlayMode()
        {
            GameplayMode.PlayMode = Common.Enums.PlayMode.Multiplayer;
            // To do: load lobby scene
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
                await UniTask.WaitForSeconds(0.25f, cancellationToken: destroyCancellationToken);
            }
            base.DoClose();
        }
    }
}
