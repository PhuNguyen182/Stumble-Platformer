using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private CameraHandler cameraHandler;
        [SerializeField] private InputReceiver inputReceiver;

        public CameraHandler CameraHandler => cameraHandler;
        public InputReceiver InputReceiver => inputReceiver;
        public static GameplayManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}
