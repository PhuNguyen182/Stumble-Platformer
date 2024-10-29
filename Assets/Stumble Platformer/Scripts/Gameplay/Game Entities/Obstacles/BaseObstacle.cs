using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public abstract class BaseObstacle : MonoBehaviour, IFixedUpdateHandler
    {
        [SerializeField] protected Rigidbody obstacleBody;
        [SerializeField] protected Collider mainCollider;
        [SerializeField] protected Collider[] componentColliders;

        public bool IsActive { get; set; }

        private void Awake()
        {
            UpdateHandlerManager.Instance.AddFixedUpdateBehaviour(this);
        }

        public abstract void ExitDamage(Collision collision);

        public abstract void DamageCharacter(Collision collision);

        public virtual void SetObstacleActive(bool active)
        {
            IsActive = active;
        }

        public void SetColliderActive(bool active)
        {
            if(mainCollider != null)
                mainCollider.enabled = active;

            for (int i = 0; i < componentColliders.Length; i++)
            {
                if (componentColliders[i] != null)
                    componentColliders[i].enabled = active;
            }
        }

        public abstract void OnFixedUpdate();

        private void OnCollisionEnter(Collision collision)
        {
            DamageCharacter(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            DamageCharacter(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            ExitDamage(collision);
        }

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
        }
    }
}
