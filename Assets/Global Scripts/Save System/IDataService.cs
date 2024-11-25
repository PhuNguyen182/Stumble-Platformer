namespace GlobalScripts.SaveSystem
{
    public interface IDataService<T>
    {
        public T LoadData(string name);
        public void SaveData(string name, T data);
        public void DeleteData(string name);
        public string GetDataPath(string name);
    }
}
