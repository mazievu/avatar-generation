using System;
using System.Collections.Generic;
using LifeSim.Core.Domain.Characters;

namespace LifeSim.Core.Domain.Game
{
    [Serializable]
    public class GameDate
    {
        public int day = 0;   // absolute day from start
        public int year = 2000;
    }

    [Serializable]
    public class GameLogEntry
    {
        public int year;
        public string characterName;
        public string messageKey;
        public List<string> messageArgs;
    }

    [Serializable]
    public class PendingSchoolChoice
    {
        public string characterId;
        public string newPhase; // e.g., "elementary", "highschool"
    }

    [Serializable]
    public class PendingUniversityChoice
    {
        public string characterId;
    }

    [Serializable]
    public class PendingMajorChoice
    {
        public string characterId;
        public List<string> options = new List<string>(); // major keys
    }

    [Serializable]
    public class PendingCareerChoice
    {
        public string characterId;
        public List<string> options = new List<string>(); // career track keys
    }

    [Serializable]
    public class PendingUnderqualifiedChoice
    {
        public string characterId;
        public string careerTrackKey;
    }

    [Serializable]
    public class PendingPromotion
    {
        public string characterId;
        public string newTitleKey;
    }

    [Serializable]
    public class PendingClubChoice
    {
        public string characterId;
        public List<string> options = new List<string>(); // club ids
    }

    [Serializable]
    public class GameState
    {
        public Dictionary<string, Character> familyMembers = new Dictionary<string, Character>();
        public int totalMembers = 0;
        public int totalChildrenBorn = 0;

        public int familyFund = 0;
        public int familyIncomePerMonth = 0;

        public List<BusinessInstance> businesses = new List<BusinessInstance>();
        public List<string> assets = new List<string>();
        public List<string> unlockedFeatures = new List<string>();

        public string newlyUnlockedFeature = null;

        public string highestEducation = "none";
        public string highestCareer = "none";

        public List<PendingSchoolChoice> pendingSchoolChoice = new List<PendingSchoolChoice>();
        public List<PendingUniversityChoice> pendingUniversityChoice = new List<PendingUniversityChoice>();
        public PendingMajorChoice pendingMajorChoice = null;
        public PendingCareerChoice pendingCareerChoice = null;
        public PendingUnderqualifiedChoice pendingUnderqualifiedChoice = null;
        public PendingClubChoice pendingClubChoice = null;
        public (int amount, int termYears)? pendingLoanChoice = null;
        public PendingPromotion pendingPromotion = null;

        public LifeSim.Core.Domain.Events.GameEvent activeEvent = null;
        public Queue<LifeSim.Core.Domain.Events.GameEvent> eventQueue = new Queue<LifeSim.Core.Domain.Events.GameEvent>();
        public int eventCooldown = 0;
        public Dictionary<string, int> characterEventCount = new Dictionary<string, int>();
        public List<string> triggeredOneTimeEvents = new List<string>();

        public string gameOverReason = null; // null means playing
        public GameDate currentDate = new GameDate();

        public string lang = "en";
        public bool HasPendingChoices()
        {
            return pendingSchoolChoice.Count > 0 ||
                   pendingUniversityChoice.Count > 0 ||
                   pendingMajorChoice != null ||
                   pendingCareerChoice != null ||
                   pendingUnderqualifiedChoice != null ||
                   pendingClubChoice != null ||
                   pendingLoanChoice != null ||
                   pendingPromotion != null;
        }
        public List<GameLogEntry> GameLog = new List<GameLogEntry>();
    }
}