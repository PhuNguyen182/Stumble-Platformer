using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private InputReceiver inputReceiver;
        [SerializeField] private CameraHandler cameraHandler;
        [SerializeField] private PlayGroundController playGroundController;
        [SerializeField] private PlayDataCollectionInitializer playDataCollectionInitializer;
        [SerializeField] private bool isTesting;

        private MessageBroketManager _messageBroketManager;

        public PlayDataCollectionInitializer PlayDataCollectionInitializer => playDataCollectionInitializer;
        public static GameplayManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            InitializeService();
        }

        private void InitializeService()
        {
            _messageBroketManager = new();
            playDataCollectionInitializer.Initialize();
        }

        public async UniTask InitGameplay()
        {
            if(!isTesting)
                await playGroundController.GenerateLevel();
            
            playGroundController.SpawnPlayer();
            cameraHandler.SetFollowTarget(playGroundController.CurrentPlayer.transform);
        }

        public void SetInputActive(bool active)
        {
            inputReceiver.IsActive = active;
        }
    }
}
