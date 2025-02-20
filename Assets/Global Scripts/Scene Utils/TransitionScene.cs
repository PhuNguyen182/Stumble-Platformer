using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StumblePlatformer.Scripts.Common.Enums;
using Cysharp.Threading.Tasks;
using GlobalScripts.Audios;

namespace GlobalScripts.SceneUtils
{
    public class TransitionScene : MonoBehaviour
    {
        [SerializeField] private Animator darkCurtain;

        private CancellationToken _token;
        private readonly int _outHash = Animator.StringToHash("Out");

        public static TransitionScene Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            _token = this.GetCancellationTokenOnDestroy();

#if !UNITY_EDITOR
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
#endif
        }

        private void Start()
        {
            LoadNextScene().Forget();
            WaitingPopup.Setup().HideWaiting();
        }

        private async UniTask LoadNextScene()
        {
            string nextSceneName = SceneBridge.Bridge;
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: _token);

                if (darkCurtain)
                    darkCurtain.SetTrigger(_outHash);

                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: _token);

                WaitingPopup.Setup().ShowWaiting();
                await SceneLoader.LoadScene(nextSceneName);

                AudioManager.Instance.StopMusic();
                SceneBridge.Bridge = null;
            }
        }

        public async UniTask UnloadTransition()
        {
            await SceneManager.UnloadSceneAsync(SceneConstants.Transition);
        }

        private void OnDisable()
        {
            WaitingPopup.Setup(true).ShowWaiting();
        }
    }
}
