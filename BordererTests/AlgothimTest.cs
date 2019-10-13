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

        [TestCase(0, 20)]
        public void TestRecursive(int skip, int take)
        {
            foreach (var name in set.Skip(skip).Take(take))
            {
                var imgbuilder = new ImageBuilder(new SquareBuilder(CreateEstimator()));
                var train = ReadImage(name, p);

                var slices = Slice.GenerateBaseSlices(p);

                var sw = new Stopwatch();
                sw.Start();
                var answer = imgbuilder.RecursiveCollect(train.Image, train.Param, slices, 4);
                sw.Stop();

                var map = answer.Apply(p);
                var bitmap = answer.Draw(train.Image);
                bitmap.Save($"C:\\huaway\\train\\answr-{name}.png");
                var permutation = map.GetPermutation().Aggregate("", (s, n) => $"{s} {n}");
                Console.WriteLine($"image: {name}\n time: {sw.Elapsed}\n" +
                                  $"{permutation}");
            }
        }

//        [TestCase(0, 5)]
//        public void TestRecursivePrototype(int skip, int take)
//        {
//            foreach (var name in set.Skip(skip).Take(take))
//            {
//                builder = new SquareBuilder(CreateEstimator());
//                var train = ReadImage(name, p);
//
//                var slices = Slice.GenerateBaseSlices(p);
//
//                var sw = new Stopwatch();
//
//                sw.Start();
//
//                SquareAndMeasure[] squares = slices.Cast<Slice>().Select(slice => new SquareAndMeasure
//                {
//                    Square = slice,
//                    F = slice.Estimate(train.Image, null)
//                }).ToArray();
//                var size = param.P;
//                do
//                {
//                    squares = builder.BuildLikelySquares(train.Image, squares.Select(x => x.Square).ToArray(), 4)
//                        .Values
//                        .OrderBy(x => x.F)
//                        .ToArray();
//                    size = squares.Any() ? squares.First().Square.Size : ImageParameters.__totalSize;
//                } while (size < ImageParameters.__totalSize);
//                sw.Stop();
//
//                var answer = squares.Any() ? squares.First().Square : SquareBuilder.MakeSquare(slices);
//                var map = answer.Apply(p);
//                var bitmap = answer.Draw(train.Image);
//                bitmap.Save($"C:\\huaway\\train\\answr-{name}.png");
//                var permutation = map.GetPermutation().Aggregate("", (s, n) => $"{s} {n}");
//                Console.WriteLine($"image: {name}\n time: {sw.Elapsed}\n" +
//                                  $"{permutation}");
//            }
//        }
//
//        [TestCase(0, 5)]
//        public void TestPrototype1(int skip, int take)
//        {
//            foreach (var name in set.Skip(skip).Take(take))
//            {
//                builder = new SquareBuilder(CreateEstimator());
//                var train = ReadImage(name, p);
//                var slices = Slice.GenerateBaseSlices(p);
//
//                var sw = new Stopwatch();
//
//                sw.Start();
//
//                var rest = slices.Cast<Slice>().ToList();
//                var first = rest.First();
//                rest.Remove(first);
//                var squares1 = builder.BuildLikelySquare(train.Image, first, rest.ToArray(), 4).Square as Square;
//
//                IList<Square> next = new List<Square>();
//                for (int i = 0; i < squares1.Parts.Length; i++)
//                {
//                    var item = builder.BuildLikelySquare(train.Image, squares1.Parts[i], rest.ToArray(), 4).Square as Square;
//                    next.Add(item);
//                    foreach (var itemPart in item.Parts)
//                        rest.Remove(itemPart as Slice);
//                }
//                var squares2 = new Square(next.ToArray());
//
//                var bitmap = squares2.Draw(train.Image);
//                bitmap.Save($"C:\\huaway\\tests\\answr-{train.Image}.png");
//                Console.WriteLine($"image: {name}\n time: {sw.Elapsed}\n");
//            }
//        }
    }
}