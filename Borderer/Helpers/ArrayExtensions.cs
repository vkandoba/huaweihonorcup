using System;
using System.Collections.Generic;
using System.Text;
using Borderer.Squares;

namespace Borderer.Helpers
{
    public static class ArrayExtensions
    {
        public static void CopyPart<T>(this T[,] array, T[,] part, int x, int y)
        {
            var lengthX = part.GetLength(0);
            var lengthY = part.GetLength(1);
            for (int i = 0; i < lengthX; i++)
                for (int j = 0; j < lengthY; j++)
                    array[x + i, y + j] = part[i, j];
        }

        public static int[] ToPermutation(this Slice[,] slices)
        {
            var lengthX = slices.GetLength(0);
            var lengthY = slices.GetLength(1);
            var sp = new List<int>();
            for (int i = 0; i < lengthY; i++)
                for (int j = 0; j < lengthX; j++)
                    sp.Add(slices[j, i].N);

            return sp.ToArray();
        }

        public static Slice[,] ToSlices(this int[] permutation, int p)
        {
            var size = (int) Math.Sqrt(permutation.Length);
            var slices = new Slice[size, size];
            int count = 0;
            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < size; i++)
                {
                    var x = permutation[count] % size;
                    var y = permutation[count] / size;
                    slices[i, j] = new Slice(new ImageParameters(p), x, y);
                    count++;
                }
            }
            return slices;
        }

        public static string Print<T>(this T[,] array, Func<T, string> printItem)
        {
            var lengthX = array.GetLength(0);
            var lengthY = array.GetLength(1);
            var acc = new StringBuilder();
            for (int i = 0; i < lengthY; i++)
            {
                for (int j = 0; j < lengthX; j++)
                {
                    var str = printItem(array[i, j]);
                    acc.Append($" {str}");
                }
                acc.AppendLine();
            }

            return acc.ToString();
        }
    }
}