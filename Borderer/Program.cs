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
                var service = new SquareService(new Estimator(), 20);
                var slices = Slice.GenerateBaseSlices(p);
                var image = new Bitmap(imagefile);
                var answer = service.RecursiveCollect(image, param, slices);
                var map = (answer as Square).Apply();
                Console.WriteLine(Path.GetFileName(imagefile));
                var sp = new List<int>();
                for (int i = 0; i < param.M; i++)
                    for (int j = 0; j < param.M; j++)
                        sp.Add(map[j, i].N);
                var permutation = sp.Aggregate("", (s, n) => $"{s} {n}");
                Console.WriteLine(permutation);
                var bitmap = answer.Draw(image);
                bitmap.Save($"C:\\huaway\\final\\answr-{Path.GetFileName(imagefile)}");
            }
        }
    }
}
