using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using NUnit.Framework;

namespace Pavel.Test.Framework {
    [TestFixture]
    public class PointTest {

        [Test]
        public void ContructorStandardTest() {
            ColumnSet columnSet = new ColumnSet(new Column("Cola"), new Column("Colb"), new Column("Colc"));
            new Point(columnSet, 0.1, 0.2, 0.3);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void ContructorLengthMismatchTest() {
            ColumnSet columnSet = new ColumnSet(new Column("Cola"), new Column("Colb"), new Column("Colc"));
            new Point(columnSet, 0.1, 0.2, 0.3, 0.4);
        }

        [Test]
        public void TestIndexer() {
            ColumnSet columnSet = new ColumnSet(new Column("Cola"), new Column("Colb"), new Column("Colc"));
            Point p = new Point(columnSet, new double[] { 0.1, 0.2, 0.3 });

            Assert.AreEqual(0.1, p[columnSet.Columns[0]]);
            Assert.AreEqual(0.3, p[columnSet.Columns[2]]);

            Assert.AreEqual(0.1, p[0]);
            Assert.AreEqual(0.3, p[2]);
        }

        [Test]
        public void TestEquals() {
            ColumnSet columnSet1 = new ColumnSet(new Column("Cola"), new Column("Colb"), new Column("Colc"));
            ColumnSet columnSet2 = new ColumnSet(new Column("Cold"));
            ColumnSet columnSet3 = ColumnSet.Union(columnSet1, columnSet2);
            Point p = new Point(columnSet1, new double[] { 0.1, 0.2, 0.3 });
            
            Point p1 = new Point(columnSet1, new double[] { 0.1, 0.2, 0.3 });
            Point p2 = new Point(columnSet1, new double[] { 0.1, 0.2, 0.3 });
            Point p3 = new Point(columnSet1, new double[] { 0.1, 0.2, 0.5 });
            Point p4 = new Point(columnSet3, new double[] { 0.1, 0.2, 0.3, 0.4 });
            Assert.IsTrue(p1.Equals(p2));
            Assert.IsFalse(p1.Equals(p3));
            Assert.IsFalse(p1.Equals(p4));
        }

        [Test]
        public void DistanceSameColumnSet() {
            PointList pl = new PointSetTest().ComplicatedPointSet.PointLists[0];

            //Euclidean Distance
            Assert.AreEqual(2304681372.0, Point.Distance(pl[0], pl[1]));
            Assert.AreEqual(2444185975.0, Point.Distance(pl[0], pl[2]));
            Assert.AreEqual(1747031105.0, Point.Distance(pl[0], pl[3]));
            Assert.AreEqual(2193521083.0, Point.Distance(pl[0], pl[4]));
        }

        [Test]
        public void DistanceWithSuperColumnSetMap() {
            ColumnSet sp1 = new ColumnSet(new Column(), new Column());
            ColumnSet sp2 = ColumnSet.Union(sp1, new ColumnSet(new Column()));
            Assert.IsTrue(sp1.IsSubSetOf(sp2));

            Point p1 = new Point(sp1, 1, 2);
            Point p2 = new Point(sp2, 3, 4, 5);

            Assert.AreEqual(8, Point.Distance(p1, p2, sp1.SuperSetMap(sp2)));

            // Distance for no Columns
            ColumnSet sp3 = new ColumnSet(new Column[] { });
            Point p3  = new Point(sp3, new double[0]);
            Assert.AreEqual(0.0, Point.Distance(p1, p3, sp3.SuperSetMap(sp2)));
            Assert.AreEqual(0.0, Point.Distance(p1, p2, sp3.SuperSetMap(sp2)));
        }

        [Test]
        public void DistanceCommonSubColumnSet() {
            PointList pl = new PointSetTest().ComplicatedPointSet.PointLists[0];
            int[] pointMap;

            // Distance for no Columns
            pointMap = new int[0];
            Assert.AreEqual(0.0, Point.Distance(pl[0], pl[3], pointMap, pointMap));

            pointMap = new int[] { 0, 1, 2 };
            Assert.AreEqual(1231531065, Point.Distance(pl[0], pl[3], pointMap, pointMap));
        }

        [Test]
        public void ScaledDistance() {
            PointList pl = new PointSetTest().ComplicatedPointSet.PointLists[0];
            int[] pointMap;
            Space space = new Space(pl.ColumnSet, "");

            // Distance for no Columns
            pointMap = new int[0];
            Assert.AreEqual(0.0, Point.ScaledDistance(pl[0], pl[3], pointMap, space, new bool[] { }));

            pointMap = new int[] { 0, 1, 2 };
            Assert.AreEqual(1231531065, Point.ScaledDistance(pl[0], pl[3], pointMap, space, new bool[] { true, true, true }));
        }


        [Test]
        public void SerializeTest() {
            MemoryStream    memoryStream = new MemoryStream();
            BinaryFormatter binFormatter = new BinaryFormatter();
            ColumnSet           columnSet        = new ColumnSet(new Column(), new Column());
            Point           p1           = new Point(columnSet, 0.1 , 0.1);
            binFormatter.Serialize(memoryStream, p1);
            memoryStream.Position=0;
            p1 = binFormatter.Deserialize(memoryStream) as Point;
            for (int i = 0; i < columnSet.Dimension; i++ ) {
                Assert.AreEqual(0.1, p1[i]);
            }
        }

        [Test]
        public void Trim() {
            Column[] cols = new Column[] { new Column(), new Column(), new Column(), new Column() };
            ColumnSet sp1 = new ColumnSet(cols);
            ColumnSet sp2 = new ColumnSet(cols[0], cols[2]);

            Point p1 = new Point(sp1, 1, 2, 3, 4);
            Point p2 = p1.Trim(sp2);

            Assert.AreEqual(1, p2[0]);
            Assert.AreEqual(3, p2[1]);
        }

        [Test]
        public void TrimSameColumnSet() {
            ColumnSet sp = new ColumnSet(new Column(), new Column(), new Column(), new Column());

            Point p1 = new Point(sp, 1, 2, 3, 4);

            Assert.AreEqual(p1, p1.Trim(sp));
        }

        [Test]
        public void QuickTrimTest() {
            Column[] cols = new Column[] { new Column(), new Column(), new Column(), new Column() };
            ColumnSet sp1 = new ColumnSet(cols);
            ColumnSet sp2 = new ColumnSet(cols[0], cols[2]);

            Point p1 = new Point(sp1, 1, 2, 3, 4);
            Point p2 = p1.Trim(sp2, sp2.SuperSetMap(sp1));

            Assert.AreEqual(1, p2[0]);
            Assert.AreEqual(3, p2[1]);
        }

    }
}

