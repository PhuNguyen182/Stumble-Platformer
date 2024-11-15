using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public abstract class BasePlayRule : MonoBehaviour, IPlayeRule
    {
        public abstract void OnPlayerFinish();
        public abstract void OnPlayerFall();

        public void Win()
        {
            
        }

        public void Lose()
        {
            
        }
    }
}
