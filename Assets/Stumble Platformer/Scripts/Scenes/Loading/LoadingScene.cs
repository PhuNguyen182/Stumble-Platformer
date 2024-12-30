using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using GlobalScripts.SceneUtils;

namespace StumblePlatformer.Scripts.Scenes.Loading
{
    public class LoadingScene : MonoBehaviour
    {
        private CancellationToken _token;

        private void Awake()
        {
            _token = this.GetCancellationTokenOnDestroy();
        }

        private void Start()
        {
            LoadScene().Forget();
        }

        private async UniTask LoadScene()
        {
            await UniTask.WaitForSeconds(1f, cancellationToken: _token);
            await SceneLoader.LoadScene(SceneConstants.Mainhome);
        }
    }
}
