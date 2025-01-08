using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using StumblePlatformer.Scripts.Common.Constants;
using Cinemachine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public class CameraPointer : NetworkBehaviour
    {
        [SerializeField] private bool isTest;
        [SerializeField] private InputReceiver inputReceiver;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Transform cameraPointer;

        [Header("Settings")]
        [SerializeField][Range(15f, 90f)] private float heightAngle = 60f;
        [SerializeField][Range(4f, 15f)] private float cameraDistance = 4.75f;
        [SerializeField][Range(0f, 10f)] private float minCameraHeight = 0;
        [SerializeField][Range(0.0f, 1.0f)] private float rotationSpeed = 0.5f;
        [SerializeField][Range(0.0f, 1.0f)] private float heightOffsetSpeed = 0.1f;

        private const string RotateXSaveKey = "RotateX";
        private const string RotateYSaveKey = "RotateY";

        private float _yRotation;
        private float _maxHeight;
        private float _adjacentLeg;
        private float _oppositeLeg;
        private bool _rotationActive;

        private Vector2 _mouseDelta;
        private Vector3 _offsetVector;

        private Transform _followTarget;
        private CinemachineTransposer _transposer;

        private void Awake()
        {
            SetUpCameraRotateSpeed();
        }

        private void FixedUpdate()
        {
            ReceiveInput();
            ControlCameraAngle();
        }

        public void SetupCameraOnStart()
        {
            if (_followTarget == null)
                return;

            FollowPosition(_followTarget.position);
            SetupCamera();
            ResetCamera();
        }

        public void SetFollowActive(bool active) => _rotationActive = active;

        public void SetFollowTarget(Transform target) => _followTarget = target;

        public void ControlCameraAngle()
        {
            if (_followTarget == null)
                return;

            FollowPosition(_followTarget.position);

            if (_rotationActive)
            {
                _adjacentLeg += _mouseDelta.y * heightOffsetSpeed;
                _yRotation -= _mouseDelta.x * rotationSpeed;

                _adjacentLeg = Mathf.Clamp(_adjacentLeg, minCameraHeight, _maxHeight);
                _oppositeLeg = Mathf.Sqrt(cameraDistance * cameraDistance - _adjacentLeg * _adjacentLeg);

                _offsetVector = new Vector3(0, _adjacentLeg, _oppositeLeg);
                _transposer.m_FollowOffset = _offsetVector;

                Quaternion targetRotation = Quaternion.Euler(new(0, _yRotation, 0));
                cameraPointer.rotation = targetRotation;
            }
        }

        private void SetupCamera()
        {
            _transposer ??= virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            _maxHeight = cameraDistance * Mathf.Sin(heightAngle * Mathf.Deg2Rad);
        }

        private void ResetCamera()
        {
            cameraPointer.rotation = Quaternion.Euler(0, 0, 0);
            _yRotation = cameraPointer.eulerAngles.y - 180;
            _adjacentLeg = _transposer.m_FollowOffset.y;
        }

        private void SetUpCameraRotateSpeed()
        {
            if (isTest)
                return;

            rotationSpeed = PlayerPrefs.GetFloat(RotateXSaveKey, CharacterConstants.DefaultRotateXCamera);
            heightOffsetSpeed = PlayerPrefs.GetFloat(RotateYSaveKey, CharacterConstants.DefaultRotateYCamera);
        }

        private void ReceiveInput() => _mouseDelta = inputReceiver.CameraDelta;

        private void FollowPosition(Vector3 position) => cameraPointer.position = position;
    }
}
