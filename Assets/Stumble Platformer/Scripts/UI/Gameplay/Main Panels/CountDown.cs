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

        private CancellationToken _token;

        private void Awake()
        {
            _token = this.GetCancellationTokenOnDestroy();
        }

        public async UniTask Countdown()
        {
            countDownText.text = "3";
            
            await UniTask.WaitForSeconds(1f, cancellationToken: _token);
            countDownText.text = "2";
            
            await UniTask.WaitForSeconds(1f, cancellationToken: _token);
            countDownText.text = "1";
            
            await UniTask.WaitForSeconds(1f, cancellationToken: _token);
            countDownText.text = "GO!";
            
            ResetCountdown(true).Forget();
        }

        public async UniTask ResetCountdown(bool useDelay)
        {
            if(useDelay)
                await UniTask.WaitForSeconds(1f, cancellationToken: _token);
            
            countDownText.text = "";
        }
    }
}
