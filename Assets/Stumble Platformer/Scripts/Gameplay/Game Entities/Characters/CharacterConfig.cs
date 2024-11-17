using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    [CreateAssetMenu(fileName = "Character Config", menuName = "Scriptable Objects/Characters/Character Config")]
    public class CharacterConfig : ScriptableObject
    {
        public float JumpHeight;
        public float MoveSpeed;
        public float DashSpeed;
        public float MaxSpeed;
        public float MinSpeed;
        public float CheckFallSpeed;
        public float RotateVelocityThreshold;
        public float RotationSmoothFactor;
    }
}
