using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using GlobalScripts.Extensions;
using Unity.Netcode;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerController : NetworkBehaviour, ISetCharacterInput, ICharacterMovement, IDamageable, ISetCharacterActive
    {
        [SerializeField] private PlayerHealth playerHealth;

        [Header("Movement")]
        [SerializeField] private PlayerPhysics playerPhysics;
        [SerializeField] private GroundChecker groundChecker;

        [Header("Graphics")]
        [SerializeField] private PlayerGraphics playerGraphics;
        [SerializeField] private CharacterVisual characterVisual;

        [Header("Settings")]
        [SerializeField] private Transform characterPivot;
        [SerializeField] private CharacterConfig characterConfig;

        private bool _isAirDashing;
        private bool _isJumpPressed;
        private bool _isInputMoving;
        private float _groundDirectionWeight;
        private bool _isStunning;

        private float _stunDuration = 0;
        private InputReceiver _inputReceiver;

        private Vector3 _moveInput;
        private Vector3 _moveVelocity;
        private Vector3 _velocity;

        private Rigidbody _playerBody;
        public bool IsActive { get; set; }
        public bool IsStunning => _isStunning;

        public int PlayerID => gameObject.GetInstanceID();
        public PlayerGraphics PlayerGraphics => playerGraphics;
        public PlayerHealth PlayerHealth => playerHealth;

        private void Start()
        {
            SetupPlayerGraphic();
            _playerBody = playerPhysics.PlayerBody;
        }

        private void Update()
        {
            ReceiveInput();
            StunningTimer();
            playerGraphics.UpdateCanvas();
        }

        private void FixedUpdate()
        {
            if (IsOwner)
            {
                groundChecker.CheckGround();

                if (!_isStunning && !_isAirDashing)
                {
                    Move();
                    Turn();
                    Jump();
                }

                else playerGraphics.SetDustEffectActive(false);
            }
        }

        private void ReceiveInput()
        {
            if (!IsOwner)
                return;

            if (IsActive)
            {
                _isInputMoving = _moveInput != Vector3.zero;
                _isJumpPressed = _inputReceiver.IsJumpPressed;
                _moveInput = _inputReceiver.RotateAndScaleInput(_inputReceiver.Movement);
            }

            else
            {
                _isJumpPressed = false;
                _moveInput = Vector3.zero;
            }

            if (groundChecker.IsGrounded)
                OnGrounded();
        }

        private void Move()
        {
            MoveCharacterPosition();
            AnimateCharacterMovement();
        }

        private void MoveCharacterPosition()
        {
            if (!_isInputMoving)
            {
                _playerBody.ClampVelocity(characterConfig.MaxSpeed);
                return;
            }

            _moveVelocity = _moveInput.normalized * characterConfig.MoveSpeed;
            _moveVelocity.y = _playerBody.velocity.y;

            if (groundChecker.GroundVelocity != Vector3.zero)
            {
                _groundDirectionWeight = Vector3.Dot(_moveVelocity, groundChecker.GroundVelocity);
                _groundDirectionWeight = Mathf.Clamp(_groundDirectionWeight, 0, 1);
                _velocity = _moveVelocity + _groundDirectionWeight * groundChecker.GroundVelocity;
            }

            else
            {
                _groundDirectionWeight = 0;
                _velocity = _moveVelocity;
            }

            _playerBody.velocity = _velocity;
            _playerBody.ClampVelocity(characterConfig.MaxSpeed);
        }

        private void AnimateCharacterMovement()
        {
            Vector3 playerLocalVelocity = _playerBody.velocity - groundChecker.GroundVelocity;
            float moveThreshold = playerLocalVelocity.sqrMagnitude / (characterConfig.MoveSpeed * characterConfig.MoveSpeed);
            
            bool isRunningOnPlatform = playerLocalVelocity.sqrMagnitude > characterConfig.MinSpeed;
            bool isRunningOnGround = _playerBody.velocity.sqrMagnitude > characterConfig.MinSpeed && groundChecker.IsGrounded;
            bool isRunning = !groundChecker.IsPlatformGround ? isRunningOnGround : isRunningOnPlatform;
            bool isFalling = !groundChecker.IsGrounded && playerPhysics.IsFalling();

            characterVisual.SetRunning(isRunning);
            characterVisual.SetMove(moveThreshold);
            characterVisual.SetInputMoving(_isInputMoving);
            characterVisual.SetFalling(isFalling);

            bool isDustGrounding = groundChecker.IsGrounded && !groundChecker.IsPlatformGround && _isInputMoving;
            bool isDustEffect = _isStunning ? false : isDustGrounding;
            playerGraphics.SetDustEffectActive(isDustEffect);
        }

        private void Turn()
        {
            if (_playerBody.velocity.sqrMagnitude <= characterConfig.RotateVelocityThreshold || _isStunning)
                return;

            Quaternion inversedRotation = Quaternion.Inverse(transform.localRotation);
            float angle = Mathf.Atan2(_playerBody.velocity.x, _playerBody.velocity.z) * Mathf.Rad2Deg;
            Quaternion toRotation = Quaternion.Euler(0, angle + inversedRotation.eulerAngles.y, 0);

            if (transform.parent != null)
                toRotation = toRotation * Quaternion.Inverse(transform.parent.rotation);

            characterPivot.localRotation = Quaternion.Slerp(characterPivot.localRotation, toRotation
                                                            , Time.deltaTime * characterConfig.RotationSmoothFactor);
        }

        private void Jump()
        {
            if (_isJumpPressed)
            {
                if (groundChecker.IsGrounded)
                {
                    float jumpHeight = characterConfig.JumpHeight * playerPhysics.JumpRestriction;
                    _moveVelocity = new Vector3(_playerBody.velocity.x, jumpHeight, _playerBody.velocity.z);
                    _playerBody.velocity = _moveVelocity;
                }

                else
                {
                    if (_moveInput != Vector3.zero)
                    {
                        _isAirDashing = true;
                        _moveVelocity = characterPivot.forward * characterConfig.DashSpeed;
                        _playerBody.velocity = _moveVelocity;
                        characterVisual.SetStumble(true);
                    }
                }
            }

            if (playerPhysics.IsJumping())
                characterVisual.SetJump(!groundChecker.IsPlatformGround);
        }

        private void StunningTimer()
        {
            if (_stunDuration > 0)
                _stunDuration = _stunDuration - Time.deltaTime;

            if (_stunDuration <= 0)
            {
                _stunDuration = 0;
                
                if (!_isAirDashing && groundChecker.IsGrounded)
                    SetStunningState(false);
            }

            _isStunning = _stunDuration > 0;
        }

        private void SetStunningState(bool isStunning)
        {
            playerPhysics.SetFreezeRotation(isStunning);
            characterVisual.SetStumble(isStunning);
        }

        public void OnGrounded()
        {
            _isAirDashing = false;
            characterVisual.SetJump(false);

            if (!_isStunning)
                characterVisual.SetStumble(false);
        }

        public void ResetPlayerOrientation(Quaternion orientation)
        {
            characterPivot.localRotation = orientation;
        }

        public void SetCharacterInput(InputReceiver inputReceiver)
        {
            _inputReceiver = inputReceiver;
        }

        public void SetupPlayerGraphic()
        {
            if (playerGraphics.CharacterVisual != null)
                characterVisual = playerGraphics.CharacterVisual;
        }

        public void AfterRespawn()
        {
            _stunDuration = 0;
            _isStunning = false;
            playerHealth.OnRespawn();
        }

        public void SetCharacterActive(bool active)
        {
            playerPhysics.SetCharacterActive(active);
        }

        public void TakePhysicalAttack(PhysicalDamage damageData)
        {
            if (!IsOwner)
                return;

            if (!_isStunning)
            {
                _isStunning = true;
                _stunDuration = damageData.StunDuration;

                playerPhysics.TakePhysicsDamage(damageData);
                SetStunningState(true);
            }
        }

        public void TakeHealthDamage(HealthDamage damageData)
        {
            if (IsOwner)
                playerHealth.TakeDamage(damageData);
        }

        public int GetCheckPointIndex()
        {
            return playerHealth.CheckPointIndex;
        }
    }
}
