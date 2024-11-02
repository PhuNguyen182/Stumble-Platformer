using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.CommonMovement
{
    public class RotationScript : MonoBehaviour, IUpdateHandler
    {
        public enum RotationAxis
        {
            X,
            Y,
            Z
        }

        public RotationAxis rotationAxis = RotationAxis.Y;
        public float rotationSpeed = 50.0f;

        public bool IsActive { get; set; }

        private void Start()
        {
            IsActive = true;
            UpdateHandlerManager.Instance.AddUpdateBehaviour(this);
        }

        public void OnUpdate(float deltaTime)
        {
            float rotationValue = rotationSpeed * deltaTime;

            Vector3 axis = Vector3.zero;
            switch (rotationAxis)
            {
                case RotationAxis.X:
                    axis = Vector3.right;
                    break;
                case RotationAxis.Y:
                    axis = Vector3.up;
                    break;
                case RotationAxis.Z:
                    axis = Vector3.forward;
                    break;
            }

            transform.Rotate(axis, rotationValue);
        }

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveUpdateBehaviour(this);
        }
    }
}
