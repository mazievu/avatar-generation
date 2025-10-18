
using UnityEngine;
using System.IO;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Services;

// Note: This file was in the wrong directory. It belongs in Core/Infra.
// Overwriting it here to fix compilation errors.
namespace LifeSim.Core.Services.Impl
{
    public class JsonFileStore : ISaveStore
    {
        private const string SaveFileName = "savegame.json";

        private string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, SaveFileName);
        }

        public void Save(GameState state)
        {
            string json = JsonUtility.ToJson(state, true);
            File.WriteAllText(GetSavePath(), json);
            Debug.Log($"Game saved to {GetSavePath()}");
        }

        public GameState Load()
        {
            string path = GetSavePath();
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Debug.Log("Loading saved game...");
                return JsonUtility.FromJson<GameState>(json);
            }
            
            Debug.Log("No save file found.");
            return null;
        }
    }
}
