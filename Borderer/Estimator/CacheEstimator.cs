using System;
using System.Collections.Concurrent;
using System.Drawing;
using Borderer.Squares;

namespace Borderer.Estimator
{
    public class CacheEstimator : IEstimator
    {
        private IEstimator estimator;

        private ConcurrentDictionary<ValueTuple<ISquare, ISquare>, double> cacheLeftRight = new ConcurrentDictionary<ValueTuple<ISquare, ISquare>, double>();

        private ConcurrentDictionary<ValueTuple<ISquare, ISquare>, double> cacheTopBottom = new ConcurrentDictionary<ValueTuple<ISquare, ISquare>, double>();

        private ConcurrentDictionary<Square, double> cacheSquare = new ConcurrentDictionary<Square, double>();

        private ConcurrentDictionary<ISquare, double> cachleDeepSquare = new ConcurrentDictionary<ISquare, double>();

        public CacheEstimator(IEstimator estimator)
        {
            this.estimator = estimator;
        }

        private double GetOrAdd<TArg>(Bitmap image, ConcurrentDictionary<TArg, double> cache, TArg arg, Func<Bitmap, TArg, double> calc)
            => cache.GetOrAdd(arg, (a) => calc(image, a));

        public double MeasureLeftRight(Bitmap image, ISquare left, ISquare right)
            => GetOrAdd(image, cacheLeftRight, (left, right), (img, arg) => estimator.MeasureLeftRight(img, arg.Item1, arg.Item2));

        public double MeasureTopBottom(Bitmap image, ISquare top, ISquare bottom)
            => GetOrAdd(image, cacheTopBottom, (top, bottom), (img, arg) => estimator.MeasureTopBottom(img, arg.Item1, arg.Item2));

        public double MeasureSquare(Bitmap image, Square square)
            => cacheSquare.GetOrAdd(square, (s) => estimator.MeasureSquare(image, s));

        public double DeepMeasureSquare(Bitmap image, ISquare square)
            => cachleDeepSquare.GetOrAdd(square, (s) => estimator.DeepMeasureSquare(image, s));
    }
}