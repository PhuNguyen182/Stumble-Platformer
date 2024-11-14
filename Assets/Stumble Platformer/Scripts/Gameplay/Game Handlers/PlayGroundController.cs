using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using Cysharp.Threading.Tasks;
using GlobalScripts.Utils;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    public class PlayGroundController : MonoBehaviour
    {
        [SerializeField] private PlayerController playerPrefab;

        private PlayerController _currentPlayer;
        private ISubscriber<InitializeLevelMessage> _initLevelSubscriber;
        private IDisposable _initLevelDisposable;

        public PlayerController CurrentPlayer => _currentPlayer;
        public EnvironmentIdentifier EnvironmentIdentifier { get; private set; }

        private void Start()
        {
            InitLevel();
            GenerateLevel();
        }

        private void InitLevel()
        {
            var builder = DisposableBag.CreateBuilder();
            _initLevelSubscriber = GlobalMessagePipe.GetSubscriber<InitializeLevelMessage>();
            _initLevelSubscriber.Subscribe(message => SetEnvironmentIdentifier(message.EnvironmentIdentifier))
                                .AddTo(builder);
            _initLevelDisposable = builder.Build();
        }

        private void GenerateLevel()
        {
            GenerateLevelAsync().Forget();
        }

        private async UniTask GenerateLevelAsync()
        {
            if (PlayGameConfig.Current != null)
            {
                string levelName = PlayGameConfig.Current.PlayLevelName;
                string path = $"Normal Levels/{levelName}.unity";
                await AddressablesUtils.LoadSceneViaAddressable(path, LoadSceneMode.Additive);
            }
        }

        public void SetEnvironmentIdentifier(EnvironmentIdentifier environmentIdentifier)
        {
            EnvironmentIdentifier = environmentIdentifier;
        }

        private void OnDestroy()
        {
            _initLevelDisposable.Dispose();
        }
    }
}
