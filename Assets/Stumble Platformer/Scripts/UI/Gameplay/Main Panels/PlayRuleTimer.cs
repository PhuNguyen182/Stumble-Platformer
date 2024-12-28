using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Gameplay.MainPanels
{
    public class PlayRuleTimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;

        private TimeSpan _timer;

        public void UpdateTime(float duration)
        {
            _timer = TimeSpan.FromSeconds(duration);
            timerText.SetText($"{_timer.Minutes:D2}:{_timer.Seconds:D2}");
        }
    }
}
