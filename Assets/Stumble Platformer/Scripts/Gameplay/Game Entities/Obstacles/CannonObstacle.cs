using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using StumblePlatformer.Scripts.Common.Containers;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class CannonObstacle : BaseObstacle
    {
        [SerializeField] private Transform shotPoint;
        [SerializeField] private Animator cannonAnimator;
        [SerializeField] private AudioClip cannonClip;
        [SerializeField] private AudioSource cannonAudio;
        [SerializeField] private bool check;

        [Header("Shooting")]
        [SerializeField] private CannonBullet bulletPrefab;
        [SerializeField] private float shotDelay = 0;
        [SerializeField] private float shotVelocity = 12;
        [SerializeField] private float shotInterval = 2f;

        private float _shotDelay = 0;
        private readonly int _fireHash = Animator.StringToHash("Fire");

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
                ShootBullet().Forget();
            }
        }

        private async UniTask ShootBullet()
        {
            cannonAnimator.SetTrigger(_fireHash);
            await UniTask.WaitForSeconds(shotDelay, cancellationToken: destroyCancellationToken);

            Vector3 shotForce = shotPoint.forward * shotVelocity;
            CannonBullet cannonBullet = SimplePool.Spawn(bulletPrefab, BulletContainer.Transform
                                                         , shotPoint.position, Quaternion.identity);
            cannonBullet.ModifyScale(transform.localScale);
            cannonAudio.PlayOneShot(cannonClip, 0.2f);
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
