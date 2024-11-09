using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using StumblePlatformer.Scripts.Common.Containers;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class CannonObstacle : BaseObstacle
    {
        [SerializeField] private Transform shotPoint;
        [SerializeField] private Animator cannonANimator;
        [SerializeField] private bool check;

        [Header("Shooting")]
        [SerializeField] private CannonBullet bulletPrefab;
        [SerializeField] private float shotVelocity = 12;
        [SerializeField] private float shotInterval = 2f;

        private float _shotDelay = 0;

        public override void DamageCharacter(Collision collision)
        {
            
        }

        public override void ExitDamage(Collision collision)
        {
            
        }

        public override void ObstacleAction()
        {
            _shotDelay += Time.deltaTime;
            if(_shotDelay > shotInterval)
            {
                _shotDelay = 0;
                ShootBullet();
            }
        }

        private void ShootBullet()
        {
            Vector3 shotForce = shotPoint.forward * shotVelocity;
            CannonBullet cannonBullet = SimplePool.Spawn(bulletPrefab, BulletContainer.Transform
                                                         , shotPoint.position, Quaternion.identity);
            cannonBullet.Shoot(shotForce);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (check)
            {
                check = false;
                var renderers = transform.GetComponentsInChildren<Renderer>();

                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].shadowCastingMode = ShadowCastingMode.Off;
                }
            }
        }
#endif
    }
}
