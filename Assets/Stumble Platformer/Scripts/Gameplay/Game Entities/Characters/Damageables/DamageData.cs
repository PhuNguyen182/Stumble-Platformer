using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables
{
    public struct DamageData
    {
        public int DamageAmount;
        public float StunDuration;
        public Vector3 ForceDirection;
        public DamageType DamageType;
        public float AttackForce;
    }
}
