using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Borderer;
using Borderer.Estimator;
using NUnit.Framework;

namespace BordererTests
{
    public class SquareServiceTest : TestBase
    {
        private SquareService service;

        [Test]
        public void TestGetSquares()
        {
            var estimator = CreateEstimator();
            service = new SquareService(estimator, 4);
            var slices = Slice.GenerateBaseSlices(64);
            var square = SquareService.MakeSquare(slices);
            foreach (var imagefile in Directory.GetFiles(imageset64).Skip(0).Take(1))
            {

                var imageName = Path.GetFileNameWithoutExtension(imagefile);
                var image = new Bitmap(imagefile);
                var adjusted = image.AdjustContrast(2);
                var sourcefile = Path.Combine(imagesource64, imageName + ".png");
                var source = new Bitmap(sourcefile);

                var sw = new Stopwatch();
                sw.Start();

                var array = slices.Cast<Slice>().ToArray();
                var squares = service.GetSquares(image, array);

                sw.Stop();

                Console.WriteLine($"image: {imageName}\n time: {sw.Elapsed}\n");

                foreach (var pair in squares.OrderBy(x => x.Value.F).Take(param.M * param.M))
                {
                    var N = (pair.Key as Slice).N;
                    Console.WriteLine($"{N} {pair.Value.F}");
                    var bitmap = pair.Value.Square.Draw(image);
                    bitmap.Save($"C:\\huaway\\tests\\drawimage-{imageName}-{N}.png");
                }
                var bitmap2 = square.Draw(source);
                bitmap2.Save($"C:\\huaway\\tests\\source-{imageName}.png");
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