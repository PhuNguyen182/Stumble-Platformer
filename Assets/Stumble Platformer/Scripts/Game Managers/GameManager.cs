using StumblePlatformer.Scripts.GameDatas;

namespace StumblePlatformer.Scripts.GameManagers
{
    public class GameManager : PersistentSingleton<GameManager>
    {
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
