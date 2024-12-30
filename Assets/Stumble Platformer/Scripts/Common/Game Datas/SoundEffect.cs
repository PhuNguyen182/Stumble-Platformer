using System;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Common.GameDatas
{
    [Serializable]
    public struct SoundEffect
    {
        public SoundEffectType SoundEffectType;
        public AudioClip SoundFXClip;
    }
}
