using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Databases;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.GameDatas;

namespace StumblePlatformer.Scripts.UI.Transition
{
    public class TransitionSceneCharacter : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private CharacterVisual characterVisual;
        [SerializeField] private CharacterVisualDatabase characterVisualDatabase;

        private void Awake()
        {
            characterVisualDatabase.Initialize();
            UpdateSkin();
            
            characterVisual.SetMove(moveSpeed);
            characterVisual.SetRunning(true);
        }

        private void UpdateSkin()
        {
            string skin = GameDataManager.Instance.PlayerGameData.SkinName;
            if(characterVisualDatabase.TryGetCharacterSkin(skin, out var characterSkin))
            {
                characterVisual.UpdateSkin(characterSkin);
            }
        }
    }
}
