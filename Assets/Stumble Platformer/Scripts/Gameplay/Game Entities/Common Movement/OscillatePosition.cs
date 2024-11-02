using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.CommonMovement
{
    public class OscillatePosition : MonoBehaviour, IUpdateHandler
    {
        public Vector3 moveAxis = Vector3.up;
        public float moveDistance = 2f;
        public float duration = 2f;
        public bool useRandomDelay = false; // Toggle random delay
        public float maxRandomDelay = 1f; // Maximum random delay

        private Vector3 startPosition;
        private float timeElapsed = 0f;
        private bool isReversing = false;
        private float randomDelay = 0f;

        public bool IsActive { get; set; }

        private void Start()
        {
            IsActive = true;
            startPosition = transform.position;

            if (useRandomDelay)
            {
                randomDelay = Random.Range(0f, maxRandomDelay);
            }
            UpdateHandlerManager.Instance.AddUpdateBehaviour(this);
        }

        public void OnUpdate(float deltaTime)
        {
            if (timeElapsed < randomDelay)
            {
                timeElapsed += deltaTime;
                return;
            }

            float progress = (timeElapsed - randomDelay) / (duration / 2f);
            progress = Mathf.Clamp01(progress);

            progress = EaseInOut(progress);

            float currentDistance = moveDistance * (isReversing ? (1 - progress) : progress);
            Vector3 currentPosition = startPosition + moveAxis.normalized * currentDistance;

            transform.position = currentPosition;

            timeElapsed += deltaTime;

            if (timeElapsed >= duration / 2f + randomDelay)
            {
                timeElapsed = randomDelay;
                isReversing = !isReversing;
            }
        }

        private float EaseInOut(float t)
        {
            return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
        }

        private void OnDestroy()
        {
            UpdateHandlerManager.Instance.RemoveUpdateBehaviour(this);
        }
    }
}
