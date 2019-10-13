using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Borderer.Estimator;

namespace Borderer.Squares
{
    public class SquareBuilder
    {

        private readonly IEstimator estimator;

        public SquareBuilder(IEstimator estimator)
        {
            this.estimator = estimator;
        }

        public IDictionary<ISquare, SquareAndMeasure> BuildLikelySquares(Bitmap image, ISquare[] parts, int deep)
        {
            var result = new ConcurrentDictionary<ISquare, SquareAndMeasure>();
            var rest = new List<ISquare>(parts);
            var tasks = rest.Select(slice => Task.Run(() =>
            {
                var squareAndF = BuildLikelySquare(image, slice, parts, deep);
                if (squareAndF != null)
                    result.AddOrUpdate(slice, (s) => squareAndF, (s, v) => squareAndF);
            })).ToArray();
            Task.WaitAll(tasks);

            return result;
        }

        public SquareAndMeasure BuildLikelySquare(Bitmap image, ISquare first, ISquare[] slices, int deep)
        {
            Square best = null;
            double bestF = double.MaxValue;

            var seconds = slices
                            .Where(x => !first.HasCross(x))
                            .OrderBy(x => estimator.MeasureLeftRight(image, first, x))
                            .ToArray();
            var count = 0;
            foreach (var second in seconds.Take(deep))
            {
                var thrids = slices
                    .Where(x => !first.HasCross(x) && !second.HasCross(x))
                    .OrderBy(x => estimator.MeasureTopBottom(image, first, x))
                    .ToArray();
                foreach (var thrid in thrids.Take(deep))
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