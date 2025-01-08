using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class IdleObstacle : NetworkBehaviour
    {
        [SerializeField] private Rigidbody obstacleBody;
        [SerializeField] private NetworkObject networkObject;

        private void Start()
        {
            SpawnNetworkObject();
        }

        private void SpawnNetworkObject()
        {
            bool isKinematic = obstacleBody ?? obstacleBody.isKinematic;
            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
            {
                if (!IsSpawned)
                {
                    networkObject.Spawn();
                    obstacleBody.isKinematic = isKinematic;
                }
            }
            else if (GameplaySetup.PlayMode == GameMode.Multiplayer)
            {
                if (GameplaySetup.PlayerType == PlayerType.Host || GameplaySetup.PlayerType == PlayerType.Server)
                {
                    networkObject.Spawn();
                    obstacleBody.isKinematic = isKinematic;
                }
            }
        }
    }
}
