using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LifeSim.Core.Data.SO;

namespace LifeSim.Core.Data
{
    public static class Database
    {
        public static Dictionary<string, EventSO> Events { get; private set; } = new Dictionary<string, EventSO>();
        public static Dictionary<string, CareerSO> Careers { get; private set; } = new Dictionary<string, CareerSO>();
        public static Dictionary<string, AssetSO> Assets { get; private set; } = new Dictionary<string, AssetSO>();
        public static Dictionary<string, EducationSO> EducationOptions { get; private set; } = new Dictionary<string, EducationSO>();
        public static Dictionary<string, BusinessSO> Businesses { get; private set; } = new Dictionary<string, BusinessSO>();
        public static Dictionary<string, ClubSO> Clubs { get; private set; } = new Dictionary<string, ClubSO>();
        public static Dictionary<string, CompanySO> Companies { get; private set; } = new Dictionary<string, CompanySO>();
        public static Dictionary<string, PetSO> Pets { get; private set; } = new Dictionary<string, PetSO>();
        public static Dictionary<string, PathMilestoneSO> PathOfLife { get; private set; } = new Dictionary<string, PathMilestoneSO>();
        public static Dictionary<string, List<string>> AvatarSprites { get; private set; } = new Dictionary<string, List<string>>();

        private static bool _isLoaded = false;

        public static void LoadAll(string language)
        {
            if (_isLoaded) return;

            Events = LoadAndCache<EventSO>("Data/Events");
            Careers = LoadAndCache<CareerSO>("Data/Careers");
            Assets = LoadAndCache<AssetSO>("Data/Assets");
            EducationOptions = LoadAndCache<EducationSO>("Data/Education");
            Businesses = LoadAndCache<BusinessSO>("Data/Businesses");
            Clubs = LoadAndCache<ClubSO>("Data/Clubs");
            Companies = LoadAndCache<CompanySO>("Data/Companies");
            Pets = LoadAndCache<PetSO>("Data/Pets");
            PathOfLife = LoadAndCache<PathMilestoneSO>("Data/PathOfLife");

            LoadAvatarData();

            _isLoaded = true;
            Debug.Log("Game Database loaded successfully.");
        }

        private static void LoadAvatarData()
        {
            AvatarSprites.Clear();
            var layerNames = new[] { "body", "hair_front", "hair_back", "eyes", "eyebrows", "nose", "mouth", "facial_hair" };

            foreach (var layer in layerNames)
            {
                var path = $"Avatars/{layer}";
                var sprites = Resources.LoadAll<Sprite>(path);
                AvatarSprites[layer] = sprites.Select(s => s.name).ToList();
                Debug.Log($"Loaded {AvatarSprites[layer].Count} sprites for layer '{layer}'");
            }
        }

        private static Dictionary<string, T> LoadAndCache<T>(string path) where T : ScriptableObject
        {
            var items = Resources.LoadAll<T>(path);
            return items.ToDictionary(item => item.name, item => item);
        }
    }
}