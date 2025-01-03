using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.GameDatas;
using StumblePlatformer.Scripts.Gameplay.Databases;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.UI.Gameplay.MainPanels
{
    public class FinalePanel : MonoBehaviour
    {
        [SerializeField] private Button claimButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private CharacterVisualDatabase characterVisualDatabase;
        [SerializeField] private CharacterVisual characterVisual;

        public Action OnQuitGame;

        private void Awake()
        {
            claimButton.onClick.AddListener(ClaimReward);
            continueButton.onClick.AddListener(ExitGame);
            characterVisualDatabase.Initialize();
            UpdateSkin();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetEndGameResult(EndResult endResult)
        {
            switch (endResult)
            {
                case EndResult.Win:
                    characterVisual.PlayWin();
                    claimButton.gameObject.SetActive(true);
                    continueButton.gameObject.SetActive(false);
                    break;
                case EndResult.Lose:
                    claimButton.gameObject.SetActive(false);
                    continueButton.gameObject.SetActive(true);
                    break;
            }
        }

        private void UpdateSkin()
        {
            string skin = GameDataManager.Instance.PlayerProfile.SkinName;
            if (characterVisualDatabase.TryGetCharacterSkin(skin, out var characterSkin))
                characterVisual.UpdateSkin(characterSkin);
        }

        private void ClaimReward()
        {
            OnQuitGame?.Invoke();
        }

        private void ExitGame()
        {
            OnQuitGame?.Invoke();
        }
    }
}
