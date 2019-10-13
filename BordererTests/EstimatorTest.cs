using System;
using System.Diagnostics;
using Borderer;
using NUnit.Framework;

namespace BordererTests
{
    public class EstimatorTest : TestBase
    {
        private Estimator estimator;

        private string[] set;
        private int p;

        public override void SetUp()
        {
            base.SetUp();
            set = set32;
            p = 32;
            estimator = new Estimator();
        }

        [Test]
        public void TestEstimateSmallSquare()
        {
            var train = ReadImage("1200");
            var allslices = Slice.GenerateBaseSlices(64);
            var slices = new[]
            {
                allslices[0, 0],
                allslices[1, 0],
                allslices[0, 1],
                allslices[1, 1]
            };
            var square = new Square(slices);
            var measure = square.Estimate(train.Image, estimator);
            Console.WriteLine($"measure: {measure}");
        }

        [Test]
        public void TestEstimateImages()
        {
            foreach (var name in set)
            {
                var train = ReadImage(name, p);

                var slices = Slice.GenerateBaseSlices(p);
                var square = SquareService.MakeSquare(slices);
                var sw = new Stopwatch();

                sw.Start();

                var imagef = square.DeepEstimate(train.Image, estimator);
                var originalf = square.DeepEstimate(train.Original, new Estimator());

                sw.Stop();

                Console.WriteLine($"image: {name}\n f: {imagef} target: {originalf} \n time: {sw.Elapsed}\n");
            }
        }

        [Test]
        public void TestEstimateSections()
        {
            foreach (var name in set)
            {
                var train = ReadImage(name, p);

                var slices = Slice.GenerateBaseSlices(p);

                var size = train.Param.M;
                var squares = new ISquare[size / 2, size / 2];
                for (int i = 0; i < size / 2; i++)
                {
                    for (int j = 0; j < size / 2; j++)
                    {
                        int x = 2 * i, y = 2 * j;
                        squares[i, j] = new Square(new[]
                        {
                            slices[x, y],
                            slices[x + 1, y],
                            slices[x, y + 1],
                            slices[x + 1, y + 1]
                        });
                    }
                }

                Console.WriteLine($"image: {name}");
                var f = new double[size, size];
                var t = new double[size, size];

                var estimator1 = new Estimator();
                for (int i = 0; i < squares.GetLength(0); i++)
                {
                    for (int j = 0; j < squares.GetLength(0); j++)
                    {
                        f[i, j] = estimator.MeasureSquare(train.Image, squares[i, j] as Square);
                        t[i, j] = estimator1.MeasureSquare(train.Original, squares[i, j] as Square);
                    }
                }
                
                Console.WriteLine($"f:");
                for (int i = 0; i < squares.GetLength(0); i++)
                {
                    for (int j = 0; j < squares.GetLength(0); j++)
                    {
                        Console.Write($"{f[i,j]:N2}\t");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine($"t:");
                for (int i = 0; i < squares.GetLength(0); i++)
                {
                    for (int j = 0; j < squares.GetLength(0); j++)
                    {
                        Console.Write($"{t[i,j]:N2}\t");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }

        [Test]
        public void TestEstimateSmallSqueres()
        {
            foreach (var name in set)
            {
                var train = ReadImage(name, p);

                var slices = Slice.GenerateBaseSlices(p);

                Console.WriteLine($"image: {train.Name}");

                var estimator = new Estimator();
                for (int y = 0; y < train.Param.M - 1; y++)
                {
                    for (int x = 0; x < train.Param.M - 1; x++)
                    {
                        var square = new Square(
                            slices[x, y],
                            slices[x + 1, y],
                            slices[x, y + 1],
                            slices[x + 1, y + 1]);
                        Console.Write($"{estimator.MeasureSquare(train.Original, square):N2}\t");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }

    }
}