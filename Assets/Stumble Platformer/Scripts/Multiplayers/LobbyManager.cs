using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;
using GlobalScripts;

namespace StumblePlatformer.Scripts.Multiplayers
{
    public class LobbyManager : MonoBehaviour
    {
        private float _heartBeatTime = 0;
        private float _listLobbiesTimer = 0;
        private Lobby _joinedLobby;

        public Action OnCreateLobbyStarted;
        public Action OnCreateLobbyFailed;

        public Action OnJoinStarted;
        public Action OnQuickJoinFailed;
        public Action OnJoinFailed;
        
        public Action<List<Lobby>> OnLobbyListChanged;
        public static LobbyManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            InitializeUnityAuthentication().Forget();
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            ListenHeartBeat();
            HandlePeriodicListLobbies();
        }

        private void HandlePeriodicListLobbies()
        {
            if (HasLobby())
                return;

            if (UnityServices.State != ServicesInitializationState.Initialized)
                return;

            if (!AuthenticationService.Instance.IsSignedIn)
                return;

            if (string.CompareOrdinal(SceneManager.GetActiveScene().name, "Lobby") != 0)
                return;

            _listLobbiesTimer -= Time.deltaTime;
            if (_listLobbiesTimer <= 0f)
            {
                _listLobbiesTimer = MultiplayerConstants.ListLobbiesTimerMax;
                ListLobbies().Forget();
            }
        }

        private void ListenHeartBeat()
        {
            _heartBeatTime -= Time.deltaTime;
            if (_heartBeatTime <= 0)
            {
                _heartBeatTime = MultiplayerConstants.MaxHeartBeatTime;

                if (IsHostLobby())
                    LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
        }

        private bool IsHostLobby()
        {
            if (!HasLobby())
                return false;

            if(string.CompareOrdinal(_joinedLobby.HostId, AuthenticationService.Instance.PlayerId) != 0)
                return false;

            return true;
        }

        private async UniTask<Allocation> AllocateRelay()
        {
            try
            {
                Allocation relayAllocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerConstants.MaxPlayerCount - 1);
                return relayAllocation;
            }
            catch(RelayServiceException e)
            {
                DebugUtils.LogError(e.Message);
                return default;
            }
        }

        private async UniTask InitializeUnityAuthentication()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions initializationOptions = new();
                initializationOptions.SetProfile($"{Random.Range(0, 1000)}");
                await UnityServices.InitializeAsync(initializationOptions);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        private async UniTask ListLobbies()
        {
            try
            {
                QueryLobbiesOptions lobbiesOptions = new()
                {
                    Filters = new()
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    }
                };

                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(lobbiesOptions);
                OnLobbyListChanged?.Invoke(queryResponse.Results);
            }
            catch(LobbyServiceException e)
            {
                DebugUtils.LogError(e.Message);
                return;
            }
        }

        public bool HasLobby() => _joinedLobby != null;

        public Lobby GetCurrentLobby() => _joinedLobby;

        public async UniTask<string> GetRelayJoinCode(Allocation allocation)
        {
            try
            {
                string code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                return code;
            }
            catch(RelayServiceException e)
            {
                DebugUtils.LogError(e.Message);
                return null;
            }
        }

        public async UniTask<JoinAllocation> JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                return allocation;
            }
            catch(RelayServiceException e)
            {
                DebugUtils.LogError(e.Message);
                return default;
            }
        }

        public async UniTask<bool> CreateLobby(string lobbyName, bool isPrivate)
        {
            OnCreateLobbyStarted?.Invoke();
            MultiplayerManager.Instance.IsPrivateRoom.Value = isPrivate;

            try
            {
                CreateLobbyOptions options = new()
                {
                    IsPrivate = isPrivate
                };

                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerConstants.MaxPlayerCount, options);
                Allocation relayAllocation = await AllocateRelay();
                string relayAllocationCode = await GetRelayJoinCode(relayAllocation);

                await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {MultiplayerConstants.KeyRelayJoinCode, new DataObject(DataObject.VisibilityOptions.Member, relayAllocationCode) }
                    }
                });

                if (NetworkManager.Singleton.TryGetComponent(out UnityTransport transport))
                    transport.SetRelayServerData(new RelayServerData(relayAllocation, "dtls"));

                MultiplayerManager.Instance.StartHost();
                return true;
            }
            catch (LobbyServiceException e)
            {
                DebugUtils.LogError(e.Message);
                OnCreateLobbyFailed?.Invoke();
                return false;
            }
        }

        public async UniTask<bool> JoinLobby(string joinCode = null)
        {
            OnJoinStarted?.Invoke();
            bool hasNoJoinCode = string.IsNullOrEmpty(joinCode);

            try
            {
                if (hasNoJoinCode)
                    _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                else 
                    _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode);

                if (_joinedLobby.Data == null || _joinedLobby.Data.Count <= 0)
                {
                    _joinedLobby = null;
                    return false;
                }

                string relayJoinCode = _joinedLobby.Data[MultiplayerConstants.KeyRelayJoinCode].Value;
                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

                if (NetworkManager.Singleton.TryGetComponent(out UnityTransport transport))
                    transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                MultiplayerManager.Instance.StartClient();
                return true;
            }
            catch (LobbyServiceException e)
            {
                DebugUtils.LogError(e.Message);

                if (hasNoJoinCode)
                    OnQuickJoinFailed?.Invoke();
                else
                    OnJoinFailed?.Invoke();
                
                return false;
            }
        }

        public async UniTask<bool> DeleteLobby()
        {
            if (HasLobby())
            {
                try
                {
                    await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
                    _joinedLobby = null;
                    return true;
                }
                catch (LobbyServiceException e)
                {
                    DebugUtils.LogError(e.Message);
                    return false;
                }
            }

            return false;
        }

        public async UniTask<bool> LeaveLobby()
        {
            if(HasLobby())
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                    return true;
                }
                catch (LobbyServiceException e)
                {
                    DebugUtils.LogError(e.Message);
                    return false;
                }
            }

            return false;
        }

        public async UniTask<bool> RemovePlayer(string playerId)
        {
            if (IsHostLobby())
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
                    return true;
                }
                catch (LobbyServiceException e)
                {
                    DebugUtils.LogError(e.Message);
                    return false;
                }
            }

            return false;
        }
    }
}
