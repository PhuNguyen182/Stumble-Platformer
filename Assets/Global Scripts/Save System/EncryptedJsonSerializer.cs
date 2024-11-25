using System;
using System.IO;
using GlobalScripts.Encryption;
using Newtonsoft.Json;

namespace GlobalScripts.SaveSystem
{
    public class EncryptedJsonSerializer<T> : ISerializer<T>
    {
        public string FileExtension => ".jsonaes";

        public string Serialize(T data)
        {
            JsonSerializerSettings settings = new()
            {
                Formatting = Formatting.Indented,
            };

            string json = JsonConvert.SerializeObject(data, settings);
            byte[] cipheredJson = AesEncryptor.Encrypt(json);
            string encryptedJson = $"{BitConverter.ToDouble(cipheredJson)}";
            return encryptedJson;
        }

        public T Deserialize(string name)
        {
            double cipheredValue = double.Parse(name);
            byte[] cipheredArray = BitConverter.GetBytes(cipheredValue);
            string decryptedJson = AesEncryptor.Decrypt(cipheredArray);

            using (StringReader stringReader = new(decryptedJson))
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
