using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.Characters
{
    public struct CharacterAnimationKeys
    {
        private const string Move = "Move";
        private const string IsRunning = "IsRunning";
        private const string IsJumpingUp = "IsJumpingUp";
        private const string IsJumpingDown = "IsJumpingDown";
        private const string IsStumbled = "IsStumbled";

        public static readonly int MoveKey = Animator.StringToHash(Move);
        public static readonly int IsRunningKey = Animator.StringToHash(IsRunning);
        public static readonly int IsJumpingUpKey = Animator.StringToHash(IsJumpingUp);
        public static readonly int IsJumpingDownKey = Animator.StringToHash(IsJumpingDown);
        public static readonly int IsStumbledKey = Animator.StringToHash(IsStumbled);
    }
}
