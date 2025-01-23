using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.Multiplayers.Datas;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;
using StumblePlatformer.Scripts.Common.Constants;
using StumblePlatformer.Scripts.Multiplayers;
using StumblePlatformer.Scripts.GameDatas;
using TMPro;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class PlayerHandler : NetworkBehaviour
    {
        [SerializeField] private bool isTest;
        [SerializeField] private TMP_Text checkPointText;
        [SerializeField] private InputReceiver inputReceiver;
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private EnvironmentHandler environmentHandler;
        [SerializeField] private PlayDataCollectionInitializer playDataCollectionInitializer;

        private RespawnArea _currentCheckPoint;
        private PlayerController _currentPlayer;
        private PlayerController _tempPlayer;

        public PlayerController CurrentPlayer => _currentPlayer;
        public int PlayerInstanceID => _currentPlayer.gameObject.GetInstanceID();
        public int OriginPlayerHealth { get; private set; }

        #region Test Checkpoint, Editor only
#if UNITY_EDITOR
        private bool _isCheckPointJump;
        private int _testCheckPointIndex = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J) && isTest)
            {
                JumpCheckPoint();
            }
        }

        private void JumpCheckPoint()
        {
            _isCheckPointJump = true;
            _testCheckPointIndex += 1;
            _testCheckPointIndex = _testCheckPointIndex % environmentHandler.EnvironmentIdentifier.PlayLevel.CheckPointCount;
            checkPointText?.SetText($"{_testCheckPointIndex}");
        }
#endif
        #endregion

        public override void OnNetworkSpawn() { }

        public void SpawnPlayer()
        {
            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
                SpawnSinglePlayer();
            else if (GameplaySetup.PlayMode == GameMode.Multiplayer)
                SpawnMultiplayerPlayer();
        }

        private void SpawnSinglePlayer()
        {
            Vector3 playerPosition = environmentHandler.EnvironmentIdentifier
                                    .SpawnCharacterArea.MainCharacterSpawnPosition;

            _currentPlayer = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
            _currentPlayer.NetworkObject.SpawnAsPlayerObject(0, true);

            string skin = GameDataManager.Instance.PlayerProfile.SkinName;
            if (playDataCollectionInitializer.CharacterVisualDatabase.TryGetCharacterSkin(skin, out CharacterSkin characterSkin))
                _currentPlayer.PlayerGraphics.SetCharacterVisual(characterSkin);

            OriginPlayerHealth = isTest ? 1000 : CharacterConstants.MaxLife;
            _currentPlayer.PlayerHealth.SetHealth(OriginPlayerHealth);
            _currentPlayer.SetCharacterInput(inputReceiver);

            SetPlayerCompleteLevel(false);
            SetPlayerPhysicsActive(false);
        }

        private void SpawnMultiplayerPlayer()
        {
            if (!IsServer)
                return;

            foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
            {
                int playerIndex = MultiplayerManager.Instance.GetPlayerDataIndexFromClientId(clientId);
                Vector3 playerPosition = environmentHandler.EnvironmentIdentifier.SpawnCharacterArea
                                                           .CharacterSpawnPositions[playerIndex];

                PlayerData playerData = MultiplayerManager.Instance.GetPlayerData(playerIndex);
                _tempPlayer = Instantiate(playerPrefab, playerPosition, Quaternion.identity);

                if (_tempPlayer.NetworkObject != null)
                    _tempPlayer.NetworkObject.SpawnAsPlayerObject(clientId, true);

                SetCurrentPlayerRpc(playerData);
                string skin = playerData.PlayerSkin.Value;

                if (playDataCollectionInitializer.CharacterVisualDatabase.TryGetCharacterSkin(skin, out CharacterSkin characterSkin))
                    _tempPlayer.PlayerGraphics.SetCharacterVisual(characterSkin);
            }
        }

        public void SetPlayerActive(bool active) => SetPlayerActiveRpc(active);

        public void SetPlayerCompleteLevel(bool isCompleted) => SetPlayerCompleteLevelRpc(isCompleted);

        public void SetPlayerPhysicsActive(bool active) => SetPlayerPhysicsActiveRpc(active);

        public void ActivateAllPlayer() => ActivateAllPlayerRpc();

        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void ActivateAllPlayerRpc()
        {
            foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
            {
                NetworkObject playerObject = NetworkManager.Singleton.SpawnManager
                                             .GetPlayerNetworkObject(clientId);

                if (playerObject.TryGetComponent(out PlayerController player))
                {
                    player.IsActive = true;
                    player.SetCharacterActiveRpc(true);
                }
            }
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void SetCurrentPlayerRpc(PlayerData playerData)
        {
            if (string.CompareOrdinal(playerData.PlayerID.Value, MultiplayerManager.Instance.GetCurrentPlayerID()) == 0)
            {
                NetworkObject player = NetworkManager.Singleton.SpawnManager
                                       .GetPlayerNetworkObject(playerData.ClientID);

                if (player.TryGetComponent(out _currentPlayer))
                {
                    _currentPlayer.SetCharacterInput(inputReceiver);
                    _currentPlayer.PlayerGraphics.SetPlayerNameRpc(playerData.PlayerName.Value);
                    SetPlayerPhysicsActive(false);
                }
            }
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void SetPlayerActiveRpc(bool active)
        {
            if (_currentPlayer)
                _currentPlayer.IsActive = active;
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void SetPlayerCompleteLevelRpc(bool isCompleted)
        {
            if (_currentPlayer != null)
                _currentPlayer.PlayerHealth.SetPlayerCompleteLevel(isCompleted);
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void SetPlayerPhysicsActiveRpc(bool active)
        {
            if (_currentPlayer != null)
                _currentPlayer.SetCharacterActive(active);
        }

        public void RespawnPlayer()
        {
            int checkPointIndex = 0;
#if UNITY_EDITOR
            checkPointIndex = _isCheckPointJump ? _testCheckPointIndex 
                              : _currentPlayer.GetCheckPointIndex();
            _isCheckPointJump = false;
#else
            checkPointIndex = _currentPlayer.GetCheckPointIndex();
#endif
            _currentCheckPoint = environmentHandler.EnvironmentIdentifier.PlayLevel
                                                   .GetCheckPointByIndex(checkPointIndex);
            Vector3 respawnPosition = _currentCheckPoint.GetRandomSpawnPosition();
            _currentPlayer.transform.position = respawnPosition;

            _currentPlayer.AfterRespawn();
            _currentPlayer.transform.rotation = _currentCheckPoint.transform.rotation;
            _currentPlayer.ResetPlayerOrientation(_currentCheckPoint.transform.localRotation);
            environmentHandler.CameraHandler.ResetCurrentCameraFollow();

            SetPlayerPhysicsActive(true);
            SetPlayerActive(true);
        }
    }
}
