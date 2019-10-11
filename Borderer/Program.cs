using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Borderer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var imageset = @"C:\huaway\data_train\64";
            int p = 64;
            foreach (var imagefile in Directory.GetFiles(imageset).Take(1))
            {
                var image = new Bitmap(imagefile);
                Console.WriteLine($"image: {imagefile} p: {p}");
                var result = MakeImage(image, p);
                result.Draw(image).Save("test.png");
            }
        }

        public static ISquare MakeImage(Bitmap image, int p)
        {
            return MakeImageInternal(ImageParameters.__totalSize, image, p).First();
        }

        private static ISquare[] MakeImageInternal(int size, Bitmap image, int p)
        {
            if (size == p)
                return Slice.GenerateBaseSlices(p).Cast<ISquare>().ToArray();

            var squares = MakeImageInternal(size / 2, image, p);
            var queue = new Queue<ISquare>(squares);
            var result = new List<ISquare>();

            while (queue.Any())
            {
                var first = queue.Dequeue();
                var second = queue.Dequeue();
                var thrid = queue.Dequeue();
                var four = queue.Dequeue();
                var item = new Square(new []{four, thrid, second, first});
                result.Add(item);
            }

            return result.ToArray();
        }
    }
}
