using R3;
using R3.Triggers;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public abstract class BaseObstacle : MonoBehaviour, IObstacle, IFixedUpdateHandler
    {
        [SerializeField] protected float attactForce = 15f;
        [SerializeField] protected Rigidbody obstacleBody;
        [Header("Obstacle Strikers")]
        [SerializeField] protected ObstacleAttacker[] obstacleAttackers;

        protected IDisposable disposable;
        protected CancellationToken destroyToken;

        public bool IsActive { get; set; }

        private void Awake()
        {
            destroyToken = this.GetCancellationTokenOnDestroy();
            RegisterObstacleAttacker();
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

        [Button]
        private void GetObstacleAttackers()
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

                obstacleAttackers[i].OnCollisionEnterAsObservable().Subscribe(DamageCharacter).AddTo(ref builder);
                obstacleAttackers[i].OnCollisionStayAsObservable().Subscribe(DamageCharacter).AddTo(ref builder);
                obstacleAttackers[i].OnCollisionExitAsObservable().Subscribe(ExitDamage).AddTo(ref builder);
            }

            disposable = builder.RegisterTo(destroyToken);
        }

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
        }
    }
}
