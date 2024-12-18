using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Constants;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using TMPro;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class PlayerHandler : MonoBehaviour
    {
        [SerializeField] private bool isTest;
        [SerializeField] private TMP_Text checkPointText;
        [SerializeField] private InputReceiver inputReceiver;
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private EnvironmentHandler environmentHandler;
        [SerializeField] private PlayDataCollectionInitializer playDataCollectionInitializer;

        private RespawnArea _currentCheckPoint;
        private PlayerController _currentPlayer;

        public PlayerController CurrentPlayer => _currentPlayer;
        public int PlayerInstanceID => _currentPlayer.gameObject.GetInstanceID();

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

        public void SpawnPlayer()
        {
            Vector3 playerPosition = environmentHandler.EnvironmentIdentifier
                                    .SpawnCharacterArea.MainCharacterSpawnPosition;
            _currentPlayer = Instantiate(playerPrefab, playerPosition, Quaternion.identity);

            CharacterSkin characterSkin; // Get a temp skin
            bool hasSkin = playDataCollectionInitializer.CharacterVisualDatabase.TryGetCharacterSkin("21", out characterSkin);

            if (hasSkin)
                _currentPlayer.PlayerGraphics.SetCharacterVisual(characterSkin);

            int lifeCount = isTest ? 1000 : CharacterConstants.MaxLife;
            _currentPlayer.PlayerHealth.SetHealth(lifeCount);
            _currentPlayer.SetCharacterInput(inputReceiver);
            SetPlayerPhysicsActive(false);
        }

        public void SetPlayerActive(bool active) => _currentPlayer.IsActive = active;

        public void SetPlayerPhysicsActive(bool active) => _currentPlayer.SetCharacterActive(active);

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
