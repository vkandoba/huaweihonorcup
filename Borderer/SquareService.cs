using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Borderer.Estimator;

namespace Borderer
{
    public class SquareService
    {
        private readonly IEstimator estimator;
        private readonly int n;

        public SquareService(IEstimator estimator, int n)
        {
            this.estimator = estimator;
            this.n = n;
        }

        public IDictionary<ISquare, SquareAndMeasure> GetSquares(Bitmap image, ISquare[] parts)
        {
            var result = new Dictionary<ISquare, SquareAndMeasure>();
            var rest = new Queue<ISquare>(parts);
            while(rest.Any())
            {
                var slice = rest.Dequeue();
                var squareAndF = GetSquare(image, slice, parts);
                if (squareAndF != null)
                    result.Add(slice, squareAndF);
            }

            return result;
        }

        public ISquare RecursiveCollect(Bitmap image, ImageParameters param, Slice[,] slices)
        {
            SquareAndMeasure[] squares = slices.Cast<Slice>().Select(slice => new SquareAndMeasure
            {
                Square = slice,
                F = slice.Estimate(image, null)
            }).ToArray();
            var size = param.P;
            do
            {
                squares = GetSquares(image, squares.Select(x => x.Square).ToArray())
                    .Values
                    .OrderBy(x => x.F)
                    .ToArray();
                size = squares.Any() ? squares.First().Square.Size : ImageParameters.__totalSize;
            } while (size < ImageParameters.__totalSize);

            var answer = squares.Any() ? squares.First().Square : SquareService.MakeSquare(slices);
            return answer;
        }

        public SquareAndMeasure GetSquare(Bitmap image, ISquare first, ISquare[] slices)
        {
            Square best = null;
            double bestF = double.MaxValue;

            var seconds = slices
                            .Where(x => !first.HasCross(x))
                            .OrderBy(x => estimator.MeasureLeftRight(image, first, x))
                            .ToArray();
            var count = 0;
            foreach (var second in seconds.Take(n))
            {
                var thrids = slices
                    .Where(x => !first.HasCross(x) && !second.HasCross(x))
                    .OrderBy(x => estimator.MeasureTopBottom(image, first, x))
                    .ToArray();
                foreach (var thrid in thrids.Take(n))
                {
                    var fours = slices
                        .Where(x => !first.HasCross(x) && !second.HasCross(x) && !thrid.HasCross(x))
                        .ToArray();
                    foreach (var four in fours)
                    {
                        count++;
                        var square = new Square(first, second, thrid, four);
                        var f = estimator.MeasureSquare(image, square);
                        if (f < bestF)
                        {
                            bestF = f;
                            best = square;
                        }
                    }
                }
            }
            return best == null ? null : new SquareAndMeasure{Square = best, F = bestF};
        }

        public static Square MakeSquare(Slice[,] slices)
        {
            var result = MakeSquareInternal(slices, slices.GetLength(0));
            return result[0, 0] as Square;
        }

        private static ISquare[,] MakeSquareInternal(ISquare[,] squares, int size)
        {
            if (size == 1)
                return squares;

            var result = new ISquare[size / 2, size / 2];
            for (int i = 0; i < size / 2; i++)
            {
                for (int j = 0; j < size / 2; j++)
                {
                    int x = 2 * i, y = 2 * j;
                    result[i, j] = new Square(new[]
                    {
                        squares[x, y],
                        squares[x + 1, y],
                        squares[x, y + 1],
                        squares[x + 1, y + 1]
                    });
                }
            }

            return MakeSquareInternal(result, size / 2);
        }
    }
}