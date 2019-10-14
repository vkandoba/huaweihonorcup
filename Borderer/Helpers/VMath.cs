using System;
using System.Collections.Generic;
using System.Drawing;

namespace Borderer.Helpers
{
    public struct NumberAndPosition
    {
        public int N;
        public int Position;
    }

    public class DuplicateFindResult
    {
        public int[] Duplicates;
        public int[] Gaps { get; set; }
    }

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

        public static NumberAndPosition[] GetPlaces(this int[] permutation, int[] numbers)
        {
            var result = new List<NumberAndPosition>();
            for (int i = 0; i < numbers.Length; i++)
            {
                for (int j = 0; j < permutation.Length; j++)
                {
                    if (permutation[j] == numbers[i])
                        result.Add(new NumberAndPosition
                        {
                            N = numbers[i],
                            Position = j
                        });
                }
            }

            return result.ToArray();
        }

        public static DuplicateFindResult FindDuplicates(this int[] permutation)
        {
            var map = new int[permutation.Length];
            var duplicates = new List<int>();
            var gaps = new List<int>();
            for (int i = 0; i < permutation.Length; i++)
                map[i] = 0;
            for (int i = 0; i < permutation.Length; i++)
            {
                map[permutation[i]]++;
                if (map[permutation[i]] > 1)
                {
                    duplicates.Add(permutation[i]);
                }
            }
            for (int i = 0; i < permutation.Length; i++)
            {
                if (map[i] == 0)
                    gaps.Add(i);
            }
            if (duplicates.Count != gaps.Count)
                throw new ApplicationException("Что-то пошло не так с поиском дубликатов");

            return new DuplicateFindResult
            {
                Duplicates = duplicates.ToArray(),
                Gaps = gaps.ToArray()
            };
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