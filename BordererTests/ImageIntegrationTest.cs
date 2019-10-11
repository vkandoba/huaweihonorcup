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
                var square = Square.MakeSquare(slices);

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
    }
}
