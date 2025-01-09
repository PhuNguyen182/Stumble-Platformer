using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using GlobalScripts.SceneUtils;
using UnityEngine.SceneManagement;
using StumblePlatformer.Scripts.Gameplay;
using StumblePlatformer.Scripts.GameDatas;
using StumblePlatformer.Scripts.Multiplayers.Datas;
using StumblePlatformer.Scripts.Multiplayers.Carriers;
using StumblePlatformer.Scripts.Common.Enums;
using Unity.Services.Authentication;
using Unity.Netcode.Transports.UTP;

namespace StumblePlatformer.Scripts.Multiplayers
{
    public class MultiplayerManager : NetworkSingleton<MultiplayerManager>
    {
        [SerializeField] public CarrierCollection CarrierCollection;

        private NetworkList<PlayerData> _playerDatas = new();

        private const string DefaultIP = "127.0.0.1";
        private const ushort DefaultPort = 7777;

        public const int MinPlayerCount = 1;
        public const int MaxPlayerCount = 7;
        public const int AcceptablePlayerCount = 2;

        public Action OnFailToJoinGame;
        public Action OnTryingToJoinGame;
        public Action OnPlayerDataNetworkListChanged;

        public Action<ulong> OnPlayerDisconnected;
        public Action<string, LoadSceneMode, List<ulong>, List<ulong>> OnSceneLoadEventCompleted;

        public int PlayerAmount { get; private set; }
        public int CurrentParticipant { get; private set; }

        protected override void OnAwake()
        {
            _playerDatas.OnListChanged += OnPlayerDatasListChanged;
            CarrierCollection ??= GetComponent<CarrierCollection>();
        }

        public string GetCurrentPlayerID()
        {
            return AuthenticationService.Instance.PlayerId;
        }

        public void StartSingleMode()
        {
            if (NetworkManager.Singleton.TryGetComponent(out UnityTransport transport))
                transport.SetConnectionData(DefaultIP, DefaultPort);
            
            NetworkManager.Singleton.StartHost();
        }

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback_Server;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback_Server;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += HandleSceneLoadEventCompleted;
        }

        public void StartClient()
        {
            OnTryingToJoinGame?.Invoke();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback_Client;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback_Client;
            NetworkManager.Singleton.StartClient();
        }

        public void RemovePlayer(ulong clientId)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            OnClientDisconnectCallback_Server(clientId);
        }

        public void Shutdown()
        {
            if (GameplaySetup.PlayerType == PlayerType.Host)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback -= ConnectionApprovalCallback;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback_Server;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback_Server;
            }

            else if(GameplaySetup.PlayerType == PlayerType.Client)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback_Client;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback_Client;
            }

            NetworkManager.Singleton.Shutdown();
        }

        public void SetPlayerCountInRoom(int amount)
        {
            PlayerAmount = amount;
        }

        public PlayerData GetPlayerData(int index)
        {
            if (index < 0 || index >= _playerDatas.Count)
                return default;

            return _playerDatas[index];
        }

        public int GetPlayerDataIndexFromClientId(ulong clientId)
        {
            for (int i = 0; i < _playerDatas.Count; i++)
            {
                if (_playerDatas[i].ClientID == clientId)
                    return i;
            }

            return -1;
        }

        private void HandleSceneLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            OnSceneLoadEventCompleted?.Invoke(sceneName, loadSceneMode, clientsCompleted, clientsTimedOut);
        }

        private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest approvalRequest, NetworkManager.ConnectionApprovalResponse approvalResponse)
        {
            if(NetworkManager.Singleton.ConnectedClientsIds.Count > PlayerAmount)
            {
                approvalResponse.Approved = false;
                approvalResponse.Reason = "Room is full!";
                return;
            }

            if(string.CompareOrdinal(SceneManager.GetActiveScene().name, SceneConstants.Lobby) != 0)
            {
                approvalResponse.Approved = false;
                approvalResponse.Reason = "Game has been started!";
                return;
            }

            CurrentParticipant = NetworkManager.Singleton.ConnectedClientsIds.Count;
            approvalResponse.Approved = true;
        }

        private void OnClientConnectedCallback_Server(ulong clientId)
        {
            _playerDatas.Add(new PlayerData
            {
                ClientID = clientId
            });

            SetPlayerSkinRpc(GameDataManager.Instance.PlayerProfile.SkinName);
            SetPlayerNameRpc(GameDataManager.Instance.PlayerProfile.PlayerName);
            SetPlayerIDRpc(AuthenticationService.Instance.PlayerId);
        }

        private void OnClientDisconnectCallback_Server(ulong clientId)
        {
            OnPlayerDisconnected?.Invoke(clientId);
            for (int i = 0; i < _playerDatas.Count; i++)
            {
                if (_playerDatas[i].ClientID == clientId)
                {
                    _playerDatas.RemoveAt(i);
                    return;
                }
            }
        }

        private void OnClientConnectedCallback_Client(ulong clientId)
        {
            SetPlayerSkinRpc(GameDataManager.Instance.PlayerProfile.SkinName);
            SetPlayerNameRpc(GameDataManager.Instance.PlayerProfile.PlayerName);
            SetPlayerIDRpc(AuthenticationService.Instance.PlayerId);
        }

        private void OnClientDisconnectCallback_Client(ulong clientId)
        {
            OnFailToJoinGame?.Invoke();
            OnPlayerDisconnected?.Invoke(clientId);
        }

        private void OnPlayerDatasListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            OnPlayerDataNetworkListChanged?.Invoke();
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void SetPlayerSkinRpc(string playerSkin, RpcParams serverRpcParams = default)
        {
            int playerIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = _playerDatas[playerIndex];
            playerData.PlayerSkin = playerSkin;
            _playerDatas[playerIndex] = playerData;
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void SetPlayerNameRpc(string playerName, RpcParams serverRpcParams = default)
        {
            int playerIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = _playerDatas[playerIndex];
            playerData.PlayerName = playerName;
            _playerDatas[playerIndex] = playerData;
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void SetPlayerIDRpc(string playerId, RpcParams serverRpcParams = default)
        {
            int playerIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = _playerDatas[playerIndex];
            playerData.PlayerID = playerId;
            _playerDatas[playerIndex] = playerData;
        }
    }
}
