using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public struct CharacterAnimationKeys
    {
        private const string Move = "Move";
        private const string IsRunning = "IsRunning";
        private const string IsJumpingUp = "IsJumpingUp";
        private const string IsStumbled = "IsStumbled";
        private const string Sad = "Sad";
        private const string Win = "Win";

        public static readonly int MoveKey = Animator.StringToHash(Move);
        public static readonly int IsRunningKey = Animator.StringToHash(IsRunning);
        public static readonly int IsJumpingUpKey = Animator.StringToHash(IsJumpingUp);
        public static readonly int IsStumbledKey = Animator.StringToHash(IsStumbled);
        public static readonly int WinKey = Animator.StringToHash(Win);
        public static readonly int SadKey = Animator.StringToHash(Sad);
    }
}
