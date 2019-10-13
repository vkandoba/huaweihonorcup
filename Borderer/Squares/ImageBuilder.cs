using System.Drawing;
using System.Linq;

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
                squares = builder.BuildLikelySquares(image, squares.Select(x => x.Square).ToArray(), deep)
                    .Values
                    .OrderBy(x => x.F)
                    .ToArray();
                size = squares.Any() ? squares.First().Square.Size : ImageParameters.__totalSize;
            } while (size < ImageParameters.__totalSize);

            var answer = squares.Any() ? squares.First().Square : SquareBuilder.MakeSquare(slices);
            return answer;
        }

    }
}