using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.TextureGenerator.Test
{
    [TestFixture]
    public class IndexRangeProcessorTest
    {
        private string _inputPath;
        private Bitmap _img;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _inputPath = ExecutionDirInfoHelper.GetInputDirPath();
            var imgPath = _inputPath + @"\testIndex.bmp";
            _img = (Bitmap) Image.FromFile(imgPath);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _inputPath = null;
            _img = null;
        }

        #region left to right index range tests
        [Test]
        public void Test1()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(0, _img, 99, validInvalidFlag: false);
            Assert.AreEqual(1, indexRange.MinIndex);
            Assert.AreEqual(99, indexRange.MaxIndex);
        }

        [Test]
        public void Test2()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(0, _img, 99, validInvalidFlag: true);
            Assert.IsNull(indexRange);
        }

        [Test]
        public void Test3()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(0, _img, 0, validInvalidFlag: false);
            Assert.AreEqual(67, indexRange.MinIndex);
            Assert.AreEqual(71, indexRange.MaxIndex);
        }

        [Test]
        public void Test4()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(71, _img, 0, validInvalidFlag: false);
            Assert.AreEqual(83, indexRange.MinIndex);
            Assert.AreEqual(92, indexRange.MaxIndex);
        }

        [Test]
        public void Test5()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(67, _img, 0, validInvalidFlag: false);
            Assert.AreEqual(68, indexRange.MinIndex);
            Assert.AreEqual(71, indexRange.MaxIndex);
        }

        [Test]
        public void Test6()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(0, _img, 1, validInvalidFlag: true);
            Assert.AreEqual(58, indexRange.MinIndex);
            Assert.AreEqual(66, indexRange.MaxIndex);
        }

        [Test]
        public void Test7()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(66, _img, 1, validInvalidFlag: true);
            Assert.AreEqual(83, indexRange.MinIndex);
            Assert.AreEqual(95, indexRange.MaxIndex);
        }

        [Test]
        public void Test8()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(60, _img, 1, validInvalidFlag: true);
            Assert.AreEqual(61, indexRange.MinIndex);
            Assert.AreEqual(66, indexRange.MaxIndex);
        }

        [Test]
        public void Test9()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(99, _img, 1, validInvalidFlag: true);
            Assert.IsNull(indexRange);
        }

        [Test]
        public void Test10()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(98, _img, 1, validInvalidFlag: false);
            Assert.AreEqual(99, indexRange.MinIndex);
            Assert.AreEqual(99, indexRange.MaxIndex);
        }

        [Test]
        public void Test11()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(1000, _img, 1, validInvalidFlag: false);
            Assert.IsNull(indexRange);
        }
        #endregion

        #region right to left index range test
        [Test]
        public void Test12()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromRight(100, _img, 99, validInvalidFlag: false);
            Assert.AreEqual(0, indexRange.MinIndex);
            Assert.AreEqual(99, indexRange.MaxIndex);
        }

        [Test]
        public void Test13()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromRight(0, _img, 99, validInvalidFlag: false);
            Assert.IsNull(indexRange);
        }

        [Test]
        public void Test14()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromRight(72, _img, 0, validInvalidFlag: false);
            Assert.AreEqual(67, indexRange.MinIndex);
            Assert.AreEqual(71, indexRange.MaxIndex);
        }

        [Test]
        public void Test15()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromRight(93, _img, 0, validInvalidFlag: false);
            Assert.AreEqual(83, indexRange.MinIndex);
            Assert.AreEqual(92, indexRange.MaxIndex);
        }

        [Test]
        public void Test16()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromRight(71, _img, 0, validInvalidFlag: false);
            Assert.AreEqual(67, indexRange.MinIndex);
            Assert.AreEqual(70, indexRange.MaxIndex);
        }

        [Test]
        public void Test17()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromRight(83, _img, 1, validInvalidFlag: true);
            Assert.AreEqual(58, indexRange.MinIndex);
            Assert.AreEqual(66, indexRange.MaxIndex);
        }

        [Test]
        public void Test18()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromRight(100, _img, 1, validInvalidFlag: true);
            Assert.AreEqual(83, indexRange.MinIndex);
            Assert.AreEqual(95, indexRange.MaxIndex);
        }

        [Test]
        public void Test19()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromRight(65, _img, 1, validInvalidFlag: true);
            Assert.AreEqual(58, indexRange.MinIndex);
            Assert.AreEqual(64, indexRange.MaxIndex);
        }

        [Test]
        public void Test20()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromRight(0, _img, 1, validInvalidFlag: true);
            Assert.IsNull(indexRange);
        }

        [Test]
        public void Test21()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromRight(1, _img, 1, validInvalidFlag: false);
            Assert.AreEqual(0, indexRange.MinIndex);
            Assert.AreEqual(0, indexRange.MaxIndex);
        }

        [Test]
        public void Test22()
        {
            var indexRange = IndexRangeProcessor.GetNextIndexRangeFromRight(1000, _img, 1, validInvalidFlag: false);
            Assert.IsNull(indexRange);
        }
        #endregion
    }
}
