using System;
using System.Collections.Generic;

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

    [Serializable]
    public class AvatarState
    {
        public string presetId = "default";
        public Dictionary<string, string> layers = new Dictionary<string, string>();
    }

    [Serializable]
    public class Character
    {
        public string id;
        public string name;
        public bool isAlive = true;
        public int ageDays = 0;
        public int generation = 0;
        public string lifeStage = "newborn"; // newborn → elementary → ...

        public CharacterStats stats = new CharacterStats();
        public AvatarState avatar = new AvatarState();
    }
}