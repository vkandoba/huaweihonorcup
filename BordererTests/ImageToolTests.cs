using System.Linq;
using Borderer;
using NUnit.Framework;

namespace BordererTests
{
    public class ImageToolTests : TestBase
    {
        private string[] set;
        private int p;

        public override void SetUp()
        {
            base.SetUp();
            set = set32;
            p = 32;
        }

        [TestCase(10)]
        public void TestToGrayStyle(int count)
        {
            foreach (var name in set.Take(count))
            {
                var train = ReadImage(name, p);
                var slices = Slice.GenerateBaseSlices(p);
                var square = SquareService.MakeSquare(slices);

                var bitmap1 = square.Draw(train.Image);
                var bitmap2 = square.Draw(train.Original);
                var bitmap3 = train.Image.ToGrayStyle();
                var bitmap4 = train.Original.ToGrayStyle();
                bitmap1.Save($"C:\\huaway\\tool\\image-{name}.png");
                bitmap2.Save($"C:\\huaway\\tool\\source-{name}.png");
                bitmap3.Save($"C:\\huaway\\tool\\image-gray-{name}.png");
                bitmap4.Save($"C:\\huaway\\tool\\source-gray-{name}.png");
            }
        }

        [TestCase(10)]
        public void TestTAdjustContrast(int count)
        {
            foreach (var name in set.Take(count))
            {
                var train = ReadImage(name, p);
                var slices = Slice.GenerateBaseSlices(p);
                var square = SquareService.MakeSquare(slices);

                var bitmap1 = square.Draw(train.Image);
                var bitmap2 = square.Draw(train.Original);
                var bitmap3 = train.Image.AdjustContrast(2);
                var bitmap4 = train.Original.AdjustContrast(2);
                bitmap1.Save($"C:\\huaway\\tool\\image-{name}.png");
                bitmap2.Save($"C:\\huaway\\tool\\source-{name}.png");
                bitmap3.Save($"C:\\huaway\\tool\\image-contrast-{name}.png");
                bitmap4.Save($"C:\\huaway\\tool\\source-contrast-{name}.png");
            }
        }
    }
}