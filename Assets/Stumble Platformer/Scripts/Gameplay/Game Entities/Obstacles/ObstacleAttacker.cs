using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class ObstacleAttacker : MonoBehaviour
    {
        public bool CanAttack { get; set; }

        public Action<Collision> ExitDamage;
        public Action<Collision> DamageAttack;

        private void OnCollisionEnter(Collision collision)
        {
            if (CanAttack)
                DamageAttack?.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (CanAttack)
                DamageAttack?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            ExitDamage?.Invoke(collision);
        }
    }
}
