using System;
using System.Drawing;
using System.Linq;
using Borderer.Estimator;
using Borderer.Helpers;

namespace Borderer.Squares
{
    public class ImageBuilder
    {
        private readonly SquareBuilder builder;
        private readonly IEstimator estimator;

        public ImageBuilder(SquareBuilder builder, IEstimator estimator)
        {
            this.builder = builder;
            this.estimator = estimator;
        }

        public ISquare RecursiveCollect(Bitmap image, ImageParameters param, Slice[,] slices, int deep)
        {
            SquareAndMeasure[] squares = slices.Cast<Slice>().Select(slice => new SquareAndMeasure
            {
                Square = slice,
                F = double.MaxValue
            }).ToArray();
            var size = param.P;
            do
            {
                var parts = squares.Select(x => x.Square).ToArray();
                squares = builder.BuildLikelySquares(image, parts, deep)
                    .Values
                    .OrderBy(x => x.F)
                    .ToArray();
                if (!squares.Any())
                    squares = builder.BuildLikelySquares1(image, parts, deep)
                        .Values
                    .OrderBy(x => x.F)
                    .ToArray();
                size = squares.Any() ? squares.First().Square.Size : ImageParameters.__totalSize;
            } while (size < ImageParameters.__totalSize);

            var answer = squares.Any() ? squares.First().Square : SquareBuilder.MakeSquare(slices);
            return answer;
        }

        public ISquare RecoveDuplicate(Bitmap image, ISquare square, int p)
        {
            var map = square.Apply(p);
            var param = new ImageParameters(p);
            var permutation = map.ToPermutation();
            var duplicatesAndGaps = permutation.FindDuplicates();
            if (duplicatesAndGaps.Duplicates.Length == 0)
                return SquareBuilder.MakeSquare(permutation.ToSlices(p));

            var duplicatesPlaces = permutation.GetPlaces(duplicatesAndGaps.Duplicates).Select(x => x.Position).Distinct().ToArray();
            if (duplicatesPlaces.Length > 4)
                return SquareBuilder.MakeSquare(permutation.ToSlices(p));

            var options = VMath.Permutations(duplicatesAndGaps.Gaps.Length, duplicatesPlaces.Length);

            var gaps = duplicatesAndGaps.Gaps;
            Square best = null;
            double bestf = double.MaxValue;
            foreach (var option in options)
            {
                var newperm = ApplyOption(permutation, duplicatesPlaces, gaps, option);
                var newSquare = SquareBuilder.MakeSquare(newperm.ToSlices(p));
                var f = estimator.DeepMeasureSquare(image, newSquare);
                if (f < bestf)
                {
                    best = newSquare;
                    bestf = f;
                }
            }

            return best;
        }

        private int[] ApplyOption(int[] permutation, int[] places, int[] gaps, int[] option)
        {
            var newp = new int[permutation.Length];
            Array.Copy(permutation, newp, permutation.Length);
            for (int i = 0; i < option.Length; i++)
            {
                var place = places[option[i]];
                newp[place] = gaps[i];
            }

            return newp;
        }
    }
}