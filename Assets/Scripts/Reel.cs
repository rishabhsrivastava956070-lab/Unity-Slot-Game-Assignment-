using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotGame
{
    /// <summary>
    /// A single vertical reel. Animates a strip of symbol cells downward,
    /// easing to a stop on the predetermined target symbol.
    /// </summary>
    public class Reel : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private RectTransform strip;          // Container that scrolls.
        [SerializeField] private Image cellPrefab;             // Prefab with an Image component.
        [SerializeField] private float cellHeight = 160f;
        [SerializeField] private int visibleCells = 3;
        [SerializeField] private int stripLength = 30;

        [Header("Spin tuning")]
        [SerializeField] private float spinDuration = 1.6f;
        [SerializeField] private AnimationCurve easing =
            AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private readonly List<Image> _cells = new List<Image>();
        private List<SlotSymbol> _stripSymbols;
        private bool _isSpinning;

        public bool IsSpinning => _isSpinning;

        private void Awake() => BuildStrip();

        private void BuildStrip()
        {
            for (int i = 0; i < stripLength; i++)
            {
                var cell = Instantiate(cellPrefab, strip);
                var rt = cell.rectTransform;
                rt.anchoredPosition = new Vector2(0, -i * cellHeight);
                _cells.Add(cell);
            }
        }

        /// <summary>Populates the strip with random symbols, placing
        /// <paramref name="finalSymbol"/> on the payline at stop.</summary>
        public IEnumerator Spin(SlotSymbol finalSymbol, IList<SlotSymbol> pool, float extraDelay)
        {
            if (_isSpinning) yield break;
            _isSpinning = true;

            _stripSymbols = new List<SlotSymbol>(stripLength);
            for (int i = 0; i < stripLength; i++)
            {
                _stripSymbols.Add(RNGService.PickWeighted(pool));
            }
            int targetIndex = stripLength - 3;
            _stripSymbols[targetIndex] = finalSymbol;

            for (int i = 0; i < _cells.Count; i++)
            {
                _cells[i].sprite = _stripSymbols[i].icon;
            }

            // Reset to top.
            strip.anchoredPosition = Vector2.zero;
            float startY = 0f;
            float endY = targetIndex * cellHeight;
            float duration = spinDuration + extraDelay;
            float t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                float k = easing.Evaluate(Mathf.Clamp01(t / duration));
                float y = Mathf.Lerp(startY, endY, k);
                strip.anchoredPosition = new Vector2(0, y);
                yield return null;
            }

            strip.anchoredPosition = new Vector2(0, endY);
            _isSpinning = false;
        }
    }
}
