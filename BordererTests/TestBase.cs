using Borderer;
using NUnit.Framework;

namespace BordererTests
{
    [TestFixture]
    public class TestBase
    {
        protected static string imageset = @"C:\huaway\data_train\64";
        protected static string imagesource = @"C:\huaway\data_train\64-sources";

        protected ImageParameters param = new ImageParameters(64);

        [SetUp]
        public virtual void SetUp()
        {

        }

        [TearDown]
        public virtual void TearDown()
        {

        }

    }
}