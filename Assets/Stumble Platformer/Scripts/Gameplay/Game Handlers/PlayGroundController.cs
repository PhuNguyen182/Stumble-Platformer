using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StumblePlatformer.Scripts.Common.SingleConfigs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using Cysharp.Threading.Tasks;
using GlobalScripts.Utils;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    public class PlayGroundController : MonoBehaviour
    {
        [SerializeField] private PlayerController playerPrefab;

        private PlayerController _currentPlayer;
        public PlayerController CurrentPlayer => _currentPlayer;

        private void Start()
        {
            GenerateLevel().Forget();
        }

        private async UniTask GenerateLevel()
        {
            if (PlayGameConfig.Current.IsAvailable)
            {
                string levelName = PlayGameConfig.Current.PlayLevelName;
                string path = $"Normal Levels/{levelName}.unity";
                await AddressablesUtils.LoadSceneViaAddressable(path, LoadSceneMode.Additive);
            }
        }
    }
}
