using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private CameraHandler cameraHandler;
        [SerializeField] private PlayGroundController playGroundController;
        [SerializeField] private PlayDataCollectionInitializer playDataCollectionInitializer;

        private MessageBroketManager _messageBroketManager;

        public PlayDataCollectionInitializer PlayDataCollectionInitializer => playDataCollectionInitializer;
        public static GameplayManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            InitializeService();
        }

        //private void Start()
        //{
        //    InitGameplay().Forget();
        //}

        private void InitializeService()
        {
            _messageBroketManager = new();
            playDataCollectionInitializer.Initialize();
        }

        public async UniTask InitGameplay()
        {
            playGroundController.SpawnPlayer();
            cameraHandler.SetFollowTarget(playGroundController.CurrentPlayer.transform);

            // To do: load level later
            await playGroundController.GenerateLevel();
        }
    }
}
