using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.Gameplay.Databases;
using StumblePlatformer.Scripts.UI.Mainhome.PlayerCustomize;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.UI.Mainhome.SettingPanels;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using GlobalScripts.SceneUtils;
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
        [SerializeField] private LevelNameCollection levelNameCollection;

        private void Awake()
        {
            UpdateSkin();
            RegisterButtonClicks();
        }

        private void Start()
        {
            EnterImmediately();
        }

        private void RegisterButtonClicks()
        {
            playButton.onClick.AddListener(PlayGame);
            customizeButton.onClick.AddListener(OpenCharacterCustomize);
            settingButton.onClick.AddListener(OpenSetting);
        }

        public void UpdateSkin()
        {
            // To do: get skin index and update the visual
        }

        private void OpenSetting()
        {
            ExitPanel();
            settingPanel.EnterPanel();
        }

        private void OpenCharacterCustomize()
        {
            ExitPanel();
            playerCustomizePanel.EnterPanel();
        }

        private void PlayGame()
        {
            string levelName = levelNameCollection.GetRandomName();
            PlayGameConfig.Current = new PlayGameConfig
            {
                PlayLevelName = levelName
            };

            SceneBridge.LoadNextScene(SceneConstants.Gameplay).Forget();
        }
    }
}
