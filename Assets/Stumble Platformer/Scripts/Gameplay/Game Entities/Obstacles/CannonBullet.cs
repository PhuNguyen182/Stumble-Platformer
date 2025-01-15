using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    [RequireComponent(typeof(Rigidbody))] 
    public class CannonBullet : Unity.Netcode.NetworkBehaviour, IObstacleDamager
    {
        [SerializeField] private float attackForce = 10f;
        [SerializeField] private float stunDuration = 3f;
        [SerializeField] private Rigidbody bulletBody;
        [SerializeField] private NetworkObject networkObject;

        private const float MaxImpactForce = 9;
        private const string DeadZoneTag = "DeadZone";
        
        private Vector3 _currentScale;
        private HashSet<int> _characterIdCollection = new();

        private void Awake()
        {
            _currentScale = transform.localScale;
        }

        private void Start()
        {
            //SpawnNetworkObject();
        }

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
            if (!IsOwner)
                return;

            if (!collision.transform.TryGetComponent(out ICharacterMovement character) || !collision.transform.TryGetComponent(out IDamageable damageable))
                return;

            int characterID = collision.transform.GetInstanceID();
            
            if (!_characterIdCollection.Contains(characterID))
            {
                Vector3 hitNormal = collision.GetContact(0).normal;
                float hitImpulse = collision.GetContact(0).impulse.magnitude;

                hitImpulse = hitImpulse + attackForce;
                hitImpulse = Mathf.Clamp(hitImpulse, 0, MaxImpactForce);

                damageable.TakePhysicalAttack(new PhysicalDamage
                {
                    AttackForce = hitImpulse,
                    ForceDirection = -hitNormal,
                    StunDuration = stunDuration
                });
            }

            _characterIdCollection.Add(characterID);
        }

        private void SpawnNetworkObject()
        {
            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
            {
                if (!IsSpawned)
                    networkObject.Spawn(true);
            }
            else if (GameplaySetup.PlayMode == GameMode.Multiplayer)
            {
                if (GameplaySetup.PlayerType == PlayerType.Host || GameplaySetup.PlayerType == PlayerType.Server)
                    networkObject.Spawn(true);
            }
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

        public void ModifyScale(Vector3 scale)
        {
            transform.localScale = Vector3.Scale(_currentScale, scale);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            bulletBody ??= GetComponent<Rigidbody>();
        }
#endif
    }
}
