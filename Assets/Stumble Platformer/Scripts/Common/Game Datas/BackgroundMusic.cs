using System;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Common.GameDatas
{
    [Serializable]
    public struct BackgroundMusic
    {
        public BackgroundMusicType BackgroundMusicType;
        public AudioClip BackgroundMusicClip;
    }
}
