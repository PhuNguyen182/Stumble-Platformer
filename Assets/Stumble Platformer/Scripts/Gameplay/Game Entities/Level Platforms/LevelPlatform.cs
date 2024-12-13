using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms;
using Sirenix.OdinInspector;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class LevelPlatform : MonoBehaviour
    {
        [SerializeField] private RespawnArea[] respawnAreas;

        [Header("Containers")]
        [SerializeField] private Transform respawnContainer;
        [SerializeField] private Transform obstacleContainer;
        [SerializeField] private Transform platformContainer;

        [Header("Hazzard Objects")]
        [SerializeField] private BaseObstacle[] obstacles;
        [SerializeField] private BasePlatform[] platforms;

        public int CheckPointCount => respawnAreas.Length;

        public RespawnArea GetCheckPointByIndex(int index)
        {
            for (int i = 0; i < respawnAreas.Length; i++)
            {
                if (respawnAreas[i].AreaIndex == index)
                    return respawnAreas[i];
            }

            return null;
        }

        public void SetLevelActive(bool active)
        {
            for (int i = 0; i < obstacles.Length; i++)
            {
                obstacles[i].SetObstacleActive(active);
            }

            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i].SetPlatformActive(active);
            }
        }

#if UNITY_EDITOR
        [Button][HorizontalGroup(GroupID = "Hazzards")]
        public void GetObstacles()
        {
            obstacles = obstacleContainer.GetComponentsInChildren<BaseObstacle>();
        }

        [Button][HorizontalGroup(GroupID = "Hazzards")]
        public void GetPlatforms()
        {
            platforms = platformContainer.GetComponentsInChildren<BasePlatform>();
        }

        [Button]
        [HorizontalGroup(GroupID = "Hazzards")]
        public void GetSpawnAreas()
        {
            respawnAreas = respawnContainer.GetComponentsInChildren<RespawnArea>();
        }
#endif
    }
}
