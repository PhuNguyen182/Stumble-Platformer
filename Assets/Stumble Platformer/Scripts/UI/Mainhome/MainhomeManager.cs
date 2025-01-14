using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.UI.Mainhome.MainPanels;
using StumblePlatformer.Scripts.Common.Constants;
using StumblePlatformer.Scripts.Common.Enums;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.UI.Mainhome
{
    public class MainhomeManager : MonoBehaviour
    {
        [SerializeField] private MainPanel mainPanel;
        [SerializeField] private MainhomeInput mainhomeInput;

        private bool _isQuitPress = false;

        public MainPanel MainPanel => mainPanel;
        public static MainhomeManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            ConfirmPopup.PreloadFromAddress(CommonPopupPaths.ConfirmPopupPath).Forget();
        }

        private void Start()
        {
            WaitingPopup.Setup().HideWaiting();
        }

        private void Update()
        {
            if (mainhomeInput.IsQuitPress && !_isQuitPress)
            {
                OpenQuitPopup().Forget();
            }
        }

        private async UniTask OpenQuitPopup()
        {
            _isQuitPress = true;
            ConfirmPopup quitPopup = await ConfirmPopup.CreateFromAddress(CommonPopupPaths.ConfirmPopupPath);
            quitPopup.AddMessageYesNo("Quit Game", "Do you want to leave game?"
                    , onYesClick: QuitGame
                    , OnCloseBoxAction: () => _isQuitPress = false)
                    .SetCanvasMode(false);
        }

        private void QuitGame()
        {
            Application.Quit();
        }

        private void OnDestroy()
        {
            ConfirmPopup.Release();
        }
    }
}
