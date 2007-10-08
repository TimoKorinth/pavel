using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;

using NUnit.Framework;

namespace Pavel.Test.Framework {
    [TestFixture]
    public class SelectionTest {
        ColumnSet columnSet = new ColumnSet(new Column(), new Column(), new Column(), new Column(), new Column(), new Column());

        [Test]
        public void TestAdd() {
            Selection selection = new Selection();
            Point p1 = new Point(columnSet, new double[] { 1, 2, 3, 4, 5, 6 });
            Point p2 = new Point(columnSet, new double[] { 7, 8, 9, 10, 11, 12 });

            selection.Add(p1);
            selection.Add(p2);
            Assert.AreEqual(selection.Length, 2);
            selection.Add(p1);
            Assert.AreEqual(selection.Length, 2);
        }

        [Test]
        public void TestAddRange() {
            Selection selection = new Selection();
            Point p1 = new Point(columnSet, new double[] { 1, 2, 3, 4, 5, 6 });
            Point p2 = new Point(columnSet, new double[] { 7, 8, 9, 10, 11, 12 });
            Point p3 = new Point(columnSet, new double[] { 0, 0, 0, 1, 1, 1 });
            List<Point> points = new List<Point>();
            points.Add(p1);
            points.Add(p2);
            selection.AddRange(points);
            Assert.AreEqual(selection.Length, 2);
            points.Add(p3);
            selection.AddRange(points);
            Assert.AreEqual(selection.Length, 3);
        }

        [Test]
        public void TestRemove() {
            Selection selection = new Selection();
            Point p1 = new Point(columnSet, new double[] { 1, 2, 3, 4, 5, 6 });
            Point p2 = new Point(columnSet, new double[] { 7, 8, 9, 10, 11, 12 });
            Point p3 = new Point(columnSet, new double[] { 0, 0, 0, 1, 1, 1 });
            List<Point> points = new List<Point>();
            points.Add(p1);
            points.Add(p2);
            selection.AddRange(points);
            Assert.AreEqual(selection.Length, 2);
            selection.Remove(p1);
            Assert.AreEqual(selection.Length, 1);
            selection.Remove(p3);
            Assert.AreEqual(selection.Length, 1);
        }

        [Test]
        public void TestRemoveRange() {
            Selection selection = new Selection();
            Point p1 = new Point(columnSet, new double[] { 1, 2, 3, 4, 5, 6 });
            Point p2 = new Point(columnSet, new double[] { 7, 8, 9, 10, 11, 12 });
            Point p3 = new Point(columnSet, new double[] { 0, 0, 0, 1, 1, 1 });
            List<Point> points = new List<Point>();
            points.Add(p1);
            points.Add(p2);
            selection.AddRange(points);
            Assert.AreEqual(selection.Length, 2);
            selection.Add(p3);
            Assert.AreEqual(selection.Length, 3);
            selection.RemovePoints(points);
            Assert.AreEqual(selection.Length, 1);
        }

        [Test]
        public void TestContains() {
            Selection selection = new Selection();
            Point p1 = new Point(columnSet, new double[] { 1, 2, 3, 4, 5, 6 });
            Point p2 = new Point(columnSet, new double[] { 7, 8, 9, 10, 11, 12 });
            Point p3 = new Point(columnSet, new double[] { 0, 0, 0, 1, 1, 1 });
            Assert.IsFalse(selection.Contains(p1));
            selection.Add(p1);
            Assert.IsTrue(selection.Contains(p1));
            selection.Add(p2);
            selection.Add(p3);
            Assert.IsTrue(selection.Contains(p3));
            selection.Remove(p3);
            Assert.IsFalse(selection.Contains(p3));
        }

        [Test]
        public void TestClear() {
            Selection selection = new Selection();
            Point p1 = new Point(columnSet, new double[] { 1, 2, 3, 4, 5, 6 });
            Point p2 = new Point(columnSet, new double[] { 7, 8, 9, 10, 11, 12 });
            Point p3 = new Point(columnSet, new double[] { 0, 0, 0, 1, 1, 1 });
            selection.Add(p1);
            selection.Add(p2);
            selection.Add(p3);
            Assert.AreEqual(selection.Length, 3);
            selection.Clear();
            Assert.AreEqual(selection.Length, 0);
        }

        [Test]
        public void TestGetPoints() {
            Selection selection = new Selection();
            Point p1 = new Point(columnSet, new double[] { 1, 2, 3, 4, 5, 6 });
            Point p2 = new Point(columnSet, new double[] { 7, 8, 9, 10, 11, 12 });
            Point p3 = new Point(columnSet, new double[] { 0, 0, 0, 1, 1, 1 });
            Point[] pointArray = new Point[3];
            pointArray[0]=p1;
            pointArray[1]=p1;
            pointArray[2]=p1;
            selection.Add(p1);
            selection.Add(p2);
            selection.Add(p3);
            List<Point> list = new List<Point>(selection.GetPoints);
            foreach ( Point p in pointArray ) {
                Assert.IsTrue(list.Contains(p));
            }
        } 
    }
}
