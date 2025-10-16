using System;
using System.Collections.Generic;

namespace LifeSim.Core.Data.DTO
{
    [Serializable] public class SchoolOptionDTO { public string key; public string nameKey; public int cost; public Dictionary<string,int> effects; }
    [Serializable] public class SchoolOptionsFile { public string version="1"; public List<SchoolOptionDTO> items = new(); }

    [Serializable] public class UniversityMajorDTO { public string key; public string nameKey; public string descriptionKey; public int cost; public int iqBonus; public int skillBonus; }
    [Serializable] public class UniversityMajorsFile { public string version="1"; public List<UniversityMajorDTO> items = new(); }

    [Serializable] public class CareerStepDTO { public string titleKey; public int requiredIQ; public int requiredEQ; public int baseSalary; }
    [Serializable] public class CareerTrackDTO { public string key; public string nameKey; public List<CareerStepDTO> ladder; }
    [Serializable] public class CareerTracksFile { public string version="1"; public List<CareerTrackDTO> items = new(); }

    [Serializable] public class ClubDTO { public string id; public string nameKey; public string descriptionKey; public int fee; public Dictionary<string,int> effects; }
    [Serializable] public class ClubsFile { public string version="1"; public List<ClubDTO> items = new(); }

    [Serializable] public class UnlockableFeatureDTO { public string id; public string type; public string nameKey; public string descriptionKey; public string iconKey; }
    [Serializable] public class UnlockableFeaturesFile { public string version="1"; public List<UnlockableFeatureDTO> items = new(); }

    [Serializable] public class PathNodeDTO { public int level; public string featureId; public string alignment; }
    [Serializable] public class PathNodesFile { public string version="1"; public List<PathNodeDTO> items = new(); }

    [Serializable] public class EventChoiceDTO { public string id; public string labelKey; }
    [Serializable] public class GameEventDTO
    {
        public string id;
        public string characterStage;
        public string titleKey;
        public string bodyKey;
        public List<EventChoiceDTO> choices;
        public Dictionary<string,string> conditions;
        public Dictionary<string,int> rewards;
    }
    [Serializable] public class EventsFile { public string version="1"; public List<GameEventDTO> items = new(); }
}