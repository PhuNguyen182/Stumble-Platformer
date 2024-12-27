using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.GameDatas;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.Databases
{
    [CreateAssetMenu(fileName = "Music Collection", menuName = "Scriptable Objects/Collections/Music Collection")]
    public class MusicCollection : ScriptableObject
    {
        [SerializeField] public BackgroundMusic[] BackgroundMusics;

        public Dictionary<BackgroundMusicType, AudioClip> BackgroundMusicCollection;

        public void Initialize()
        {
            BackgroundMusicCollection = BackgroundMusics.ToDictionary(kvp => kvp.BackgroundMusicType, kvp => kvp.BackgroundMusicClip);
        }

        public AudioClip GetBackgroundMusic(BackgroundMusicType musicType)
        {
            return BackgroundMusicCollection.TryGetValue(musicType, out var clip) ? clip : null;
        }
    }
}
