using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.UI.Mainhome
{
    public class BasePanel : MonoBehaviour
    {
        [SerializeField] protected PanelAnimationEnum animationType;
        [SerializeField] protected Animator panelAnimator;

        private const string ExitTrigger = "Exit";
        private const string EnterTrigger = "Enter";
        private const string EnterIdleTrigger = "Enter Idle";
        private const string ExitIdleTrigger = "Exit Idle";

        private readonly int _exitHashKey = Animator.StringToHash(ExitTrigger);
        private readonly int _enterHashKey = Animator.StringToHash(EnterTrigger);
        private readonly int _enterIdleHashKey = Animator.StringToHash(EnterIdleTrigger);
        private readonly int _exitIdleHashKey = Animator.StringToHash(ExitIdleTrigger);

        protected virtual void Start()
        {
            AnimatePanelStart();
        }

        private void AnimatePanelStart()
        {
            switch (animationType)
            {
                case PanelAnimationEnum.EnterIdle:
                    EnterImmediately();
                    break;
                case PanelAnimationEnum.Enter:
                    EnterPanel();
                    break;
                case PanelAnimationEnum.ExitIdle:
                    ExitImmediately();
                    break;
                case PanelAnimationEnum.Exit:
                    ExitPanel();
                    break;
            }
        }

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
