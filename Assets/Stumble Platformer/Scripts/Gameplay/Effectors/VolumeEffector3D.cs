using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.Effectors
{
    public class VolumeEffector3D : MonoBehaviour
    {
        [SerializeField] private float forceMagnitude;
        [SerializeField] private float forceVariation;
        [SerializeField] private Vector3 forceDirection;
        [SerializeField] private ForceMode forceMode = ForceMode.Force;
        [SerializeField] private bool useLocalRotation;

        private bool _hasColliderAttached;
        private Vector3 _forceDirection;
        private Collider _currentCollider;

        private void Awake()
        {
            _forceDirection = useLocalRotation ? transform.localRotation * forceDirection.normalized 
                                               : forceDirection.normalized;
            _hasColliderAttached = TryGetComponent(out _currentCollider);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null)
                AddForceToCollider(other.attachedRigidbody);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.attachedRigidbody != null)
                AddForceToCollider(other.attachedRigidbody);
        }

        private void AddForceToCollider(Rigidbody rigidbody)
        {
            float magnitude = forceMagnitude != 0 ? Random.Range(0, forceVariation) + forceMagnitude : forceMagnitude;
            rigidbody.AddForce(magnitude * _forceDirection, forceMode);
        }
    }
}
