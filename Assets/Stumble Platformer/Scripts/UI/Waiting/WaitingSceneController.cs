using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.GameDatas;
using StumblePlatformer.Scripts.Gameplay.Databases;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.Multiplayers;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Waiting
{
    public class WaitingSceneController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private CharacterVisual characterVisual;
        [SerializeField] private CharacterVisualDatabase characterVisualDatabase;
        [SerializeField] private GameObject hostNotice;
        [SerializeField] private TMP_Text roomCodeText;

        private void Awake()
        {
            characterVisualDatabase.Initialize();
            UpdateSkin();

            characterVisual.SetMove(moveSpeed);
            characterVisual.SetRunning(true);
        }

        private void UpdateSkin()
        {
            string skin = GameDataManager.Instance.PlayerProfile.SkinName;
            if (characterVisualDatabase.TryGetCharacterSkin(skin, out var characterSkin))
            {
                characterVisual.UpdateSkin(characterSkin);
            }
        }

        public void SetHostNoticeActive(bool active)
        {
            hostNotice.SetActive(active);
            if(active && LobbyManager.Instance.HasLobby())
            {
                roomCodeText.text = LobbyManager.Instance.GetCurrentLobby().LobbyCode;
            }
        }
    }
}
