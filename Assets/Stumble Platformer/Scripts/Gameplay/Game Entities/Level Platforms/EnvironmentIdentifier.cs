using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.PlayRules;
using Cysharp.Threading.Tasks;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class EnvironmentIdentifier : MonoBehaviour
    {
        public bool IsTest;
        [SerializeField] public string LevelName;

        [Header("Environment")]
        [SerializeField] public Material Skybox;
        [SerializeField] public LevelPlatform PlayLevel;
        [SerializeField] public SpawnCharacterArea SpawnCharacterArea;

        [Header("Teaser Path")]
        [SerializeField] public CinemachineDollyCart TeaserFollower;
        [SerializeField] public CinemachineSmoothPath TeaserPath;

        [Header("Settings")]
        [SerializeField] public float stopTimeAmount = 2f;
        [SerializeField] public float teaserDefaultSpeed = 0.25f;

        private IPublisher<SetupLevelMessage> _initLevelPublisher;
        public IPlayRule PlayRule { get; private set; }

        private void Awake()
        {
            SetTeaserActive(false);
            PlayRule = GetComponent<IPlayRule>();
        }

        private void Start()
        {
            _initLevelPublisher = GlobalMessagePipe.GetPublisher<SetupLevelMessage>();
            
            _initLevelPublisher.Publish(new SetupLevelMessage
            {
                EnvironmentIdentifier = this
            });
        }

        public void SetTeaserActive(bool active)
        {
            if (IsTest)
                return;

            TeaserFollower.m_Speed = active ? teaserDefaultSpeed : 0;
        }

        public async UniTask WaitForEndOfTeaser()
        {
            UniTask teaserTask = TeaserFollower.m_PositionUnits switch
            {
                CinemachinePathBase.PositionUnits.Normalized => UniTask.WaitWhile(() => TeaserFollower.m_Position < 1f
                                                                                  , cancellationToken: destroyCancellationToken),
                CinemachinePathBase.PositionUnits.Distance => UniTask.WaitWhile(() => TeaserFollower.m_Position < TeaserPath.PathLength
                                                                                , cancellationToken: destroyCancellationToken),
                CinemachinePathBase.PositionUnits.PathUnits => UniTask.WaitWhile(() => TeaserFollower.m_Position < TeaserPath.m_Waypoints.Length - 1
                                                                                 , cancellationToken: destroyCancellationToken),
                _ => UniTask.CompletedTask
            };

            await teaserTask;
            await UniTask.WaitForSeconds(stopTimeAmount, cancellationToken: destroyCancellationToken);
        }
    }
}
