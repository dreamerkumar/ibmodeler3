using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.ImgToObj.Test
{
    [TestFixture]
    public class MoldFileCreatorTest
    {
        private string _outputFilePath;

        [SetUp]
        public void SetUp()
        {
            _outputFilePath = ExecutionDirInfoHelper.CreateUniqueOutputPath();
        }

        //[Test]
        //public void TestCreateMoldFile()
        //{
        //    MoldFileCreator.CreateNewMoldFile(_outputFilePath + @"\testmold.mld", 100);
        //}

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
