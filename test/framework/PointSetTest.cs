using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;

using NUnit.Framework;

namespace Pavel.Test.Framework {

    [TestFixture]
    public class PointSetTest {

        Column[] cols = new Column[] { new Column(), new Column(), new Column(), new Column(), new Column(), new Column() };
        double[][] values = new double[][] {
                    new double[] {  -3644   , 20409   ,  -6960   ,  -9972   ,  8373   , -24562 },
                    new double[] {  21735   , 31763   ,   6737   , -10842   , -4782   ,   9647 },
                    new double[] {  26979   ,  5965   ,  25687   , -19656   , -2700   , -20616 },
                    new double[] {  22321   , 22305   , -30492   , -26062   , 16477   , -10744 },
                    new double[] { -18074   , -5244   ,  -7068   ,  24693   , -1270   , -18856 }
                };
        ColumnSet columnSet;

        public PointSetTest() {
            columnSet = new ColumnSet(cols);
        }

        public PointSet ComplicatedPointSet {
            get {
                // -3644   , 20409   ,  -6960   ,  -9972   ,  8373   , -24562
                // 21735   , 31763   ,   6737   , -10842   , -4782   ,   9647
                // 26979   ,  5965   ,  25687   , -19656   , -2700   , -20616
                // 22321   , 22305   , -30492   , -26062   , 16477   , -10744
                //-18074   , -5244   ,  -7068   ,  24693   , -1270   , -18856
                //  9863.4 , 15039.6 ,  -2419.2 ,  -8367.8 ,  3219.6 , -13026.2 // Mean
                

                Point[] points = new Point[] {
                    new Point(columnSet, values[0]),
                    new Point(columnSet, values[1]),
                    new Point(columnSet, values[2]),
                    new Point(columnSet, values[3]),
                    new Point(columnSet, values[4]),
                };

                PointSet ps = new PointSet("ComplicatedPointSet", columnSet);
                ps.AddRange(points);
                return ps;
            }
        }

        [Test]
        public void TestEnumerator() {
            PointSet  ps = ComplicatedPointSet;

            int i = 0; foreach (Point p in ps) {
                for (int j = 0; j < p.ColumnSet.Dimension; j++) {
                    Assert.AreEqual(values[i%5][j], p[j]);
                }
                i++;
            }
        }

        [Test]
        public void TestRename( ) {
            PointSet ps = new PointSet("Test", columnSet);
            Assert.AreEqual(ps.Label, "Test");
            ps.Label = "Toll";
            Assert.AreEqual(ps.Label, "Toll");
        }

        [Test]
        public void TestIndexer() {
            PointSet  ps = ComplicatedPointSet;
            List<Point> pointsCopy = new List<Point>();
            for ( int i = 0; i < ps.Length; i++ ) {
                pointsCopy.Add(ps[i]);
            }
            ps.AddRange(pointsCopy);

            Assert.AreEqual(values[3], ps[3].Values);
            Assert.AreEqual(values[3], ps[8].Values);

            Assert.AreEqual(values[0], ps[0].Values);
            Assert.AreEqual(ps[9].Values, ps[ps.Length - 1].Values);
            Assert.AreEqual(values[4], ps[ps.Length-1].Values);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIndexerOutOfBound() {
            ComplicatedPointSet[100].ToString();
        }

        [Test]
        public void TestMinMaxMean() {
            ColumnSet sp1 = new ColumnSet(new Column(), new Column());

            Point p0 = new Point(sp1, 1,  10);
            Point p1 = new Point(sp1, 3, -20);
            Point p2 = new Point(sp1, 5,   4);

            PointSet ps = new PointSet("Test", sp1);
            ps.Add(p0);
            ps.Add(p1);
            ps.Add(p2);

            Assert.AreEqual(3, ps.Length);

            Point[] minMaxMean = ps.MinMaxMean();

            Assert.IsTrue(minMaxMean[Result.MIN].ColumnSet == ps.ColumnSet);

            Assert.AreEqual(new double[] { 1, -20 }, minMaxMean[Result.MIN] .Values);
            Assert.AreEqual(new double[] { 5,  10 }, minMaxMean[Result.MAX] .Values);
            Assert.AreEqual(new double[] { 3,  -2 }, minMaxMean[Result.MEAN].Values);
        }

        [Test]
        public void TestRemove() {
            ColumnSet sp1 = new ColumnSet(new Column(), new Column());

            Point p0 = new Point(sp1, 1, 10);
            Point p1 = new Point(sp1, 3, -20);
            Point p2 = new Point(sp1, 5, -10);

            PointSet ps1 = new PointSet("Test", sp1);
            ps1.Add(p0);
            ps1.Add(p1);
            ps1.Add(p2);

            Assert.AreEqual(3, ps1.Length);
            ps1.Remove(p1);
            Assert.AreEqual(2, ps1.Length);
            Assert.AreEqual(p0, ps1[0]);
            Assert.AreEqual(p2, ps1[1]);
            ps1.RemoveAt(0);
            Assert.AreEqual(1, ps1.Length);
            Assert.AreEqual(p2, ps1[0]);
        }

        [Test]
        public void TestMinMaxMeanNaN() {
            ColumnSet    sp = new ColumnSet(new Column());
            PointSet ps = new PointSet("",sp);
            Point[] minMaxMean = ps.MinMaxMean();
            Assert.AreEqual(Double.PositiveInfinity, minMaxMean[Result.MIN] [0]);
            Assert.AreEqual(Double.NegativeInfinity, minMaxMean[Result.MAX] [0]);
            Assert.AreEqual(Double.NaN             , minMaxMean[Result.MEAN][0]);
        }

        [Test]
        public void DeletedAndUndoTest() {
            PointSet ps = this.ComplicatedPointSet;
            Point p0 = ps[0];
            Point p1 = ps[1];
            Point p2 = ps[2];
            Point p3 = ps[3];
            Point p4 = ps[4];
            Assert.AreEqual(ps.Length, 5);
            ProjectController.CurrentSelection.Add(p3);
            Assert.AreEqual(ps.UndoSteps, 0);
            ps.DeleteSelectedPoints();
            Assert.AreEqual(ps.UndoSteps, 1);
            Assert.AreEqual(ps.Length, 4);
            ProjectController.CurrentSelection.Add(p0);
            ProjectController.CurrentSelection.Add(p1);
            ps.DeleteSelectedPoints();
            Assert.AreEqual(ps.UndoSteps, 2);
            Assert.AreEqual(ps.Length, 2);
            ps.Undo();
            Assert.AreEqual(ps.UndoSteps, 1);
            Assert.AreEqual(ps.Length, 4);
            ps.Undo();
            Assert.AreEqual(ps.UndoSteps, 0);
            Assert.AreEqual(ps.Length, 5);
        }

    }
}