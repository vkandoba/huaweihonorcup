using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Borderer;
using Borderer.Estimator;
using NUnit.Framework;

namespace BordererTests
{
    [TestFixture]
    public class TestBase
    {
        private static string basedir = @"C:\huaway\data_train\";
        protected static string[] set16 = ToFileNames(16);
        protected static string[] set32 = ToFileNames(32);
        protected static string[] set64 = ToFileNames(64);

        protected static string imageset64 = @"C:\huaway\data_train\64";
        protected static string imageset32 = @"C:\huaway\data_train\32";
        protected static string imagesource64 = @"C:\huaway\data_train\64-sources";

        protected ImageParameters param = new ImageParameters(64);

        protected TrainImage ReadImage(string name, int p = 64)
        {
            var dir = Path.Combine(basedir, $"{p}");
            var sourcedir = Path.Combine(basedir, $"{p}-sources");

            return new TrainImage
            {
                Name = name,
                Image = new Bitmap(Path.Combine(dir, $"{name}.png")),
                Original = new Bitmap(Path.Combine(sourcedir, $"{name}.png")),
                Param = new ImageParameters(p),
            };
        }

        [SetUp]
        public virtual void SetUp()
        {

        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        private static string[] ToFileNames(int p)
        {
            return Directory.GetFiles(Path.Combine(basedir, $"{p}")).Select(Path.GetFileNameWithoutExtension).ToArray();
        }

        protected static IEstimator CreateEstimator() => new CacheEstimator(new RecursiveEstimator(new CacheEstimator(new Estimator())));
    }
}