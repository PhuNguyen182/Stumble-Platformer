using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public abstract class BasePlatform : MonoBehaviour, IPlatform, ISetPlatformActive, IFixedUpdateHandler
    {
        [SerializeField] protected Rigidbody platformBody;

        public bool IsActive { get; set; }

        private void Awake()
        {
            OnAwake();
            IsActive = true;
        }

        private void Start()
        {
            OnStart();
            UpdateHandlerManager.Instance.AddFixedUpdateBehaviour(this);
        }

        protected virtual void OnAwake() { }

        protected virtual void OnStart() { }

        public virtual void OnFixedUpdate()
        {
            PlatformAction();
        }

        public void SetPlatformActive(bool active)
        {
            IsActive = active;
        }

        public abstract void PlatformAction();

        public abstract void OnPlatformCollide(Collision collision);

        public abstract void OnPlatformStay(Collision collision);

        public abstract void OnPlatformExit(Collision collision);

        private void OnCollisionEnter(Collision collision)
        {
            OnPlatformCollide(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            OnPlatformStay(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            OnPlatformExit(collision);
        }

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
        }
    }
}
