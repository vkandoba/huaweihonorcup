using System.Drawing;
using Borderer.Squares;

namespace Borderer.Estimator
{
    public interface IEstimator
    {
        double MeasureLeftRight(Bitmap image, ISquare left, ISquare right);
        double MeasureTopBottom(Bitmap image, ISquare top, ISquare bottom);
        double MeasureSquare(Bitmap image, Square square);
        double DeepMeasureSquare(Bitmap image, ISquare square);
    }
}