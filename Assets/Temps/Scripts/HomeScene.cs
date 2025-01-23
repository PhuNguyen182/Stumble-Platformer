using Cysharp.Threading.Tasks;
using StumblePlatformer.Scripts.Multiplayers;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HomeScene : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            NetworkManager.Singleton.SceneManager.LoadScene("Level 3", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void Host()
    {
        CreateRoom().Forget();
    }

    public void Client()
    {
        JoinRoom().Forget();
    }

    private async UniTask CreateRoom()
    {
        bool canCreate = await LobbyManager.Instance.CreateLobby("a", false);
        if (canCreate)
            NetworkManager.Singleton.SceneManager.LoadScene("Waiting Test", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private async UniTask JoinRoom()
    {
        await LobbyManager.Instance.JoinLobby();
    }
}
