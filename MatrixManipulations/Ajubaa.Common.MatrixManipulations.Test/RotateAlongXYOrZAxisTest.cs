using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using Ajubaa.Common.PolygonDataWriters;
using NUnit.Framework;

namespace Ajubaa.Common.MatrixManipulations.Test
{
    [TestFixture]
    public class RotateAlongXyorZAxisTest
    {
        private Point3D[] _ptsForACube;

        [SetUp]
        public void SetUp()
        {
           
            _ptsForACube = new[]
                                   {
                                       //bottom face
                                       new Point3D(1,0,1),
                                       new Point3D(0,0,1),
                                       new Point3D(0,0,0),
                                       new Point3D(0,0,1),
                                       new Point3D(1,0,1),
                                       new Point3D(0,0,0),

                                       //top face
                                       new Point3D(0,1,1),
                                       new Point3D(1,1,1),
                                       new Point3D(0,1,0),
                                       new Point3D(1,1,1),
                                       new Point3D(0,1,1),
                                       new Point3D(0,1,0),

                                       //back face
                                       new Point3D(0,1,0),
                                       new Point3D(1,0,0),
                                       new Point3D(0,0,0),
                                       new Point3D(0,1,0),
                                       new Point3D(1,1,0),
                                       new Point3D(1,0,0),

                                       //front face
                                       new Point3D(1,0,1),
                                       new Point3D(0,1,1),
                                       new Point3D(0,0,1),
                                       new Point3D(1,1,1),
                                       new Point3D(0,1,1),
                                       new Point3D(1,0,1),

                                       //left face
                                       new Point3D(0,0,0),
                                       new Point3D(0,0,1),
                                       new Point3D(0,1,0),
                                       new Point3D(0,1,0),
                                       new Point3D(0,0,1),
                                       new Point3D(0,1,1),

                                       //right face
                                       new Point3D(1,0,1),
                                       new Point3D(1,0,0),
                                       new Point3D(1,1,0),
                                       new Point3D(1,0,1),
                                       new Point3D(1,1,0),
                                       new Point3D(1,1,1),
                                   };
        }

        [Test]
        public void Create_Some_Rotated_Pts_Along_Z_Axis()
        {
            //translate to +y
            for (var ctr = 0; ctr < _ptsForACube.Count(); ctr++)
            {
                _ptsForACube[ctr].Y += 5.0;
            }
            var ptList = new List<Point3D>();
            for (var angle = 0.0; angle <= Math.PI; angle+= Math.PI/10)
            {
                Point3D[] rotatedPts = RotateAlongXYOrZAxis.GetRotatedPts(Axis.Z, angle, _ptsForACube);
                ptList.AddRange(rotatedPts);
            }
            XamlWriter.WritePolygonsToXamlFile(
                GetTemplatePath(),
                ExecutionDirInfoHelper.GetOutputDirPath() + @"\testRotationAlongZAxis.xaml",
                ptList,
                false);

        }
        private string GetTemplatePath()
        {
            return ExecutionDirInfoHelper.GetExecutionPath() + @"\Xaml\ModelTemplate.xaml";
        }
        [Test]
        public void Create_Some_Rotated_Pts_Along_X_Axis()
        {
            //translate to +y
            for (var ctr = 0; ctr < _ptsForACube.Count(); ctr++)
            {
                _ptsForACube[ctr].Y += 5.0;
            }
            var ptList = new List<Point3D>();
            for (var angle = 0.0; angle <= Math.PI; angle += Math.PI / 10)
            {
                Point3D[] rotatedPts = RotateAlongXYOrZAxis.GetRotatedPts(Axis.X, angle, _ptsForACube);
                ptList.AddRange(rotatedPts);
            }
            XamlWriter.WritePolygonsToXamlFile(
                 GetTemplatePath(),
                 ExecutionDirInfoHelper.GetOutputDirPath() + @"\testRotationAlongXAxis.xaml",
                ptList,
                false
                );

        }
        [Test]
        public void Create_Some_Rotated_Pts_Along_Y_Axis()
        {
            //translate to +z
            for (var ctr = 0; ctr < _ptsForACube.Count(); ctr++)
            {
                _ptsForACube[ctr].Z += 5.0;
            }
            var ptList = new List<Point3D>();
            for (var angle = 0.0; angle <= Math.PI; angle += Math.PI / 10)
            {
                Point3D[] rotatedPts = RotateAlongXYOrZAxis.GetRotatedPts(Axis.Y, angle, _ptsForACube);
                ptList.AddRange(rotatedPts);
            }
            XamlWriter.WritePolygonsToXamlFile(
               GetTemplatePath(),
                 ExecutionDirInfoHelper.GetOutputDirPath() + @"\testRotationAlongYAxis.xaml",
                ptList,
                false
                );

        }

        [TearDown]
        public void TearDown()
        {
            _ptsForACube = null;
        }
    }
}
