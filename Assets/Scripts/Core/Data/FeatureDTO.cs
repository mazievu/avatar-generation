using System;
using System.Collections.Generic;

namespace LifeSim.Core.Data.DTO
{
    [Serializable] public class UnlockableFeatureDTO { public string id; public string type; public string nameKey; public string descriptionKey; public string iconKey; }
    [Serializable] public class UnlockableFeaturesFile { public string version="1"; public List<UnlockableFeatureDTO> items = new(); }
}