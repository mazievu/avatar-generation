using System;
using System.Collections.Generic;

namespace LifeSim.Core.Domain.Characters
{
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