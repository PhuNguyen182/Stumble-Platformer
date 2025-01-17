using GlobalScripts.SceneUtils;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScene : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        WaitingPopup.Setup().HideWaiting();

        if(IsServer)
            NetworkManager.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
            NetworkManager.SceneManager.LoadScene("Test Gameplay", LoadSceneMode.Additive);
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (string.CompareOrdinal(sceneName, "Test Gameplay") == 0)
        {
            NetworkManager.SceneManager.LoadScene("Test Scene", LoadSceneMode.Additive);
        }
    }
}
