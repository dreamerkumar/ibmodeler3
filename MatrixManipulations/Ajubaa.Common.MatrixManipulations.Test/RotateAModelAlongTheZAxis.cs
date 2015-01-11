using System;
using System.IO;
using System.Linq;
using Ajubaa.Common.PolygonDataReaders;
using Ajubaa.Common.PolygonDataWriters;
using NUnit.Framework;

namespace Ajubaa.Common.MatrixManipulations.Test
{
    [TestFixture]
    public class RotateAModelAlongTheZAxis
    {
        [Test]
        public void TestRotationAlongTheZAxis()
        {
            var inputPts = new MdlFilePolygonDataReader(ExecutionDirInfoHelper.GetInputDirPath() + @"\1.mdl").Points;
            var outputDirPath = ExecutionDirInfoHelper.GetOutputDirPath() + @"\TestRotationAlongTheZAxis";
            if (!Directory.Exists(outputDirPath))
                Directory.CreateDirectory(outputDirPath);
            XamlWriter.WritePolygonsToXamlFile("", string.Format(@"{0}\Input.xaml", outputDirPath), inputPts, false);
            for (var angleInRadian = 0.0; angleInRadian <= Math.PI * 2; angleInRadian += Math.PI * 2.0 / 18.0)//every 20 degrees
            {
                var outputPts =RotateAlongXYOrZAxis.GetRotatedPtList(Axis.Z, angleInRadian, inputPts.ToArray());
                XamlWriter.WritePolygonsToXamlFile("", string.Format(@"{0}\Output_Z_{1}.xaml", outputDirPath, CommonFunctions.RadianToDegrees(angleInRadian,0)), outputPts, false);
            }
        }
    }
}
