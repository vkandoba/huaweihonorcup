using System;
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
            var result = new Dictionary<ISquare, SquareAndMeasure>();
            var rest = new List<ISquare>(parts);
            foreach (var slice in rest)
            {
                var squareAndF = BuildLikelySquare(image, slice,
                        first => parts
                        .Where(x => !first.HasCross(x))
                        .OrderBy(x => estimator.MeasureLeftRight(image, first, x))
                        .Take(deep)
                        .ToArray(),
                        (first, second) => parts
                            .Where(x => !first.HasCross(x) && !second.HasCross(x))
                            .OrderBy(x => estimator.MeasureTopBottom(image, first, x))
                            .Take(deep)
                            .ToArray(),
                        (first, second, thrid) => parts
                            .Where(x => !first.HasCross(x) && !second.HasCross(x) && !thrid.HasCross(x))
                            .OrderBy(x => (estimator.MeasureLeftRight(image, thrid, x) + estimator.MeasureTopBottom(image, second, x)) / 2.0)
                            .Take(deep)
                            .ToArray()
                    );
                if (squareAndF != null)
                {
                    result.Add(slice, squareAndF);
                }
            }

            return result;
        }

        public SquareAndMeasure BuildLikelySquare(Bitmap image, ISquare first,
            Func<ISquare, ISquare[]> selectSeconds,
            Func<ISquare, ISquare, ISquare[]> selectThrids,
            Func<ISquare, ISquare, ISquare, ISquare[]> selectFours)
        {
            Square best = null;
            double bestF = double.MaxValue;

            var count = 0;
            var seconds = selectSeconds(first);
            foreach (var second in seconds)
            {
                var thrids = selectThrids(first, second);
                foreach (var thrid in thrids)
                {
                    var fours = selectFours(first, second, thrid);
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