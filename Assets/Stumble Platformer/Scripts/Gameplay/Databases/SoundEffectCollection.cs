using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.GameDatas;
using StumblePlatformer.Scripts.Common.Enums;
using System.Linq;

namespace StumblePlatformer.Scripts.Gameplay.Databases
{
    [CreateAssetMenu(fileName = "SoundFX Collection", menuName = "Scriptable Objects/Collections/SoundFX Collection")]
    public class SoundEffectCollection : ScriptableObject
    {
        [SerializeField] public SoundEffect[] SoundEffects;

        public Dictionary<SoundEffectType, AudioClip> SoundFXCollection;

        public void Initialize()
        {
            SoundFXCollection = SoundEffects.ToDictionary(kvp => kvp.SoundEffectType, kvp => kvp.SoundFXClip);
        }

        public AudioClip GetSoundEffect(SoundEffectType soundEffect)
        {
            return SoundFXCollection.TryGetValue(soundEffect, out var clip) ? clip : null;
        }
    }
}
