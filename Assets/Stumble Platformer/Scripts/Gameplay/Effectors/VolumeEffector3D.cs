using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using GlobalScripts.Extensions;

namespace StumblePlatformer.Scripts.Gameplay.Effectors
{
    public class VolumeEffector3D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] public bool useLocalRotation;
        [SerializeField] public float forceMagnitude;
        [SerializeField] public float forceVariation;
        [SerializeField] public Vector3 forceDirection;

        [Header("Mode")]
        [SerializeField] public ForceMode forceMode = ForceMode.Force;
        [SerializeField] public EffectorMode effectorMode = EffectorMode.Force;
        [SerializeField] public LayerMask interactibleLayer;

        private Vector3 _forceDirection;

        private void Awake()
        {
            _forceDirection = useLocalRotation ? transform.rotation * forceDirection.normalized 
                                               : forceDirection.normalized;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.HasLayer(interactibleLayer))
                return;

            if (!other.attachedRigidbody)
                return;

            switch (effectorMode)
            {
                case EffectorMode.Force:
                    AddForceToCollider(other.attachedRigidbody);
                    break;
                case EffectorMode.Velocity:
                    ApplyVelocityToCollider(other.attachedRigidbody);
                    break;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.HasLayer(interactibleLayer))
                return;

            if (!other.attachedRigidbody)
                return;

            switch (effectorMode)
            {
                case EffectorMode.Force:
                    AddForceToCollider(other.attachedRigidbody);
                    break;
                case EffectorMode.Velocity:
                    ApplyVelocityToCollider(other.attachedRigidbody);
                    break;
            }
        }

        private void AddForceToCollider(Rigidbody rigidbody)
        {
            float magnitude = forceMagnitude != 0 ? Random.Range(0, forceVariation) + forceMagnitude : forceMagnitude;
            rigidbody.AddForce(magnitude * _forceDirection, forceMode);
        }

        private void ApplyVelocityToCollider(Rigidbody rigidbody)
        {
            float magnitude = forceMagnitude != 0 ? Random.Range(0, forceVariation) + forceMagnitude : forceMagnitude;
            rigidbody.velocity = magnitude * _forceDirection;
        }
    }
}
