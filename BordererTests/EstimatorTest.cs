using System;
using System.Diagnostics;
using Borderer;
using Borderer.Estimator;
using Borderer.Helpers;
using Borderer.Squares;
using NUnit.Framework;

namespace BordererTests
{
    public class EstimatorTest : TestBase
    {
        private IEstimator estimator;

        private string[] set;
        private int p;

        public override void SetUp()
        {
            base.SetUp();
            set = set32;
            p = 32;
            estimator = CreateEstimator();
        }

        [Test]
        public void TestEstimateImages()
        {
            foreach (var name in set)
            {
                var train = ReadImage(name, p);

                var slices = Slice.GenerateBaseSlices(p);
                var square = SquareBuilder.MakeSquare(slices);
                var sw = new Stopwatch();

                sw.Start();

                var imagef = estimator.DeepMeasureSquare(train.Image, square);
                var originalf = CreateEstimator().DeepMeasureSquare(train.Original, square);

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

                var size = train.Param.M / 2;
                var squares = new ISquare[size, size];
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
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

                var estimator1 = CreateEstimator();
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        f[i, j] = estimator.MeasureSquare(train.Image, squares[i, j] as Square);
                        t[i, j] = estimator1.MeasureSquare(train.Original, squares[i, j] as Square);
                    }
                }
                
                Console.WriteLine($"f:\n{f.Print(d => $"{d:N2}\t")}");
                Console.WriteLine($"t:\n{t.Print(d => $"{d:N2}\t")}");
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

                var f = new double[train.Param.M - 1, train.Param.M - 1];
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
                        f[x, y] = estimator.MeasureSquare(train.Original, square);
                    }
                }
                Console.WriteLine(f.Print(d => $"{d:N2}\t"));
                Console.WriteLine();
            }
        }

    }
}