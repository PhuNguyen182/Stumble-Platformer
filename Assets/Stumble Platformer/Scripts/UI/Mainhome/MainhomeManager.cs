using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.UI.Mainhome.MainPanels;
using StumblePlatformer.Scripts.Gameplay;

namespace StumblePlatformer.Scripts.UI.Mainhome
{
    public class MainhomeManager : MonoBehaviour
    {
        [SerializeField] private MainPanel mainPanel;

        public MainPanel MainPanel => mainPanel;

        public static MainhomeManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            WaitingPopup.Setup().HideWaiting();
            GameplaySetup.PlayerType = PlayerType.None;
        }
    }
}
