using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;

namespace Borderer
{
    public class SquareAndF
    {
        public ISquare Square { get; set; }
        public double F { get; set; }
    }

    public class SquareService
    {
        private readonly IEstimator estimator;
        private readonly int n;

        public SquareService(IEstimator estimator, int n)
        {
            this.estimator = estimator;
            this.n = n;
        }

        public IDictionary<ISquare, SquareAndF> GetSquares(Bitmap image, ISquare[] parts)
        {
            var result = new Dictionary<ISquare, SquareAndF>();
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

        public SquareAndF GetSquare(Bitmap image, ISquare first, ISquare[] slices)
        {
            Square best = null;
            double bestF = double.MaxValue;

            var seconds = slices
                            .Where(x => !first.HasCross(x))
                            .OrderBy(x => estimator.MeasureH(image, first, x))
                            .ToArray();
            foreach (var second in seconds.Take(n))
            {
                var thrids = slices
                    .Where(x => !first.HasCross(x) && !second.HasCross(x))
                    .OrderBy(x => estimator.MeasureV(image, first, x))
                    .ToArray();
                foreach (var thrid in thrids.Take(n))
                {
                    var fours = slices
                        .Where(x => !first.HasCross(x) && !second.HasCross(x) && !thrid.HasCross(x))
                        .ToArray();
                    foreach (var four in fours)
                    {
                        var square = new Square(first, second, thrid, four);
                        var f = square.Estimate(image, estimator);
                        if (f < bestF)
                        {
                            bestF = f;
                            best = square;
                        }
                    }
                }
            }

            return best == null ? null : new SquareAndF{Square = best, F = bestF};
        }
    }
}