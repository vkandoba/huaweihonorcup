using System;
using System.Linq;
using Borderer.Helpers;
using NUnit.Framework;

namespace BordererTests
{
    public class VMathTest : TestBase
    {
        [TestCase(0, (ulong) 1)]
        [TestCase(1, (ulong) 2)]
        [TestCase(3, (ulong) 8)]
        [TestCase(11, (ulong) 2048)]
        [TestCase(63, 9223372036854775808)]
        public void TestBitOut(int n, ulong bit)
        {
            Assert.That(VMath.BitOut(n), Is.EqualTo(bit));
        }

        [TestCase(2, 3)]
        [TestCase(3, 5)]
        public void TestGeneratePermutations(int k, int N)
        {
            var permutations = VMath.Permutations(k, N);
            foreach (var pm in permutations)
            {
                var str = pm.Aggregate("", (s, n) => $"{s} {n}");
                Console.WriteLine(str);
            }
        }

        [TestCase(2, 3)]
        [TestCase(3, 5)]
        [TestCase(4, 63)]
        [TestCase(3, 63)]
        [TestCase(4, 16*16 - 1)]
        [TestCase(4, 32*32 - 1)]
        public void TestPermutationsCount(int k, int N)
        {
            long c = 1;
            for (int i = N; i > (N-k); i--)
                c *= i;
            Console.WriteLine($"permutations ({k},{N}) count: {c}");
        }
    }
}