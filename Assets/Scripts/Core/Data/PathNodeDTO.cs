using System;
using System.Collections.Generic;

namespace LifeSim.Core.Data.DTO
{
    [Serializable] public class PathNodeDTO { public int level; public string featureId; public string alignment; }
    [Serializable] public class PathNodesFile { public string version="1"; public List<PathNodeDTO> items = new(); }
}