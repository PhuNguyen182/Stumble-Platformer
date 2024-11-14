using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StumblePlatformer.Scripts.Common.Messages;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class EnvironmentIdentifier : MonoBehaviour
    {
        [SerializeField] public Material Skybox;
        [SerializeField] public LevelPlatform PlayLevel;
        [SerializeField] public CinemachineDollyCart TearserFollower;
        [SerializeField] public CinemachineSmoothPath TeaserPath;

        private IPublisher<InitializeLevelMessage> _initLevelPublisher;

        private void Awake()
        {
            _initLevelPublisher = GlobalMessagePipe.GetPublisher<InitializeLevelMessage>();
            _initLevelPublisher.Publish(new InitializeLevelMessage
            {
                EnvironmentIdentifier = this
            });
        }
    }
}
