using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;
using StumblePlatformer.Scripts.Common.Enums;
using Cinemachine;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    [ExecuteAlways]
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private CinemachineBrain cinemachineBrain;
        [SerializeField] private TeaserCamera teaserCamera;
        [SerializeField] private CameraPointer cameraPointer;

        private void Awake()
        {
            if (cinemachineBrain != null)
                cinemachineBrain.useGUILayout = false;
        }

        #region Editor Only
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
        #endregion

        public void SetupVirtualCameraBody(EnvironmentIdentifier environmentIdentifier)
        {
            switch (environmentIdentifier.CameraBodyMode)
            {
                case CameraBodyMode.TrackedDolly:
                    teaserCamera.SetupTrackedDollyCamera(environmentIdentifier.TrackedDollyConfig);
                    break;
                case CameraBodyMode.Transposer:
                    teaserCamera.SetupTransposerCamera(environmentIdentifier.TransposerConfig);
                    break;
            }
        }

        public void SetFollowCameraActive(bool active) => cameraPointer.SetFollowActive(active);

        public void SetTeaserCameraActive(bool active) => teaserCamera.SetCameraActive(active);

        public void SetFollowTarget(Transform followTarget)
        {
            // This function can be used to switch to other players
            cameraPointer.SetFollowTarget(followTarget);
            cameraPointer.SetupCameraOnStart();
        }

        public void ResetCurrentCameraFollow() => cameraPointer.SetupCameraOnStart();

        public void SetupTeaserCamera(Transform follow, Transform lookAt, CinemachinePathBase path)
        {
            teaserCamera.SetFollowTarget(follow);
            teaserCamera.SetLookAtTarget(lookAt);
            teaserCamera.SetTeaserTrackPath(path);
        }
    }
}
