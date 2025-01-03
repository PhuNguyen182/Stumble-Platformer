using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.GameDatas;
using StumblePlatformer.Scripts.UI.Mainhome.Popups;
using Cysharp.Threading.Tasks;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Mainhome.MainPanels
{
    public class PlayerProfileBox : MonoBehaviour
    {
        [SerializeField] private Button profileButton;
        [SerializeField] private TMP_Text playerNameText;

        private const string ProfilePopupPath = "Popups/Mainhome/Player Profile Popup.prefab";

        private void Awake()
        {
            PreloadPopups();
            LoadProfileOnAwake();

            profileButton.onClick.AddListener(() =>
            {
                OpenProfilePopup().Forget();
            });
        }

        private void PreloadPopups()
        {
            ProfilePopup.PreloadFromAddress(ProfilePopupPath).Forget();
        }

        private void LoadProfileOnAwake()
        {
            UpdatePlayerProfile();
        }

        public void UpdatePlayerProfile()
        {
            string playerName = GameDataManager.Instance.PlayerProfile.PlayerName;
            
            if (string.IsNullOrEmpty(playerName))
            {
                playerName = $"#Player{Random.Range(100000, 1000000)}";
                GameDataManager.Instance.PlayerProfile.PlayerName = playerName;
            }
            
            playerNameText.text = playerName;
        }

        private async UniTask OpenProfilePopup()
        {
            var popup = await ProfilePopup.CreateFromAddress(ProfilePopupPath);
            popup.SetProfileBox(this);
        }

        private void OnDestroy()
        {
            ProfilePopup.Release();
        }
    }
}
