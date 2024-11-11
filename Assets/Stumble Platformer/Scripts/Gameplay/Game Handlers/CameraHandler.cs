using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    [ExecuteAlways]
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private CinemachineVirtualCamera followPlayerCamera;

        public Transform CameraPointer => followTarget;
        public CinemachineVirtualCamera FollowPlayerCamera => followPlayerCamera;

        public void FollowPosition(Vector3 position)
        {
            followTarget.position = position;
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
