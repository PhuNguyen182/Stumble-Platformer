using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Miscs
{
    public class FollowLocalPosition : MonoBehaviour, IFixedUpdateHandler
    {
        public Vector3 LocalPosition;
        public bool CheckPosition;

        public bool IsActive { get; set; }

        private void Start()
        {
            UpdateHandlerManager.Instance.AddFixedUpdateBehaviour(this);
        }

        public void OnFixedUpdate()
        {
            transform.localPosition = LocalPosition;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (CheckPosition)
            {
                CheckPosition = false;
                LocalPosition = transform.localPosition;
            }
        }
#endif

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
        }
    }
}
