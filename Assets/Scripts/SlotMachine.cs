using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace SlotGame
{
    /// <summary>
    /// Top-level controller. Wires UI, RNG, reels, and payout evaluation.
    /// Drop on a single GameObject in the scene and assign references.
    /// </summary>
    public class SlotMachine : MonoBehaviour
    {
        [Header("Reels")]
        [SerializeField] private List<Reel> reels = new List<Reel>();
        [SerializeField] private List<SlotSymbol> symbolPool = new List<SlotSymbol>();
        [SerializeField] private float reelStaggerSeconds = 0.35f;

        [Header("Economy")]
        [SerializeField] private int startingBalance = 1000;
        [SerializeField] private int bet = 10;
        [SerializeField] private int bonusFreeSpinsAwarded = 5;
        [SerializeField] private string jackpotSymbolId = "seven";

        [Header("UI")]
        [SerializeField] private TMP_Text balanceLabel;
        [SerializeField] private TMP_Text betLabel;
        [SerializeField] private TMP_Text winLabel;
        [SerializeField] private TMP_Text bonusLabel;
        [SerializeField] private Button spinButton;

        [Header("Audio (optional)")]
        [SerializeField] private AudioSource spinSfx;
        [SerializeField] private AudioSource winSfx;
        [SerializeField] private AudioSource jackpotSfx;

        [Header("Events")]
        public UnityEvent<SpinOutcome> OnSpinComplete;

        private int _balance;
        private int _bonusSpins;
        private bool _spinning;

        private void Start()
        {
            _balance = startingBalance;
            UpdateUI();
            if (spinButton != null) spinButton.onClick.AddListener(RequestSpin);
        }

        public void RequestSpin()
        {
            if (_spinning) return;
            if (_bonusSpins == 0 && _balance < bet) return;
            StartCoroutine(SpinRoutine());
        }

        private IEnumerator SpinRoutine()
        {
            _spinning = true;
            if (_bonusSpins > 0) _bonusSpins--;
            else _balance -= bet;

            UpdateUI();
            if (spinSfx != null) spinSfx.Play();

            // Pre-roll RNG outcomes for each reel — fairness via single source.
            var finals = new List<SlotSymbol>(reels.Count);
            for (int i = 0; i < reels.Count; i++)
                finals.Add(RNGService.PickWeighted(symbolPool));

            // Spin reels in parallel with staggered stop times.
            var coroutines = new List<Coroutine>();
            for (int i = 0; i < reels.Count; i++)
            {
                coroutines.Add(StartCoroutine(reels[i].Spin(finals[i], symbolPool, i * reelStaggerSeconds)));
            }
            foreach (var c in coroutines) yield return c;

            // Evaluate.
            var outcome = PayoutEvaluator.Evaluate(finals, bet, jackpotSymbolId);

            if (outcome.WinAmount > 0)
            {
                _balance += outcome.WinAmount;
                if (outcome.IsJackpot && jackpotSfx != null) jackpotSfx.Play();
                else if (winSfx != null) winSfx.Play();
            }
            if (outcome.IsBonus) _bonusSpins += bonusFreeSpinsAwarded;

            UpdateUI(outcome);
            OnSpinComplete?.Invoke(outcome);
            _spinning = false;
        }

        public void IncreaseBet() { bet = Mathf.Min(100, bet + 5); UpdateUI(); }
        public void DecreaseBet() { bet = Mathf.Max(5, bet - 5); UpdateUI(); }

        private void UpdateUI(SpinOutcome? outcome = null)
        {
            if (balanceLabel != null) balanceLabel.text = $"BALANCE: {_balance}";
            if (betLabel != null) betLabel.text = $"BET: {bet}";
            if (bonusLabel != null) bonusLabel.text = _bonusSpins > 0 ? $"FREE SPINS: {_bonusSpins}" : "";
            if (winLabel != null)
            {
                if (outcome.HasValue && outcome.Value.IsJackpot) winLabel.text = "JACKPOT!";
                else if (outcome.HasValue && outcome.Value.WinAmount > 0) winLabel.text = $"WIN +{outcome.Value.WinAmount}";
                else if (outcome.HasValue && outcome.Value.IsBonus) winLabel.text = "BONUS!";
                else if (!_spinning) winLabel.text = "PLACE YOUR BET";
            }
        }
    }
}
