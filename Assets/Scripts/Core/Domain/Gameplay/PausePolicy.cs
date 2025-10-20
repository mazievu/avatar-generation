using LifeSim.Core.Domain.Game;

namespace LifeSim.Core.Domain.Gameplay
{
    public static class PausePolicy
    {
        public static bool IsPaused(GameState s)
        {
            return s.isManuallyPaused
                || s.activeEvent != null
                || (s.pendingSchoolChoice != null && s.pendingSchoolChoice.Count > 0)
                || (s.pendingUniversityChoice != null && s.pendingUniversityChoice.Count > 0)
                || s.pendingMajorChoice != null
                || s.pendingCareerChoice != null
                || s.pendingUnderqualifiedChoice != null
                || s.pendingClubChoice != null
                || s.pendingLoanChoice.HasValue
                || s.pendingPromotion != null;
        }
    }
}