using Unity.Netcode;
using StumblePlatformer.Scripts.Common.Enums;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay
{
    public static class GameplaySetup
    {
        public static GameMode PlayMode;
        public static PlayerType PlayerType => GetCurrentPlayerType();

        static GameplaySetup()
        {
#if UNITY_EDITOR
            Debug.Log(PlayerType);
#endif
        }

        private static PlayerType GetCurrentPlayerType()
        {
            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsHost)
                    return PlayerType.Host;
                else if (NetworkManager.Singleton.IsServer)
                    return PlayerType.Server;
                else
                    return PlayerType.Client;
            }

            else return PlayerType.None;
        }
    }
}
