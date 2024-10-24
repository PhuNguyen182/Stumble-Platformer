using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StumblePlatformer.Scripts.Gameplay.Inputs;
using UnityEngine.InputSystem;

namespace StumblePlatformer.Scripts.Gameplay.Characters
{
    public class CameraPointer : MonoBehaviour
    {
        [SerializeField] private Transform cameraPointer;
        [SerializeField] private InputReceiver inputReceiver;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        [Header("Settings")]
        [SerializeField] private float heightAngle = 60f;
        [SerializeField] private float cameraDistance = 4.75f;
        [SerializeField] private float minCameraHeight = 0;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float rotationSpeed = 0.5f;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float heightOffsetSpeed = 0.1f;

        public Transform Pointer => cameraPointer;

        private float adjacentLeg;
        private float yRotation;
        private float maxHeight;
        private float oppositeLeg;

        private Vector3 offsetVector;
        private Vector2 mouseDelta;

        private CinemachineTransposer transposer;

        private void Start()
        {
            maxHeight = cameraDistance * Mathf.Sin(heightAngle * Mathf.Deg2Rad);
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

            ResetCamera();
        }

        private void FixedUpdate()
        {
            mouseDelta = inputReceiver.CameraDelta;

            if (mouseDelta != Vector2.zero)
            {
                adjacentLeg += mouseDelta.y * heightOffsetSpeed;
                yRotation -= mouseDelta.x * rotationSpeed;

                adjacentLeg = Mathf.Clamp(adjacentLeg, minCameraHeight, maxHeight);
                oppositeLeg = Mathf.Sqrt(cameraDistance * cameraDistance - adjacentLeg * adjacentLeg);

                offsetVector = new Vector3(0, adjacentLeg, oppositeLeg);
                transposer.m_FollowOffset = offsetVector;

                Quaternion targetRotation = Quaternion.Euler(new(0, yRotation, 0));
                cameraPointer.rotation = targetRotation;
            }
        }

        private void ResetCamera()
        {
            yRotation = cameraPointer.eulerAngles.y - 180;
            adjacentLeg = transposer.m_FollowOffset.y;
        }
    }
}
