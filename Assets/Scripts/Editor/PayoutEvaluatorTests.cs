#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace SlotGame.Tests
{
    public class PayoutEvaluatorTests
    {
        private SlotSymbol Make(string id, int payout, bool bonus = false)
        {
            var s = ScriptableObject.CreateInstance<SlotSymbol>();
            s.symbolId = id; s.payoutMultiplier = payout; s.isBonus = bonus; s.weight = 1;
            return s;
        }

        [Test] public void ThreeOfAKind_Pays()
        {
            var a = Make("cherry", 10);
            var o = PayoutEvaluator.Evaluate(new List<SlotSymbol>{a,a,a}, 10);
            Assert.AreEqual(100, o.WinAmount);
        }

        [Test] public void ThreeSevens_AreJackpot()
        {
            var s = Make("seven", 100);
            var o = PayoutEvaluator.Evaluate(new List<SlotSymbol>{s,s,s}, 10);
            Assert.IsTrue(o.IsJackpot);
        }

        [Test] public void TwoBonus_TriggersBonus()
        {
            var w = Make("wild", 0, true);
            var c = Make("cherry", 10);
            var o = PayoutEvaluator.Evaluate(new List<SlotSymbol>{w,w,c}, 10);
            Assert.IsTrue(o.IsBonus);
        }

        [Test] public void NoMatch_NoWin()
        {
            var a = Make("a", 5); var b = Make("b", 5); var c = Make("c", 5);
            var o = PayoutEvaluator.Evaluate(new List<SlotSymbol>{a,b,c}, 10);
            Assert.AreEqual(0, o.WinAmount);
        }
    }
}
#endif
