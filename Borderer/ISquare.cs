using System.Drawing;
using Borderer.Estimator;

namespace Borderer
{
    public interface ISquare
    {
        int Size { get; }
        (int, int) ToAbsolute(int x, int y);
        Bitmap Draw(Bitmap original);
        Slice[,] Apply();

        double Estimate(Bitmap image, IEstimator estimator);
        bool HasCross(ISquare other);
    }
}