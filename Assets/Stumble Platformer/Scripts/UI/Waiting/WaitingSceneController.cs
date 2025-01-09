using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using StumblePlatformer.Scripts.Gameplay;
using StumblePlatformer.Scripts.Multiplayers;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Gameplay.Databases;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.GameDatas;
using GlobalScripts.SceneUtils;
using Cysharp.Threading.Tasks;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Waiting
{
    public class WaitingSceneController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private CharacterVisual characterVisual;
        [SerializeField] private CharacterVisualDatabase characterVisualDatabase;
        [SerializeField] private GameObject hostNotice;

        [Header("UI Elements")]
        [SerializeField] private TMP_Text roomCodeText;
        [SerializeField] private Button readyButton;
        [SerializeField] private Button backButton;

        private Dictionary<ulong, bool> _playerIdCollection = new();

        private void Awake()
        {
            RegisterButtons();
            characterVisualDatabase.Initialize();
            UpdateSkin();

            characterVisual.SetMove(moveSpeed);
            characterVisual.SetRunning(true);
        }

        private void Start()
        {
            SetHostNoticeActive();
        }

        private void RegisterButtons()
        {
            readyButton.onClick.AddListener(ReadyToPlay);
            backButton.onClick.AddListener(BackMainHome);
        }

        private void UpdateSkin()
        {
            string skin = GameDataManager.Instance.PlayerProfile.SkinName;
            if (characterVisualDatabase.TryGetCharacterSkin(skin, out var characterSkin))
            {
                characterVisual.UpdateSkin(characterSkin);
            }
        }

        private void SetHostNoticeActive()
        {
            bool active = GameplaySetup.PlayerType == PlayerType.Host;
            hostNotice.SetActive(active);
            if(active && LobbyManager.Instance.HasLobby())
            {
                roomCodeText.text = LobbyManager.Instance.GetCurrentLobby().LobbyCode;
            }
        }

        private void ReadyToPlay()
        {
            WaitingPopup.Setup().ShowWaiting();
            SetPlayerReady();
        }

        private void BackMainHome()
        {
            OnLeaveRoom().Forget();
        }

        private async UniTask OnLeaveRoom()
        {
            await LobbyManager.Instance.LeaveLobby();
            MultiplayerManager.Instance.Shutdown();
            WaitingPopup.Setup().ShowWaiting();
            await SceneLoader.LoadScene(SceneConstants.Mainhome);
        }

        private void SetPlayerReady()
        {
            SetPlayerIdServerRpc();
        }

        [ServerRpc]
        private void SetPlayerIdServerRpc(ServerRpcParams rpcParams = default)
        {
            bool allPlayerReady = true;
            SetPlayerIdClientRpc(rpcParams.Receive.SenderClientId);
            _playerIdCollection[rpcParams.Receive.SenderClientId] = true;

            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!_playerIdCollection.ContainsKey(clientId) || !_playerIdCollection[clientId])
                {
                    allPlayerReady = false;
                    break;
                }
            }

            if (allPlayerReady)
            {
                LoadPlayScene().Forget();
            }
        }

        [ClientRpc]
        private void SetPlayerIdClientRpc(ulong clientId)
        {
            _playerIdCollection[clientId] = true;
        }

        private async UniTask LoadPlayScene()
        {
            await LobbyManager.Instance.DeleteLobby();
            SceneLoader.LoadNetworkScene(SceneConstants.Gameplay);
        }
    }
}
