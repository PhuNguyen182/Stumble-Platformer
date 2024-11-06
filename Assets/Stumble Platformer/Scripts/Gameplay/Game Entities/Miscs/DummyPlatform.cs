using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Miscs
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class DummyPlatform : MonoBehaviour
    {
        [SerializeField] private bool isCustomCheck;
        [SerializeField] private BoxCollider platformCollider;

        public void SetSizeAndCenter(Vector3 size, Vector3 center)
        {
            platformCollider.size = size;
            platformCollider.center = center;
        }

#if UNITY_EDITOR
        private Rigidbody _rigidbody;

        private void OnValidate()
        {
            _rigidbody ??= GetComponent<Rigidbody>();
            platformCollider ??= GetComponent<BoxCollider>();

            if (!isCustomCheck)
            {
                _rigidbody.isKinematic = true;
                platformCollider.isTrigger = true;
            }
        }
#endif
    }
}
