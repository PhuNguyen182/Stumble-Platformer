using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StumblePlatformer.Scripts.UI
{
    public class UIBlocker : Singleton<UIBlocker>
    {
        [SerializeField] private Image blocker;

        public void Block() => blocker.gameObject.SetActive(true);

        public void Unblock() => blocker.gameObject.SetActive(false);
    }
}
