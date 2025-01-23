using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.UI.Mainhome.MainPanels;
using StumblePlatformer.Scripts.Common.Constants;
using Cysharp.Threading.Tasks;
using GlobalScripts.Audios;

namespace StumblePlatformer.Scripts.UI.Mainhome
{
    public class MainhomeManager : MonoBehaviour
    {
        [SerializeField] private MainPanel mainPanel;
        [SerializeField] private MainhomeInput mainhomeInput;
        [SerializeField] private AudioClip mainMenuBgm;

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
            PlaySound();
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

        private void PlaySound()
        {
            if (!AudioManager.Instance.IsMusicPlaying())
            {
                AudioManager.Instance.PlayBackgroundMusic(mainMenuBgm);
            }
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
