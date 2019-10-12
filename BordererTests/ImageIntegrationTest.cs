using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Borderer;
using NUnit.Framework;

namespace BordererTests
{
    public class ImageIntegrationTest : TestBase
    {
        [Test]
        public void TestReadImage()
        {
            foreach (var imagefile in Directory.GetFiles(imageset).Take(1))
            {
                var image = new Bitmap(imagefile);
                var pixel = image.GetPixel(100, 100);
                Console.WriteLine($"image: {imagefile} \n" +
                                  $"width: {image.Width} height: {image.Height} format: {image.PixelFormat} " +
                                  $"{image.RawFormat} \n" +
                                  $"pixel: {pixel} \n");
            }
        }

        [Test]
        public void TestDrawSquare()
        {
            foreach (var imagefile in Directory.GetFiles(imageset).Take(1))
            {
                var image = new Bitmap(imagefile);
                var allslices = Slice.GenerateBaseSlices(64);
                var slices = new[]
                {
                    allslices[0, 0],
                    allslices[1, 0],
                    allslices[1, 2],
                    allslices[4, 2]
                };
                var square = new Square(slices);
                var bitmap = square.Draw(image);
                bitmap.Save(@"C:\huaway\tests\integrationdrawsquare.png");
            }
        }

        [Test]
        public void TestSplitAndDrawImage()
        {
            foreach (var imagefile in Directory.GetFiles(imageset).Take(3))
            {
                var slices = Slice.GenerateBaseSlices(64);
                var square = SquareService.MakeSquare(slices);

                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource, imageName);
                var source = new Bitmap(sourcefile);

                var bitmap1 = square.Draw(image);
                var bitmap2 = square.Draw(source);
                bitmap1.Save($"C:\\huaway\\tests\\drawimage-{imageName}");
                bitmap2.Save($"C:\\huaway\\tests\\drawsource-{imageName}");
            }
        }

        [Test]
        public void TestRecoveImage()
        {
            var order = new int[]
            {
                32, 21, 22, 16, 14, 60, 29, 39,
                33, 31, 54, 26, 10, 20, 1,  41,
                56, 28, 12, 17, 25, 58, 59, 43,
                45, 46, 44, 18, 0,  3, 62,  6,
                38, 48, 13, 63, 27, 19, 50, 51,
                23, 37, 40, 53, 42, 24, 61, 57,
                52, 15, 5, 34, 36,  4, 35,  8,
                49, 9,  2, 55, 7,  11, 47,  3
            };
            foreach (var imagefile in Directory.GetFiles(imageset).Take(1))
            {
                var slices = Slice.GenerateBaseSlices(64);

                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);

                var array = slices.Cast<Slice>().OrderBy(s => s.N).ToArray();
                for (int i = 0; i < order.Length; i++)
                {
                    var x = i % param.M;
                    var y = i / param.M;
                    slices[x, y] = array[order[i]];
                }
                var square = SquareService.MakeSquare(slices);

                var bitmap1 = square.Draw(image);
                bitmap1.Save($"C:\\huaway\\tests\\recoveimage-{imageName}");
            }
        }
    }
}
