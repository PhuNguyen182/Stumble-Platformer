using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Gameplay.Cameras;
using StumblePlatformer.Scripts.Gameplay.PlayRules;
using StumblePlatformer.Scripts.Common.Messages;
using Sirenix.OdinInspector;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class EnvironmentIdentifier : MonoBehaviour
    {
        public bool IsTest;
        [SerializeField] public string LevelName;

        [Header("Environment")]
        [SerializeField] public bool FogEnable;
        [SerializeField] public Color FogColor;
        [SerializeField] public FogMode FogMode;
        [SerializeField] public float FogDensity;
        [SerializeField] public Material Skybox;
        [SerializeField] public Color AmbientColor = new(0.5f, 0.5f, 0.5f, 1);
        [SerializeField] public Light SunSource;
        [SerializeField] public LevelPlatform PlayLevel;
        [SerializeField] public SpawnCharacterArea SpawnCharacterArea;

        [Header("Teaser Path")]
        [SerializeField] public Transform TeaserLookAt;
        [SerializeField] public CinemachineDollyCart TeaserFollower;
        [SerializeField] public CinemachineSmoothPath TeaserPath;

        [Header("Settings")]
        [SerializeField] public float stopTimeAmount = 2f;
        [SerializeField] public float teaserDefaultSpeed = 0.25f;

        [Header("Camera")]
        [SerializeField] public CameraBodyMode CameraBodyMode; 
        [SerializeField] public TransposerConfig TransposerConfig;
        [SerializeField] public TrackedDollyConfig TrackedDollyConfig;

        private IPublisher<SetupLevelMessage> _initLevelPublisher;
        public BasePlayRule PlayRule { get; private set; }

        private void Awake()
        {
            SetLevelActive(true);
            SetTeaserActive(false);
            PlayRule = GetComponent<BasePlayRule>();
        }

        private void Start()
        {
            _initLevelPublisher = GlobalMessagePipe.GetPublisher<SetupLevelMessage>();
            
            _initLevelPublisher.Publish(new SetupLevelMessage
            {
                EnvironmentIdentifier = this
            });
        }

        public void SetLevelActive(bool active) => PlayLevel.SetLevelActive(active);

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

        [HorizontalGroup("Setup")][Button]
        private void GetEnvironmentProperty()
        {
            Skybox = RenderSettings.skybox;
            SunSource = RenderSettings.sun;
            AmbientColor = RenderSettings.ambientLight;
            FogEnable = RenderSettings.fog;
            FogMode = RenderSettings.fogMode;
            FogColor = RenderSettings.fogColor;
            FogDensity = RenderSettings.fogDensity;
        }

        [HorizontalGroup("Setup")][Button]
        private void SetEnvironmentProperty()
        {
            RenderSettings.skybox = Skybox;
            RenderSettings.sun = SunSource;
            RenderSettings.ambientLight = AmbientColor;
            RenderSettings.fog = FogEnable;
            RenderSettings.fogMode = FogMode;
            RenderSettings.fogColor = FogColor;
            RenderSettings.fogDensity = FogDensity;
        }
    }
}
