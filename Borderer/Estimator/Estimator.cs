using System.Drawing;

namespace Borderer.Estimator
{
    public class Estimator : IEstimator
    {
        public double MeasureLeftRight(Bitmap image, ISquare left, ISquare right)
        {
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
            return measure;
        }

        public double MeasureTopBottom(Bitmap image, ISquare top, ISquare bottom)
        {
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
            var ownEstimate = MeasureSquare(image, s);
            return (internalEstimate + ownEstimate) / 2.0;
        }
    }
}