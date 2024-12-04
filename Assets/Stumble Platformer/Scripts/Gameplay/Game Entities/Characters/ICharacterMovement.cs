using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public interface ICharacterMovement
    {
        public bool IsStunning { get; }
        public void OnGrounded();
        public void ResetPlayerOrientation(Quaternion orientation);
    }
}
