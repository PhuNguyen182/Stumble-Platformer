using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.GameDatas;
using StumblePlatformer.Scripts.Gameplay.Databases;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.UI.Mainhome.PlayerCustomize
{
    public class PlayerCustomizePanel : DerivedPanel
    {
        [SerializeField] private Button backButton;
        [SerializeField] private ScrollRect characterCellScroller;
        [SerializeField] private float focusSpeed = 9;
        [SerializeField] private bool check;

        [Space(10)]
        [SerializeField] private Transform cellContainer;
        [SerializeField] private CharacterCell[] characterCells;
        [SerializeField] private CharacterVisualDatabase characterVisualDatabase;
        [SerializeField] private CharacterVisual characterVisual;

        private string _currentSkin;

        private void Awake()
        {
            RegisterPlayerCells();
            GetCharacterCells();
        }

        protected override void Start()
        {
            base.Start();
            InitCharacterCells();
            UpdateSkinOnStart();
        }

        public void GetCharacterCells()
        {
            characterVisualDatabase.Initialize();
            characterCells = cellContainer.GetComponentsInChildren<CharacterCell>();

            for (int i = 0; i < characterCells.Length; i++)
            {
                string key = characterVisualDatabase.SkinCollections.Keys.ElementAt(i);
                characterCells[i].SetID(key);
                characterCells[i].gameObject.name = $"Character Skin {key}";
            }
        }

        private void UpdateSkinOnStart()
        {
            _currentSkin = GameDataManager.Instance.PlayerProfile.SkinName;
            
            if (string.IsNullOrEmpty(_currentSkin))
            {
                _currentSkin = characterVisualDatabase.SkinCollections.Keys.ElementAt(0);
                GameDataManager.Instance.PlayerProfile.SkinName = _currentSkin;
            }

            SelectSkin(_currentSkin);
        }

        private int GetSkinIndex(string skin)
        {
            for (int i = 0; i < characterCells.Length; i++)
            {
                if (string.CompareOrdinal(skin, characterCells[i].ID) == 0)
                    return i;
            }

            return -1;
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
                if (characterVisual)
                {
                    characterVisual.UpdateSkin(characterSkin);
                    GameDataManager.Instance.PlayerProfile.SkinName = skinId;
                    _currentSkin = skinId;
                }
            }
        }

        private async UniTask ScrollTo(int index)
        {
            await characterCellScroller.FocusOnItemCoroutine(characterCells[index].RectTransform, focusSpeed);
        }

        public void ScrollToCurrentSkin()
        {
            int skinIndex = GetSkinIndex(_currentSkin);
            ScrollTo(skinIndex).Forget();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (check)
            {
                check = false;
                GetCharacterCells();
            }
        }
#endif
    }
}
