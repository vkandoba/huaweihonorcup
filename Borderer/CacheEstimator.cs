using System;
using System.Collections.Generic;
using System.Drawing;

namespace Borderer
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

        public double MeasureLeftRight(Bitmap image, ISquare left, ISquare right)
        {
            if (cacheLeftRight.TryGetValue((left, right), out double f))
                return f;

            var measure = estimator.MeasureLeftRight(image, left, right);
            cacheLeftRight.Add((left, right), measure);
            return measure;
        }

        public double MeasureTopBottom(Bitmap image, ISquare top, ISquare bottom)
        {
            if (cacheTopBottom.TryGetValue((top, bottom), out double f))
                return f;

            var measure = estimator.MeasureTopBottom(image, top, bottom);
            cacheTopBottom.Add((top, bottom), measure);
            return measure;
        }

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