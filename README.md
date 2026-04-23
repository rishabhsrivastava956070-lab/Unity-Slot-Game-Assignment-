# 🎰 Lucky Sevens — Unity Slot Game

A clean, testable slot machine implemented in Unity using OOP principles,
ScriptableObject-driven symbols, cryptographic RNG, and coroutine-based
reel animation.

## 🎮 Game Overview

- **Win condition:** all 3 reels show the same symbol on the payline.
- **Jackpot:** 3 × Lucky Seven pays the highest multiplier.
- **Bonus feature:** 2 or more Wild Star symbols on the payline trigger
  5 free spins (no balance cost).
- **RNG:** `System.Security.Cryptography.RandomNumberGenerator` for
  cryptographically fair, unpredictable outcomes. Each reel's outcome
  is decided up-front before the visual spin starts.

![Game Screenshot](screenshot.png)
## 📁 Folder Structure

```
Assets/
├── Scripts/
│   ├── SlotSymbol.cs          # ScriptableObject — symbol data
│   ├── RNGService.cs          # Crypto-strength weighted RNG
│   ├── Reel.cs                # Single reel animation + strip
│   ├── PayoutEvaluator.cs     # Pure logic, easy to unit test
│   ├── SlotMachine.cs         # Top-level controller / UI glue
│   └── Editor/
│       └── PayoutEvaluatorTests.cs  # NUnit tests
├── Prefabs/                   # Reel + SlotMachine prefabs
├── Animations/                # Win flash, bulb flicker, etc.
├── UI/                        # Sprites, fonts, paytable
└── Sounds/                    # Spin / win / jackpot SFX
Build/
└── WebGL/                     # Built WebGL output goes here
```

## 🛠 Scene Setup

1. Create symbol assets: **Assets > Create > SlotGame > Slot Symbol**.
   Make 6–8 (cherry, lemon, grape, bar, bell, diamond, seven, wild).
   Set `weight`, `payoutMultiplier`, and tick `isBonus` on the wild.
2. Build a Reel prefab: a `RectTransform` mask containing a child
   "Strip" RectTransform plus a Cell prefab (`Image` + `LayoutElement`).
   Attach `Reel.cs` and assign references.
3. Place 3 Reels horizontally inside a SlotMachine root. Attach
   `SlotMachine.cs` and drag the reels + symbol pool into the inspector.
4. Wire UI: TMP labels for balance/bet/win/bonus and a Spin Button.

## ▶️ Run the WebGL Build

1. Clone this repo.
2. Open `Build/WebGL/index.html` via a local server, e.g.:
   ```
   python -m http.server 8000 --directory Build/WebGL
   ```
3. Visit `http://localhost:8000`.

> WebGL needs to be served over HTTP — opening `index.html` directly
> will fail because of CORS / module restrictions.

## ✨ Bonus Features

- **Free spins** triggered by 2+ Wild Stars.
- **Jackpot detection** with separate SFX hook.
- **Cryptographic RNG** instead of `UnityEngine.Random`.
- **Unit tests** (`PayoutEvaluatorTests.cs`) — pure logic,
  runs in the Unity Test Runner without a scene.
- **Adjustable bet** (5–100) without restarting.

## 🧠 Approach

I separated concerns aggressively:

- `SlotSymbol` (data) — designer-friendly ScriptableObject.
- `RNGService` (utility) — single source of randomness so fairness
  audits only need to inspect one file.
- `Reel` (view) — animation only; owns no game state.
- `PayoutEvaluator` (pure logic) — no Unity dependencies, fully
  unit-testable.
- `SlotMachine` (controller) — orchestrates RNG → reels → evaluator
  → UI/SFX. The only place that mutates balance.

The visual spin is purely cosmetic: outcomes are pre-rolled, then each
reel is told *which symbol to land on*. This is how real slot games
(and most modern web casinos) work — it cleanly decouples fairness
from animation timing.

## 📝 Commit Strategy (suggested)

When pushing to GitHub, use small, meaningful commits:

1. `chore: project scaffold and folders`
2. `feat: SlotSymbol ScriptableObject`
3. `feat: cryptographic weighted RNG`
4. `feat: animated Reel with strip + easing`
5. `feat: payout evaluator + jackpot/bonus rules`
6. `feat: SlotMachine controller + UI`
7. `test: NUnit coverage for PayoutEvaluator`
8. `feat: WebGL build`
9. `docs: README`
