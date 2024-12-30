using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.Gameplay.Databases;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using Sirenix.OdinInspector;

namespace StumblePlatformer.Scripts.UI.Mainhome.PlayerCustomize
{
    public class PlayerCustomizePanel : DerivedPanel
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Transform cellContainer;
        [SerializeField] private CharacterCell[] characterCells;
        [SerializeField] private CharacterVisualDatabase characterVisualDatabase;
        [SerializeField] private CharacterVisual characterVisual;

        private void Awake()
        {
            RegisterPlayerCells();
            characterVisualDatabase.Initialize();
        }

        private void Start()
        {
            ExitImmediately();
            InitCharacterCells();
        }

        [Button]
        public void GetCharacterCells()
        {
            characterVisualDatabase.Initialize();
            characterCells = cellContainer.GetComponentsInChildren<CharacterCell>();

            for (int i = 0; i < characterCells.Length; i++)
            {
                string key = characterVisualDatabase.SkinCollections.Keys.ElementAt(i);
                characterCells[i].SetID(key);
            }
        }

        private void RegisterPlayerCells()
        {
            backButton.onClick.AddListener(BackToMain);
            for (int i = 0; i < characterCells.Length; i++)
            {
                var cell = characterCells[i];
                cell.OnCellClick = SelectSkin;
            }
        }

        private void InitCharacterCells()
        {
            for (int i = 0; i < characterCells.Length; i++)
            {
                string id = characterCells[i].ID;
                if (characterVisualDatabase.TryGetCharacterSkin(id, out var characterSkin))
                    characterCells[i].SetAvatar(characterSkin.Avatar);
            }
        }

        private void SelectSkin(string skinId)
        {
            for (int i = 0; i < characterCells.Length; i++)
            {
                bool isSelected = string.CompareOrdinal(characterCells[i].ID, skinId) == 0;
                characterCells[i].SetSelectState(isSelected);
            }

            if(characterVisualDatabase.TryGetCharacterSkin(skinId, out CharacterSkin characterSkin))
            {
                // To do: Update character skin
                if (characterVisual)
                    characterVisual.UpdateSkin(characterSkin);
            }
        }
    }
}
