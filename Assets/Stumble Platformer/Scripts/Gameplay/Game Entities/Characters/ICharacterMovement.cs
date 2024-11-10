using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public interface ICharacterMovement
    {
        public bool IsStunning { get; }
    }

    public interface ICharacterParentSetter
    {
        public void SetParent(Transform parent, bool stayWorldPosition = true);
    }
}
