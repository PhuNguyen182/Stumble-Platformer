using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using Cinemachine;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    [ExecuteAlways]
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private TeaserCamera teaserCamera;
        [SerializeField] private CameraPointer cameraPointer;
        [SerializeField] private CinemachineVirtualCamera followPlayerCamera;

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

        public void SetTeaserCameraActive(bool active)
        {
            teaserCamera.SetCameraActive(active);
        }

        public void SetFollowTarget(Transform followTarget)
        {
            // This function can be used to switch to other players
            cameraPointer.SetFollowTarget(followTarget);
            cameraPointer.SetupCameraOnStart();
        }

        public void SetupTeaserCamera(Transform target, CinemachinePathBase path)
        {
            teaserCamera.SetFollowTarget(target);
            teaserCamera.SetTeaserTrackPath(path);
        }
    }
}
