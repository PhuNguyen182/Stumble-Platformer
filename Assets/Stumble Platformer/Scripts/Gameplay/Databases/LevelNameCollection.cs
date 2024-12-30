using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace StumblePlatformer.Scripts.Gameplay.Databases
{
    [CreateAssetMenu(fileName = "Level Name Collection", menuName = "Scriptable Objects/Collections/Level Name Collection")]
    public class LevelNameCollection : ScriptableObject
    {
        [SerializeField] public string path;
        [SerializeField] public string[] LevelNames;

        [Button]
        public void GetLevelName()
        {
            if (Directory.Exists(path))
            {
                LevelNames = Directory.GetFiles(path, "*.unity");
                for (int i = 0; i < LevelNames.Length; i++)
                {
                    string levelName = LevelNames[i];
                    string[] splitedName = levelName.Split('/', '\\');
                    string finalName = splitedName[splitedName.Length - 1];
                    string[] splitedResult = finalName.Split('.');
                    LevelNames[i] = splitedResult[0];
                }
            }
        }

        public string GetRandomName()
        {
            int index = Random.Range(0, LevelNames.Length);
            return LevelNames[index];
        }
    }
}
