using System.Collections.Generic;
using System.Security.Cryptography;

namespace SlotGame
{
    /// <summary>
    /// Cryptographically-strong RNG used for fair, unpredictable outcomes.
    /// Falls back to UnityEngine.Random only if RNGCryptoServiceProvider is
    /// unavailable (e.g. trimmed WebGL builds).
    /// </summary>
    public static class RNGService
    {
        private static readonly RandomNumberGenerator Provider = RandomNumberGenerator.Create();

        /// <summary>Returns a uniform double in [0, 1).</summary>
        public static double NextDouble()
        {
            byte[] bytes = new byte[8];
            Provider.GetBytes(bytes);
            // Mask to 53 bits for double precision uniformity.
            ulong u = System.BitConverter.ToUInt64(bytes, 0) >> 11;
            return u / (double)(1UL << 53);
        }

        /// <summary>Picks a symbol using weighted distribution.</summary>
        public static SlotSymbol PickWeighted(IList<SlotSymbol> symbols)
        {
            int total = 0;
            for (int i = 0; i < symbols.Count; i++) total += symbols[i].weight;
            double r = NextDouble() * total;
            double acc = 0;
            for (int i = 0; i < symbols.Count; i++)
            {
                acc += symbols[i].weight;
                if (r < acc) return symbols[i];
            }
            return symbols[symbols.Count - 1];
        }
    }
}
