using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using Point = System.Windows.Point;

namespace Ajubaa.TextureGenerator.Test
{
    [TestFixture]
    public class ImgWidthOptimizerTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void TestOptimizeTexImageWidthsAtYZero()
        {
            var unmergedCoods = new List<TexCoodAndImgIndex>
            {
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.0,0)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.1,0)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.2,1 )},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.3, 1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.4,1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.5,1)}
            };

            var imgSpecifics = new[] {new AddTexImageInfo{ImageBitmap = new Bitmap(100,100)}};

            var xLimitsAtYIndices = ImgWidthOptimizer.OptimizeTexImageWidths(unmergedCoods, imgSpecifics);

            Assert.IsNotNull(xLimitsAtYIndices);
            Assert.IsTrue(xLimitsAtYIndices.Length > 0);
            //var limitsAtYZero = xLimitsAtYIndices[0,0];
            //Assert.AreEqual( 0,limitsAtYZero.Min);
            //Assert.AreEqual( 50,limitsAtYZero.Width);

        }

        [Test]
        public void TestGetOrderedDistinctYValues()
        {
            //create a list which has unordered y values with some duplicates
            var texCoodAndImgIndices = new List<TexCoodAndImgIndex>
            {
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.0, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.1, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.2,  .5 )},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.3,  .2)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.4, .4)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.5, .3)}
            };
            var result = ImgWidthOptimizer.GetOrderedDistinctYs(texCoodAndImgIndices,0);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(.1, result[0]);
            Assert.AreEqual(.2, result[1]);
            Assert.AreEqual(.3, result[2]);
            Assert.AreEqual(.4, result[3]);
            Assert.AreEqual(.5, result[4]);
        }

        [Test]
        public void TestGetFilteredXCoodsForTwoYValues()
        {
            var texCoodAndImgIndices = new List<TexCoodAndImgIndex>
            {
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.0, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.1, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.15, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.23, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.34, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.35, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.23, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.343, .1)},

                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.2,  .5 )},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.3,  .2)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.4,  .4)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.5,  .3)}
            };
            var result = ImgWidthOptimizer.GetFilteredXCoodsForTwoYs(texCoodAndImgIndices, .1, .2,0);
            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.Length);
            Assert.AreEqual(.0, result[0]);
            Assert.AreEqual(.1, result[1]);
            Assert.AreEqual(.15, result[2]);
            Assert.AreEqual(.23, result[3]);
            Assert.AreEqual(.34, result[4]);
            Assert.AreEqual(.35, result[5]);
            Assert.AreEqual(.23, result[6]);
            Assert.AreEqual(.343, result[7]);
            Assert.AreEqual(.3, result[8]);
            Assert.AreEqual(.0, result.Min());
            Assert.AreEqual(.35, result.Max());
        }

        [Test]
        public void TestGetMinMaxXCoodsForTwoYValues()
        {
            var texCoodAndImgIndices = new List<TexCoodAndImgIndex>
            {
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.0, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.1, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.15, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.23, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.34, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.35, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.23, .1)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point( 0.343, .1)},

                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.2,  .5 )},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.3,  .2)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.4,  .4)},
                new TexCoodAndImgIndex {ImgIndex = 0, TexCood = new Point(0.5,  .3)}
            };
            var result = ImgWidthOptimizer.GetMinMaxXCoodsForTwoYs(texCoodAndImgIndices, .1, .2,0);
            Assert.IsNotNull(result);
            Assert.AreEqual(.0, result.Min);
            Assert.AreEqual(.35, result.Max);
        }

        [Test]
        public void TestGetPixelValueForTexCoodValue()
        {
            Assert.AreEqual(0,ImgWidthOptimizer.GetPixelForTexCood(0, 100));
            Assert.AreEqual(0, ImgWidthOptimizer.GetPixelForTexCood(0.001, 100));
            Assert.AreEqual(0, ImgWidthOptimizer.GetPixelForTexCood(0.002, 100));
            Assert.AreEqual(0, ImgWidthOptimizer.GetPixelForTexCood(0.003, 100));
            Assert.AreEqual(0, ImgWidthOptimizer.GetPixelForTexCood(0.004, 100));
            Assert.AreEqual(0, ImgWidthOptimizer.GetPixelForTexCood(0.005, 100));
            Assert.AreEqual(0, ImgWidthOptimizer.GetPixelForTexCood(0.006, 100));
            Assert.AreEqual(0, ImgWidthOptimizer.GetPixelForTexCood(0.007, 100));
            Assert.AreEqual(0, ImgWidthOptimizer.GetPixelForTexCood(0.008, 100));
            Assert.AreEqual(0, ImgWidthOptimizer.GetPixelForTexCood(0.009, 100));
            Assert.AreEqual(0, ImgWidthOptimizer.GetPixelForTexCood(0.01, 100));
            Assert.AreEqual(1, ImgWidthOptimizer.GetPixelForTexCood(0.011, 100));
            Assert.AreEqual(1, ImgWidthOptimizer.GetPixelForTexCood(0.012, 100));
            Assert.AreEqual(1, ImgWidthOptimizer.GetPixelForTexCood(0.013, 100));
            Assert.AreEqual(1, ImgWidthOptimizer.GetPixelForTexCood(0.014, 100));
            Assert.AreEqual(1, ImgWidthOptimizer.GetPixelForTexCood(0.015, 100));
            Assert.AreEqual(1, ImgWidthOptimizer.GetPixelForTexCood(0.016, 100));
            Assert.AreEqual(1, ImgWidthOptimizer.GetPixelForTexCood(0.017, 100));
            Assert.AreEqual(1, ImgWidthOptimizer.GetPixelForTexCood(0.018, 100));
            Assert.AreEqual(1, ImgWidthOptimizer.GetPixelForTexCood(0.019, 100));
            Assert.AreEqual(1, ImgWidthOptimizer.GetPixelForTexCood(0.02, 100));
            Assert.AreEqual(99, ImgWidthOptimizer.GetPixelForTexCood(1.0, 100));
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
