using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StumblePlatformer.Scripts.Gameplay.Inputs
{
    public class InputReceiver : MonoBehaviour
    {
        private Camera _mainCamera;
        private PlayerInputAction _playerInput;

        public bool IsJumpHeld { get; private set; }
        public bool IsJumpPressed => IsJumpPressDown();
        
        public Vector3 Movement { get; private set; }
        public Vector2 CameraDelta { get; private set; }

        public bool IsActive { get; set; }

        private void Awake()
        {
            IsActive = true;
            _playerInput = new();

            _mainCamera = Camera.main;
            RegisterInputCallbacks();
        }

        private void OnEnable()
        {
            _playerInput.Enable();
        }

        private void RegisterInputCallbacks()
        {
            _playerInput.Player.Jump.started += JumpHandle;
            _playerInput.Player.Movement.started += MovementHandle;
            _playerInput.Player.Camera.started += CameraHandle;

            _playerInput.Player.Jump.performed += JumpHandle;
            _playerInput.Player.Movement.performed += MovementHandle;
            _playerInput.Player.Camera.performed += CameraHandle;

            _playerInput.Player.Jump.canceled += JumpHandle;
            _playerInput.Player.Movement.canceled += MovementHandle;
            _playerInput.Player.Camera.canceled += CameraHandle;
        }

        private void CameraHandle(InputAction.CallbackContext context)
        {
            CameraDelta = IsActive ? context.ReadValue<Vector2>() : Vector2.zero;
        }

        private void JumpHandle(InputAction.CallbackContext context)
        {
            IsJumpHeld = IsActive ? context.ReadValueAsButton() : false;
        }

        private void MovementHandle(InputAction.CallbackContext context)
        {
            Movement = IsActive ? context.ReadValue<Vector2>() : Vector3.zero;
        }

        private bool IsJumpPressDown()
        {
            return IsActive ? _playerInput.Player.Jump.WasPressedThisFrame() : false;
        }

        public Vector3 RotateAndScaleInput(Vector3 input)
        {
            Vector3 rotatedInput = _mainCamera.transform.rotation * new Vector3(input.x, 0, input.y);
            return Vector3.Scale(rotatedInput, new Vector3(1, 0, 1));
        }

        private void OnDisable()
        {
            _playerInput.Disable();
        }

        private void OnDestroy()
        {
            _playerInput.Dispose();
        }
    }
}
