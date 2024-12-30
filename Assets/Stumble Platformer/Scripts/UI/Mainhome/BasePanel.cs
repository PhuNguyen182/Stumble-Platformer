using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.UI.Mainhome.MainPanels;

namespace StumblePlatformer.Scripts.UI.Mainhome
{
    public class BasePanel : MonoBehaviour
    {
        [SerializeField] protected Animator panelAnimator;

        private const string ExitTrigger = "Exit";
        private const string EnterTrigger = "Enter";
        private const string EnterIdleTrigger = "Enter Idle";
        private const string ExitIdleTrigger = "Exit Idle";

        private readonly int _exitHashKey = Animator.StringToHash(ExitTrigger);
        private readonly int _enterHashKey = Animator.StringToHash(EnterTrigger);
        private readonly int _enterIdleHashKey = Animator.StringToHash(EnterIdleTrigger);
        private readonly int _exitIdleHashKey = Animator.StringToHash(ExitIdleTrigger);

        public void EnterImmediately()
        {
            panelAnimator.SetTrigger(_enterIdleHashKey);
        }

        public void ExitImmediately()
        {
            panelAnimator.SetTrigger(_exitIdleHashKey);
        }

        public void EnterPanel()
        {
            panelAnimator.SetTrigger(_enterHashKey);
        }

        public void ExitPanel()
        {
            panelAnimator.SetTrigger(_exitHashKey);
        }
    }
}
