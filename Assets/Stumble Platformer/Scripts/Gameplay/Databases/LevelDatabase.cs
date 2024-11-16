using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.GameDatas;

namespace StumblePlatformer.Scripts.Gameplay.Databases
{
    [CreateAssetMenu(fileName = "Level Database", menuName = "Scriptable Objects/Databases/Level Database")]
    public class LevelDatabase : ScriptableObject
    {
        [SerializeField] private List<LevelEntryData> levelEntryDatas;

        public LevelEntryData GetLevelAt(int index)
        {
            return index < 0 || index >= levelEntryDatas.Count ? new() : levelEntryDatas[index];
        }

        public LevelEntryData GetRandomLevel()
        {
            int rand = Random.Range(0, levelEntryDatas.Count);
            return GetLevelAt(rand);
        }
    }
}
