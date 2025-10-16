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

    [Serializable]
    public class AvatarState
    {
         public Sprite hairSprite;
        public Sprite eyeSprite;
        public Sprite bodySprite;
        public Sprite outfitSprite;
    }

    [Serializable]
    public class Character
    {
        public string id;
        public string name;
        public string relation;
        public bool isAlive = true;
        public int ageDays = 0;
        public int generation = 0;
        public string lifeStage = "newborn"; // newborn → elementary → ...

        public CharacterStats stats = new CharacterStats();
        public AvatarState avatar = new AvatarState();
        public CharacterCareer career = new CharacterCareer();
        public CharacterEducation education = new CharacterEducation();
    }
}