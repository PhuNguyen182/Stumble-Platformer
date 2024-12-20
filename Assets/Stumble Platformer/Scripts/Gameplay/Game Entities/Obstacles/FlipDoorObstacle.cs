using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.Extensions;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class FlipDoorObstacle : BaseObstacle
    {
        [SerializeField] private float pressDownForce = 2;
        [SerializeField] private float activateDuration = 3f;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private Transform attackPoint;

        private float _duration = 0;
        private bool _hasStepped = false;

        public override void OnAwake()
        {
            base.OnAwake();
            obstacleBody.isKinematic = true;
        }

        public override void SetObstacleActive(bool active)
        {
            base.SetObstacleActive(active);
            if (!active)
                obstacleBody.isKinematic = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.HasLayer(playerLayer))
            {
                _hasStepped = true;
            }
        }

        public override void DamageCharacter(Collision collision)
        {
            
        }

        public override void ExitDamage(Collision collision)
        {
            
        }

        public override void ObstacleAction()
        {
            if (!_hasStepped)
                return;

            if(_duration < activateDuration)
            {
                _duration += Time.deltaTime;
                if (_duration >= activateDuration && IsActive)
                {
                    obstacleBody.isKinematic = false;
                    obstacleBody.AddForceAtPosition(Vector3.down * pressDownForce, attackPoint.position, ForceMode.Impulse);
                }
            }
        }
    }
}
