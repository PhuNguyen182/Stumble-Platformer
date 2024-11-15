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
        [SerializeField] private CinemachineVirtualCamera followPlayerCamera;
        [SerializeField] private CameraPointer cameraPointer;

#if UNITY_EDITOR
        [Header("Testing")]
        [SerializeField] private Transform followTarget; 
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

        public void SetFollowTarget(Transform followTarget)
        {
            // This function can be used to switch to other players
            cameraPointer.SetFollowTarget(followTarget);
            cameraPointer.SetupCameraOnStart();
        }
    }
}
