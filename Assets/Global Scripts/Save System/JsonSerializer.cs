using System.IO;
using Newtonsoft.Json;

namespace GlobalScripts.SaveSystem
{
    public class JsonSerializer<T> : ISerializer<T>
    {
        public string FileExtension => ".json";

        public string Serialize(T data)
        {
            JsonSerializerSettings settings = new()
            {
                Formatting = Formatting.Indented,
            };

            string json = JsonConvert.SerializeObject(data, settings);
            return json;
        }

        public T Deserialize(string name)
        {
            using (StringReader stringReader = new(name))
            {
                using (JsonTextReader jsonReader = new(stringReader))
                {
                    JsonSerializer jsonSerializer = new();
                    T data = jsonSerializer.Deserialize<T>(jsonReader);

                    stringReader.Close();
                    return data;
                }
            }
        }
    }
}
