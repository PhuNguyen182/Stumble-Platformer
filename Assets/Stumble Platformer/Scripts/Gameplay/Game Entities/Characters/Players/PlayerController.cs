using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using GlobalScripts.Extensions;
using Unity.Netcode;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerController : NetworkBehaviour, ISetCharacterInput, ICharacterMovement, ICharacterParentSetter, IDamageable, ISetCharacterActive
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
        private bool _isStunning;

        private float _stunDuration = 0;
        private InputReceiver _inputReceiver;

        private Vector3 _moveInput;
        private Vector3 _flatMoveVelocity;
        private Vector3 _moveVelocity;

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
                groundChecker.CheckGround();

            if (!_isStunning && !_isAirDashing)
            {
                Move();
                Turn();
                Jump();
            }

            else playerGraphics.SetDustEffectActive(false);
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

        private void ReceiveInput()
        {
            if (!IsOwner)
                return;

            if (IsActive)
            {
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
            if (_moveInput != Vector3.zero)
            {
                _moveVelocity = _moveInput.normalized * characterConfig.MoveSpeed;
                _moveVelocity.y = _playerBody.velocity.y;
                _playerBody.velocity = _moveVelocity;
            }

            _playerBody.ClampVelocity(characterConfig.MaxSpeed);
            _flatMoveVelocity = _playerBody.GetFlatVelocity();

            bool isInputMoving = _moveInput != Vector3.zero;
            bool isRunning = _flatMoveVelocity.magnitude > characterConfig.MinSpeed && groundChecker.IsGrounded;
            bool isFalling = !groundChecker.IsGrounded && playerPhysics.IsFalling();
            float moveThreshold = _flatMoveVelocity.magnitude / characterConfig.MoveSpeed;

            characterVisual.SetRunning(isRunning);
            characterVisual.SetInputMoving(isInputMoving);
            characterVisual.SetMove(moveThreshold);
            characterVisual.SetFalling(isFalling);

            bool isDustEffect = _isStunning ? false : groundChecker.IsGrounded && !groundChecker.IsPlatformGround && isInputMoving;
            playerGraphics.SetDustEffectActive(isDustEffect);
        }

        private void Turn()
        {
            if(_flatMoveVelocity.sqrMagnitude > characterConfig.RotateVelocityThreshold && !_isStunning)
            {
                Quaternion inversedRotation = Quaternion.Inverse(transform.localRotation);
                float angle = Mathf.Atan2(_playerBody.velocity.x, _playerBody.velocity.z) * Mathf.Rad2Deg;
                Quaternion toRotation = Quaternion.Euler(0, angle + inversedRotation.eulerAngles.y, 0);
                
                if (transform.parent != null)
                    toRotation = toRotation * Quaternion.Inverse(transform.parent.rotation);

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
                characterVisual.SetJump(true);
        }

        public void OnGrounded()
        {
            _isAirDashing = false;
            characterVisual.SetJump(false);

            if (!_isStunning)
                characterVisual.SetStumble(false);
        }

        private void SetStunningState(bool isStunning)
        {
            playerPhysics.SetFreezeRotation(isStunning);
            characterVisual.SetStumble(isStunning);
        }

        public void SetParent(Transform parent, bool stayWorldPosition = true)
        {
            if(parent != null)
                transform.SetParent(parent, stayWorldPosition);
            else 
                transform.SetParent(null);
        }

        public bool SetNetworkParent(NetworkObject parent)
        {
            if (!IsOwner) return false;
            return parent ? NetworkObject.TrySetParent(parent) : NetworkObject.TryRemoveParent();
        }

        public void SetCharacterActive(bool active)
        {
            playerPhysics.SetCharacterActive(active);
        }

        public void TakePhysicalAttack(PhysicalDamage damageData)
        {
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
            playerHealth.TakeDamage(damageData);
        }

        public int GetCheckPointIndex()
        {
            return playerHealth.CheckPointIndex;
        }
    }
}
