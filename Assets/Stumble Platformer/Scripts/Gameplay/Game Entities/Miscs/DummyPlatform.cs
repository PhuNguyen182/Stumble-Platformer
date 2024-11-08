using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Miscs
{
    [RequireComponent(typeof(Rigidbody))]
    public class DummyPlatform : MonoBehaviour
    {
        [SerializeField] private bool isCustomCheck;
        [SerializeField] private Collider platformCollider;

        public void SetSizeAndCenter(Vector3 size, Vector3 center)
        {
            if (platformCollider is BoxCollider collider)
            {
                collider.size = size;
                collider.center = center;
            }
        }

#if UNITY_EDITOR
        private Rigidbody _rigidbody;

        private void OnValidate()
        {
            _rigidbody ??= GetComponent<Rigidbody>();
            platformCollider ??= GetComponent<Collider>();

            if (!isCustomCheck)
            {
                _rigidbody.isKinematic = true;
                platformCollider.isTrigger = true;
            }
        }
#endif
    }
}
