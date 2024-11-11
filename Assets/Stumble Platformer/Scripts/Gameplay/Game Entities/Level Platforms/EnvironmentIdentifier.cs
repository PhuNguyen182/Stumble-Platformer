using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StumblePlatformer.Scripts.Gameplay.GameHandlers;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class EnvironmentIdentifier : MonoBehaviour
    {
        [SerializeField] public Material Skybox;
        [SerializeField] public LevelPlatform PlayLevel;
        [SerializeField] public CinemachineDollyCart TearserFollower;
        [SerializeField] public CinemachineSmoothPath TeaserPath;

        private void Awake()
        {
            if(GameplayManager.Instance != null)
            {
                GameplayManager.Instance.OnPlaygroundLoaded(this);
            }
        }
    }
}
