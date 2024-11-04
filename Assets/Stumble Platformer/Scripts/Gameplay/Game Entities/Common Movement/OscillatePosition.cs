using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.CommonMovement
{
    public class OscillatePosition : MonoBehaviour, IUpdateHandler
    {
        [Header("Movement")]
        [SerializeField] private AnimationCurve easeCurve;
        [SerializeField] private Vector3 moveAxis = Vector3.up;
        [SerializeField] private float moveDistance = 2f;
        [SerializeField] private float duration = 2f;

        [Header("Delay")]
        [SerializeField] private bool useRandomDelay = false;
        [SerializeField] private float maxRandomDelay = 1f;

        private bool isReversing = false;
        private float timeElapsed = 0f;
        private float randomDelay = 0f;
        private Vector3 startPosition;

        public bool IsActive { get; set; }

        private void Start()
        {
            IsActive = true;
            startPosition = transform.position;
            randomDelay = useRandomDelay ? Random.Range(0f, maxRandomDelay) : maxRandomDelay;
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
            progress = EaseByCurve(progress);

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

        private float EaseByCurve(float t)
        {
            return easeCurve.Evaluate(t);
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
