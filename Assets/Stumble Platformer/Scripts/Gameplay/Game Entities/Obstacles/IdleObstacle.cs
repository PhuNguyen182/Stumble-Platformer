using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class IdleObstacle : BaseObstacle
    {
        [SerializeField] private bool initializedIsKinematic;

        public override void OnAwake()
        {
            initializedIsKinematic = obstacleBody.isKinematic;
        }

        public override void ExitDamage(Collision collision)
        {
            
        }

        public override void DamageCharacter(Collision collision)
        {
            
        }

        public override void ObstacleAction()
        {
            
        }
    }
}
