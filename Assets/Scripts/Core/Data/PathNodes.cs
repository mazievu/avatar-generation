using System;
using System.Collections.Generic;

namespace LifeSim.Core.Data
{
    [Serializable]
    public class PathNode
    {
        public int level;
        public string featureId;
        public string alignment; // "left" | "right"
    }

    public static class PathNodes
    {
        public static readonly List<PathNode> Nodes = new List<PathNode>
        {
            new PathNode{ level=0,  featureId="feat_mystery_box_1", alignment="left" },
            new PathNode{ level=3,  featureId="feat_icon_pack",     alignment="right"},
            new PathNode{ level=5,  featureId="feat_boost_small",   alignment="left" },
            new PathNode{ level=10, featureId="feat_big_reward",    alignment="right"},
        };
    }
}