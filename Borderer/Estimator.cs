using System;
using System.Drawing;

namespace Borderer
{
    public interface IEstimator
    {
        double MeasureH(Bitmap image, ISquare left, ISquare right);
        double MeasureV(Bitmap image, ISquare top, ISquare bottom);
    }

    public class Estimator : IEstimator
    {
        public double MeasureH(Bitmap image, ISquare left, ISquare right)
        {
            var total = 0.0;
            for (int i = 0; i < left.Size; i++)
            {
                var lxy = left.ToAbsolute(left.Size - 1, i);
                var leftp = image.GetPixel(lxy.Item1, lxy.Item2);
                var rxy = right.ToAbsolute(0, i);
                var rightp = image.GetPixel(rxy.Item1, rxy.Item2);
                total += Compare(leftp, rightp);
            }
            return total / left.Size;
        }

        public double MeasureV(Bitmap image, ISquare top, ISquare bottom)
        {
            var total = 0.0;
            for (int i = 0; i < bottom.Size; i++)
            {
                var bxy = bottom.ToAbsolute(i, 0);
                var bottomp = image.GetPixel(bxy.Item1, bxy.Item2);
                var txy = top.ToAbsolute(i, bottom.Size - 1);
                var topp = image.GetPixel(txy.Item1, txy.Item2);
                total += Compare(bottomp, topp);
            }
            return total / bottom.Size;
        }

        private int Compare(Color p1, Color p2)
        {
            return Math.Abs(p1.R - p2.R) + Math.Abs(p1.G - p2.G) + Math.Abs(p1.B - p2.B);
        }
    }
}