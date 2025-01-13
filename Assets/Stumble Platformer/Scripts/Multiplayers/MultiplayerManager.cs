using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
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
    public class MultiplayerManager : NetworkBehaviour
    {
        [SerializeField] public NetworkObject networkObject;
        [SerializeField] public CarrierCollection CarrierCollection;

        private NetworkList<PlayerData> _playerDatas = new();

        public Action OnFailToJoinGame;
        public Action OnTryingToJoinGame;
        public Action OnPlayerDataNetworkListChanged;
        public Action OnClientApprove;

        public Action<ulong> OnPlayerDisconnected;
        public Action<string, LoadSceneMode, List<ulong>, List<ulong>> OnSceneLoadEventCompleted;

        public NetworkVariable<bool> IsPrivateRoom { get; private set; }
        public NetworkVariable<int> MaxPlayerAmount { get; private set; }
        public NetworkVariable<int> ParticipantCount { get; private set; }
        public static MultiplayerManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            IsPrivateRoom = new();
            MaxPlayerAmount = new();
            ParticipantCount = new();

            _playerDatas.OnListChanged += OnPlayerDatasListChanged;
            CarrierCollection ??= GetComponent<CarrierCollection>();
            DontDestroyOnLoad(gameObject);
        }

        public string GetCurrentPlayerID()
        {
            return AuthenticationService.Instance.PlayerId;
        }

        public void StartSingleMode()
        {
            if (NetworkManager.Singleton.TryGetComponent(out UnityTransport transport))
                transport.SetConnectionData(MultiplayerConstants.DefaultIP, MultiplayerConstants.DefaultPort);
            
            NetworkManager.Singleton.StartHost();
        }

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback_Server;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback_Server;
            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += HandleSceneLoadEventCompleted;
            ParticipantCount.Value = NetworkManager.Singleton.ConnectedClientsIds.Count;
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
            if(IsServer)
                NetworkManager.Singleton.DisconnectClient(clientId);
            else
                Shutdown();
        }

        public void LeaveRoom()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClient.ClientId;
            RemovePlayer(localClientId);
        }

        public void Shutdown()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback -= ConnectionApprovalCallback;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback_Server;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback_Server;
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= HandleSceneLoadEventCompleted;
            }

            else
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback_Client;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback_Client;
            }

            NetworkManager.Singleton.Shutdown();
        }

        public void SetPlayerCountInRoom(int amount)
        {
            MaxPlayerAmount.Value = amount;
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
            OnClientApprove?.Invoke();
            if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MaxPlayerAmount.Value)
            {
                approvalResponse.Approved = false;
                approvalResponse.Reason = "Room is full!";
                return;
            }

            approvalResponse.Approved = true;
        }

        private void OnClientConnectedCallback_Server(ulong clientId)
        {
            _playerDatas.Add(new PlayerData
            {
                ClientID = clientId
            });

            SetPlayerSkinServerRpc(GameDataManager.Instance.PlayerProfile.SkinName);
            SetPlayerNameServerRpc(GameDataManager.Instance.PlayerProfile.PlayerName);
            SetPlayerIDServerRpc(AuthenticationService.Instance.PlayerId);
        }

        private void OnClientDisconnectCallback_Server(ulong clientId)
        {
            OnPlayerDisconnected?.Invoke(clientId);
            for (int i = 0; i < _playerDatas.Count; i++)
            {
                if (_playerDatas[i].ClientID == clientId)
                {
                    _playerDatas.RemoveAt(i);
                    break;
                }
            }
        }

        private void OnClientConnectedCallback_Client(ulong clientId)
        {
            SetPlayerSkinServerRpc(GameDataManager.Instance.PlayerProfile.SkinName);
            SetPlayerNameServerRpc(GameDataManager.Instance.PlayerProfile.PlayerName);
            SetPlayerIDServerRpc(AuthenticationService.Instance.PlayerId);
        }

        private void OnClientDisconnectCallback_Client(ulong clientId)
        {
            OnFailToJoinGame?.Invoke();
            OnPlayerDisconnected?.Invoke(clientId);
        }

        private void OnPlayerDatasListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            if (IsServer)
                ParticipantCount.Value = _playerDatas.Count;

            OnPlayerDataNetworkListChanged?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerSkinServerRpc(string playerSkin, ServerRpcParams serverRpcParams = default)
        {
            int playerIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = _playerDatas[playerIndex];
            playerData.PlayerSkin = playerSkin;
            _playerDatas[playerIndex] = playerData;
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
