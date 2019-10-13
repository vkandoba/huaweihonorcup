using System.Drawing;
using Borderer.Estimator;

namespace Borderer.Squares
{
    public interface ISquare
    {
        int Size { get; }
        (int, int) ToAbsolute(int x, int y);
        Bitmap Draw(Bitmap original);
        Slice[,] Apply(int p);

        double Estimate(Bitmap image, IEstimator estimator);
        bool HasCross(ISquare other);
    }
}