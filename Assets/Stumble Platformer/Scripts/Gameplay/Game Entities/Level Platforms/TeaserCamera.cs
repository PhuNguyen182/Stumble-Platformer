using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class TeaserCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera teaserCamera;

        private CinemachineTrackedDolly _trackedDolly;

        private void Awake()
        {
            _trackedDolly = teaserCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        }

        public void SetTeaserTrackPath(CinemachinePathBase cinemachinePath)
        {
            _trackedDolly.m_Path = cinemachinePath;
        }

        public void SetFollowTarget(Transform target)
        {
            teaserCamera.m_Follow = target;
        }

        public void SetCameraActive(bool active)
        {
            teaserCamera.gameObject.SetActive(active);
        }
    }
}
