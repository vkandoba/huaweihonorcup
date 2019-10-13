using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Borderer;
using Borderer.Estimator;
using Borderer.Helpers;
using Borderer.Squares;
using NUnit.Framework;

namespace BordererTests
{
    public class SquareServiceTest : TestBase
    {
        private SquareService service;
        private string[] set;
        private int p;

        public override void SetUp()
        {
            base.SetUp();
            service = new SquareService(CreateEstimator(), 4);
            set = set64;
            p = 64;
        }

        [TestCase(0, 10)]
        public void TestGetSquares(int skip, int take)
        {
            foreach (var name in set.Skip(skip).Take(take))
            {
                var train = ReadImage(name, p);

                var slices = Slice.GenerateBaseSlices(p);

                var sw = new Stopwatch();
                sw.Start();

                var array = slices.Cast<Slice>().ToArray();
                var squares = service.GetSquares(train.Image, array);

                sw.Stop();

                Console.WriteLine($"image: {name}\n time: {sw.Elapsed}\n");

                var f = new double[train.Param.M, train.Param.M];
                for (int i = 0; i < train.Param.M; i++)
                {
                    for (int j = 0; j < train.Param.M; j++)
                    {
                        f[i, j] = squares[slices[i, j]].F;
                    }
                }

                Console.WriteLine($"f:\n{f.Print(d => $"{d:N2}\t")}");
                Console.WriteLine();
            }
        }

        [Test]
        public void TestRecursivePrototype()
        {
            foreach (var imagefile in Directory.GetFiles(imageset64).Take(10))
            {
                var slices = Slice.GenerateBaseSlices(64);
                service = new SquareService(CreateEstimator(), 4);
                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);

                var sw = new Stopwatch();

                sw.Start();

                SquareAndMeasure[] squares = slices.Cast<Slice>().Select(slice => new SquareAndMeasure
                {
                    Square = slice,
                    F = slice.Estimate(image, null)
                }).ToArray();
                var size = param.P;
                do
                {
                    squares = service.GetSquares(image, squares.Select(x => x.Square).ToArray())
                        .Values
                        .OrderBy(x => x.F)
                        .ToArray();
                    size = squares.Any() ? squares.First().Square.Size : ImageParameters.__totalSize;
                } while (size < ImageParameters.__totalSize);
                sw.Stop();

                var answer = squares.Any() ? squares.First().Square : SquareService.MakeSquare(slices);
                var map = answer.Apply();
                var bitmap = answer.Draw(image);
                bitmap.Save($"C:\\huaway\\tests\\answr-{imageName}");
                var permutation = map.GetPermutation().Aggregate("", (s, n) => $"{s} {n}");
                Console.WriteLine($"image: {imageName}\n time: {sw.Elapsed}\n" +
                                  $"{permutation}");
            }
        }
        [Test]
        public void TestPrototype1()
        {
            var estimator = new Estimator();
            service = new SquareService(estimator, 10);
            var slices = Slice.GenerateBaseSlices(64);
            var square = SquareService.MakeSquare(slices);
            foreach (var imagefile in Directory.GetFiles(imageset64).Take(1))
            {

                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource64, imageName);
                var source = new Bitmap(sourcefile);

                var sw = new Stopwatch();

                sw.Start();

                var rest = slices.Cast<Slice>().ToList();
                var first = rest.First();
                rest.Remove(first);
                var squares1 = service.GetSquare(image, first, rest.ToArray()).Square as Square;

                IList<Square> next = new List<Square>();
                for (int i = 0; i < squares1.Parts.Length; i++)
                {
                    var item = service.GetSquare(image, squares1.Parts[i], rest.ToArray()).Square as Square;
                    next.Add(item);
                    foreach (var itemPart in item.Parts)
                        rest.Remove(itemPart as Slice);
                }
                var squares2 = new Square(next.ToArray());

                var bitmap = squares2.Draw(image);
                bitmap.Save($"C:\\huaway\\tests\\answr-{imageName}");
                Console.WriteLine($"image: {imageName}\n time: {sw.Elapsed}\n");
            }
        }

    }
}