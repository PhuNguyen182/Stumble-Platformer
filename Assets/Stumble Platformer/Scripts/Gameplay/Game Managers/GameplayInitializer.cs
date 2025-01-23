using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cysharp.Threading.Tasks;
using StumblePlatformer.Scripts.Multiplayers;
using StumblePlatformer.Scripts.Common.Constants;
using GlobalScripts.SceneUtils;
using GlobalScripts.Audios;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class GameplayInitializer : MonoBehaviour
    {
        [SerializeField] private AudioClip gameplayClip;
        [SerializeField] private PlayDataCollectionInitializer playDataCollectionInitializer;

        private MessageBroketManager _messageBroketManager;
        public static GameplayInitializer Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            InitializeService();
            NetworkManager.Singleton.OnClientDisconnectCallback += OnCharacterDisconnected;
        }

        private void Start()
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayBackgroundMusic(gameplayClip, volume: 0.15f);
        }

        private void InitializeService()
        {
            _messageBroketManager = new();
            _messageBroketManager.Forget();
            playDataCollectionInitializer.Initialize();
        }

        private void OnCharacterDisconnected(ulong clientId)
        {
            OnCharacterDisconnectedDelayed(clientId).Forget();
        }

        private async UniTask OnCharacterDisconnectedDelayed(ulong clientId)
        {
            if (NetworkManager.Singleton.IsServer)
                return;

            await UniTask.WaitForSeconds(1f, cancellationToken: destroyCancellationToken);
            if (destroyCancellationToken.IsCancellationRequested)
                return;

            if (PlayGroundManager.Instance.PlayRule.IsEndGame)
                return;

            // Server ID acts like the last ID from connected clients Ids
            int playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            ulong serverClientId = NetworkManager.Singleton.ConnectedClientsIds[playerCount - 1];

            if (clientId == serverClientId)
                ShowDisconnectedPopup().Forget();
        }

        private async UniTask ShowDisconnectedPopup()
        {
            Cursor.lockState = CursorLockMode.None;
            ConfirmPopup confirmPopup = await ConfirmPopup.CreateFromAddress(CommonPopupPaths.ConfirmPopupPath);
            confirmPopup.AddMessageOK("Error!", "Server Is Disconnected!", BackMainHome)
                        .SetCanvasMode(true).ShowCloseButton(true);
        }

        private void BackMainHome()
        {
            OnLeaveRoom().Forget();
        }

        private async UniTask OnLeaveRoom()
        {
            WaitingPopup.Setup().ShowWaiting();
            MultiplayerManager.Instance.Shutdown();
            await SceneLoader.LoadScene(SceneConstants.Mainhome);
        }

        public bool IsAllMessagesInit()
        {
            return _messageBroketManager != null && _messageBroketManager.IsInitialize;
        }
    }
}
