using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerSound : MonoBehaviour
    {
        [SerializeField] private AudioSource playerAudio;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip hurtClip;

        public void PlayJumpSound()
        {
            playerAudio.PlayOneShot(jumpClip, 0.9f);
        }

        public void PlayHurtSound()
        {
            playerAudio.PlayOneShot(hurtClip, 0.75f);
        }
    }
}
