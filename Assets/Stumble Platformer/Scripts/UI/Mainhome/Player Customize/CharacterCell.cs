using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StumblePlatformer.Scripts.UI.Mainhome.PlayerCustomize
{
    public class CharacterCell : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private Image icon;
        [SerializeField] private GameObject selectMark;
        [SerializeField] private Button cellButton;

        public Action<string> OnCellClick;
        public string ID => id;

        private void Awake()
        {
            cellButton.onClick.AddListener(() => OnCellClick?.Invoke(id));
        }

        public void SetSelectState(bool isSelected)
        {
            selectMark.SetActive(isSelected);
        }

        public void SetID(string id)
        {
            this.id = id;
        }

        public void SetAvatar(Sprite avatar)
        {
            icon.sprite = avatar;
        }
    }
}
