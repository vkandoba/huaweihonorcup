using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Borderer.Estimator;
using Borderer.Helpers;

namespace Borderer.Squares
{
    public class Square : ISquare
    {
        public ISquare First { get; }

        public ISquare Second { get; }

        public ISquare Thrid { get; }

        public ISquare Four { get; }

        private readonly int baseSize;

        private ulong numbers = 0;

        private ulong Numbers
        {
            get
            {
                if (numbers == 0)
                    numbers = MakeNumbersSet();
                return numbers;
            }
        }

        private ulong MakeNumbersSet()
        {
            ulong set = 0;
            if (First is Slice)
            {
                set |= VMath.BitOut((First as Slice).N);
                set |= VMath.BitOut((Second as Slice).N);
                set |= VMath.BitOut((Thrid as Slice).N);
                set |= VMath.BitOut((Four as Slice).N);
            }
            else
            {
                set |= (First as Square).Numbers;
                set |= (Second as Square).Numbers;
                set |= (Thrid as Square).Numbers;
                set |= (Four as Square).Numbers;
            }
            return set;
        }

        public Square(ISquare[] parts)
        {
            First = parts[0];
            Second = parts[1];
            Thrid = parts[2];
            Four = parts[3];
            baseSize = First.Size;
            Size = baseSize * 2;
        }

        public Square(ISquare first, ISquare second, ISquare thrid, ISquare four)
        {
            this.First = first;
            this.Second = second;
            this.Thrid = thrid;
            this.Four = four;
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

        public Slice[,] Apply(int p)
        {
            var slices = new Slice[Size / p, Size / p];
            var shift = baseSize / p;

            slices.CopyPart(First.Apply(p), 0, 0);
            slices.CopyPart(Second.Apply(p), shift, 0);
            slices.CopyPart(Thrid.Apply(p), 0, shift);
            slices.CopyPart(Four.Apply(p), shift, shift);

            return slices;
        }

        public bool HasCross(ISquare other)
        {
            var square = other as Square;
            if (square == null || square.Size != this.Size)
                throw new ArgumentException("slice iscross has invalid argument", nameof(other));

            return (Numbers & square.Numbers) != 0;
        }

        private string str = null;

        private string String
        {
            get
            {
                if (str == null)
                    str = ToString();

                return str;
            }
        }

        public override string ToString() => $"{First} {Second} {Thrid} {Four}";

        protected bool Equals(Square other)
        {
            return Numbers == other.Numbers && String == other.ToString();
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
               return (int) Numbers;
            }
        }
    }
}