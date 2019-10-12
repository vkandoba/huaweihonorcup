using System.Drawing;
using System.IO;
using System.Linq;
using Borderer;
using NUnit.Framework;

namespace BordererTests
{
    public class ImageToolTests : TestBase
    {
        [Test]
        public void TestToGrayStyle()
        {
            foreach (var imagefile in Directory.GetFiles(imageset).Take(10))
            {
                var slices = Slice.GenerateBaseSlices(64);
                var square = SquareService.MakeSquare(slices);

                var imageName = Path.GetFileName(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource, imageName);
                var source = new Bitmap(sourcefile);

                var bitmap1 = square.Draw(image);
                var bitmap2 = square.Draw(source);
                var bitmap3 = image.ToGrayStyle();
                var bitmap4 = source.ToGrayStyle();
                bitmap1.Save($"C:\\huaway\\tests\\image-{imageName}");
                bitmap2.Save($"C:\\huaway\\tests\\source-{imageName}");
                bitmap3.Save($"C:\\huaway\\tests\\image-gray-{imageName}");
                bitmap4.Save($"C:\\huaway\\tests\\source-gray-{imageName}");
            }
        }

        [Test]
        public void TestTAdjustContrast()
        {
            foreach (var imagefile in Directory.GetFiles(imageset).Take(10))
            {
                var slices = Slice.GenerateBaseSlices(64);
                var square = SquareService.MakeSquare(slices);

                var imageName = Path.GetFileName(imagefile);
                var name = Path.GetFileNameWithoutExtension(imagefile);
                var image = new Bitmap(imagefile);
                var sourcefile = Path.Combine(imagesource, imageName);
                var source = new Bitmap(sourcefile);

                var bitmap1 = square.Draw(image);
                var bitmap2 = square.Draw(source);
                var bitmap3 = image.AdjustContrast(2);
                var bitmap4 = source.AdjustContrast(2);
                bitmap1.Save($"C:\\huaway\\tests\\{imageName}-split.png");
                bitmap2.Save($"C:\\huaway\\tests\\{imageName}.png");
                bitmap3.Save($"C:\\huaway\\tests\\{imageName}-split-contrast.png");
                bitmap4.Save($"C:\\huaway\\tests\\{imageName}-contrast.png");
            }
        }
    }
}