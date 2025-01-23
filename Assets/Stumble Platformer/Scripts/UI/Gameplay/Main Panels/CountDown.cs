using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cysharp.Threading.Tasks;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Gameplay.MainPanels
{
    public class CountDown : NetworkBehaviour
    {
        [SerializeField] private TMP_Text countDownText;
        [SerializeField] private AudioSource countDownAudio;
        [SerializeField] private AudioClip countDownClip;
        [SerializeField] private AudioClip countGoClip;

        private CancellationToken _token;

        private void Awake()
        {
            _token = this.GetCancellationTokenOnDestroy();
        }

        public async UniTask Countdown()
        {
            CountSound(countDownClip);
            countDownText.text = "3";

            await UniTask.WaitForSeconds(1f, cancellationToken: _token);
            countDownText.text = "2";
            CountSound(countDownClip);
            
            await UniTask.WaitForSeconds(1f, cancellationToken: _token);
            countDownText.text = "1";
            CountSound(countDownClip);
            
            await UniTask.WaitForSeconds(1f, cancellationToken: _token);
            countDownText.text = "GO!";
            CountSound(countGoClip);
            
            ResetCountdown(true).Forget();
        }

        public async UniTask ResetCountdown(bool useDelay)
        {
            if(useDelay)
                await UniTask.WaitForSeconds(1f, cancellationToken: _token);
            
            countDownText.text = "";
        }

        private void CountSound(AudioClip clip)
        {
            countDownAudio.PlayOneShot(clip, 0.1f);
        }
    }
}
