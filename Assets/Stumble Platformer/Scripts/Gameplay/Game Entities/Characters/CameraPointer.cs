using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.Gameplay.GameHandlers;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Miscs;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public class CameraPointer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float heightAngle = 60f;
        [SerializeField] private float cameraDistance = 4.75f;
        [SerializeField] private float minCameraHeight = 0;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float rotationSpeed = 0.5f;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float heightOffsetSpeed = 0.1f;

        private bool _hasPlayerTag;
        private float _adjacentLeg;
        private float _yRotation;
        private float _maxHeight;
        private float _oppositeLeg;

        private Vector3 _offsetVector;
        private Vector2 _mouseDelta;

        private PlayerTag _playerTag;
        private Transform _cameraPointer;
        private CinemachineVirtualCamera _virtualCamera;
        private CinemachineTransposer _transposer;

        private void Awake()
        {
            _hasPlayerTag = TryGetComponent(out _playerTag);
        }

        private void Start()
        {
            if (_hasPlayerTag)
                FollowPosition(_playerTag.transform.position);

            _cameraPointer = GameplayManager.Instance.CameraHandler.CameraPointer;
            _maxHeight = cameraDistance * Mathf.Sin(heightAngle * Mathf.Deg2Rad);
            _virtualCamera = GameplayManager.Instance.CameraHandler.FollowPlayerCamera;
            _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

            ResetCamera();
        }

        private void FixedUpdate()
        {
            ReceiveInput();
            ControlCameraAngle();
        }

        public void ControlCameraAngle()
        {
            FollowPosition(_playerTag.transform.position);

            _adjacentLeg += _mouseDelta.y * heightOffsetSpeed;
            _yRotation -= _mouseDelta.x * rotationSpeed;

            _adjacentLeg = Mathf.Clamp(_adjacentLeg, minCameraHeight, _maxHeight);
            _oppositeLeg = Mathf.Sqrt(cameraDistance * cameraDistance - _adjacentLeg * _adjacentLeg);

            _offsetVector = new Vector3(0, _adjacentLeg, _oppositeLeg);
            _transposer.m_FollowOffset = _offsetVector;

            Quaternion targetRotation = Quaternion.Euler(new(0, _yRotation, 0));
            _cameraPointer.rotation = targetRotation;
        }

        private void ReceiveInput()
        {
            _mouseDelta = InputReceiver.Instance.CameraDelta;
        }

        private void ResetCamera()
        {
            _yRotation = _cameraPointer.eulerAngles.y - 180;
            _adjacentLeg = _transposer.m_FollowOffset.y;
        }

        private void FollowPosition(Vector3 position)
        {
            GameplayManager.Instance.CameraHandler.FollowPosition(position);
        }
    }
}
