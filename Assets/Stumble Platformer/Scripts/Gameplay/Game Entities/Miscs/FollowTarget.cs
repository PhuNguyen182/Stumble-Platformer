using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;
using Sirenix.OdinInspector;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Miscs
{
    public class FollowTarget : MonoBehaviour, IFixedUpdateHandler
    {
        [HorizontalGroup("Follow Toggle")]
        public bool FollowX;
        [HorizontalGroup("Follow Toggle")]
        public bool FollowY;
        [HorizontalGroup("Follow Toggle")]
        public bool FollowZ;

        [Space(10)]
        public bool UseTransform;
        public bool LookAtTarget;
        public Vector3 Offset;
        [ShowIf("UseTransform", true)] public Transform TargetPoint;
        [HideIf("UseTransform", true)] public Vector3 ToPosition;

        private Vector3 _toPosition;
        private Vector3 _targetPosition;
        public bool IsActive { get; set; }

        private void Start()
        {
            IsActive = true;
            UpdateHandlerManager.Instance.AddFixedUpdateBehaviour(this);
        }

        public void OnFixedUpdate()
        {
            _toPosition = UseTransform ? TargetPoint.position : ToPosition;
            
            if (FollowX)
                _targetPosition = new Vector3(_toPosition.x, transform.position.y, transform.position.z);

            else if (FollowY)
                _targetPosition = new Vector3(transform.position.x, _toPosition.y, transform.position.z);

            else if (FollowZ)
                _targetPosition = new Vector3(transform.position.x, transform.position.y, _toPosition.z);

            else if(FollowX && FollowY)
                _targetPosition = new Vector3(_toPosition.x, _toPosition.y, transform.position.z);

            else if (FollowY && FollowZ)
                _targetPosition = new Vector3(transform.position.x, _toPosition.y, _toPosition.z);

            else if (FollowZ && FollowX)
                _targetPosition = new Vector3(_toPosition.x, transform.position.y, _toPosition.z);

            else if(FollowX && FollowY && FollowZ)
                _targetPosition = _toPosition;

            else
                _targetPosition = transform.position;

            transform.position = _targetPosition + Offset;
            
            if (LookAtTarget && TargetPoint)
                transform.LookAt(TargetPoint);
        }

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
        }
    }
}
