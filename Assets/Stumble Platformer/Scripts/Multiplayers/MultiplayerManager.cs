using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using StumblePlatformer.Scripts.Gameplay;
using StumblePlatformer.Scripts.GameDatas;
using StumblePlatformer.Scripts.Multiplayers.Datas;
using StumblePlatformer.Scripts.Common.Enums;
using Unity.Services.Authentication;
using GlobalScripts.SceneUtils;

namespace StumblePlatformer.Scripts.Multiplayers
{
    public class MultiplayerManager : NetworkSingleton<MultiplayerManager>
    {
        private NetworkList<PlayerData> _playerDatas;

        public const int MinPlayerCount = 1;
        public const int MaxPlayerCount = 7;
        public const int AcceptablePlayerCount = 2;

        public Action OnFailToJoinGame;
        public Action OnTryingToJoinGame;
        public Action OnPlayerDataNetworkListChanged;

        public Action<ulong> OnPlayerDisconnected;
        public Action<string, LoadSceneMode, List<ulong>, List<ulong>> OnSceneLoadEventCompleted;

        public int PlayerAmount { get; set; }

        protected override void OnAwake()
        {
            _playerDatas = new();
            _playerDatas.OnListChanged += OnPlayerDatasListChanged;
        }

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback_Server;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback_Server;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += HandleSceneLoadEventCompleted;
        }

        private void HandleSceneLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            OnSceneLoadEventCompleted?.Invoke(sceneName, loadSceneMode, clientsCompleted, clientsTimedOut);
        }

        public void StartClient()
        {
            OnTryingToJoinGame?.Invoke();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback_Client;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback_Client;
            NetworkManager.Singleton.StartClient();
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

        public int GetPlayerDataIndexFromClientId(ulong clientId)
        {
            for (int i = 0; i < _playerDatas.Count; i++)
            {
                if (_playerDatas[i].ClientID == clientId)
                    return i;
            }

            return -1;
        }

        private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest approvalRequest, NetworkManager.ConnectionApprovalResponse approvalResponse)
        {
            if(NetworkManager.Singleton.ConnectedClientsIds.Count > MaxPlayerCount)
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

            approvalResponse.Approved = true;
        }

        private void OnPlayerDatasListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            OnPlayerDataNetworkListChanged?.Invoke();
        }

        private void OnClientConnectedCallback_Server(ulong clientId)
        {
            _playerDatas.Add(new PlayerData
            {
                ClientID = clientId
            });

            SetPlayerNameServerRpc(GameDataManager.Instance.PlayerProfile.PlayerName);
            SetPlayerIDServerRpc(AuthenticationService.Instance.PlayerId);
        }

        private void OnClientDisconnectCallback_Server(ulong clientId)
        {
            OnPlayerDisconnected?.Invoke(clientId);
            for (int i = 0; i < _playerDatas.Count; i++)
            {
                if (_playerDatas[i].ClientID == clientId)
                    _playerDatas.RemoveAt(i);
            }
        }

        private void OnClientConnectedCallback_Client(ulong clientId)
        {
            SetPlayerNameServerRpc(GameDataManager.Instance.PlayerProfile.PlayerName);
            SetPlayerIDServerRpc(AuthenticationService.Instance.PlayerId);
        }

        private void OnClientDisconnectCallback_Client(ulong clientId)
        {
            OnFailToJoinGame?.Invoke();
            OnPlayerDisconnected?.Invoke(clientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
        {
            int playerIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = _playerDatas[playerIndex];
            playerData.PlayerName = playerName;
            _playerDatas[playerIndex] = playerData;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerIDServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
        {
            int playerIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = _playerDatas[playerIndex];
            playerData.PlayerID = playerId;
            _playerDatas[playerIndex] = playerData;
        }
    }
}
