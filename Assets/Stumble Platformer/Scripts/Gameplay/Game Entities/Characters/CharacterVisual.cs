using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public class CharacterVisual : MonoBehaviour
    {
        public Animator CharacterAnimator;

#if UNITY_EDITOR
        private void OnValidate()
        {
            CharacterAnimator ??= GetComponent<Animator>();
        }
#endif
    }
}
