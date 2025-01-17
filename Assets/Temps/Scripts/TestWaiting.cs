using Cysharp.Threading.Tasks;
using StumblePlatformer.Scripts.Multiplayers;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestWaiting : NetworkBehaviour
{
    private int _participants;
    private int _maxPlayerCount;
    private bool _isReady = false;
    private bool _ableToLoadScene = false;
    private Dictionary<ulong, bool> _joinedPlayersIdsCollection = new();

    private void Awake()
    {
        WaitingPopup.Setup().HideWaiting();
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

    private void OnClientApprove()
    {
        _participants = MultiplayerManager.Instance.ParticipantCount.Value;
        _maxPlayerCount = 2;
        _isReady = _participants >= _maxPlayerCount;
    }

    private void ReadyToPlay()
    {
        SetPlayerReady();
    }

    private void SetPlayerReady()
    {
        SetPlayerIdServerRpc();
    }

    private async UniTask LoadPlayScene()
    {
        await LobbyManager.Instance.DeleteLobby();
        NetworkManager.Singleton.SceneManager.LoadScene("Test Gameplay", UnityEngine.SceneManagement.LoadSceneMode.Single);
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
            if (IsServer)
                LoadPlayScene().Forget();
        }
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void SetPlayerIdClientRpc(ulong clientId)
    {
        if (!_joinedPlayersIdsCollection.TryAdd(clientId, true))
            _joinedPlayersIdsCollection[clientId] = true;
    }
}
