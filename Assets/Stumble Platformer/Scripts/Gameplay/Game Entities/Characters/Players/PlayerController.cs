using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using GlobalScripts.Extensions;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("Attachments")]
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private InputReceiver inputReceiver;
        [SerializeField] private GroundChecker groundChecker;
        [SerializeField] private Rigidbody playerBody;

        [Header("Settings")]
        [SerializeField] private Transform characterPivot;
        [SerializeField] private CharacterConfig characterConfig;

        private bool _isMoving;
        private bool _isDoubleJump;
        private bool _isJumpPressed;
        private bool _isStunning;

        private int _jumpCount = 0;
        private float _stunDuration = 0;

        private Vector3 _moveInput;
        private Vector3 _moveVelocity;

        private void Update()
        {
            ReceiveInput();
            StunningTimer();
        }

        private void FixedUpdate()
        {
            if (!_isStunning)
            {
                Move();
                Turn();
                Jump();
            }
        }

        private void StunningTimer()
        {
            if (_stunDuration > 0)
                _stunDuration = _stunDuration - Time.deltaTime;

            if (_stunDuration <= 0)
                _stunDuration = 0;

            _isStunning = _stunDuration > 0;
        }

        private void ReceiveInput()
        {
            _moveInput = inputReceiver.Movement;
            _isJumpPressed = inputReceiver.IsJumpPressed;
            _isMoving = _moveInput != Vector3.zero;

            if (groundChecker.IsGrounded)
            {
                _jumpCount = 0;
                OnGrounded();
            }
        }

        private void Move()
        {
            if (_moveInput != Vector3.zero)
            {
                _moveVelocity = _moveInput * characterConfig.MoveSpeed;
                _moveVelocity.y = playerBody.velocity.y;
                playerBody.velocity = _moveVelocity + groundChecker.GroundVelocity;
            }

            playerBody.ClampVelocity(characterConfig.MaxSpeed);
            Vector3 flatMoveVelocity = new Vector3(playerBody.velocity.x, 0, playerBody.velocity.z);

            bool isRunning = flatMoveVelocity.sqrMagnitude > characterConfig.MinSpeed * characterConfig.MinSpeed;
            characterAnimator.SetBool(CharacterAnimationKeys.IsRunningKey, isRunning);

            float moveThreshold = flatMoveVelocity.magnitude / characterConfig.MoveSpeed;
            characterAnimator.SetFloat(CharacterAnimationKeys.MoveKey, moveThreshold);
        }

        private void Turn()
        {
            if(_moveInput != Vector3.zero)
            {
                float angle = Mathf.Atan2(playerBody.velocity.x, playerBody.velocity.z) * Mathf.Rad2Deg;
                Quaternion toRotation = Quaternion.Euler(0, angle, 0);
                characterPivot.localRotation = Quaternion.Slerp(characterPivot.localRotation, toRotation
                                                                , Time.deltaTime * characterConfig.RotationSmoothFactor);
            }
        }

        private void Jump()
        {
            if (_isJumpPressed)
            {
                if (groundChecker.IsGrounded)
                {
                    _jumpCount = _jumpCount + 1;
                    _moveVelocity = new Vector3(playerBody.velocity.x, characterConfig.JumpHeight, playerBody.velocity.z); 
                    playerBody.velocity = _moveVelocity;
                }
            }

            bool isFalling = IsFalling();
            bool isJumping = IsJumping();

            if(isJumping)
                characterAnimator.SetBool(CharacterAnimationKeys.IsJumpingUpKey, true);
        }

        private void OnGrounded()
        {
            _isDoubleJump = false;
            characterAnimator.SetBool(CharacterAnimationKeys.IsJumpingUpKey, false);
        }

        private bool IsJumping()
        {
            return playerBody.velocity.y >= -characterConfig.CheckFallSpeed;
        }

        private bool IsFalling()
        {
            return playerBody.velocity.y <= characterConfig.CheckFallSpeed;
        }

        public void TakeDamage(DamageData damageData)
        {
            _stunDuration = damageData.StunDuration;

        }
    }
}