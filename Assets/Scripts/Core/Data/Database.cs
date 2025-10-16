using System.Collections.Generic;
using LifeSim.Core.Domain.Careers;
using LifeSim.Core.Domain.Education;
using LifeSim.Core.Data.Loaders;

namespace LifeSim.Core.Data
{
    public static class Database
    {
        public static List<SchoolOption> SchoolOptions { get; private set; }
        public static List<UniversityMajor> UniversityMajors { get; private set; }
        public static List<LifeSim.Core.Data.UnlockableFeature> UnlockableFeatures { get; private set; }
        public static List<LifeSim.Core.Data.PathNode> PathNodes { get; private set; }
        public static List<CareerTrack> CareerTracks { get; private set; }

        public static void LoadAll(string lang)
        {
            SchoolOptions     = JsonLoader.LoadSchoolOptions(lang);
            UniversityMajors  = JsonLoader.LoadUniversityMajors(lang);
            CareerTracks      = JsonLoader.LoadCareerTracks(lang);
            UnlockableFeatures= JsonLoader.LoadUnlockableFeatures(lang);
            PathNodes         = JsonLoader.LoadPathNodes();
        }
    }
}