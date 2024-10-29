using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables
{
    public interface IDamageable
    {
        public void TakeDamage(DamageData damageData);
    }
}
