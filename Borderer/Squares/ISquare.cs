using System.Drawing;

namespace Borderer.Squares
{
    public interface ISquare
    {
        int Size { get; }
        (int, int) ToAbsolute(int x, int y);
        Bitmap Draw(Bitmap original);
        Slice[,] Apply(int p);

        bool HasCross(ISquare other);
    }
}