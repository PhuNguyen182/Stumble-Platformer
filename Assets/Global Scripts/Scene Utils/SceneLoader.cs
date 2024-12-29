using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using GlobalScripts.Utils;

namespace GlobalScripts.SceneUtils
{
    public class SceneLoader
    {
        public static async UniTask LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            await SceneManager.LoadSceneAsync(sceneName, loadMode);
        }

        public static async UniTask LoadScene(string sceneName, IProgress<float> progress, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            AsyncOperation sceneOperator = SceneManager.LoadSceneAsync(sceneName, loadMode);
            await sceneOperator.ToUniTask(progress);
        }

        public static async UniTask LoadSceneWithCondition(string sceneName, LoadSceneCondition condition, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            AsyncOperation sceneOperator = SceneManager.LoadSceneAsync(sceneName, loadMode);
            sceneOperator.allowSceneActivation = false;

            while (!sceneOperator.isDone)
            {
                if(sceneOperator.progress >= 0.9f)
                {
                    if (condition.AllowSceneLoad)
                        sceneOperator.allowSceneActivation = true;
                }

                await UniTask.NextFrame();
            }
        }

#if UNITASK_ADDRESSABLE_SUPPORT
        public static async UniTask LoadSceneViaAddressable(string key, LoadSceneMode loadMode = LoadSceneMode.Single
            , bool activateOnLoad = true, int priority = 100, CancellationToken cancellationToken = default)
        {
            await AddressablesUtils.LoadSceneViaAddressable(key, loadMode, activateOnLoad, priority, cancellationToken);
        }
#endif
    }
}
