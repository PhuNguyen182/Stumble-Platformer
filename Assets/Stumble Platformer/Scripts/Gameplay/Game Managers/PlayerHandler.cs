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
using GlobalScripts;

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

        public override void OnNetworkSpawn()
        {
            DebugUtils.Log(nameof(PlayerHandler));
        }

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

            CharacterSkin characterSkin;
            string skin = GameDataManager.Instance.PlayerProfile.SkinName;
            bool hasSkin = playDataCollectionInitializer.CharacterVisualDatabase.TryGetCharacterSkin(skin, out characterSkin);

            if (hasSkin)
                _currentPlayer.PlayerGraphics.SetCharacterVisual(characterSkin);

            int lifeCount = isTest ? 1000 : CharacterConstants.MaxLife;
            OriginPlayerHealth = lifeCount;
            _currentPlayer.PlayerHealth.SetHealth(lifeCount);
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
                    _tempPlayer.NetworkObject.SpawnWithOwnership(clientId, true);

                SetCurrentPlayerRpc(playerData);

                CharacterSkin characterSkin;
                string skin = playerData.PlayerSkin.Value;
                bool hasSkin = playDataCollectionInitializer.CharacterVisualDatabase.TryGetCharacterSkin(skin, out characterSkin);

                if (hasSkin)
                    _tempPlayer.PlayerGraphics.SetCharacterVisual(characterSkin);

                _currentPlayer.SetCharacterInput(inputReceiver);
                SetPlayerPhysicsActive(_tempPlayer, false);
            }
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void SetCurrentPlayerRpc(PlayerData playerData)
        {
            if (string.CompareOrdinal(playerData.PlayerID.Value, MultiplayerManager.Instance.GetCurrentPlayerID()) == 0)
                _currentPlayer = _tempPlayer;
        }

        public void SetPlayerCompleteLevel(bool isCompleted) 
        { 
            _currentPlayer.PlayerHealth.SetPlayerCompleteLevel(isCompleted);
        } 

        public void SetPlayerActive(bool active) => _currentPlayer.IsActive = active;

        public void SetPlayerPhysicsActive(bool active) => _currentPlayer.SetCharacterActive(active);

        public void SetPlayerPhysicsActive(PlayerController player, bool active) => player.SetCharacterActive(active);

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
