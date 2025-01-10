using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StumblePlatformer.Scripts.Gameplay.Inputs
{
    public class MainhomeInput : MonoBehaviour
    {
        private PlayerInputAction _playerInput;

        public bool IsQuitPress => IsQuit();

        private void Awake()
        {
            _playerInput = new();
        }

        private void OnEnable()
        {
            _playerInput.Enable();
        }

        private bool IsQuit()
            => _playerInput.Mainhome.Quit.WasPressedThisFrame();

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
