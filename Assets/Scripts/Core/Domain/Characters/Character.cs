using System;
using System.Collections.Generic;
using UnityEngine; // Cần cho Sprite

namespace LifeSim.Core.Domain.Characters
{
    [Serializable]
    public partial class CharacterStats
    {
        public int happiness = 50;   // 0..100
        public int health = 50;      // 0..100
        public int iq = 100;         // 0..200
        public int eq = 50;          // 0..100
        public int skill = 50;       // 0..100
    }

    [Serializable] public class EducationState { public string currentMajor = null; }
    [Serializable] public class CareerState { public string currentTrack = null; public int level = 0; }

    [Serializable]
    public class Character
    {
        public string id;
        public string parentId; // For tree structure
        public string name;
        public string gender; // "male" or "female"
        public int monthlyIncome;
        public string relation = "self";   // hiển thị FamilyTree
        public bool isAlive = true;
        public int ageDays = 0;
        public int generation = 0;

        public LifePhase lifePhase = LifePhase.Newborn;
        public CharacterStatus status = CharacterStatus.Idle;
        public bool isIntern = false;
        public bool seekingLowerTier = false;
        public string companyId = null;

        public CharacterStats stats = new CharacterStats();

        // State simplified to IDs for data-oriented design
        public string educationMajorId = null;
        public string careerTrackId = null;
        public int careerLevel = 0;
        public string clubId = null;

        // Serializable workaround for Dictionary
        public List<string> avatarState_keys = new List<string>();
        public List<string> avatarState_values = new List<string>();

        public Dictionary<string, string> GetAvatarState()
        {
            var avatarState = new Dictionary<string, string>();
            for (int i = 0; i < avatarState_keys.Count; i++)
            {
                if (i < avatarState_values.Count) // Safety check
                {
                    avatarState[avatarState_keys[i]] = avatarState_values[i];
                }
            }
            return avatarState;
        }
    }
}