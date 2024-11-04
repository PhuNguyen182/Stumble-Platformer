using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    [RequireComponent(typeof(Rigidbody))]
    public class ObstacleAttacker : MonoBehaviour
    {
        [SerializeField] private bool canFreeCheckBodyType;

        public bool CanAttack { get; set; }

#if UNITY_EDITOR
        private Rigidbody _attactBody;

        private void OnValidate()
        {
            _attactBody ??= GetComponent<Rigidbody>();

            if (_attactBody != null && !canFreeCheckBodyType)
                _attactBody.isKinematic = true;

        }
#endif
    }
}
