using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class GameplayInitializer : MonoBehaviour
    {
        [SerializeField] private PlayDataCollectionInitializer playDataCollectionInitializer;

        private MessageBroketManager _messageBroketManager;

        private void Awake()
        {
            InitializeService();
        }

        private void InitializeService()
        {
            _messageBroketManager = new();
            playDataCollectionInitializer.Initialize();
        }
    }
}
