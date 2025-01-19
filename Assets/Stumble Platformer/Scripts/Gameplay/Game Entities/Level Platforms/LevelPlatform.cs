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
        [SerializeField] private Transform levelRoot;
        [SerializeField] private RespawnArea[] respawnAreas;

        [Header("Containers")]
        [SerializeField] private Transform respawnContainer;
        [SerializeField] private Transform obstacleContainer;
        [SerializeField] private Transform platformContainer;
        [SerializeField] private Transform secondaryObstacleContainer;
        [SerializeField] private Transform secondaryPlatformContainer;

        [Header("Hazzard Objects")]
        [SerializeField] private BaseObstacle[] obstacles;
        [SerializeField] private BasePlatform[] platforms;

        [Header("Secondary")]
        [SerializeField] private BaseObstacle[] secondaryObstacles;
        [SerializeField] private BasePlatform[] secondaryPlatforms;

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

        public void SetSecondaryLevelComponentActive(bool active)
        {
            for (int i = 0; i < secondaryObstacles.Length; i++)
            {
                secondaryObstacles[i].SetObstacleActive(active);
            }

            for (int i = 0; i < secondaryPlatforms.Length; i++)
            {
                secondaryPlatforms[i].SetPlatformActive(active);
            }
        }

        public void SetLevelActive(bool active)
        {
            for (int i = 0; i < obstacles.Length; i++)
            {
                if (obstacles[i] != null)
                    obstacles[i].SetObstacleActive(active);
            }

            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i] != null)
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

        [Button][HorizontalGroup(GroupID = "Hazzards")]
        public void GetSpawnAreas()
        {
            respawnAreas = respawnContainer.GetComponentsInChildren<RespawnArea>();
        }

        [Button][HorizontalGroup(GroupID = "Secondary")]
        public void GetSecondaryObstacles()
        {
            secondaryObstacles = secondaryObstacleContainer.GetComponentsInChildren<BaseObstacle>();
        }

        [Button][HorizontalGroup(GroupID = "Secondary")]
        public void GetSecondaryPlatforms()
        {
            secondaryPlatforms = secondaryPlatformContainer.GetComponentsInChildren<BasePlatform>();
        }

        [Button][HorizontalGroup(GroupID = "Utils")]
        public void SetParentLevelComponents()
        {
            for (int i = 0; i < obstacles.Length; i++)
                obstacles[i].transform.SetParent(obstacleContainer);

            for (int i = 0; i < platforms.Length; i++)
                platforms[i].transform.SetParent(platformContainer);

            for (int i = 0; i < secondaryObstacles.Length; i++)
                secondaryObstacles[i].transform.SetParent(secondaryObstacleContainer);

            for (int i = 0; i < secondaryPlatforms.Length; i++)
                secondaryPlatforms[i].transform.SetParent(secondaryPlatformContainer);
        }

        [Button]
        [HorizontalGroup(GroupID = "Utils")]
        public void DeparentLevelComponents()
        {
            if (levelRoot == null)
                return;

            for (int i = 0; i < obstacles.Length; i++)
                obstacles[i].transform.SetParent(levelRoot);

            for (int i = 0; i < platforms.Length; i++)
                platforms[i].transform.SetParent(levelRoot);

            for (int i = 0; i < secondaryObstacles.Length; i++)
                secondaryObstacles[i].transform.SetParent(levelRoot);

            for (int i = 0; i < secondaryPlatforms.Length; i++)
                secondaryPlatforms[i].transform.SetParent(levelRoot);
        }
#endif
    }
}
