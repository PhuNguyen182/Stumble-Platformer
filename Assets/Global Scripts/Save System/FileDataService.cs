using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalScripts.SaveSystem
{
    public class FileDataService<T> : IDataService<T>
    {
        private readonly string _filePath;
        private readonly string _fileExtension;
        private readonly ISerializer<T> _serializer;

        public FileDataService(ISerializer<T> serializer)
        {
            _serializer = serializer;
            _filePath = Application.persistentDataPath;
            _fileExtension = _serializer.FileExtension;
        }

        public string GetDataPath(string name)
        {
            string dataPath = Path.Combine(Application.persistentDataPath, $"{name}{_fileExtension}");
            return dataPath;
        }

        public T LoadData(string name)
        {
            string dataPath = GetDataPath(name);

            if (File.Exists(dataPath))
            {
                using (StreamReader streamReader = new(dataPath))
                {
                    string serializedData = streamReader.ReadToEnd();
                    T data = _serializer.Deserialize(serializedData);
                    streamReader.Close();
                    return data;
                }
            }

            return default;
        }

        public void SaveData(string name, T data)
        {
            string dataPath = GetDataPath(name);

            using (StreamWriter writer = new(dataPath))
            {
                string serializedData = _serializer.Serialize(data);
                writer.Write(serializedData);
                writer.Close();
            }
        }

        public void DeleteData(string name)
        {
            string dataPath = GetDataPath(name);

            if (File.Exists(dataPath))
            {
                File.Delete(dataPath);
            }
        }
    }
}
