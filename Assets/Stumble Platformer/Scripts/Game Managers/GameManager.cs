using UnityEngine;
using StumblePlatformer.Scripts.GameDatas;
using StumblePlatformer.Scripts.GameManagers.PeriodicallyHandlers;

namespace StumblePlatformer.Scripts.GameManagers
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        [SerializeField] public ConectionHandler ConectionHandler;

        private void SaveGameData()
        {
            GameDataManager.Instance.SaveData();
        }

        private void OnDestroy()
        {
            SaveGameData();
        }

#if !UNITY_EDITOR
        private void OnApplicationQuit()
        {
            SaveGameData();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
                SaveGameData();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
                SaveGameData();
        }
#endif
    }
}
