using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.SceneUtils;
using Cysharp.Threading.Tasks;

public class TestLoadScene : MonoBehaviour
{
    private LoadSceneCondition condition = new();

    private void Start()
    {
        LoadScene().Forget();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            condition.AllowSceneLoad = true;
    }

    private async UniTask LoadScene()
    {
        await SceneLoader.LoadSceneWithCondition(SceneConstants.Mainhome, condition);
    }
}
