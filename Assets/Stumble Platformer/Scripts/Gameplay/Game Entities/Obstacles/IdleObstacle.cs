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
        [SerializeField] private bool initializedIsKinematic;

        private void Awake()
        {
            initializedIsKinematic = obstacleBody.isKinematic;
        }

        private void Start()
        {
            SpawnNetworkObject();
        }

        private void SpawnNetworkObject()
        {
            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
            {
                if (!IsSpawned)
                {
                    networkObject.Spawn();
                    obstacleBody.isKinematic = initializedIsKinematic;
                }
            }
            else if (GameplaySetup.PlayMode == GameMode.Multiplayer)
            {
                if (GameplaySetup.PlayerType == PlayerType.Host || GameplaySetup.PlayerType == PlayerType.Server)
                {
                    networkObject.Spawn();
                    obstacleBody.isKinematic = initializedIsKinematic;
                }
            }
        }
    }
}
