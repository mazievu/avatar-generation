using System.Collections.Generic;

namespace LifeSim.Core.Domain.Economy
{
    public class Asset
    {
        public string id;
        public string name;
        public int cost;
        public Dictionary<string, int> effects;
    }
}
