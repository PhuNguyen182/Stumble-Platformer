using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.Pool;

namespace GlobalScripts.Effects
{
    [RequireComponent(typeof(AutoDespawn))]
    [RequireComponent(typeof(AudioSource))]
    public class ItemSoundEffect : MonoBehaviour
    {
        [SerializeField] private AudioSource soundPlayer;
        [SerializeField] private AutoDespawn autoDespawn;

        public void PlaySound(AudioClip clip)
        {
            float duration = clip.length * 1.5f > 0.1f 
                             ? clip.length * 1.5f : 0.1f;
            
            autoDespawn.SetDuration(duration);
            soundPlayer.PlayOneShot(clip);
        }

        private void OnDisable()
        {
            autoDespawn.SetDuration(1);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            autoDespawn ??= GetComponent<AutoDespawn>();
            soundPlayer ??= GetComponent<AudioSource>();
        }
#endif
    }
}
