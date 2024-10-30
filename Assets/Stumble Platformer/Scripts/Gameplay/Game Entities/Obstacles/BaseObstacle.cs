using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public abstract class BaseObstacle : MonoBehaviour, IFixedUpdateHandler, ISetObstacleAttack
    {
        [SerializeField] protected Rigidbody obstacleBody;
        [SerializeField] protected ObstacleAttacker[] obstacleAttackers;

        public bool IsActive { get; set; }

        private void Awake()
        {
            UpdateHandlerManager.Instance.AddFixedUpdateBehaviour(this);

            for (int i = 0; i < obstacleAttackers.Length; i++)
            {
                if (obstacleAttackers[i] == null)
                    continue;

                obstacleAttackers[i].ExitDamage = ExitDamage;
                obstacleAttackers[i].DamageAttack = DamageCharacter;
            }
        }

        public abstract void ExitDamage(Collision collision);

        public abstract void DamageCharacter(Collision collision);

        public virtual void SetObstacleActive(bool active)
        {
            IsActive = active;
        }

        public void SetObstacleCanAttack(bool canAttack)
        {
            for (int i = 0; i < obstacleAttackers.Length; i++)
            {
                if (obstacleAttackers[i] != null)
                    obstacleAttackers[i].CanAttack = canAttack;
            }
        }

        public abstract void OnFixedUpdate();

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
        }
    }
}
