using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Borderer.Estimator;
using Borderer.Helpers;
using Borderer.Squares;

namespace Borderer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var imageset = @"C:\huaway\data_train\64";
            int p = 64;
            int r = 1;
            var imageset = @"C:\huaway\data_test1_blank\" + p;
            var param = new ImageParameters(p);
            var dout = $"debug{p}{r}.txt";
            foreach (var imagefile in Directory.GetFiles(imageset))
            {
                var estimator = CreateEstimator();
                var builder = new ImageBuilder(new SquareBuilder(estimator), estimator);
                var slices = Slice.GenerateBaseSlices(p);
                var image = new Bitmap(imagefile);
                var sw = new Stopwatch();
                sw.Start();
                var answer = builder.RecursiveCollect(image, param, slices, 4);
                var recovered = builder.RecoveDuplicate(image, answer, p);
                sw.Stop();
                var f = estimator.DeepMeasureSquare(image, recovered);
                var map = recovered.Apply(p);
                File.AppendAllText(dout, $"image: {imagefile} f:{f} time:{sw.Elapsed}\n");
                var permutation = map.ToPermutation().Aggregate("", (s, n) => $"{s} {n}");
                Console.WriteLine(Path.GetFileName(imagefile));
                Console.WriteLine(permutation);
                //File.AppendAllText(fout, $"{Path.GetFileName(imagefile)}\n{permutation}\n");

                var bitmap = recovered.Draw(image);
                //bitmap.Save($"C:\\huaway\\final\\answr-{Path.GetFileName(imagefile)}");
            }
        }

        private static IEstimator CreateEstimator()
        {
            return new CacheEstimator(new RecursiveEstimator(new CacheEstimator(new Estimator.Estimator())));
        }
    }
}
