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
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;
using GlobalScripts;

namespace StumblePlatformer.Scripts.Multiplayers
{
    public class LobbyManager : PersistentSingleton<LobbyManager>
    {
        private const float MaxHeartBeatTime = 15f;
        private const string KeyRelayJoinCode = "RelayJoinCode";

        private float _heartBeatTime = 0;
        private Lobby _joinedLobby;

        protected override void OnAwake()
        {
            InitializeUnityAuthentication().Forget();
        }

        private void Update()
        {
            ListenHeartBeat();
        }

        private void ListenHeartBeat()
        {
            if (IsHostLobby())
            {
                _heartBeatTime -= Time.deltaTime;
                if(_heartBeatTime <= 0)
                {
                    _heartBeatTime = MaxHeartBeatTime;
                    LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
                }
            }
        }

        private bool IsHostLobby() => _joinedLobby != null && string.CompareOrdinal(_joinedLobby.HostId, AuthenticationService.Instance.PlayerId) == 0;

        private async UniTask<Allocation> AllocateRelay()
        {
            try
            {
                Allocation relayAllocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerManager.MaxPlayerCount - 1);
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

        public async UniTask CreateLobby(string lobbyName, bool isPrivate)
        {
            try
            {
                CreateLobbyOptions options = new()
                {
                    IsPrivate = isPrivate
                };

                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerManager.MaxPlayerCount, options);
                Allocation relayAllocation = await AllocateRelay();
                string relayAllocationCode = await GetRelayJoinCode(relayAllocation);

                await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {KeyRelayJoinCode, new DataObject(DataObject.VisibilityOptions.Member, relayAllocationCode) }
                    }
                });

                if (NetworkManager.Singleton.TryGetComponent(out UnityTransport transport))
                    transport.SetRelayServerData(new RelayServerData(relayAllocation, "dtls"));

                MultiplayerManager.Instance.StartHost();
                // To do: Load play scene here
            }
            catch (LobbyServiceException e)
            {
                DebugUtils.LogError(e.Message);
            }
        }

        public async UniTask JoinLobby(string joinCode = null)
        {
            try
            {
                if(string.IsNullOrEmpty(joinCode))
                _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                else 
                    _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode);

                string relayJoinCode = _joinedLobby.Data[KeyRelayJoinCode].Value;
                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

                if (NetworkManager.Singleton.TryGetComponent(out UnityTransport transport))
                    transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                MultiplayerManager.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                DebugUtils.LogError(e.Message);
            }
        }

        public async UniTask DeleteLobby()
        {
            if (_joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
                    _joinedLobby = null;
                }
                catch (LobbyServiceException e)
                {
                    DebugUtils.LogError(e.Message);
                }
            }
        }

        public async UniTask LeaveLobby()
        {
            if(_joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                }
                catch (LobbyServiceException e)
                {
                    DebugUtils.LogError(e.Message);
                }
            }
        }
    }
}
