using System;
using System.Collections.Generic;
using System.Drawing;

namespace Borderer
{
    public interface IEstimator
    {
        double MeasureLeftRight(Bitmap image, ISquare left, ISquare right);
        double MeasureTopBottom(Bitmap image, ISquare top, ISquare bottom);
        double MeasureSquare(Bitmap image, Square square);
        double DeepMeasureSquare(Bitmap image, ISquare square);
        double RecursiveMeasureSquare(Bitmap image, Square square);
        double RecursiveMeasureLeftRight(Bitmap image, ISquare left, ISquare right);
        double RecursiveMeasureTopBottom(Bitmap image, ISquare top, ISquare bottom);
    }

    public class Estimator : IEstimator
    {
        private IDictionary<ValueTuple<ISquare, ISquare>, double> cacheLeftRight = new Dictionary<ValueTuple<ISquare, ISquare>, double>();
        private IDictionary<ValueTuple<ISquare, ISquare>, double> cacheTopBottom = new Dictionary<ValueTuple<ISquare, ISquare>, double>();

        public double MeasureLeftRight(Bitmap image, ISquare left, ISquare right)
        {
            if (cacheLeftRight.TryGetValue((left, right), out double f))
                return f;

            var total = 0.0;
            for (int i = 0; i < left.Size; i++)
            {
                var lxy = left.ToAbsolute(left.Size - 1, i);
                var leftp = image.GetPixel(lxy.Item1, lxy.Item2);
                var rxy = right.ToAbsolute(0, i);
                var rightp = image.GetPixel(rxy.Item1, rxy.Item2);
                total += VMath.Diff(leftp, rightp);
            }

            var measure = total / left.Size;
            cacheLeftRight.Add((left, right), measure);
            return measure;
        }

        public double MeasureTopBottom(Bitmap image, ISquare top, ISquare bottom)
        {
            if (cacheTopBottom.TryGetValue((top, bottom), out double f))
                return f;

            var total = 0.0;
            for (int i = 0; i < bottom.Size; i++)
            {
                var bxy = bottom.ToAbsolute(i, 0);
                var bottomp = image.GetPixel(bxy.Item1, bxy.Item2);
                var txy = top.ToAbsolute(i, bottom.Size - 1);
                var topp = image.GetPixel(txy.Item1, txy.Item2);
                total += VMath.Diff(bottomp, topp);
            }

            var measure = total / top.Size;
            cacheTopBottom.Add((top, bottom), measure);
            return measure;
        }

        public double MeasureSquare(Bitmap image, Square square) =>
                    (MeasureLeftRight(image, square.First, square.Second) +
                    MeasureTopBottom(image, square.First, square.Thrid) +
                    MeasureLeftRight(image, square.Thrid, square.Four) +
                    MeasureTopBottom(image, square.Second, square.Four))/ 4.0;

        public double DeepMeasureSquare(Bitmap image, ISquare square)
        {
            var s = square as Square;
            if (s == null)
                return 0;

            var internalEstimate = (DeepMeasureSquare(image, s.First) +
                                    DeepMeasureSquare(image, s.Second) +
                                    DeepMeasureSquare(image, s.Thrid) +
                                    DeepMeasureSquare(image, s.Four)) / 4.0;
            var ownEstimate = RecursiveMeasureSquare(image, s);
            return (internalEstimate + ownEstimate) / 2.0;

        }

        public double RecursiveMeasureSquare(Bitmap image, Square square) =>
            (RecursiveMeasureLeftRight(image, square.First, square.Second) +
             RecursiveMeasureTopBottom(image, square.First, square.Thrid) +
             RecursiveMeasureLeftRight(image, square.Thrid, square.Four) +
             RecursiveMeasureTopBottom(image, square.Second, square.Four)) / 4.0;

        public double RecursiveMeasureLeftRight(Bitmap image, ISquare left, ISquare right)
        {
            var leftS = left as Square;
            var rightS = right as Square;
            if (leftS == null || rightS == null)
                return MeasureLeftRight(image, left, right);

            return (RecursiveMeasureLeftRight(image, leftS.Second, rightS.First) +
                    RecursiveMeasureLeftRight(image, leftS.Four, rightS.Thrid)) / 2.0;
        }
        public double RecursiveMeasureTopBottom(Bitmap image, ISquare top, ISquare bottom)
        {
            var topS = top as Square;
            var bottomS = bottom as Square;
            if (topS == null || bottomS == null)
                return MeasureTopBottom(image, top, bottom);

            return (RecursiveMeasureTopBottom(image, topS.Thrid, bottomS.First) +
                    RecursiveMeasureTopBottom(image, topS.Four, bottomS.Second)) / 2.0;
        }
    }
}