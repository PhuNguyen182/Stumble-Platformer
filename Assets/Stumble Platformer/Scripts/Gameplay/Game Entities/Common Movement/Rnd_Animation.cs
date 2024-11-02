using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.CommonMovement
{
    public class Rnd_Animation : MonoBehaviour
    {
        private Animator anim;
        private float offsetAnim;

        [SerializeField] private string titleAnim;

        private void Start()
        {
            anim = GetComponent<Animator>();
            offsetAnim = Random.Range(0f, 1f);
            anim.Play(titleAnim, 0, offsetAnim);
        }
    }
}
