using System.Collections.Generic;
using LifeSim.Core.Domain.Careers;
using LifeSim.Core.Domain.Education;

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
            SchoolOptions     = Loaders.JsonLoader.LoadSchoolOptions(lang);
            UniversityMajors  = Loaders.JsonLoader.LoadUniversityMajors(lang);
            CareerTracks      = Loaders.JsonLoader.LoadCareerTracks(lang);
            UnlockableFeatures= Loaders.JsonLoader.LoadUnlockableFeatures(lang);
            PathNodes         = Loaders.JsonLoader.LoadPathNodes();
        }
    }
}