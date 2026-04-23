using UnityEngine;

namespace SlotGame
{
    /// <summary>
    /// ScriptableObject describing a single slot symbol: its visual,
    /// RNG weight, and payout multiplier when matched 3-of-a-kind.
    /// Create assets via: Assets > Create > SlotGame > Slot Symbol.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSlotSymbol", menuName = "SlotGame/Slot Symbol", order = 0)]
    public class SlotSymbol : ScriptableObject
    {
        [Tooltip("Unique identifier used for win comparisons.")]
        public string symbolId;

        [Tooltip("Display name shown in paytable / UI.")]
        public string displayName;

        [Tooltip("Sprite rendered on the reel cell.")]
        public Sprite icon;

        [Tooltip("Higher weights appear more often. Keep rare symbols low.")]
        [Min(1)] public int weight = 10;

        [Tooltip("Bet multiplier when 3 of this symbol land on the payline.")]
        [Min(0)] public int payoutMultiplier = 5;

        [Tooltip("If true, two or more on the payline triggers free spins.")]
        public bool isBonus = false;
    }
}
