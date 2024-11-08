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

        private bool _canAttack;
        private bool _hasCollider;
        private Collider _collider;

        public bool CanAttack 
        {
            get => _canAttack;
            set
            {
                _canAttack = value;

                if (_hasCollider)
                    _collider.enabled = _canAttack;
            }
        }

        private void Awake()
        {
            _hasCollider = TryGetComponent<Collider>(out _collider);
        }

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
