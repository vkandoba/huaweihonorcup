using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

namespace Borderer
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

        public static int[] GetPermutation(this Slice[,] slices)
        {
            var lengthX = slices.GetLength(0);
            var lengthY = slices.GetLength(1);
            var sp = new List<int>();
            for (int i = 0; i < lengthY; i++)
                for (int j = 0; j < lengthX; j++)
                    sp.Add(slices[j, i].N);

            return sp.ToArray();
        }
    }
}