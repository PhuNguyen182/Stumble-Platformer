using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using StumblePlatformer.Scripts.Gameplay;
using StumblePlatformer.Scripts.Multiplayers;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.Gameplay.Databases;
using StumblePlatformer.Scripts.Common.Constants;
using StumblePlatformer.Scripts.GameDatas;
using GlobalScripts.SceneUtils;
using Cysharp.Threading.Tasks;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Waiting
{
    public class WaitingSceneController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private CharacterVisual characterVisual;
        [SerializeField] private CharacterVisualDatabase characterVisualDatabase;
        [SerializeField] private GameObject hostNotice;

        [Header("UI Elements")]
        [SerializeField] private TMP_Text roomCodeText;
        [SerializeField] private TMP_Text participantCount;
        [SerializeField] private Button backButton;

        private int _participants;
        private int _maxPlayerCount;
        private bool _isReady = false;
        private bool _ableToLoadScene = false;
        private Dictionary<ulong, bool> _joinedPlayersIdsCollection = new();

        private void Awake()
        {
            RegisterButtons();
            characterVisualDatabase.Initialize();
            UpdateSkin();

            characterVisual.SetMove(moveSpeed);
            characterVisual.SetRunning(true);

            WaitingPopup.Setup().HideWaiting();
        }

        private void Start()
        {
            OnClientApprove();
            SetHostNoticeActive();

            ConfirmPopup.PreloadFromAddress(CommonPopupPaths.ConfirmPopupPath).Forget();
            MultiplayerManager.Instance.OnPlayerDisconnected += OnCharacterDisconnected;
        }

        private void Update()
        {
            OnClientApprove();

            if (_isReady)
            {
                if (!_ableToLoadScene)
                {
                    _ableToLoadScene = true;
                    ReadyToPlay();
                }
            }
        }

        private void RegisterButtons()
        {
            backButton.onClick.AddListener(BackMainHome);
        }

        private void OnClientApprove()
        {
            _participants = MultiplayerManager.Instance.ParticipantCount.Value;
            _maxPlayerCount = MultiplayerManager.Instance.MaxPlayerAmount.Value;
            participantCount.text = _maxPlayerCount != 0 ? $"Waiting for players: {_participants}/{_maxPlayerCount}" : "Waiting";
            _isReady = _participants >= _maxPlayerCount;
            backButton.interactable = !_isReady;
        }

        private void UpdateSkin()
        {
            string skin = GameDataManager.Instance.PlayerProfile.SkinName;
            if (characterVisualDatabase.TryGetCharacterSkin(skin, out var characterSkin))
                characterVisual.UpdateSkin(characterSkin);
        }

        private void SetHostNoticeActive()
        {
            bool isHost = GameplaySetup.PlayerType == PlayerType.Host
                          || GameplaySetup.PlayerType == PlayerType.Server;
            bool isPrivate = MultiplayerManager.Instance.IsPrivateRoom.Value;

            hostNotice.SetActive(isHost && isPrivate);
            if(isHost && isPrivate && LobbyManager.Instance.HasLobby())
                roomCodeText.text = LobbyManager.Instance.GetCurrentLobby().LobbyCode;
        }

        private void ReadyToPlay()
        {
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

        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void SetPlayerIdServerRpc(RpcParams rpcParams = default)
        {
            bool allPlayerReady = true;
            SetPlayerIdClientRpc(rpcParams.Receive.SenderClientId);
            _joinedPlayersIdsCollection[rpcParams.Receive.SenderClientId] = true;

            foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
            {
                if (!_joinedPlayersIdsCollection.ContainsKey(clientId) || !_joinedPlayersIdsCollection[clientId])
                {
                    allPlayerReady = false;
                    break;
                }
            }

            if (allPlayerReady && _isReady)
            {
                ShowWaitingRpc();
                if (IsServer)
                    LoadPlayScene().Forget();
            }
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = true)]
        private void SetPlayerIdClientRpc(ulong clientId)
        {
            if (!_joinedPlayersIdsCollection.TryAdd(clientId, true))
                _joinedPlayersIdsCollection[clientId] = true;
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void ShowWaitingRpc()
        {
            WaitingPopup.Setup().ShowWaiting();
        }

        private void OnCharacterDisconnected(ulong clientId)
        {
            // Server ID acts like the last ID from connected clients Ids
            if (!NetworkManager.IsServer)
            {
                int playerCount = NetworkManager.ConnectedClientsIds.Count;
                if (playerCount - 1 < 0)    
                    ShowRoomFullPopup().Forget(); // If room is full

                else
                {
                    ulong serverClientId = NetworkManager.ConnectedClientsIds[playerCount - 1];
                    if (clientId == serverClientId)
                        ShowDisconnectedPopup().Forget();
                }
            }
        }

        private async UniTask ShowRoomFullPopup()
        {
            ConfirmPopup confirmPopup = await ConfirmPopup.CreateFromAddress(CommonPopupPaths.ConfirmPopupPath);
            confirmPopup.AddMessageOK("Error!", "Room Is Full!", BackMainHome).SetCanvasMode(false).ShowCloseButton(true);
        }

        private async UniTask ShowDisconnectedPopup()
        {
            ConfirmPopup confirmPopup = await ConfirmPopup.CreateFromAddress(CommonPopupPaths.ConfirmPopupPath);
            confirmPopup.AddMessageOK("Error!", "Server Is Disconnected!", BackMainHome).SetCanvasMode(false).ShowCloseButton(true);
        }

        private async UniTask LoadPlayScene()
        {
            await LobbyManager.Instance.DeleteLobby();
            SceneLoader.LoadNetworkScene(SceneConstants.Gameplay);
        }

        public override void OnDestroy()
        {
            ConfirmPopup.Release();
            MultiplayerManager.Instance.OnClientApprove -= OnClientApprove;
            MultiplayerManager.Instance.OnPlayerDisconnected -= OnCharacterDisconnected;
            base.OnDestroy();
        }
    }
}
