using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public interface IPlatform
    {
        public void PlatformAction();

        public void OnPlatformStay(Collision collision);

        public void OnPlatformExit(Collision collision);
    }
}
