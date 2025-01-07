using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace StumblePlatformer.Scripts.Multiplayers.Carriers
{
    public abstract class CarrierBehaviour : MonoBehaviour
    {

    }

    public class CarrierBehaviour<T> : CarrierBehaviour
    {
        public NetworkVariable<T> NetworkData { get; private set; }

        public void Initialize(T data)
        {
            NetworkData = new(data);
        }
    }
}
