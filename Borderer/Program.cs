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
            //var imageset = @"C:\huaway\data_train\64";
            int p = 64;
            var imageset = @"C:\huaway\data_test1_blank\64";
            var param = new ImageParameters(p);
            foreach (var imagefile in Directory.GetFiles(imageset))
            {
                var service = new SquareService(new Estimator.Estimator(), 4);
                var slices = Slice.GenerateBaseSlices(p);
                var image = new Bitmap(imagefile);
                var answer = service.RecursiveCollect(image, param, slices);
                var map = answer.Apply();
                var permutation = map.GetPermutation().Aggregate("", (s, n) => $"{s} {n}");
                Console.WriteLine(Path.GetFileName(imagefile));
                Console.WriteLine(permutation);

                var bitmap = answer.Draw(image);
                bitmap.Save($"C:\\huaway\\final\\answr-{Path.GetFileName(imagefile)}");
            }
        }
    }
}
