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
        [TestCase("1200")]
        public void TestReadImage(string name)
        {
            var train = ReadImage(name);
            Console.WriteLine($"image: {train.Name} \n" +
                              $"width: {train.Image.Width} height: {train.Image.Height} format: {train.Image.PixelFormat} " +
                              $"{train.Image.RawFormat} \n");
        }

        [TestCase("1200")]
        public void TestDrawSquare(string name)
        {
            var image = ReadImage(name).Image;
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
            bitmap.Save(@"C:\huaway\tests\integrationsquare.png");
        }

        [Test]
        public void TestSplitAndDrawImage()
        {
            foreach (var imagefile in Directory.GetFiles(imageset64).Take(3))
            {
                var slices = Slice.GenerateBaseSlices(64);
                var square = SquareService.MakeSquare(slices);

                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource64, imageName);
                var source = new Bitmap(sourcefile);

                var bitmap1 = square.Draw(image);
                var bitmap2 = square.Draw(source);
                bitmap1.Save($"C:\\huaway\\tests\\drawimage-{imageName}");
                bitmap2.Save($"C:\\huaway\\tests\\drawsource-{imageName}");
            }
        }

        [TestCase("1200")]
        public void TestRecoveImageByPermutation(string name)
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

            var slices = Slice.GenerateBaseSlices(64);

            var train = ReadImage(name);

            var array = slices.Cast<Slice>().OrderBy(s => s.N).ToArray();
            for (int i = 0; i < order.Length; i++)
            {
                var x = i % param.M;
                var y = i / param.M;
                slices[x, y] = array[order[i]];
            }
            var square = SquareService.MakeSquare(slices);

            var bitmap1 = square.Draw(train.Image);
            bitmap1.Save($"C:\\huaway\\tests\\recoveimage-{train.Name}.png");
        }
    }
}
