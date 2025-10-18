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
        public string name;
        public string relation = "self";   // hiển thị FamilyTree
        public bool isAlive = true;
        public int ageDays = 0;
        public int generation = 0;

        public LifePhase lifePhase = LifePhase.Newborn;
        public CharacterStatus status = CharacterStatus.Idle;

        public CharacterStats stats = new CharacterStats();

        // State simplified to IDs for data-oriented design
        public string educationMajorId = null;
        public string careerTrackId = null;
        public int careerLevel = 0;

        // Serializable workaround for Dictionary
        public List<string> avatarState_keys = new List<string>();
        public List<string> avatarState_values = new List<string>();
    }
}