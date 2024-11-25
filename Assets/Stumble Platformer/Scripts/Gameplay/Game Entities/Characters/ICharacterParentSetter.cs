using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public interface ICharacterParentSetter
    {
        public void SetParent(Transform parent, bool stayWorldPosition = true);
    }
}
