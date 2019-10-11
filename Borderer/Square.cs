﻿using System;
using System.Drawing;
using System.Net;

namespace Borderer
{
    public class Square : ISquare
    {
        private ISquare first;
        private ISquare second;
        private ISquare thrid;
        private ISquare four;
        private ISquare[] parts;
        private int baseSize;

        public Square(ISquare[] parts)
        {
            first = parts[0];
            second = parts[1];
            thrid = parts[2];
            four = parts[3];
            this.parts = parts;
            baseSize = first.Size;
            Size = baseSize * 2;
        }

        public Square(ISquare first, ISquare second, ISquare thrid, ISquare four)
        {
            this.first = first;
            this.second = second;
            this.thrid = thrid;
            this.four = four;
            parts = new[] {first, second, thrid, four};
            baseSize = first.Size;
            Size = baseSize * 2;
        }

        public int Size { get; }
        public (int, int) ToAbsolute(int x, int y)
        {
            if (x < baseSize)
            {
                if (y < baseSize)
                    return first.ToAbsolute(x, y);
                else
                    return thrid.ToAbsolute(x, y - baseSize);
            }
            else
            {
                if (y < baseSize)
                    return second.ToAbsolute(x - baseSize, y);
                else
                    return four.ToAbsolute(x - baseSize, y - baseSize);
            }
        }

        public Bitmap Draw(Bitmap original)
        {
            var bitmap = new Bitmap(Size, Size);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(first.Draw(original), 0 , 0);
                g.DrawImage(second.Draw(original), baseSize, 0);
                g.DrawImage(thrid.Draw(original), 0, baseSize);
                g.DrawImage(four.Draw(original), baseSize, baseSize);
            }
            return bitmap;
        }

        public double Estimate(Bitmap image, IEstimator estimator) =>
            (estimator.MeasureH(image, first, second) +
             estimator.MeasureV(image, first, thrid) +
             estimator.MeasureH(image, thrid, four) +
             estimator.MeasureV(image, second, four)) / 4.0;

        public double DeepEstimate(Bitmap image, IEstimator estimator)
        {
            var internalEstimate = (first.DeepEstimate(image, estimator) +
                                    second.DeepEstimate(image, estimator) +
                                    thrid.DeepEstimate(image, estimator) +
                                    four.DeepEstimate(image, estimator)) / 4.0;
            var ownEstimate = Estimate(image, estimator);
            return (internalEstimate + ownEstimate) / 2.0;
        }

        public bool HasCross(ISquare other)
        {
            var square = other as Square;
            if (square == null || square.Size != this.Size)
                throw new ArgumentException("slice iscross has invalid argument", nameof(other));

            foreach (var myPart in parts)
                foreach (var otherPart in square.parts)
                    if (Equals(myPart, otherPart) || myPart.HasCross(otherPart))
                        return true;

            return false;
        }

        protected bool Equals(Square other)
        {
            return Equals(first, other.first) && Equals(second, other.second) && Equals(thrid, other.thrid) && Equals(four, other.four);
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
                var hashCode = (first != null ? first.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (second != null ? second.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (thrid != null ? thrid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (four != null ? four.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static Square MakeSquare(Slice[,] slices)
        {
            var result = MakeSquareInternal(slices, slices.GetLength(0));
            return result[0, 0] as Square;
        }

        private static ISquare[,] MakeSquareInternal(ISquare[,] squares, int size)
        {
            if (size == 1)
                return squares;

            var result = new ISquare[size / 2, size / 2];
            for (int i = 0; i < size / 2; i++)
            {
                for (int j = 0; j < size / 2; j++)
                {
                    int x = 2 * i, y = 2 * j;
                    result[i, j] = new Square(new []
                    {
                        squares[x, y],
                        squares[x + 1, y],
                        squares[x, y + 1],
                        squares[x + 1, y + 1]
                    });
                }       
            }

            return MakeSquareInternal(result, size / 2);
        }
    }
}