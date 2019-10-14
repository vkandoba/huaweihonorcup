using System;
using System.Collections.Generic;
using System.Drawing;

namespace Borderer.Helpers
{
    public static class VMath
    {
        public static ulong BitOut(int number)
        {
            if (number > 1024)
                throw new Exception("Number is too large");
            if (number < 0)
                throw new Exception("Number is negative");
            if (number == 0)
                return 1;

            return (ulong) 1 << number;
        }

        public static IEnumerable<int[]> Permutations(int k, int n)
        {
            var used = new bool[n];
            var a = new Stack<int>();
            for (int i = 0; i < n; i++)
                used[i] = false;
            return MakePermutationsInternal(a, used, 0, n, k);
        }

        private static IEnumerable<int[]> MakePermutationsInternal(Stack<int> a, bool[] used, int num, int n, int k)
        {
            if (num == k)
            {
                //Если размещение готово, то печатаем его
                yield return a.ToArray();
                yield break;
            }

            for (int i = 0; i < n; i++)
                if (!used[i])
                {
                    //Добавляем еще не взятый элемент
                    used[i] = true; a.Push(i);
                    foreach (var permutation in MakePermutationsInternal(a, used, num + 1, n, k))
                        yield return permutation;
                    used[i] = false; a.Pop();
                }
        }

        public static int Diff(Color p1, Color p2)
        {
            return Math.Abs(p1.R - p2.R) + Math.Abs(p1.G - p2.G) + Math.Abs(p1.B - p2.B);
        }

        public static int Diff2(Color p1, Color p2)
        {
            var diffR = Math.Abs(p1.R - p2.R);
            var diffG = Math.Abs(p1.G - p2.G);
            var diffB = Math.Abs(p1.B - p2.B);
            return diffR * diffR + diffG * diffG + diffB * diffB;
        }
    }
}