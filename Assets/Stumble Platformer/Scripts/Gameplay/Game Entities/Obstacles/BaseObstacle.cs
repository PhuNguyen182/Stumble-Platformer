using R3;
using R3.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using GlobalScripts.UpdateHandlerPattern;
using Sirenix.OdinInspector;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public abstract class BaseObstacle : MonoBehaviour, IObstacle, IObstacleDamager, IObstacleCollider, IFixedUpdateHandler
    {
        [SerializeField] protected bool attackOnce;
        [SerializeField] protected float attactForce = 15f;
        [SerializeField] protected float stunDuration = 1.5f;
        [SerializeField] protected bool isKinematic;
        [SerializeField] protected Rigidbody obstacleBody;
        
        [Header("Obstacle Strikers")]
        [SerializeField] protected ObstacleAttacker[] obstacleAttackers;

        protected bool hasNetworkObject;
        protected NetworkObject obstacleNetworkObject;

        public bool IsActive { get; set; }

        private void Awake()
        {
            RegisterObstacleAttacker();
            OnAwake();
        }

        private void Start()
        {
            UpdateHandlerManager.Instance.AddFixedUpdateBehaviour(this);
        }

        public abstract void ExitDamage(Collision collision);
        public abstract void DamageCharacter(Collision collision);
        public abstract void ObstacleAction();

        public virtual void OnAwake()
        {
            hasNetworkObject = TryGetComponent(out obstacleNetworkObject);
        }

        public virtual void SetObstacleActive(bool active)
        {
            IsActive = active;

            for (int i = 0; i < obstacleAttackers.Length; i++)
            {
                obstacleAttackers[i].CanAttack = active;
            }
        }

        public virtual void OnFixedUpdate()
        {
            ObstacleAction();
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        protected void SpawnSelfRpc()
        {
            obstacleNetworkObject.enabled = true;
            if (hasNetworkObject && !obstacleNetworkObject.IsSpawned)
                obstacleNetworkObject.Spawn(true);
        }

        protected void DamageTargetOnStay(Collision collision)
        {
            if (!attackOnce)
                DamageCharacter(collision);
        }

        [Button]
        public void GetObstacleAttackers()
        {
            obstacleAttackers = transform.GetComponentsInChildren<ObstacleAttacker>();
        }

        private void RegisterObstacleAttacker()
        {
            var builder = Disposable.CreateBuilder();

            for (int i = 0; i < obstacleAttackers.Length; i++)
            {
                if (obstacleAttackers[i] == null)
                    continue;

                obstacleAttackers[i].OnCollisionEnterAsObservable()
                                    .Subscribe(DamageCharacter)
                                    .AddTo(ref builder);
                
                obstacleAttackers[i].OnCollisionStayAsObservable()
                                    .Subscribe(DamageTargetOnStay)
                                    .AddTo(ref builder);
                
                obstacleAttackers[i].OnCollisionExitAsObservable()
                                    .Subscribe(ExitDamage)
                                    .AddTo(ref builder);
            }

            builder.RegisterTo(this.destroyCancellationToken);
        }

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
        }
    }
}
