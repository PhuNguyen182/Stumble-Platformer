using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.Characters
{
    [CreateAssetMenu(fileName = "Character Config", menuName = "Scriptable Objects/Character/Character Config")]
    public class CharacterConfig : ScriptableObject
    {
        public float JumpHeight;
        public float MoveSpeed;
        public float MaxSpeed;
        public float MinSpeed;
        public float CheckFallSpeed;
        public float RotationSmoothFactor;
    }
}
