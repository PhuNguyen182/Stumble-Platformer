using UnityEngine;
using GlobalScripts.UpdateHandlerPattern;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.CommonMovement
{
    public class OscillatePosition : MonoBehaviour, IFixedUpdateHandler
    {
        [Header("Movement")]
        [SerializeField] public AnimationCurve easeCurve;
        [SerializeField] public Vector3 moveAxis = Vector3.up;
        [SerializeField] public float moveDistance = 2f;
        [SerializeField] public float duration = 2f;
        [SerializeField] public bool useRotationAsDirection;

        [Header("Delay")]
        [SerializeField] public bool useRandomDelay = false;
        [SerializeField] public float maxRandomDelay = 1f;

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
            UpdateHandlerManager.Instance.AddFixedUpdateBehaviour(this);
        }

        public void OnFixedUpdate()
        {
            if (timeElapsed < randomDelay)
            {
                timeElapsed += Time.fixedDeltaTime;
                return;
            }

            float progress = (timeElapsed - randomDelay) / (duration / 2f);
            progress = Mathf.Clamp01(progress);
            progress = EaseByCurve(progress);

            float currentDistance = moveDistance * (isReversing ? (1 - progress) : progress);
            Vector3 direction = useRotationAsDirection ? transform.rotation * moveAxis.normalized : moveAxis.normalized;
            Vector3 currentPosition = startPosition + direction * currentDistance;
            transform.position = currentPosition;

            timeElapsed += Time.fixedDeltaTime;
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
            UpdateHandlerManager.Instance.RemoveFixedUpdateBehaviour(this);
        }
    }
}
