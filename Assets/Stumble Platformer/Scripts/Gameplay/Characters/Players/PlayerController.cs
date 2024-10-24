using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using GlobalScripts.Extensions;

namespace StumblePlatformer.Scripts.Gameplay.Characters.Players
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Attachments")]
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private InputReceiver inputReceiver;
        [SerializeField] private GroundChecker groundChecker;
        [SerializeField] private Rigidbody playerBody;

        [Header("Settings")]
        [SerializeField] private Transform characterPivot;
        [SerializeField] private CharacterConfig characterConfig;

        private bool _isJumped;
        private bool _isStunning;
        private int _jumpCount = 0;

        private Vector3 _moveInput;
        private Vector3 _moveVelocity;

        private void Update()
        {
            ReceiveInput();
        }

        private void FixedUpdate()
        {
            Move();
            Turn();
            Jump();
        }

        private void ReceiveInput()
        {
            _isJumped = inputReceiver.IsJumpPressed;
            _moveInput = inputReceiver.Movement;

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
            if (_isJumped)
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
    }
}
