using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;

using NUnit.Framework;

namespace Pavel.Test.Framework {
    [TestFixture]
    public class PointListTest {

        static Column[]  cols = new Column[] { new Column(), new Column(), new Column(), new Column(), new Column(), new Column() };
        static Point[]   points;
        static ColumnSet     columnSet;
        static PointList pointList;

        [SetUp]
        public void SetUp() {
            /*
              -3644   , 20409   ,  -6960   ,  -9972   ,  8373   , -24562
              21735   , 31763   ,   6737   , -10842   , -4782   ,   9647
              26979   ,  5965   ,  25687   , -19656   , -2700   , -20616
              22321   , 22305   , -30492   , -26062   , 16477   , -10744
             -18074   , -5244   ,  -7068   ,  24693   , -1270   , -18856
               9863.4 , 15039.6 ,  -2419.2 ,  -8367.8 ,  3219.6 , -13026.2 // Mean
             */

            columnSet = new ColumnSet(cols);

            points = new Point[] {
                    new Point(columnSet,  -3644   , 20409   ,  -6960   ,  -9972   ,  8373   , -24562),
                    new Point(columnSet,  21735   , 31763   ,   6737   , -10842   , -4782   ,   9647),
                    new Point(columnSet,  26979   ,  5965   ,  25687   , -19656   , -2700   , -20616),
                    new Point(columnSet,  22321   , 22305   , -30492   , -26062   , 16477   , -10744),
                    new Point(columnSet, -18074   , -5244   ,  -7068   ,  24693   , -1270   , -18856),
                };

            pointList = new PointList(columnSet, points);
        }

        [TearDown]
        public void TearDown() {
            pointList = null;
            columnSet     = null;
        }

        [Test]
        public void TestCount() {
            Assert.AreEqual(5, pointList.Count);
            pointList.Add(new Point(columnSet, -3644   , 20409   ,  -6960   ,  -9972   ,  8373   , -24562));
            Assert.AreEqual(6, pointList.Count);
        }

        [Test]
        public void TestConstructor() {
            PointList pl;
            try {
                pl = new PointList(null, null);
                Assert.Fail();
            } catch (ArgumentNullException e) {
                Assert.AreEqual("columnSet", e.ParamName);
            }

            try {
                pl = new PointList(columnSet, null);
                Assert.Fail();
            } catch (ArgumentNullException e) {
                Assert.AreEqual("points", e.ParamName);
            }

            pl = new PointList(columnSet, pointList);
        }

        [Test]
        public void TestAddValid() {
            pointList.Add(new Point(columnSet, -3644   , 20409   ,  -6960   ,  -9972   ,  8373   , -24562));
            Assert.AreEqual(6, pointList.Count);
        }

        [Test, ExpectedException(typeof(ArgumentException), "point.ColumnSet != Pointlist.ColumnSet")]
        public void TestAddInvalid() {
            ColumnSet invalidColumnSet = ColumnSet.Union(columnSet, new ColumnSet(new Column()));
            pointList.Add(new Point(invalidColumnSet, 1,2,3,4,5,6,7));
        }

        [Test]
        public void TestAddRange() {
            pointList.AddRange(points);
            Assert.AreEqual(10, pointList.Count);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestAddRangeEnumInvalid() {
            Point[] invalidPoints = new Point[] { new Point(new ColumnSet(new Column()), 5) };
            pointList.AddRange(invalidPoints);
            Assert.AreEqual(5, pointList.Count);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestAddRangePointListInvalid() {
            ColumnSet invalidColumnSet = new ColumnSet(new Column());
            PointList invalidPoints = new PointList(invalidColumnSet, new Point[] { new Point(invalidColumnSet, 5) });
            pointList.AddRange(invalidPoints);
            Assert.AreEqual(5, pointList.Count);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestAddRangeNull() {
            pointList.AddRange(null);
        }

        [Test]
        public void TestRemove() {
            pointList.Remove(points[2]);
            pointList.Remove(points[0]);
            Assert.AreEqual(3, pointList.Count);
            Assert.AreEqual(pointList[0], points[1]);
            Assert.AreEqual(pointList[1], points[3]);
            Assert.AreEqual(pointList[2], points[4]);
        }

        [Test]
        public void TestRemoveAt() {
            pointList.RemoveAt(2);
            pointList.RemoveAt(0);
            Assert.AreEqual(3, pointList.Count);
            Assert.AreEqual(pointList[0], points[1]);
            Assert.AreEqual(pointList[1], points[3]);
            Assert.AreEqual(pointList[2], points[4]);
        }

        [Test]
        public void TestRemoveRange() {
            pointList.RemoveRange(new Point[] { points[0], points[2] });
            Assert.AreEqual(3, pointList.Count);
            Assert.AreEqual(pointList[0], points[1]);
            Assert.AreEqual(pointList[1], points[3]);
            Assert.AreEqual(pointList[2], points[4]);
        }

        [Test]
        public void TestRemoveAtRange() {
            pointList.RemoveAtRange(new int[] { 2, 1, 4 });
            Assert.AreEqual(2, pointList.Count);
            Assert.AreEqual(pointList[0], points[0]);
            Assert.AreEqual(pointList[1], points[3]);
        }

        [Test]
        public void TestMinMaxMean() {
            Point[] minMaxMean = pointList.MinMaxMean();
            Point min  = new Point(columnSet, new double[] { -18074, -5244, -30492, -26062, -4782, -24562 });
            Point max  = new Point(columnSet, new double[] { 26979, 31763, 25687, 24693, 16477, 9647 });
            Point mean = new Point(columnSet, new double[] { 9863.4, 15039.6, -2419.2, -8367.8, 3219.6, -13026.2 });
            Assert.IsTrue(minMaxMean[Result.MIN].Equals(min));
            Assert.IsTrue(minMaxMean[Result.MAX].Equals(max));
            Assert.IsTrue(minMaxMean[Result.MEAN].Equals(mean));
            //TODO: zusehen dass die auch nach Änderung noch stimmen
        }
    }
}
