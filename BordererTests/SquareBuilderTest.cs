using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Borderer;
using Borderer.Estimator;
using Borderer.Helpers;
using Borderer.Squares;
using NUnit.Framework;

namespace BordererTests
{
    public class SquareBuilderTest : TestBase
    {
        private SquareBuilder builder;
        private string[] set;
        private int p;

        public override void SetUp()
        {
            base.SetUp();
            set = set32;
            p = 32;
        }

        [TestCase(0, 5)]
        [TestCase(0, 100)]
        [TestCase(100, 200)]
        [TestCase(200, 300)]
        [TestCase(300, 400)]
        [TestCase(400, 500)]
        [TestCase(500, 600)]
        public void TestGetSquares(int skip, int take)
        {
            foreach (var name in set.Skip(skip).Take(take))
            {
                builder = new SquareBuilder(CreateEstimator());
                var train = ReadImage(name, p);

                var slices = Slice.GenerateBaseSlices(p);

                var sw = new Stopwatch();
                sw.Start();

                var array = slices.Cast<Slice>().ToArray();
                var squares = builder.BuildLikelySquares(train.Image, array, 4);

                sw.Stop();

                Console.WriteLine($"image: {name}\n time: {sw.Elapsed}\n");

                var f = new double[train.Param.M, train.Param.M];
                for (int i = 0; i < train.Param.M; i++)
                {
                    for (int j = 0; j < train.Param.M; j++)
                    {
                        f[i, j] = squares[slices[i, j]].F;
                    }
                }

                Console.WriteLine($"f:\n{f.Print(d => $"{d:N2}\t")}");
                Console.WriteLine();
            }
        }


    }
}