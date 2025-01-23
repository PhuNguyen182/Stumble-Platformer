using UnityEngine;
using Unity.Netcode;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public interface ICharacterParentSetter
    {
        public void SetParent(Transform parent, bool stayWorldPosition = true);
        public bool SetNetworkParent(NetworkObject parent);
    }
}
