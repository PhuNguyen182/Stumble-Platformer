using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StumblePlatformer.Scripts.Multiplayers;

namespace StumblePlatformer.Scripts.Scenes.Mainhome
{
    public class MainhomeCleaner : MonoBehaviour
    {
        private void Awake()
        {
            if (NetworkManager.Singleton != null)
                Destroy(NetworkManager.Singleton.gameObject);

            if (MultiplayerManager.Instance != null)
                Destroy(MultiplayerManager.Instance.gameObject);

            if (LobbyManager.Instance != null)
                Destroy(LobbyManager.Instance.gameObject);
        }
    }
}
