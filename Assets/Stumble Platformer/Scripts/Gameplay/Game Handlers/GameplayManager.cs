using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;

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

        public void OnPlaygroundLoaded(EnvironmentIdentifier environment)
        {
            // To do: after a playground is loaded, do execute essential logic for level
        }
    }
}
