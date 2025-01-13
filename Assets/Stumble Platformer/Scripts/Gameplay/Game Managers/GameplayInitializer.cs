using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StumblePlatformer.Scripts.Multiplayers;
using StumblePlatformer.Scripts.Common.Constants;
using GlobalScripts.SceneUtils;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class GameplayInitializer : MonoBehaviour
    {
        [SerializeField] private PlayDataCollectionInitializer playDataCollectionInitializer;

        private MessageBroketManager _messageBroketManager;

        private void Awake()
        {
            InitializeService();
            NetworkManager.Singleton.OnClientDisconnectCallback += OnCharacterDisconnected;
        }

        private void InitializeService()
        {
            _messageBroketManager = new();
            playDataCollectionInitializer.Initialize();
        }

        private void OnCharacterDisconnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                // Server ID acts like the last ID from connected clients Ids
                int playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
                ulong serverClientId = NetworkManager.Singleton.ConnectedClientsIds[playerCount - 1];

                if (clientId == serverClientId)
                    ShowDisconnectedPopup().Forget();
            }
        }

        private async UniTask ShowDisconnectedPopup()
        {
            var confirmPopup = await ConfirmPopup.CreateFromAddress(CommonPopupPaths.ConfirmPopupPath);
            confirmPopup.AddMessageOK("Error!", "Server Is Disconnected!", BackMainHome)
                        .ShowCloseButton(true);
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

        private void OnDestroy()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnCharacterDisconnected;
        }
    }
}
