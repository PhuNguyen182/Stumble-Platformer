using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

namespace StumblePlatformer.Scripts.Scenes.Transition
{
    public class LoadingText : MonoBehaviour
    {
        [SerializeField] private TMP_Text loadingText;
        private CancellationToken _token;

        private void Awake()
        {
            _token = this.GetCancellationTokenOnDestroy();
        }

        private void Start()
        {
            Loading().Forget();
        }

        private async UniTask Loading()
        {
            int count = 0;
            while (true)
            {
                count = count + 1;
                if (count > 3)
                    count = 0;

                if (count == 0)
                    loadingText.text = "Loading ";
                else if (count == 1)
                    loadingText.text = "Loading .";
                else if (count == 2)
                    loadingText.text = "Loading ..";
                else if (count == 3)
                    loadingText.text = "Loading ...";

                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: _token);
                if (_token.IsCancellationRequested)
                    return;
            }
        }
    }
}
