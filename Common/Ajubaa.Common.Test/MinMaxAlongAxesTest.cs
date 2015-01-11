using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Ajubaa.Common.Test
{
    [TestFixture]
    public class MinMaxAlongAxesTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void Sets_Valid_Min_Max_Values_For_Triangles()
        {
            var triangles = new List<Triangle>();
            
            //get a sample of thousand triangles
            const int decimalPlaces = Constants.DecimalPlacesToCheck;
            var r = new Random(1);
            for (var ctr = 1; ctr <= 1000; ctr++)
            {
                var v1 = new Point3D(Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces));
                var v2 = new Point3D(Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces));
                var v3 = new Point3D(Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces));
                triangles.Add(new Triangle(v1, v2, v3));
            }
            
            var test = new MinMaxAlongAxes(triangles, null);

            //now test that min max values are valid
            var noOfTests = 0;
            foreach(var t in triangles)
            {
                Assert.IsFalse(t.V1.X < test.MinX);
                Assert.IsFalse(t.V1.X > test.MaxX);
                Assert.IsFalse(t.V1.Y < test.MinY);
                Assert.IsFalse(t.V1.Y > test.MaxY);
                Assert.IsFalse(t.V1.Z < test.MinZ);
                Assert.IsFalse(t.V1.Z > test.MaxZ);

                Assert.IsFalse(t.V2.X < test.MinX);
                Assert.IsFalse(t.V2.X > test.MaxX);
                Assert.IsFalse(t.V2.Y < test.MinY);
                Assert.IsFalse(t.V2.Y > test.MaxY);
                Assert.IsFalse(t.V2.Z < test.MinZ);
                Assert.IsFalse(t.V2.Z > test.MaxZ);

                Assert.IsFalse(t.V3.X < test.MinX);
                Assert.IsFalse(t.V3.X > test.MaxX);
                Assert.IsFalse(t.V3.Y < test.MinY);
                Assert.IsFalse(t.V3.Y > test.MaxY);
                Assert.IsFalse(t.V3.Z < test.MinZ);
                Assert.IsFalse(t.V3.Z > test.MaxZ);

                noOfTests++;
            }
            Assert.AreEqual(1000, noOfTests);
        }

        [Test]
        public void Sets_Valid_Min_Max_Values_For_Points()
        {
            var pts = new List<Point3D>();

            //get a sample of thousand points
            const int decimalPlaces = Constants.DecimalPlacesToCheck;
            var r = new Random(1);
            for (var ctr = 1; ctr <= 1000; ctr++)
                pts.Add(new Point3D(Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces)));

            var test = new MinMaxAlongAxes(null, pts);

            //now test that min max values are valid
            var noOfTests = 0;
            foreach (var pt in pts)
            {
                Assert.IsFalse(pt.X < test.MinX);
                Assert.IsFalse(pt.X > test.MaxX);
                Assert.IsFalse(pt.Y < test.MinY);
                Assert.IsFalse(pt.Y > test.MaxY);
                Assert.IsFalse(pt.Z < test.MinZ);
                Assert.IsFalse(pt.Z > test.MaxZ);
                noOfTests++;
            }
            Assert.AreEqual(1000, noOfTests);
        }

        [Test]
        public void Sets_Valid_Min_Max_Values_For_Triangles_And_Points()
        {
            var triangles = new List<Triangle>();
            
            //get a sample of thousand triangles
            const int decimalPlaces = Constants.DecimalPlacesToCheck;
            var r = new Random(1);
            for (var ctr = 1; ctr <= 1000; ctr++)
            {
                var v1 = new Point3D(Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces));
                var v2 = new Point3D(Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces));
                var v3 = new Point3D(Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces));
                triangles.Add(new Triangle(v1, v2, v3));
            }
            
            var pts = new List<Point3D>();
            //get a sample of thousand points
            for (var ctr = 1; ctr <= 1000; ctr++)
                pts.Add(new Point3D(Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces), Math.Round(r.NextDouble(), decimalPlaces)));
            
            var test = new MinMaxAlongAxes(triangles, pts);

            //test that min max values are valid for each triangle
            var noOfTests = 0;
            foreach(var t in triangles)
            {
                Assert.IsFalse(t.V1.X < test.MinX);
                Assert.IsFalse(t.V1.X > test.MaxX);
                Assert.IsFalse(t.V1.Y < test.MinY);
                Assert.IsFalse(t.V1.Y > test.MaxY);
                Assert.IsFalse(t.V1.Z < test.MinZ);
                Assert.IsFalse(t.V1.Z > test.MaxZ);

                Assert.IsFalse(t.V2.X < test.MinX);
                Assert.IsFalse(t.V2.X > test.MaxX);
                Assert.IsFalse(t.V2.Y < test.MinY);
                Assert.IsFalse(t.V2.Y > test.MaxY);
                Assert.IsFalse(t.V2.Z < test.MinZ);
                Assert.IsFalse(t.V2.Z > test.MaxZ);

                Assert.IsFalse(t.V3.X < test.MinX);
                Assert.IsFalse(t.V3.X > test.MaxX);
                Assert.IsFalse(t.V3.Y < test.MinY);
                Assert.IsFalse(t.V3.Y > test.MaxY);
                Assert.IsFalse(t.V3.Z < test.MinZ);
                Assert.IsFalse(t.V3.Z > test.MaxZ);

                noOfTests++;
            }
            Assert.AreEqual(1000, noOfTests);

            //test that min max values are valid for each point
            noOfTests = 0;
            foreach (var pt in pts)
            {
                Assert.IsFalse(pt.X < test.MinX);
                Assert.IsFalse(pt.X > test.MaxX);
                Assert.IsFalse(pt.Y < test.MinY);
                Assert.IsFalse(pt.Y > test.MaxY);
                Assert.IsFalse(pt.Z < test.MinZ);
                Assert.IsFalse(pt.Z > test.MaxZ);
                noOfTests++;
            }
            Assert.AreEqual(1000, noOfTests);
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
