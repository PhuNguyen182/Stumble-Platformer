using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using Sirenix.OdinInspector;

namespace StumblePlatformer.Scripts.Common.GameDatas
{
    [Serializable]
    public struct LevelEntryData
    {
        public string EntryName;
        public LevelObjective LevelObjective;
        [PreviewField(90, Alignment = ObjectFieldAlignment.Left)] 
        public Sprite Preview;
    }
}
