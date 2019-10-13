﻿using System;
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
            int p = 32;
            var imageset = @"C:\huaway\data_test1_blank\32";
            var param = new ImageParameters(p);
            foreach (var imagefile in Directory.GetFiles(imageset).Take(10))
            {
                var estimator = CreateEstimator();
                var builder = new ImageBuilder(new SquareBuilder(estimator));
                var slices = Slice.GenerateBaseSlices(p);
                var image = new Bitmap(imagefile);
                var answer = builder.RecursiveCollect(image, param, slices, 4);
                var map = answer.Apply(p);
                var permutation = map.GetPermutation().Aggregate("", (s, n) => $"{s} {n}");
                Console.WriteLine(Path.GetFileName(imagefile));
                Console.WriteLine(permutation);

                var bitmap = answer.Draw(image);
                bitmap.Save($"C:\\huaway\\final\\answr-{Path.GetFileName(imagefile)}");
            }
        }

        private static IEstimator CreateEstimator()
        {
            return new RecursiveEstimator(new CacheEstimator(new Estimator.Estimator()));
        }
    }
}
