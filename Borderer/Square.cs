using System;
using System.Drawing;
using Borderer.Estimator;

namespace Borderer
{
    public class Square : ISquare
    {
        public ISquare First { get; }

        public ISquare Second { get; }

        public ISquare Thrid { get; }

        public ISquare Four { get; }

        public ISquare[] Parts { get; set; }

        private readonly int baseSize;

        public Square(ISquare[] parts)
        {
            First = parts[0];
            Second = parts[1];
            Thrid = parts[2];
            Four = parts[3];
            this.Parts = parts;
            baseSize = First.Size;
            Size = baseSize * 2;
        }

        public Square(ISquare first, ISquare second, ISquare thrid, ISquare four)
        {
            this.First = first;
            this.Second = second;
            this.Thrid = thrid;
            this.Four = four;
            Parts = new[] {first, second, thrid, four};
            baseSize = first.Size;
            Size = baseSize * 2;
        }

        public int Size { get; }
        public (int, int) ToAbsolute(int x, int y)
        {
            if (x < baseSize)
            {
                if (y < baseSize)
                    return First.ToAbsolute(x, y);
                else
                    return Thrid.ToAbsolute(x, y - baseSize);
            }
            else
            {
                if (y < baseSize)
                    return Second.ToAbsolute(x - baseSize, y);
                else
                    return Four.ToAbsolute(x - baseSize, y - baseSize);
            }
        }

        public Bitmap Draw(Bitmap original)
        {
            var bitmap = new Bitmap(Size, Size);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(First.Draw(original), 0 , 0);
                g.DrawImage(Second.Draw(original), baseSize, 0);
                g.DrawImage(Thrid.Draw(original), 0, baseSize);
                g.DrawImage(Four.Draw(original), baseSize, baseSize);
            }
            return bitmap;
        }

        public double Estimate(Bitmap image, IEstimator estimator) =>
            (estimator.MeasureLeftRight(image, First, Second) +
             estimator.MeasureTopBottom(image, First, Thrid) +
             estimator.MeasureLeftRight(image, Thrid, Four) +
             estimator.MeasureTopBottom(image, Second, Four)) / 4.0;

        public Slice[,] Apply()
        {
            var slices = new Slice[Size / 64, Size / 64];
            var shift = baseSize / 64;

            slices.CopyPart(First.Apply(), 0, 0);
            slices.CopyPart(Second.Apply(), shift, 0);
            slices.CopyPart(Thrid.Apply(), 0, shift);
            slices.CopyPart(Four.Apply(), shift, shift);

            return slices;
        }

        public bool HasCross(ISquare other)
        {
            var square = other as Square;
            if (square == null || square.Size != this.Size)
                throw new ArgumentException("slice iscross has invalid argument", nameof(other));

            foreach (var myPart in Parts)
                foreach (var otherPart in square.Parts)
                    if (Equals(myPart, otherPart) || myPart.HasCross(otherPart))
                        return true;

            return false;
        }

        public override string ToString() => $"{First} {Second} {Thrid} {Four}";

        protected bool Equals(Square other)
        {
            return Equals(First, other.First) && Equals(Second, other.Second) && Equals(Thrid, other.Thrid) && Equals(Four, other.Four);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Square) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (First != null ? First.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Second != null ? Second.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Thrid != null ? Thrid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Four != null ? Four.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}