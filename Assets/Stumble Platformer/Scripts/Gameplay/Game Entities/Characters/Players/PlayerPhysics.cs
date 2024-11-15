using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerPhysics : MonoBehaviour, IDamageable
    {
        [SerializeField] private Rigidbody playerBody;
        [SerializeField] private CharacterConfig characterConfig;

        public Rigidbody GetPlayerBody()
        {
            return playerBody;
        }

        public void SetCharacterActive(bool active)
        {
            playerBody.isKinematic = !active;
        }

        public void SetFreezeRotation(bool isCharacterStunning)
        {
            playerBody.constraints = isCharacterStunning ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ
                                                         : RigidbodyConstraints.FreezeRotation;
        }

        public bool IsJumping()
        {
            return playerBody.velocity.y >= -characterConfig.CheckFallSpeed;
        }

        public bool IsFalling()
        {
            return playerBody.velocity.y <= characterConfig.CheckFallSpeed;
        }

        public void TakeDamage(DamageData damageData)
        {
            if (damageData.AttackForce != 0 && damageData.ForceDirection != Vector3.zero)
                playerBody.AddForce(damageData.AttackForce * damageData.ForceDirection, ForceMode.Impulse);
        }
    }
}
