using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class PushPillarObstacle : BaseObstacle
    {
        [SerializeField] private float pushForce = 6;
        [SerializeField] private Animator platformAnimator;

        public override void DamageCharacter(Collision collision)
        {
            if(collision.collider.attachedRigidbody != null)
            {
                Vector3 hitNormal = collision.GetContact(0).normal;
                float hitImpulse = collision.GetContact(0).impulse.magnitude;

                collision.collider.attachedRigidbody.velocity = -hitNormal * (hitImpulse + pushForce);
            }
        }

        public override void ExitDamage(Collision collision)
        {
            
        }

        public override void ObstacleAction()
        {
            
        }
    }
}
