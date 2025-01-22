using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using GlobalScripts.UpdateHandlerPattern;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public abstract class BasePlatform : MonoBehaviour, IPlatform, ISetPlatformActive, IFixedUpdateHandler
    {
        [SerializeField] protected Rigidbody platformBody;
        [SerializeField] protected bool usePhysics = true;

        protected bool hasNetworkObject;
        protected NetworkObject obstacleNetworkObject;

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
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnStart() { }

        public virtual void OnFixedUpdate()
        {
            PlatformAction();
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        protected void SpawnSelfRpc()
        {
            obstacleNetworkObject.enabled = true;
            if (hasNetworkObject && !obstacleNetworkObject.IsSpawned)
                obstacleNetworkObject.Spawn(true);
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

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
        }
    }
}
