using System.Drawing;

namespace Borderer.Estimator
{
    public class RecursiveEstimator : IEstimator
    {
        private IEstimator estimator;

        public RecursiveEstimator(IEstimator estimator)
        {
            this.estimator = estimator;
        }

        public double MeasureLeftRight(Bitmap image, ISquare left, ISquare right)
        {
            var leftS = left as Square;
            var rightS = right as Square;
            if (leftS == null || rightS == null)
                return estimator.MeasureLeftRight(image, left, right);

            return (MeasureLeftRight(image, leftS.Second, rightS.First) +
                    MeasureLeftRight(image, leftS.Four, rightS.Thrid)) / 2.0;
        }

        public double MeasureTopBottom(Bitmap image, ISquare top, ISquare bottom)
        {
            var topS = top as Square;
            var bottomS = bottom as Square;
            if (topS == null || bottomS == null)
                return estimator.MeasureTopBottom(image, top, bottom);

            return (MeasureTopBottom(image, topS.Thrid, bottomS.First) +
                    MeasureTopBottom(image, topS.Four, bottomS.Second)) / 2.0;
        }

        public double MeasureSquare(Bitmap image, Square square) =>
            (MeasureLeftRight(image, square.First, square.Second) +
             MeasureTopBottom(image, square.First, square.Thrid) +
             MeasureLeftRight(image, square.Thrid, square.Four) +
             MeasureTopBottom(image, square.Second, square.Four)) / 4.0;


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