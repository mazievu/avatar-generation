using LifeSim.Core.Domain.Game;
using LifeSim.Core.Data;

namespace LifeSim.Core.Domain.Economy
{
    public static class EconomyService
    {
        public static void ApplyNetIncome(GameState s, int netIncome)
        {
            s.familyFund += netIncome;
            if (s.familyFund < Constants.DebtGameOverThreshold)
                s.gameOverReason = "debt";
        }

        public static int ApplyTax(int income) => (int)(income * 0.9f);
    }
}