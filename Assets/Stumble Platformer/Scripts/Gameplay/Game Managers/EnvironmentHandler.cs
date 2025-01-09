using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using GlobalScripts.Utils;
using UnityEngine.SceneManagement;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class EnvironmentHandler : NetworkBehaviour
    {
        [SerializeField] private CameraHandler cameraHandler;

        public CameraHandler CameraHandler => cameraHandler;
        public EnvironmentIdentifier EnvironmentIdentifier { get; private set; }

        public void SetLevelActive(bool active) => EnvironmentIdentifier.SetLevelActive(active);

        public void SetLevelSecondaryComponentActive(bool active)
        {
            EnvironmentIdentifier.PlayLevel.SetSecondaryLevelComponentActive(active);
        }

        public void SetEnvironmentIdentifier(EnvironmentIdentifier environmentIdentifier)
        {
            EnvironmentIdentifier = environmentIdentifier;
            SetupEnvironment();
        }

        public async UniTask WaitForTeaser()
        {
            cameraHandler.SetTeaserCameraActive(true);
            EnvironmentIdentifier.SetTeaserActive(true);
            await EnvironmentIdentifier.WaitForEndOfTeaser();
            cameraHandler.SetTeaserCameraActive(false);
        }

        private void SetupSky()
        {
            RenderSettings.skybox = EnvironmentIdentifier.Skybox;
        }

        private void SetupLight()
        {
            if (EnvironmentIdentifier.SunSource != null)
                RenderSettings.sun = EnvironmentIdentifier.SunSource;
        }

        private void SetupAmbient()
        {
            RenderSettings.ambientLight = EnvironmentIdentifier.AmbientColor;
            DynamicGI.UpdateEnvironment();
        }

        private void SetupFog()
        {
            RenderSettings.fog = EnvironmentIdentifier.FogEnable;

            if (!EnvironmentIdentifier.FogEnable)
                return;

            RenderSettings.fogMode = EnvironmentIdentifier.FogMode;
            RenderSettings.fogColor = EnvironmentIdentifier.FogColor;
            RenderSettings.fogDensity = EnvironmentIdentifier.FogDensity;
        }

        private void SetupCamera()
        {
            cameraHandler.SetupTeaserCamera(EnvironmentIdentifier.TeaserFollower.transform
                                            , EnvironmentIdentifier.TeaserLookAt
                                            , EnvironmentIdentifier.TeaserPath);
        }

        private void SetupEnvironment()
        {
            SetupSky();
            SetupFog();
            SetupLight();
            SetupAmbient();
            SetupCamera();
        }

        public async UniTask GenerateLevel(string levelName)
        {
            string path = $"Normal Levels/{levelName}.unity";
            await AddressablesUtils.LoadSceneViaAddressable(path, LoadSceneMode.Additive);
        }
    }
}
