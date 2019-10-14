using System.Drawing;
using System.Linq;
using Borderer.Helpers;

namespace Borderer.Squares
{
    public class ImageBuilder
    {
        private readonly SquareBuilder builder;

        public ImageBuilder(SquareBuilder builder)
        {
            this.builder = builder;
        }

        public ISquare RecursiveCollect(Bitmap image, ImageParameters param, Slice[,] slices, int deep)
        {
            SquareAndMeasure[] squares = slices.Cast<Slice>().Select(slice => new SquareAndMeasure
            {
                Square = slice,
                F = slice.Estimate(image, null)
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

        public ISquare RecoveDuplicate(ISquare square, int p)
        {
            var permutation = square.Apply(p).ToPermutation();
            var duplicatesAndGaps = permutation.FindDuplicates();
            for (int i = 0; i < duplicatesAndGaps.Gaps.Length; i++)
            {
                permutation[duplicatesAndGaps.Duplicates[i].Position] = duplicatesAndGaps.Gaps[i];
            }

            return SquareBuilder.MakeSquare(permutation.ToSlices(p));
        }

    }
}