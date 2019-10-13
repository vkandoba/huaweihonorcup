using System;
using System.Collections.Generic;
using System.Drawing;

namespace Borderer.Estimator
{
    public class CacheEstimator : IEstimator
    {
        private IEstimator estimator;

        private IDictionary<ValueTuple<ISquare, ISquare>, double> cacheLeftRight = new Dictionary<ValueTuple<ISquare, ISquare>, double>();

        private IDictionary<ValueTuple<ISquare, ISquare>, double> cacheTopBottom = new Dictionary<ValueTuple<ISquare, ISquare>, double>();

        public CacheEstimator(IEstimator estimator)
        {
            this.estimator = estimator;
        }

        private double GetOrAdd<TArg>(Bitmap image, IDictionary<TArg, double> cache, TArg arg, Func<Bitmap, TArg, double> calc)
        {
            if (cache.TryGetValue(arg, out double f))
                return f;

            var measure = calc(image, arg);
            cache.Add(arg, measure);
            return measure;
        }

        public double MeasureLeftRight(Bitmap image, ISquare left, ISquare right)
            => GetOrAdd(image, cacheLeftRight, (left, right), (img, arg) => estimator.MeasureLeftRight(img, arg.Item1, arg.Item2));

        public double MeasureTopBottom(Bitmap image, ISquare top, ISquare bottom)
            => GetOrAdd(image, cacheTopBottom, (top, bottom), (img, arg) => estimator.MeasureTopBottom(img, arg.Item1, arg.Item2));

        public double MeasureSquare(Bitmap image, Square square)
        {
            return estimator.MeasureSquare(image, square);
        }

        public double DeepMeasureSquare(Bitmap image, ISquare square)
        {
            return estimator.DeepMeasureSquare(image, square);
        }
    }
}