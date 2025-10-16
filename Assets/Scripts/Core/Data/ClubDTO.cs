using System;
using System.Collections.Generic;

namespace LifeSim.Core.Data.DTO
{
    [Serializable] public class ClubDTO { public string id; public string nameKey; public string descriptionKey; public int fee; public Dictionary<string,int> effects; }
    [Serializable] public class ClubsFile { public string version="1"; public List<ClubDTO> items = new(); }
}