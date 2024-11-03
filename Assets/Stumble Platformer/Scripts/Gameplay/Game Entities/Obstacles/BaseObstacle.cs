using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;
using Sirenix.OdinInspector;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public abstract class BaseObstacle : MonoBehaviour, IObstacle, ISetObstacleAttack, IFixedUpdateHandler
    {
        [SerializeField] protected Rigidbody obstacleBody;
        [SerializeField] protected ObstacleAttacker[] obstacleAttackers;

        public bool IsActive { get; set; }

        private void Awake()
        {
            for (int i = 0; i < obstacleAttackers.Length; i++)
            {
                if (obstacleAttackers[i] == null)
                    continue;

                obstacleAttackers[i].ExitDamage = ExitDamage;
                obstacleAttackers[i].DamageAttack = DamageCharacter;
            }

            OnAwake();
        }

        private void Start()
        {
            IsActive = true;
            UpdateHandlerManager.Instance.AddFixedUpdateBehaviour(this);
        }

        public abstract void ExitDamage(Collision collision);

        public abstract void DamageCharacter(Collision collision);

        public abstract void ObstacleAction();

        public virtual void OnAwake() { }

        public virtual void SetObstacleActive(bool active)
        {
            IsActive = active;
        }

        public virtual void OnFixedUpdate()
        {
            ObstacleAction();
        }

        public void SetObstacleCanAttack(bool canAttack)
        {
            for (int i = 0; i < obstacleAttackers.Length; i++)
            {
                if (obstacleAttackers[i] != null)
                    obstacleAttackers[i].CanAttack = canAttack;
            }
        }

        [Button]
        private void GetObstacleAttackers()
        {
            obstacleAttackers = transform.GetComponentsInChildren<ObstacleAttacker>();
        }

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
        }
    }
}
