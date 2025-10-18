
using UnityEngine;

namespace LifeSim.Core.Data.SO
{
    [CreateAssetMenu(fileName = "NewPet", menuName = "LifeSim/Pet")]
    public class PetSO : ScriptableObject
    {
        public string petId;
        public string petNameKey; // Localization key
        public string petType; // e.g., "dog", "cat", "goldfish"

        [Header("Economics")]
        public int adoptionFee;
        public int monthlyCost;

        [Header("Stat Boosts (Applied to Owner)")]
        public int happinessBoost;
        public int healthBoost;
    }
}
