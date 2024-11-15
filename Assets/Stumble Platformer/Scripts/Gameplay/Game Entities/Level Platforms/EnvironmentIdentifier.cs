using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.PlayRules;
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
        public IPlayeRule PlayRule { get; private set; }

        private void Awake()
        {
            PlayRule = GetComponent<IPlayeRule>();
            _initLevelPublisher = GlobalMessagePipe.GetPublisher<InitializeLevelMessage>();
            
            _initLevelPublisher.Publish(new InitializeLevelMessage
            {
                EnvironmentIdentifier = this
            });
        }
    }
}
