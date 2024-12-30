using GlobalScripts.SaveSystem;

namespace StumblePlatformer.Scripts.GameDatas
{
    public class GameDataManager : SingletonClass<GameDataManager>
    {
        private PlayerGameData _playerGameData;
        private GameResourceData _gameResourceData;

        private const string PlayerDataKey = "PlayerGameData";
        private const string GameResourceDataKey = "GameResources";

        public PlayerGameData PlayerGameData => _playerGameData;
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
            _playerGameData = SimpleSaveSystem<PlayerGameData>.LoadDataByJson(PlayerDataKey) ?? new();
            _gameResourceData = SimpleSaveSystem<GameResourceData>.LoadDataByJson(GameResourceDataKey) ?? new();
        }

        public void SaveData()
        {
            ReleaseBackData();
            SimpleSaveSystem<PlayerGameData>.SaveDataByJson(PlayerDataKey, _playerGameData);
            SimpleSaveSystem<GameResourceData>.SaveDataByJson(GameResourceDataKey, _gameResourceData);
        }

        public void DeleteData()
        {
            SimpleSaveSystem<PlayerGameData>.DeleteJsonData(PlayerDataKey);
            SimpleSaveSystem<GameResourceData>.DeleteJsonData(GameResourceDataKey);
        }
    }
}
