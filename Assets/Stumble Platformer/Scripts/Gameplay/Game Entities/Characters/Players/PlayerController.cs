using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using GlobalScripts.Extensions;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerController : MonoBehaviour, ICharacterMovement, ICharacterParentSetter, IDamageable, ISetCharacterActive
    {
        [Header("Movement")]
        [SerializeField] private Rigidbody playerBody;
        [SerializeField] private PlayerPhysics playerPhysics;
        [SerializeField] private GroundChecker groundChecker;
        [SerializeField] private CameraPointer cameraPointer;

        [Header("Graphics")]
        [SerializeField] private PlayerGraphics playerGraphics;
        [SerializeField] private CharacterVisual characterVisual;

        [Header("Settings")]
        [SerializeField] private Transform characterPivot;
        [SerializeField] private CharacterConfig characterConfig;

        private bool _isMoving;
        private bool _isAirDashing;
        private bool _isJumpPressed;
        private bool _isStunning;

        private float _stunDuration = 0;
        private InputReceiver _inputReceiver;

        private Vector3 _moveInput;
        private Vector3 _flatMoveVelocity;
        private Vector3 _moveVelocity;

        public bool IsStunning => _isStunning;
        public PlayerGraphics PlayerGraphics => playerGraphics;

        private void Start()
        {
            _inputReceiver = InputReceiver.Instance;

            if (playerGraphics.CharacterVisual != null)
                characterVisual = playerGraphics.CharacterVisual;
        }

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
            _isJumpPressed = _inputReceiver.IsJumpPressed;
            _moveInput = _inputReceiver.RotateAndScaleInput(_inputReceiver.Movement);
            _isMoving = _moveInput != Vector3.zero;

            if (groundChecker.IsGrounded)
                OnGrounded();
        }

        private void Move()
        {
            if (_moveInput != Vector3.zero)
            {
                _moveVelocity = _moveInput.normalized * characterConfig.MoveSpeed;
                _moveVelocity.y = playerBody.velocity.y;
                playerBody.velocity = _moveVelocity;
            }

            playerBody.ClampVelocity(characterConfig.MaxSpeed);
            _flatMoveVelocity = playerBody.GetFlatVelocity();

            bool isRunning = _flatMoveVelocity.sqrMagnitude > characterConfig.MinSpeed * characterConfig.MinSpeed;
            characterVisual.CharacterAnimator.SetBool(CharacterAnimationKeys.IsRunningKey, isRunning);

            float moveThreshold = _flatMoveVelocity.magnitude / characterConfig.MoveSpeed;
            characterVisual.CharacterAnimator.SetFloat(CharacterAnimationKeys.MoveKey, moveThreshold);

            characterVisual.CharacterAnimator.SetBool(CharacterAnimationKeys.IsFallingKey, !groundChecker.IsGrounded && playerPhysics.IsFalling());
        }

        private void Turn()
        {
            if(_flatMoveVelocity.sqrMagnitude > characterConfig.RotateVelocityThreshold && !_isStunning)
            {
                Quaternion inversedRotation = Quaternion.Inverse(transform.localRotation);
                float angle = Mathf.Atan2(playerBody.velocity.x, playerBody.velocity.z) * Mathf.Rad2Deg;
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
                    _moveVelocity = new Vector3(playerBody.velocity.x, characterConfig.JumpHeight, playerBody.velocity.z);
                    playerBody.velocity = _moveVelocity;
                }

                else
                {
                    if (_moveInput != Vector3.zero)
                    {
                        _isAirDashing = true;
                        _moveVelocity = characterPivot.forward * characterConfig.DashSpeed;
                        playerBody.velocity = _moveVelocity;
                        characterVisual.CharacterAnimator.SetBool(CharacterAnimationKeys.IsStumbledKey, true);
                    }
                }
            }

            bool isJumping = playerPhysics.IsJumping();

            if(isJumping)
                characterVisual.CharacterAnimator.SetBool(CharacterAnimationKeys.IsJumpingUpKey, true);
        }

        private void OnGrounded()
        {
            _isAirDashing = false;
            characterVisual.CharacterAnimator.SetBool(CharacterAnimationKeys.IsJumpingUpKey, false);

            if (!_isStunning)
                characterVisual.CharacterAnimator.SetBool(CharacterAnimationKeys.IsStumbledKey, false);
        }

        private void SetStunningState(bool isStunning)
        {
            playerPhysics.SetFreezeRotation(isStunning);
            characterVisual.CharacterAnimator.SetBool(CharacterAnimationKeys.IsStumbledKey, isStunning);
        }

        public void SetParent(Transform parent, bool stayWorldPosition = true)
        {
            if(parent != null)
                transform.SetParent(parent, stayWorldPosition);
            else 
                transform.SetParent(null);
        }

        public void SetCharacterActive(bool active)
        {
            playerPhysics.SetCharacterActive(active);
        }

        public void TakeDamage(DamageData damageData)
        {
            if (_isStunning)
                return;

            _isStunning = true;
            _stunDuration = damageData.StunDuration;

            SetStunningState(true);
            playerPhysics.TakeDamage(damageData);
        }
    }
}
