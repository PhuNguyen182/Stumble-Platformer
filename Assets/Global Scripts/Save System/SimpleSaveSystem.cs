namespace GlobalScripts.SaveSystem
{
    public static class SimpleSaveSystem<T>
    {
        private static ISerializer<T> s_JsonSerializer;
        private static ISerializer<T> s_EncryptedJsonSerializer;

        private static IDataService<T> s_JsonDataService;
        private static IDataService<T> s_EncryptedJsonDataService;

        static SimpleSaveSystem()
        {
            s_JsonSerializer = new JsonSerializer<T>();
            s_EncryptedJsonSerializer = new EncryptedJsonSerializer<T>();

            s_JsonDataService = new FileDataService<T>(s_JsonSerializer);
            s_EncryptedJsonDataService = new FileDataService<T>(s_EncryptedJsonSerializer);
        }

        public static T LoadDataByJson(string name)
        {
            return s_JsonDataService.LoadData(name);
        }

        public static T LoadDataWithEncryption(string name)
        {
            return s_EncryptedJsonDataService.LoadData(name);
        }

        public static void SaveDataByJson(string name, T data)
        {
            s_JsonDataService.SaveData(name, data);
        }

        public static void SaveDataWithEncryption(string name, T data)
        {
            s_EncryptedJsonDataService.SaveData(name, data);
        }

        public static void DeleteJsonData(string name)
        {
            s_JsonDataService.DeleteData(name);
        }

        public static void DeleteEncryptedJsonData(string name)
        {
            s_EncryptedJsonDataService.DeleteData(name);
        }
    }
}
