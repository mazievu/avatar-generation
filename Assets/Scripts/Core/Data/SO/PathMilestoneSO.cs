
using UnityEngine;

namespace LifeSim.Core.Data.SO
{
    [CreateAssetMenu(fileName = "NewPathMilestone", menuName = "LifeSim/Path Milestone")]
    public class PathMilestoneSO : ScriptableObject
    {
        public string featureId; // e.g., "unlock_business", "unlock_twins"
        public string descriptionKey;
        public int childrenRequired;
    }
}
