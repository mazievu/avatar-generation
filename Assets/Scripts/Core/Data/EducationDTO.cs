using System;
using System.Collections.Generic;

namespace LifeSim.Core.Data.DTO
{
    [Serializable] public class SchoolOptionDTO { public string key; public string nameKey; public int cost; public Dictionary<string,int> effects; }
    [Serializable] public class SchoolOptionsFile { public string version="1"; public List<SchoolOptionDTO> items = new(); }
    [Serializable] public class UniversityMajorDTO { public string key; public string nameKey; public string descriptionKey; public int cost; public int iqBonus; public int skillBonus; }
    [Serializable] public class UniversityMajorsFile { public string version="1"; public List<UniversityMajorDTO> items = new(); }
}