using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using GlobalScripts.Extensions;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerController : MonoBehaviour, ICharacterMovement, IDamageable, ISetCharacterActive
    {
        [Header("Attachments")]
        [SerializeField] private Rigidbody playerBody;
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private InputReceiver inputReceiver;
        [SerializeField] private GroundChecker groundChecker;
        [SerializeField] private CameraPointer cameraPointer;

        [Header("Settings")]
        [SerializeField] private Transform characterPivot;
        [SerializeField] private CharacterConfig characterConfig;

        private bool _isMoving;
        private bool _isAirDashing;
        private bool _isJumpPressed;
        private bool _isStunning;

        private float _stunDuration = 0;

        private Vector3 _moveInput;
        private Vector3 _moveVelocity;

        public bool IsStunning => _isStunning;

        private void Update()
        {
            ReceiveInput();
            StunningTimer();
        }

        private void FixedUpdate()
        {
            if (!_isStunning && !_isAirDashing)
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
            {
                _stunDuration = 0;
                
                if (!_isAirDashing)
                    SetStunningState(false);
            }

            _isStunning = _stunDuration > 0;
        }

        private void ReceiveInput()
        {
            _isJumpPressed = inputReceiver.IsJumpPressed;
            _moveInput = inputReceiver.RotateAndScaleInput(inputReceiver.Movement);
            _isMoving = _moveInput != Vector3.zero;

            if (groundChecker.IsGrounded)
                OnGrounded();
        }

        private void Move()
        {
            if (_moveInput != Vector3.zero)
            {
                _moveVelocity = _moveInput * characterConfig.MoveSpeed;
                _moveVelocity.y = playerBody.velocity.y;
                playerBody.velocity = _moveVelocity + groundChecker.FlatGroundVelocity;
            }

            ProcessGroundVelocity();
            playerBody.ClampVelocity(characterConfig.MaxSpeed);
            Vector3 flatMoveVelocity = new Vector3(playerBody.velocity.x, 0, playerBody.velocity.z) - groundChecker.FlatGroundVelocity;

            bool isRunning = flatMoveVelocity.sqrMagnitude > characterConfig.MinSpeed * characterConfig.MinSpeed;
            characterAnimator.SetBool(CharacterAnimationKeys.IsRunningKey, isRunning);

            float moveThreshold = flatMoveVelocity.magnitude / characterConfig.MoveSpeed;
            characterAnimator.SetFloat(CharacterAnimationKeys.MoveKey, moveThreshold);

            characterAnimator.SetBool(CharacterAnimationKeys.IsFallingKey, !groundChecker.IsGrounded && IsFalling());
        }

        private void Turn()
        {
            if(_moveInput != Vector3.zero && !_isStunning)
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
                    _moveVelocity = new Vector3(playerBody.velocity.x, characterConfig.JumpHeight, playerBody.velocity.z);
                    playerBody.velocity = _moveVelocity + groundChecker.GroundVelocity;
                }

                else
                {
                    if (_moveInput != Vector3.zero)
                    {
                        _isAirDashing = true;
                        _moveVelocity = characterPivot.forward * characterConfig.DashSpeed;
                        playerBody.velocity = _moveVelocity;
                        characterAnimator.SetBool(CharacterAnimationKeys.IsStumbledKey, true);
                    }
                }
            }

            bool isJumping = IsJumping();

            if(isJumping)
                characterAnimator.SetBool(CharacterAnimationKeys.IsJumpingUpKey, true);
        }

        private void OnGrounded()
        {
            _isAirDashing = false;
            characterAnimator.SetBool(CharacterAnimationKeys.IsJumpingUpKey, false);

            if (!_isStunning)
                characterAnimator.SetBool(CharacterAnimationKeys.IsStumbledKey, false);
        }

        private bool IsJumping()
        {
            return playerBody.velocity.y >= -characterConfig.CheckFallSpeed;
        }

        private bool IsFalling()
        {
            return playerBody.velocity.y <= characterConfig.CheckFallSpeed;
        }

        private void ProcessGroundVelocity()
        {
            if (groundChecker.HasMoveableGround && _moveInput == Vector3.zero)
            {
                Vector3 groundVelocity = groundChecker.GroundVelocity;
                Vector3 horizontalGroundVelocity = new(groundVelocity.x, 0, groundVelocity.z);
                float verticalVelocity = groundChecker.GroundVelocity.y >= 0 ? playerBody.velocity.y
                                               : groundChecker.GroundVelocity.y + playerBody.velocity.y;

                Vector3 newVelocity = horizontalGroundVelocity + Vector3.up * verticalVelocity;
                playerBody.velocity = newVelocity;
            }
        }

        private void SetStunningState(bool isStunning)
        {
            SetFreezeRotation(isStunning);
            characterAnimator.SetBool(CharacterAnimationKeys.IsStumbledKey, isStunning);
        }

        private void SetFreezeRotation(bool isCharacterStunning)
        {
            playerBody.constraints = isCharacterStunning ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ
                                                         : RigidbodyConstraints.FreezeRotation;
            if (!isCharacterStunning)
            {
                playerBody.rotation = Quaternion.identity;

                if (_isStunning)
                {
                    cameraPointer.ControlCameraAngle();
                }
            }
        }

        public void SetCharacterActive(bool active)
        {
            playerBody.isKinematic = !active;
        }

        public void TakeDamage(DamageData damageData)
        {
            if (_isStunning)
                return;

            _isStunning = true;
            _stunDuration = damageData.StunDuration;
            SetStunningState(true);

            if (damageData.AttackForce != 0 && damageData.ForceDirection != Vector3.zero)
                playerBody.AddForce(damageData.AttackForce * damageData.ForceDirection, ForceMode.Impulse);
        }
    }
}
