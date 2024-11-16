using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using GlobalScripts.Utils;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    public class EnvironmentSetup : MonoBehaviour
    {
        public void SetupSky(Material skybox)
        {
            if (skybox == null)
                return;

            RenderSettings.skybox = skybox;
            DynamicGI.UpdateEnvironment();
        }

        public async UniTask GenerateLevel(string levelName)
        {
            string path = $"Normal Levels/{levelName}.unity";
            await AddressablesUtils.LoadSceneViaAddressable(path, LoadSceneMode.Additive);
        }
    }
}
