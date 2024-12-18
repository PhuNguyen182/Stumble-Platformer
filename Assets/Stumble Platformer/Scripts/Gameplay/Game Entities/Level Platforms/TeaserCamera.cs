using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Cameras;
using Cinemachine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class TeaserCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera teaserCamera;

        private CinemachineTransposer _transposer;
        private CinemachineTrackedDolly _trackedDolly;

        public void SetupTransposerCamera(TransposerConfig transposer)
        {
            _transposer = teaserCamera.AddCinemachineComponent<CinemachineTransposer>();

            _transposer.m_BindingMode = transposer.BindingMode;
            _transposer.m_FollowOffset = transposer.FollowOffset;
            _transposer.m_AngularDampingMode = transposer.AngularDampingMode;

            _transposer.m_XDamping = transposer.XDamping;
            _transposer.m_YDamping = transposer.YDamping;
            _transposer.m_ZDamping = transposer.ZDamping;

            _transposer.m_PitchDamping = transposer.PitchDamping;
            _transposer.m_YawDamping = transposer.YawDamping;
            _transposer.m_RollDamping = transposer.RollDamping;
        }

        public void SetupTrackedDollyCamera(TrackedDollyConfig trackedDolly)
        {
            _trackedDolly = teaserCamera.AddCinemachineComponent<CinemachineTrackedDolly>();
            
            _trackedDolly.m_PositionUnits = trackedDolly.PositionUnits;
            _trackedDolly.m_PathOffset = trackedDolly.PathOffset;
            
            _trackedDolly.m_XDamping = trackedDolly.XDamping;
            _trackedDolly.m_YDamping = trackedDolly.YDamping;
            _trackedDolly.m_ZDamping = trackedDolly.ZDamping;
            _trackedDolly.m_CameraUp = trackedDolly.CameraUpMode;
            
            _trackedDolly.m_PitchDamping = trackedDolly.PitchDamping;
            _trackedDolly.m_YawDamping = trackedDolly.YawDamping;
            _trackedDolly.m_RollDamping = trackedDolly.RollDamping;

            _trackedDolly.m_AutoDolly = new CinemachineTrackedDolly.AutoDolly
            {
                m_Enabled = trackedDolly.AutoDolly,
                m_PositionOffset = trackedDolly.PositionOffset,
                m_SearchRadius = trackedDolly.SearchRadius,
                m_SearchResolution = trackedDolly.SearchResolution
            };
        }

        public void SetTeaserTrackPath(CinemachinePathBase cinemachinePath)
        {
            _trackedDolly.m_Path = cinemachinePath;
        }

        public void SetFollowTarget(Transform target)
        {
            teaserCamera.m_Follow = target;
        }

        public void SetLookAtTarget(Transform target)
        {
            if(target != null)
            {
                teaserCamera.m_LookAt = target;
            }
        }

        public void SetCameraActive(bool active)
        {
            teaserCamera.gameObject.SetActive(active);
        }
    }
}
