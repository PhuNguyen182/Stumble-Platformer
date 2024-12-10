using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    [RequireComponent(typeof(Rigidbody))] 
    public class CannonBullet : MonoBehaviour, IObstacleDamager
    {
        [SerializeField] private float attackForce = 10f;
        [SerializeField] private float stunDuration = 3f;
        [SerializeField] private Rigidbody bulletBody;

        private const string DeadZoneTag = "DeadZone";
        private HashSet<int> _characterIdCollection = new();

        private void OnEnable()
        {
            ResetBulletIDCollection();
        }

        public void Shoot(Vector3 force)
        {
            bulletBody.velocity = force;
        }

        public void DamageCharacter(Collision collision)
        {
            if (!collision.transform.TryGetComponent(out ICharacterMovement character) || !collision.transform.TryGetComponent(out IDamageable damageable))
                return;

            int characterID = collision.transform.GetInstanceID();
            
            if (!_characterIdCollection.Contains(characterID))
            {
                Vector3 hitNormal = collision.GetContact(0).normal;
                float hitImpulse = collision.GetContact(0).impulse.magnitude * 1.5f;

                damageable.TakeDamage(new DamageData
                {
                    AttackForce = attackForce + hitImpulse,
                    ForceDirection = -hitNormal,
                    StunDuration = stunDuration
                });
            }

            _characterIdCollection.Add(characterID);
        }

        private void OnCollisionEnter(Collision collision)
        {
            DamageCharacter(collision);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(DeadZoneTag))
            {
                SimplePool.Despawn(this.gameObject);
            }
        }

        private void ResetBulletIDCollection()
        {
            _characterIdCollection.Clear();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            bulletBody ??= GetComponent<Rigidbody>();
        }
#endif
    }
}
