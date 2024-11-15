using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using Cinemachine;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    [ExecuteAlways]
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private CinemachineVirtualCamera followPlayerCamera;
        [SerializeField] private CameraPointer cameraPointer;

        public void SetFollowTarget(Transform followTarget)
        {
            cameraPointer.SetFollowTarget(followTarget);
            cameraPointer.SetupCameraOnStart();
        }

#if UNITY_EDITOR
        [SerializeField] private Transform testPlayerPoint;

        private void Update()
        {
            if(!Application.isPlaying)
                ResetFollowPosition();
        }

        public void ResetFollowPosition()
        {
            if(testPlayerPoint != null)
                followTarget.position = testPlayerPoint.position;
        }
#endif
    }
}
