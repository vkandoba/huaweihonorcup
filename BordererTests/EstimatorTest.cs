using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Borderer;
using NUnit.Framework;

namespace BordererTests
{
    public class EstimatorTest : TestBase
    {
        private Estimator estimator;

        public override void SetUp()
        {
            base.SetUp();
            estimator = new Estimator();
        }

        [Test]
        public void TestEstimateSmallSquare()
        {
            foreach (var imagefile in Directory.GetFiles(imageset).Take(1))
            {
                var image = new Bitmap(imagefile);
                var allslices = Slice.GenerateBaseSlices(64);
                var slices = new[]
                {
                    allslices[0, 0],
                    allslices[1, 0],
                    allslices[0, 1],
                    allslices[1, 1]
                };
                var square = new Square(slices);
                var measure = square.Estimate(image, estimator);
                Console.WriteLine($"measure: {measure}");
            }
        }

        [Test]
        public void TestEstimateImages()
        {
            var slices = Slice.GenerateBaseSlices(64);
            var square = Square.MakeSquare(slices);
            foreach (var imagefile in Directory.GetFiles(imageset))
            {

                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource, imageName);
                var source = new Bitmap(sourcefile);

                var sw = new Stopwatch();

                sw.Start();

                var imagef = square.DeepEstimate(image, estimator);
                var originalf = square.DeepEstimate(source, estimator);

                sw.Stop();

                Console.WriteLine($"image: {imageName}\n f: {imagef} target: {originalf} \n time: {sw.Elapsed}\n");
            }
        }

        [Test]
        public void TestEstimateImages1()
        {
            var slices = Slice.GenerateBaseSlices(64);
            foreach (var imagefile in Directory.GetFiles(imageset))
            {

                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource, imageName);
                var source = new Bitmap(sourcefile);

                var size = param.M;
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

                Console.WriteLine($"image: {imageName}");
                var f = new double[size, size];
                var t = new double[size, size];

                for (int i = 0; i < squares.GetLength(0); i++)
                {
                    for (int j = 0; j < squares.GetLength(0); j++)
                    {
                        f[i, j] = squares[i, j].Estimate(image, estimator);
                        t[i, j] = squares[i, j].Estimate(source, estimator);
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
        public void TestEstimateImages2()
        {
            var slices = Slice.GenerateBaseSlices(64);
            var s = new Slice[slices.Length];

            var square = Square.MakeSquare(slices);
            foreach (var imagefile in Directory.GetFiles(imageset).Take(3))
            {

                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource, imageName);
                var source = new Bitmap(sourcefile);

                var size = param.M;
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

                Console.WriteLine($"image: {imageName}");
                var f = new double[size, size];
                var t = new double[size, size];

                for (int i = 0; i < squares.GetLength(0); i++)
                {
                    for (int j = 0; j < squares.GetLength(0); j++)
                    {
                        f[i, j] = squares[i, j].Estimate(image, estimator);
                        t[i, j] = squares[i, j].Estimate(source, estimator);
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
    }
}