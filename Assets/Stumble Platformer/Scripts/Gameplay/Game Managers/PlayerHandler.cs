using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Constants;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;
using StumblePlatformer.Scripts.Gameplay.Inputs;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class PlayerHandler : MonoBehaviour
    {
        public bool IsTest;
        [SerializeField] private InputReceiver inputReceiver;
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private EnvironmentHandler environmentHandler;
        [SerializeField] private PlayDataCollectionInitializer playDataCollectionInitializer;

        private RespawnArea _currentCheckPoint;
        private PlayerController _currentPlayer;

        public PlayerController CurrentPlayer => _currentPlayer;
        public int PlayerInstanceID => _currentPlayer.gameObject.GetInstanceID();

        public void SpawnPlayer()
        {
            Vector3 playerPosition = environmentHandler.EnvironmentIdentifier
                                    .SpawnCharacterArea.MainCharacterSpawnPosition;
            _currentPlayer = Instantiate(playerPrefab, playerPosition, Quaternion.identity);

            CharacterSkin characterSkin; // Get a temp skin
            bool hasSkin = playDataCollectionInitializer.CharacterVisualDatabase.TryGetCharacterSkin("21", out characterSkin);

            if (hasSkin)
                _currentPlayer.PlayerGraphics.SetCharacterVisual(characterSkin);

            int lifeCount = IsTest ? 1000 : CharacterConstants.MaxLife;
            _currentPlayer.PlayerHealth.SetHealth(lifeCount);
            _currentPlayer.SetCharacterInput(inputReceiver);
        }

        public void SetPlayerActive(bool active) => _currentPlayer.IsActive = active;

        public void RespawnPlayer()
        {
            int checkPointIndex = _currentPlayer.GetCheckPointIndex();
            _currentCheckPoint = environmentHandler.EnvironmentIdentifier.PlayLevel
                                                   .GetCheckPointByIndex(checkPointIndex);
            
            Vector3 respawnPosition = _currentCheckPoint.GetRandomSpawnPosition();
            _currentPlayer.transform.position = respawnPosition;

            _currentPlayer.SetCharacterActive(true);
            SetPlayerActive(true);
        }
    }
}
