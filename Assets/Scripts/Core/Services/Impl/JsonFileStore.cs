using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifeSim.Core.Services
{
    public class JsonFileStore : ISaveStore
    {
        private string Root => Application.persistentDataPath;

        public async Task SaveAsync<T>(string key, T data)
        {
            var path = Path.Combine(Root, key + ".json");
            var json = JsonUtility.ToJson(data, false);
            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
                await writer.WriteAsync(json);
        }

        public async Task<T> LoadAsync<T>(string key, T fallback = default)
        {
            var path = Path.Combine(Root, key + ".json");
            if (!File.Exists(path)) return fallback;
            using (var reader = new StreamReader(path, Encoding.UTF8))
            {
                var json = await reader.ReadToEndAsync();
                return JsonUtility.FromJson<T>(json);
            }
        }
    }

    public static class TaskExt
    {
        public static async void Forget(this Task t) { await t; }
    }
}