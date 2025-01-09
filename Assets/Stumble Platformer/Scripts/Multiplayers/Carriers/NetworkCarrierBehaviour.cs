using Unity.Netcode;

namespace StumblePlatformer.Scripts.Multiplayers.Carriers
{
    public abstract class NetworkCarrierBehaviour : NetworkBehaviour
    {
    }

    public class NetworkCarrierBehaviour<T> : NetworkCarrierBehaviour
    {
        public NetworkVariable<T> NetworkData { get; private set; }

        private void Awake()
        {
            NetworkData = new();
        }

        public void Initialize(T data)
        {
            NetworkData.Value = data;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (IsHost || IsServer)
                NetworkData?.Dispose();
        }
    }
}
