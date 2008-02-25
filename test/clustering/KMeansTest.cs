using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;
using Pavel.Test.Framework;
using Pavel.Clustering;

using NUnit.Framework;

namespace Pavel.Test.Clustering {
    [TestFixture]
    public class KMeansTest {
        [Test]
        public void TestDoClustering() {
            PointSetTest pst = new PointSetTest();
            List<Point> ps = new List<Point>(pst.ComplicatedPointSet);

            KMeans km = new KMeans();
            km.PointSet  = pst.ComplicatedPointSet;
            km.Space = new Space(pst.ComplicatedPointSet.ColumnSet, "");
            km.Name      = "K-Means-Clustering";
            km.RelevantColumns = new bool[] { true, true, true, true, true, true };
            // Use special RandomSeed to acheive reproducibility
            km.RandomSeed       = 4711;
            km.NumberOfClusters = 4;

            ClusterSet result = km.Start();

            Assert.AreEqual(4, result.Length);
            IEnumerator<Point> clusters = result.GetEnumerator();
            Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(1, (clusters.Current as Cluster).PointSet.Length);
            Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(2, (clusters.Current as Cluster).PointSet.Length);
            Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(1, (clusters.Current as Cluster).PointSet.Length);
            Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(1, (clusters.Current as Cluster).PointSet.Length);
            Assert.IsFalse(clusters.MoveNext());

            foreach (Column col in pst.ComplicatedPointSet.ColumnSet) {
                clusters = result.GetEnumerator();
                Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(ps[0][col], (clusters.Current as Cluster)[col]);
                Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(new PointList(pst.ComplicatedPointSet.ColumnSet, new Point[] { ps[1], ps[2] }).MinMaxMean()[Result.MEAN][col], (clusters.Current as Cluster)[col]);
                Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(ps[3][col], (clusters.Current as Cluster)[col]);
                Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(ps[4][col], (clusters.Current as Cluster)[col]);
                Assert.IsFalse(clusters.MoveNext());
            }
   
        }

        [Test]
        public void TestDoClusteringWithScalingAndSelection() {
            PointSetTest pst = new PointSetTest();
            List<Point> ps = new List<Point>(pst.ComplicatedPointSet);

            KMeans km = new KMeans();
            km.PointSet = pst.ComplicatedPointSet;
            km.Space = new Space(pst.ComplicatedPointSet.ColumnSet, "");
            // Manipulate Space
            //km.Space.
            km.Name = "K-Means-Clustering";
            km.RelevantColumns = new bool[] { true, true, true, true, true, true };
            // Use special RandomSeed to acheive reproducibility
            km.RandomSeed = 4711;
            km.NumberOfClusters = 4;

            ClusterSet result = km.Start();

            Assert.AreEqual(4, result.Length);
            IEnumerator<Point> clusters = result.GetEnumerator();
            Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(1, (clusters.Current as Cluster).PointSet.Length);
            Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(2, (clusters.Current as Cluster).PointSet.Length);
            Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(1, (clusters.Current as Cluster).PointSet.Length);
            Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(1, (clusters.Current as Cluster).PointSet.Length);
            Assert.IsFalse(clusters.MoveNext());

            foreach (Column col in pst.ComplicatedPointSet.ColumnSet) {
                clusters = result.GetEnumerator();
                Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(ps[0][col], (clusters.Current as Cluster)[col]);
                Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(new PointList(pst.ComplicatedPointSet.ColumnSet, new Point[] { ps[1], ps[2] }).MinMaxMean()[Result.MEAN][col], (clusters.Current as Cluster)[col]);
                Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(ps[3][col], (clusters.Current as Cluster)[col]);
                Assert.IsTrue(clusters.MoveNext()); Assert.AreEqual(ps[4][col], (clusters.Current as Cluster)[col]);
                Assert.IsFalse(clusters.MoveNext());
            }

        }

        [Test, Explicit, Category("Profiling")]
        public void TestSpeed2() {
            int numColumns  = 50;
            int numPoints   = 2000;
            int numClusters = 100;

            int randomSeed  = 4711;

            Random random = new Random(1234);
            Column[] cols = new Column[numColumns];
            for (int i = 0; i < numColumns; i++) { cols[i] = new Column(); }
            ColumnSet columnSet = new ColumnSet(cols);
            Column[] columns = columnSet.Columns;
            PointSet ps = new PointSet("Random Set", columnSet);
            for(int i = 0; i<numPoints; i++) {
                double[] vals = new double[numColumns];
                for (int j = 0; j < numColumns; j++ ) {
                    vals[j] = random.NextDouble();
                }
                ps.Add(new Point(columnSet, vals));
            }
            
            KMeans km = new KMeans();
            km.PointSet  = ps;
            km.Space = new Space(ps.ColumnSet, "");
            km.Name      = "K-Means-Clustering";
            bool[] relevantColumns = new bool[numColumns];
            for ( int i = 0; i < numColumns; i++ ) {
                relevantColumns[i] = true;
            }
            km.RelevantColumns = relevantColumns;
            // Use special RandomSeed to acheive reproducibility
            km.RandomSeed = randomSeed;
            km.NumberOfClusters = numClusters;
            
            long time = DateTime.Now.Ticks;
            Console.WriteLine("TestSpeed2: Los gehts!");
            Console.WriteLine("  Points:  " + numPoints);
            Console.WriteLine("  Spalten: " + numColumns);
            Console.WriteLine("  Cluster: " + numClusters);
            km.Start();
            time = DateTime.Now.Ticks - time;
            time /= 10000;
            Console.WriteLine("TestSpeed2: " + time + "ms");
        }

        [Test]
        public void TestCreateDifferentRandoms() {
            int[] rands;
            try {
                rands = KMeans.CreateDifferentRandoms(new Random(4711), 4, 0, 2);
                Assert.Fail();
            } catch (ArgumentException e) {
                Assert.AreEqual("Not enought range for unique Randoms", e.Message);
            }

            try {
                rands = KMeans.CreateDifferentRandoms(new Random(4711), 0, 1, 2);
                Assert.Fail();
            } catch (ArgumentException e) {
                Assert.AreEqual("IllegalArguments: Count=0 Min=1 Max=2", e.Message);
            }

            try {
                rands = KMeans.CreateDifferentRandoms(new Random(4711), 1, 2, 1);
                Assert.Fail();
            } catch (ArgumentException e) {
                Assert.AreEqual("IllegalArguments: Count=1 Min=2 Max=1", e.Message);
            }


            rands = KMeans.CreateDifferentRandoms(new Random(4711), 4, 0, 4);
            Assert.AreEqual(new int[] {3, 0, 2, 1}, rands);
        }
    }
}
