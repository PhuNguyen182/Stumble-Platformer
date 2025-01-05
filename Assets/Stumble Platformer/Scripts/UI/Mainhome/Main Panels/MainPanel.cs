using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.UI.Mainhome.SettingPanels;
using StumblePlatformer.Scripts.UI.Mainhome.PlayerCustomize;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.UI.Mainhome.Popups;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.UI.Mainhome.MainPanels
{
    public class MainPanel : BasePanel
    {
        [Header("Panels")]
        [SerializeField] private SettingPanel settingPanel;
        [SerializeField] private PlayerCustomizePanel playerCustomizePanel;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button customizeButton;

        [Space(10)]
        [SerializeField] private CharacterVisual characterVisual;

        private const string PlayGamePopupPath = "Popups/Mainhome/Play Game Popup.prefab";

        private void Awake()
        {
            PreloadPopups();
            RegisterButtonClicks();
        }

        private void PreloadPopups()
        {
            PlayModePopup.PreloadFromAddress(PlayGamePopupPath).Forget();
        }

        private void RegisterButtonClicks()
        {
            playButton.onClick.AddListener(PlayGame);
            customizeButton.onClick.AddListener(OpenCharacterCustomize);
            settingButton.onClick.AddListener(OpenSetting);
        }

        private void OpenSetting()
        {
            ExitPanel();
            settingPanel.EnterPanel();
        }

        private void OpenCharacterCustomize()
        {
            ExitPanel();
            playerCustomizePanel.ScrollToCurrentSkin();
            playerCustomizePanel.EnterPanel();
        }

        private void PlayGame()
        {
            PlayModePopup.CreateFromAddress(PlayGamePopupPath).Forget();
        }

        private void OnDestroy()
        {
            PlayModePopup.Release();
        }
    }
}
