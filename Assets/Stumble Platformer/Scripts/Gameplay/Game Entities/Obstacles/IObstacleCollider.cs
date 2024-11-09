using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public interface IObstacleCollider
    {
        public void ExitDamage(Collision collision);
    }
}
