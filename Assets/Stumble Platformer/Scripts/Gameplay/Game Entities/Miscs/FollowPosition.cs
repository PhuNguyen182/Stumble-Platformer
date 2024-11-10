using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Miscs
{
    public class FollowPosition : MonoBehaviour, IFixedUpdateHandler
    {
        public bool UseTransform;
        public Transform TargetPoint;
        public Vector3 ToPosition;

        public bool IsActive { get; set; }

        private void Start()
        {
            IsActive = true;
            UpdateHandlerManager.Instance.AddFixedUpdateBehaviour(this);
        }

        public void OnFixedUpdate()
        {
            transform.position = UseTransform ? TargetPoint.position : ToPosition;
        }

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
        }
    }
}
