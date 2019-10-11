using System.Drawing;

namespace Borderer
{
    public class Slice : ISquare
    {
        private readonly ImageParameters parameters;

        public Slice(ImageParameters parameters, int positionX, int positionY)
        {
            this.parameters = parameters;
            PositionX = positionX;
            PositionY = positionY;
            ShiftX = PositionX * parameters.P;
            ShiftY = PositionY * parameters.P;
            N = PositionX * parameters.M + PositionY;
        }

        public int Size => parameters.P;
        public int PositionX { get; }
        public int PositionY { get; }
        public (int, int) ToAbsolute(int x, int y) => (ShiftX + x, ShiftY + y);

        public int ShiftX { get; }
        public int ShiftY { get; }
        public int N { get; }

        public Bitmap Draw(Bitmap original) =>
            original.Clone(new Rectangle(ShiftX, ShiftY, parameters.P, parameters.P), original.PixelFormat);

        public double Estimate(Bitmap image, IEstimator estimator) => 0;
        public double DeepEstimate(Bitmap image, IEstimator estimator) => 0;

        public static Slice[,] GenerateBaseSlices(int p)
        {
            var parameters = new ImageParameters(p);
            var result = new Slice[parameters.M, parameters.M];
            for (int i = 0; i < parameters.M; i++)
            {
                for (int j = 0; j < parameters.M; j++)
                {
                    result[i, j] = new Slice(parameters, i, j);
                }
            }

            return result;
        }

        protected bool Equals(Slice other)
        {
            return Equals(parameters, other.parameters) && N == other.N;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Slice) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((parameters != null ? parameters.GetHashCode() : 0) * 397) ^ N;
            }
        }
    }
}