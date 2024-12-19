using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private Rigidbody playerBody;
        [SerializeField] private CharacterConfig characterConfig;

        private float _linearDrag;
        private float _angularDrag;

        public Rigidbody PlayerBody => playerBody;
        public float JumpRestriction { get; private set; }

        private void Awake()
        {
            _linearDrag = playerBody.drag;
            _angularDrag = playerBody.angularDrag;
            JumpRestriction = 1;
        }

        public void ResetBodyValue()
        {
            playerBody.drag = _linearDrag;
            playerBody.angularDrag = _angularDrag;
            JumpRestriction = 1;
        }

        public void SetLinearDrag(float drag) => playerBody.drag = drag;

        public void SetAngularDrag(float drag) => playerBody.angularDrag = drag;

        public void SetJumpRestriction(float restriction) => JumpRestriction = restriction;

        public void SetCharacterActive(bool active) => playerBody.isKinematic = !active;
        
        public void SetFreezeRotation(bool isCharacterStunning)
        {
            playerBody.constraints = isCharacterStunning ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ
                                                         : RigidbodyConstraints.FreezeRotation;
        }

        public bool IsJumping() => playerBody.velocity.y >= -characterConfig.CheckFallSpeed;
        
        public bool IsFalling() => playerBody.velocity.y <= characterConfig.CheckFallSpeed;
        
        public void TakePhysicsDamage(PhysicalDamage damageData)
        {
            if (damageData.AttackForce != 0 && damageData.ForceDirection != Vector3.zero)
                playerBody.AddForce(damageData.AttackForce * damageData.ForceDirection, ForceMode.Impulse);
        }
    }
}
