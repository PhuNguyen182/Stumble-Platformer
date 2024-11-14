using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using Cysharp.Threading.Tasks;
using GlobalScripts.Utils;
using UnityEngine.SceneManagement;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private CameraHandler cameraHandler;
        [SerializeField] private InputReceiver inputReceiver;

        private MessageBroketManager _messageBroketManager;

        public CameraHandler CameraHandler => cameraHandler;
        public InputReceiver InputReceiver => inputReceiver;
        public static GameplayManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            InitializeService();
        }

        private void InitializeService()
        {
            _messageBroketManager = new();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                TestLoadScene().Forget();
            }
        }

        private async UniTask TestLoadScene()
        {
            string path = $"Templates/Template Level.unity";
            Scene scene = SceneManager.GetActiveScene();
            await AddressablesUtils.LoadSceneViaAddressableAndSetParentContainer(path, scene, LoadSceneMode.Additive);
        }
    }
}
