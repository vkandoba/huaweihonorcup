using System.Drawing;
using Borderer.Helpers;
using Borderer.Squares;

namespace Borderer.Estimator
{
    public class Estimator : IEstimator
    {
        private Color[] GetLineY(Bitmap image, ISquare square, int x)
        {
            var size = square.Size;
            var colors = new Color[size];
            lock (image)
            {
                for (int i = 0; i < size; i++)
                {
                    var coor = square.ToAbsolute(x, i);
                    colors[i] = image.GetPixel(coor.Item1, coor.Item2);
                }
            }

            return colors;
        }

        private Color[] GetLineX(Bitmap image, ISquare square, int y)
        {
            var size = square.Size;
            var colors = new Color[size];
            lock (image)
            {
                for (int i = 0; i < size; i++)
                {
                    var coor = square.ToAbsolute(i, y);
                    colors[i] = image.GetPixel(coor.Item1, coor.Item2);
                }
            }

            return colors;
        }

        public double MeasureLeftRight(Bitmap image, ISquare left, ISquare right)
        {
            var total = 0.0;
            var leftline = GetLineY(image, left, left.Size - 1);
            var rightline = GetLineY(image, right, 0);
            for (int i = 0; i < left.Size; i++)
                total += VMath.Diff(leftline[i], rightline[i]);

            var measure = total / left.Size;
            return measure;
        }

        public double MeasureTopBottom(Bitmap image, ISquare top, ISquare bottom)
        {
            var total = 0.0;
            var topline = GetLineX(image, top, top.Size - 1);
            var bottomline = GetLineX(image, bottom, 0);
            for (int i = 0; i < bottom.Size; i++)
                total += VMath.Diff(topline[i], bottomline[i]);

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