using System;
using System.Diagnostics;
using System.Linq;
using Borderer.Helpers;
using Borderer.Squares;
using NUnit.Framework;

namespace BordererTests
{
    public class AlgothimTest : TestBase
    {

        private SquareBuilder builder;
        private string[] set;
        private int p;

        public override void SetUp()
        {
            base.SetUp();
            set = set64;
            p = 64;
        }

        [TestCase(0, 100)]
        [TestCase(0, 20)]
        [TestCase(1, 1)]
        [TestCase(5, 1)]
        public void TestRecursive(int skip, int take)
        {
            foreach (var name in set.Skip(skip).Take(take))
            {
                var estimator = CreateEstimator();
                var imgbuilder = new ImageBuilder(new SquareBuilder(estimator), estimator);
                var train = ReadImage(name, p);

                var slices = Slice.GenerateBaseSlices(p);

                var sw = new Stopwatch();
                sw.Start();
                var answer = imgbuilder.RecursiveCollect(train.Image, train.Param, slices, 4);
                var recovered = imgbuilder.RecoveDuplicate(train.Image, answer, p);
                sw.Stop();

                var map = recovered.Apply(p);
                var bitmap = recovered.Draw(train.Image);
                bitmap.Save($"C:\\huaway\\train\\answr-{name}.png");
                var permutation = map.ToPermutation().Aggregate("", (s, n) => $"{s} {n}");
                Console.WriteLine($"image: {name}\n time: {sw.Elapsed}\n" +
                                  $"{permutation}");
            }
        }
    }
}