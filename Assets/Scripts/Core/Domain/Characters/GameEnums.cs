
namespace LifeSim.Core.Domain.Characters
{
    public enum LifePhase
    {
        Newborn,          // 0-5
        ElementarySchool, // 6-11
        MiddleSchool,     // 12-15
        HighSchool,       // 16-18
        University,       // 19-22
        WorkingLife,      // 23-59
        Retired           // 60+
    }

    public enum CharacterStatus
    {
        Idle,
        InEducation,
        Working,
        Unemployed,
        Internship,
        VocationalTraining,
        Retired,
        Trainee
    }

    public enum Gender
    {
        Male,
        Female
    }
}
