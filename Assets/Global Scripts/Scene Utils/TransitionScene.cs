using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
//using CandyMatch3.Scripts.Common.Enums;
using GlobalScripts.Audios;

namespace GlobalScripts.SceneUtils
{
    public static class SceneBridge
    {
        public static string Bridge;

        /// <summary>
        /// Load target scene via a transition scene
        /// </summary>
        /// <param name="destinationSceneName">Desired scene name to load</param>
        /// <returns></returns>
        public static async UniTask LoadNextScene(string destinationSceneName)
        {
            Bridge = destinationSceneName;
            await SceneLoader.LoadScene(SceneConstants.Transition);
        }
    }

    public class TransitionScene : MonoBehaviour
    {
        [SerializeField] private Animator darkCurtain;

        private CancellationToken _token;
        private readonly int _outHash = Animator.StringToHash("Out");

        private void Awake()
        {
            _token = this.GetCancellationTokenOnDestroy();

#if !UNITY_EDITOR
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
#endif
        }

        private void Start()
        {
            LoadNextScene().Forget();
        }

        private async UniTask LoadNextScene()
        {
            string nextSceneName = SceneBridge.Bridge;
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: _token);
                darkCurtain.SetTrigger(_outHash);

                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: _token);
                await SceneLoader.LoadScene(nextSceneName);

#if !UNITY_EDITOR
                //MusicManager.Instance.StopMusic();
                //BackgroundMusicType bgm = GetMusicBySceneName(nextSceneName);
                //MusicManager.Instance.PlayBackgroundMusic(bgm, volume: 0.6f);
#endif

                SceneBridge.Bridge = null;
            }
        }

        //private BackgroundMusicType GetMusicBySceneName(string sceneName)
        //{
        //    if (string.CompareOrdinal(sceneName, SceneConstants.Mainhome) == 0)
        //        return BackgroundMusicType.Mainhome;
            
        //    if (string.CompareOrdinal(sceneName, SceneConstants.Gameplay) == 0)
        //        return BackgroundMusicType.Gameplay;

        //    return BackgroundMusicType.None;
        //}
    }
}
