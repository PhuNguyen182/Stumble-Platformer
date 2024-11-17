using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StumblePlatformer.Scripts.Gameplay.Inputs;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public class CameraPointer : MonoBehaviour
    {
        [SerializeField] private Transform cameraPointer;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

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

        private Transform _followTarget;
        private InputReceiver _inputReceiver;
        private CinemachineTransposer _transposer;

        private void Start()
        {
            _inputReceiver = InputReceiver.Instance;
        }

        private void FixedUpdate()
        {
            ReceiveInput();
            ControlCameraAngle();
        }

        public void SetupCameraOnStart()
        {
            if (_followTarget != null)
            {
                FollowPosition(_followTarget.position);

                _transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
                _maxHeight = cameraDistance * Mathf.Sin(heightAngle * Mathf.Deg2Rad);

                ResetCamera();
            }
        }

        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;
        }

        public void ControlCameraAngle()
        {
            if (_followTarget != null)
            {
                FollowPosition(_followTarget.position);

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

        private void ReceiveInput()
        {
            if (_inputReceiver != null)
                _mouseDelta = _inputReceiver.CameraDelta;
        }

        private void ResetCamera()
        {
            _yRotation = cameraPointer.eulerAngles.y - 180;
            _adjacentLeg = _transposer.m_FollowOffset.y;
        }

        private void FollowPosition(Vector3 position)
        {
            cameraPointer.position = position;
        }
    }
}
