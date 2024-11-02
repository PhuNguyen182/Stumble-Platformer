using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public interface IObstacle
    {
        public void ExitDamage(Collision collision);
        public void DamageCharacter(Collision collision);
        public void ObstacleAction();
    }
}
