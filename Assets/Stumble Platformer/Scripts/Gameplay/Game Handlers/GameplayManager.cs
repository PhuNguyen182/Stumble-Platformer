using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using Cysharp.Threading.Tasks;
using GlobalScripts.Utils;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private CameraHandler cameraHandler;
        [SerializeField] private InputReceiver inputReceiver;
        [SerializeField] private PlayGroundController playGroundController;

        private MessageBroketManager _messageBroketManager;

        public CameraHandler CameraHandler => cameraHandler;
        public InputReceiver InputReceiver => inputReceiver;
        public static GameplayManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            InitializeService();
        }

        private void Start()
        {
            InitGameplay().Forget();
        }

        private void InitializeService()
        {
            _messageBroketManager = new();
        }

        private async UniTask InitGameplay()
        {
            cameraHandler.SetFollowTarget(playGroundController.CurrentPlayer.transform);
            await playGroundController.GenerateLevelAsync();
            playGroundController.SpawnPlayer();
        }
    }
}
