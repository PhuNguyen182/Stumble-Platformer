using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.CommonMovement
{
    [RequireComponent(typeof(Animator))]
    public class RandomOffsetAnimation : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private string titleAnim;

        private int _animHash;
        private float _offsetAnim;

        private void Awake()
        {
            _animHash = Animator.StringToHash(titleAnim);
        }

        private void Start()
        {
            _offsetAnim = Random.Range(0f, 1f);
            anim.Play(_animHash, 0, _offsetAnim);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            anim ??= GetComponent<Animator>();
        }
#endif
    }
}
