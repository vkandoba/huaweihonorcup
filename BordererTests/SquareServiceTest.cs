using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Borderer;
using NUnit.Framework;

namespace BordererTests
{
    public class SquareServiceTest : TestBase
    {
        private SquareService service;

        [Test]
        public void TestGetSquares()
        {
            var estimator = new Estimator();
            service = new SquareService(estimator, 10);
            var slices = Slice.GenerateBaseSlices(64);
            var square = Square.MakeSquare(slices);
            foreach (var imagefile in Directory.GetFiles(imageset).Skip(3).Take(1))
            {

                var imageName = Path.GetFileNameWithoutExtension(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource, imageName + ".png");
                var source = new Bitmap(sourcefile);

                var sw = new Stopwatch();

                sw.Start();

                var squares = service.GetSquares(image, slices.Cast<Slice>().ToArray());

                sw.Stop();

                Console.WriteLine($"image: {imageName}\n time: {sw.Elapsed}\n");

                foreach (var pair in squares.OrderBy(x => x.Value.F))
                {
                    var N = (pair.Key as Slice).N;
                    Console.WriteLine($"{N} {pair.Value.F}");
                    var bitmap = pair.Value.Square.Draw(image);
                    bitmap.Save($"C:\\huaway\\tests\\drawimage-{imageName}-{N}.png");
                }
                var bitmap2 = square.Draw(source);
                bitmap2.Save($"C:\\huaway\\tests\\source-{imageName}.png");
                Console.WriteLine($"done\n");
            }
        }

        [Test]
        public void TestRecursivePrototype()
        {
            var estimator = new Estimator();
            service = new SquareService(estimator, 20);
            var slices = Slice.GenerateBaseSlices(64);
            var square = Square.MakeSquare(slices);
            foreach (var imagefile in Directory.GetFiles(imageset).Take(1))
            {

                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource, imageName);
                var source = new Bitmap(sourcefile);

                var sw = new Stopwatch();

                sw.Start();

                SquareAndF[] squares = slices.Cast<Slice>().Select(slice => new SquareAndF
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
                    size = squares.First().Square.Size;
                } while (size < ImageParameters.__totalSize);

                var answer = squares.First();
                var bitmap = answer.Square.Draw(image);
                bitmap.Save($"C:\\huaway\\tests\\answr-{imageName}");
                Console.WriteLine($"image: {imageName}\n time: {sw.Elapsed}\n");
            }
        }
        [Test]
        public void TestPrototype1()
        {
            var estimator = new Estimator();
            service = new SquareService(estimator, 10);
            var slices = Slice.GenerateBaseSlices(64);
            var square = Square.MakeSquare(slices);
            foreach (var imagefile in Directory.GetFiles(imageset).Take(1))
            {

                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource, imageName);
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