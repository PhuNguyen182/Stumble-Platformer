namespace GlobalScripts.SaveSystem
{
    public interface ISerializer<T>
    {
        public string FileExtension { get; }
        public string Serialize(T data);
        public T Deserialize(string name);
    }
}
