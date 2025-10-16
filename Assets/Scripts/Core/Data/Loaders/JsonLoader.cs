using UnityEngine;
using LifeSim.Core.Data.DTO;
using LifeSim.Core.Domain.Careers;
using LifeSim.Core.Domain.Education;
using System.Collections.Generic;

namespace LifeSim.Core.Data.Loaders
{
    public static class JsonLoader
    {
        private static T LoadJson<T>(string resourcesPath)
        {
            var textAsset = Resources.Load<TextAsset>(resourcesPath);
            if (textAsset == null)
            {
                Debug.LogError($"[JsonLoader] Missing Resources/{resourcesPath}.json");
                return default;
            }
            return JsonUtility.FromJson<T>(textAsset.text);
        }

        public static List<SchoolOption> LoadSchoolOptions(string lang)
        {
            var file = LoadJson<SchoolOptionsFile>($"Databases/Education/school_options_{lang}");
            var list = new List<SchoolOption>();
            if (file?.items == null) return list;
            foreach (var dto in file.items)
            {
                list.Add(new SchoolOption
                {
                    key = dto.key,
                    nameKey = dto.nameKey,
                    cost = dto.cost,
                    effects = dto.effects ?? new Dictionary<string, int>()
                });
            }
            return list;
        }

        public static List<UniversityMajor> LoadUniversityMajors(string lang)
        {
            var file = LoadJson<UniversityMajorsFile>($"Databases/Education/university_majors_{lang}");
            var list = new List<UniversityMajor>();
            if (file?.items == null) return list;
            foreach (var dto in file.items)
            {
                list.Add(new UniversityMajor
                {
                    key = dto.key,
                    nameKey = dto.nameKey,
                    descriptionKey = dto.descriptionKey,
                    cost = dto.cost,
                    iqBonus = dto.iqBonus,
                    skillBonus = dto.skillBonus
                });
            }
            return list;
        }

        public static List<CareerTrack> LoadCareerTracks(string lang)
        {
            var file = LoadJson<CareerTracksFile>($"Databases/Careers/career_tracks_{lang}");
            var list = new List<CareerTrack>();
            if (file?.items == null) return list;
            foreach (var dto in file.items)
            {
                var track = new CareerTrack { key = dto.key, nameKey = dto.nameKey, ladder = new List<CareerStep>() };
                if (dto.ladder != null)
                {
                    foreach (var step in dto.ladder)
                    {
                        track.ladder.Add(new CareerStep
                        {
                            titleKey = step.titleKey,
                            requiredIQ = step.requiredIQ,
                            requiredEQ = step.requiredEQ,
                            baseSalary = step.baseSalary
                        });
                    }
                }
                list.Add(track);
            }
            return list;
        }

        public static List<LifeSim.Core.Data.UnlockableFeature> LoadUnlockableFeatures(string lang)
        {
            var file = LoadJson<UnlockableFeaturesFile>($"Databases/Features/unlockable_features_{lang}");
            var list = new List<LifeSim.Core.Data.UnlockableFeature>();
            if (file?.items == null) return list;
            foreach (var dto in file.items)
            {
                var type = LifeSim.Core.Data.FeatureType.QoL;
                if (dto.type == "MysteryBox") type = LifeSim.Core.Data.FeatureType.MysteryBox;
                else if (dto.type == "SpecificFeature") type = LifeSim.Core.Data.FeatureType.SpecificFeature;

                list.Add(new LifeSim.Core.Data.UnlockableFeature
                {
                    id = dto.id,
                    type = type,
                    nameKey = dto.nameKey,
                    descriptionKey = dto.descriptionKey,
                    iconKey = dto.iconKey
                });
            }
            return list;
        }

        public static List<LifeSim.Core.Data.PathNode> LoadPathNodes()
        {
            var file = LoadJson<PathNodesFile>($"Databases/PathOfLife/path_nodes");
            var list = new List<LifeSim.Core.Data.PathNode>();
            if (file?.items == null) return list;
            foreach (var dto in file.items)
            {
                list.Add(new LifeSim.Core.Data.PathNode
                {
                    level = dto.level,
                    featureId = dto.featureId,
                    alignment = dto.alignment
                });
            }
            return list;
        }
    }
}