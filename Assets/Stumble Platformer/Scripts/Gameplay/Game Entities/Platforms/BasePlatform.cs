using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;
using Unity.Netcode;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public abstract class BasePlatform : NetworkBehaviour, IPlatform, ISetPlatformActive, IFixedUpdateHandler
    {
        [SerializeField] protected Rigidbody platformBody;
        [SerializeField] protected NetworkObject networkObject;
        [SerializeField] protected bool isKinematic;

        public bool IsActive { get; set; }
        public bool IsPlatformActive { get; set; }

        private void Awake()
        {
            OnAwake();
            IsActive = true;
        }

        private void Start()
        {
            OnStart();
            UpdateHandlerManager.Instance.AddFixedUpdateBehaviour(this);
            SpawnNetworkObject();
        }

        protected virtual void OnAwake()
        {
            if (platformBody != null)
                isKinematic = platformBody.isKinematic;
        }

        protected virtual void OnStart() { }

        public virtual void OnFixedUpdate()
        {
            PlatformAction();
        }

        public virtual void SetPlatformActive(bool active)
        {
            IsPlatformActive = active;
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

        private void SpawnNetworkObject()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                if (!networkObject.IsSpawned)
                    networkObject.Spawn(true);

                if (platformBody != null)
                    platformBody.isKinematic = isKinematic;
            }
        }

        public override void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
            base.OnDestroy();
        }
    }
}
