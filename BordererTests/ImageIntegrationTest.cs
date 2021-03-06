﻿using System;
using System.Linq;
using Borderer;
using Borderer.Squares;
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

        [TestCase(3)]
        public void TestSplitAndDrawImage(int count)
        {
            foreach (var name in set64.Take(count))
            {
                var slices = Slice.GenerateBaseSlices(64);
                var square = SquareBuilder.MakeSquare(slices);

                var train = ReadImage(name);

                var bitmap1 = square.Draw(train.Image);
                var bitmap2 = square.Draw(train.Original);
                bitmap1.Save($"C:\\huaway\\tests\\drawimage-{name}.png");
                bitmap2.Save($"C:\\huaway\\tests\\drawsource-{name}.png");
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
            var square = SquareBuilder.MakeSquare(slices);

            var bitmap1 = square.Draw(train.Image);
            bitmap1.Save($"C:\\huaway\\tests\\recoveimage-{train.Name}.png");
        }
    }
}
