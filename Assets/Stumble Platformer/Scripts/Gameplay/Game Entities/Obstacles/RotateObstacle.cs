using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class RotateObstacle : BaseObstacle
    {
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private RotateAxis rotateAxis;

        private Vector3 _rotateAxis;

        public override void OnAwake()
        {
            _rotateAxis = rotateAxis switch
            {
                RotateAxis.X => Vector3.right,
                RotateAxis.Y => Vector3.up,
                RotateAxis.Z => Vector3.forward,
                _ => Vector3.zero
            };
        }

        public override void DamageCharacter(Collision collision)
        {
            
        }

        public override void ExitDamage(Collision collision)
        {
            
        }

        public override void ObstacleAction()
        {
            Quaternion rotation = Quaternion.Euler(_rotateAxis * rotateSpeed * Time.fixedDeltaTime);
            obstacleBody.MoveRotation(obstacleBody.rotation * rotation);
        }
    }
}
