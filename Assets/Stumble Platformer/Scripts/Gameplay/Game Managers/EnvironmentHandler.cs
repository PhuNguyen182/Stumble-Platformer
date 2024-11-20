using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using Cysharp.Threading.Tasks;
using GlobalScripts.Utils;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class EnvironmentHandler : MonoBehaviour
    {
        [SerializeField] private CameraHandler cameraHandler;

        public EnvironmentIdentifier EnvironmentIdentifier { get; private set; }

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

        private void SetupSky(Material skybox)
        {
            RenderSettings.skybox = skybox;
            DynamicGI.UpdateEnvironment();
        }

        private void SetupEnvironment()
        {
            SetupSky(EnvironmentIdentifier.Skybox);
            cameraHandler.SetupTeaserCamera(EnvironmentIdentifier.TeaserFollower.transform, EnvironmentIdentifier.TeaserPath);
        }

        public async UniTask GenerateLevel(string levelName)
        {
            string path = $"Normal Levels/{levelName}.unity";
            await AddressablesUtils.LoadSceneViaAddressable(path, LoadSceneMode.Additive);
        }
    }
}
