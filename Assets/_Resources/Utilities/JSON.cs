using System.IO;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

namespace Utilities
{
    public static class JSON
    {
        public static string ToJSON<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJSON<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async UniTask<string> ToJSONAsync<T>(T obj)
        {
            return await UniTask.RunOnThreadPool(() => ToJSON(obj));
        }

        public static async UniTask<T> FromJSONAsync<T>(string json)
        {
            return await UniTask.RunOnThreadPool<T>(() => FromJSONAsync<T>(json));
        }

        public static string ToJSON<T>(T obj, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T FromJSON<T>(string json, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static async UniTask<string> ToJSONAsync<T>(T obj, JsonSerializerSettings settings)
        {
            return await UniTask.RunOnThreadPool(() => ToJSON(obj, settings));
        }

        public static async UniTask<T> FromJSONAsync<T>(string json, JsonSerializerSettings settings)
        {
            return await UniTask.RunOnThreadPool(() => FromJSON<T>(json, settings));
        }

        public static void SaveToJSON<T>(T obj, string dirPath, string fileName)
        {
            string jsonData = ToJSON(obj);
            string unionPath = $"{dirPath}/{fileName}.json";

            if (!File.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            File.WriteAllText(unionPath, jsonData);
        }

        public static void SaveToJSON<T>(T obj, string dirPath, string fileName, JsonSerializerSettings settings)
        {
            string jsonData = ToJSON(obj, settings);
            string unionPath = $"{dirPath}/{fileName}.json";

            if (!File.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            File.WriteAllText(unionPath, jsonData);
        }

        public static async UniTask SaveToJSONAsync<T>(T obj, string dirPath, string fileName)
        {
            string jsonData = await ToJSONAsync(obj);
            string unionPath = $"{dirPath}/{fileName}.json";

            if (!File.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            await File.WriteAllTextAsync(unionPath, jsonData);
        }

        public static async UniTask SaveToJSONAsync<T>(T obj, string dirPath, string fileName, JsonSerializerSettings settings)
        {
            string jsonData = await ToJSONAsync(obj, settings);
            string unionPath = $"{dirPath}/{fileName}.json";

            if (!File.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            await File.WriteAllTextAsync(unionPath, jsonData);
        }
    }
}
