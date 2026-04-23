using System.Collections.Generic;

namespace SlotGame
{
    /// <summary>Result of evaluating one spin against the paytable.</summary>
    public struct SpinOutcome
    {
        public int WinAmount;
        public bool IsJackpot;
        public bool IsBonus;
        public SlotSymbol[] Symbols;
    }

    /// <summary>
    /// Pure logic class — easy to unit-test independently of MonoBehaviours.
    /// Win condition: all reels show the same symbol.
    /// Bonus: 2+ bonus symbols anywhere on the payline.
    /// </summary>
    public static class PayoutEvaluator
    {
        public static SpinOutcome Evaluate(IList<SlotSymbol> payline, int bet, string jackpotId = "seven")
        {
            var result = new SpinOutcome
            {
                Symbols = new SlotSymbol[payline.Count],
            };
            for (int i = 0; i < payline.Count; i++) result.Symbols[i] = payline[i];

            bool allSame = true;
            int bonusCount = 0;
            for (int i = 0; i < payline.Count; i++)
            {
                if (payline[i].isBonus) bonusCount++;
                if (i > 0 && payline[i].symbolId != payline[0].symbolId) allSame = false;
            }

            if (allSame)
            {
                result.WinAmount = bet * payline[0].payoutMultiplier;
                result.IsJackpot = payline[0].symbolId == jackpotId;
            }
            else if (bonusCount >= 2)
            {
                result.IsBonus = true;
            }

            return result;
        }
    }
}
