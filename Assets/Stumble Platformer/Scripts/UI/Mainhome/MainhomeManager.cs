using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.UI.Mainhome.MainPanels;

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
    }
}
