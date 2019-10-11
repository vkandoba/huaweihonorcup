using System;
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
            service = new SquareService(estimator, 4);
            var slices = Slice.GenerateBaseSlices(64);
            var square = Square.MakeSquare(slices);
            foreach (var imagefile in Directory.GetFiles(imageset).Take(3))
            {

                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource, imageName);
                var source = new Bitmap(sourcefile);

                var sw = new Stopwatch();

                sw.Start();

                var squares = service.GetSquares(image, slices.Cast<Slice>().ToArray());

                sw.Stop();

                Console.WriteLine($"image: {imageName}\n time: {sw.Elapsed}\n");

                foreach (var pair in squares.OrderBy(x => x.Value.F))
                {
                    Console.WriteLine($"{(pair.Key as Slice).N} {pair.Value.F}");
                    //var bitmap = squares[slice].Draw(image);
                    //bitmap.Save($"C:\\huaway\\tests\\drawimage-{slice.N}-{imageName}");
                }
                Console.WriteLine($"done\n");
            }
        }

        [Test]
        public void TestRecursivePrototype()
        {
            var estimator = new Estimator();
            service = new SquareService(estimator, 64);
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

    }
}