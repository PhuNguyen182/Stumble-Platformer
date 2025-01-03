using GlobalScripts.SaveSystem;

namespace StumblePlatformer.Scripts.GameDatas
{
    public class GameDataManager : SingletonClass<GameDataManager>
    {
        private PlayerProfile _playerProfile;
        private GameResourceData _gameResourceData;

        private const string PlayerDataKey = "PlayerProfile";
        private const string GameResourceDataKey = "GameResources";

        public PlayerProfile PlayerProfile => _playerProfile;
        public GameResourceData GameResourceData => _gameResourceData;

        private void ReleaseBackData()
        {
            _gameResourceData.ReleaseBack();
        }

        public void InitializeData()
        {
            _gameResourceData.Initialize();
        }

        public void LoadData()
        {
            _playerProfile = SimpleSaveSystem<PlayerProfile>.LoadDataByJson(PlayerDataKey) ?? new();
            _gameResourceData = SimpleSaveSystem<GameResourceData>.LoadDataByJson(GameResourceDataKey) ?? new();
        }

        public void SaveData()
        {
            ReleaseBackData();
            SimpleSaveSystem<PlayerProfile>.SaveDataByJson(PlayerDataKey, _playerProfile);
            SimpleSaveSystem<GameResourceData>.SaveDataByJson(GameResourceDataKey, _gameResourceData);
        }

        public void DeleteData()
        {
            SimpleSaveSystem<PlayerProfile>.DeleteJsonData(PlayerDataKey);
            SimpleSaveSystem<GameResourceData>.DeleteJsonData(GameResourceDataKey);
        }
    }
}
