using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public abstract class BasePlatform : MonoBehaviour, IPlatform, IFixedUpdateHandler
    {
        public bool IsActive { get; set; }

        private void Start()
        {
            UpdateHandlerManager.Instance.AddFixedUpdateBehaviour(this);
        }

        public virtual void OnFixedUpdate()
        {
            PlatformAction();
        }

        public abstract void PlatformAction();

        public abstract void OnPlatformCollide(Collision collision);

        public abstract void OnPlatformExit(Collision collision);

        private void OnCollisionEnter(Collision collision)
        {
            OnPlatformCollide(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            OnPlatformCollide(collision);
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
