using System;
using System.Collections.Generic;
using System.Text;

using Pavel.Test.Framework;
using Pavel.Framework;
using Pavel.Clustering;
using Node = Pavel.Clustering.HierarchicalClusterSet.Node;
using NUnit.Framework;

namespace Pavel.Test.Clustering {
    [TestFixture]
    public class ExtendedHACMTest {
        #region Test MatrixEntry<T>

        [Test]
        public void TestCompareTo() {
            MatrixEntry<float>[] entries = new MatrixEntry<float>[] {
                new MatrixEntry<float>(0, 0, 1.5f),
                new MatrixEntry<float>(1, 0, 1.5f),
                new MatrixEntry<float>(0, 1, 0.5f),
                new MatrixEntry<float>(2, 0, 1.5f),
                new MatrixEntry<float>(2, 1, 200.5f),
                new MatrixEntry<float>(2, 2, 1.5f),
                new MatrixEntry<float>(2, 3, 0.5f),
                new MatrixEntry<float>(3, 0, -1.5f)};
            Array.Sort<MatrixEntry<float>>(entries);
            Assert.AreEqual(3, entries[0].Row);
            Assert.AreEqual(0, entries[0].Column);
            Assert.AreEqual(0, entries[1].Row);
            Assert.AreEqual(1, entries[1].Column);
            Assert.AreEqual(2, entries[2].Row);
            Assert.AreEqual(3, entries[2].Column);
            Assert.AreEqual(0, entries[3].Row);
            Assert.AreEqual(0, entries[3].Column);
            Assert.AreEqual(1, entries[4].Row);
            Assert.AreEqual(0, entries[4].Column);
            Assert.AreEqual(2, entries[5].Row);
            Assert.AreEqual(0, entries[5].Column);
            Assert.AreEqual(2, entries[6].Row);
            Assert.AreEqual(2, entries[6].Column);
            Assert.AreEqual(2, entries[7].Row);
            Assert.AreEqual(1, entries[7].Column);


        }

        #endregion

        #region ListSorted

        [Test]
        public void TestAddSorted() {
            ListSorted<int> list = new ListSorted<int>(new int[] { 5, 4, 7, 6 });
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(4, list[0]);
            Assert.AreEqual(5, list[1]);
            Assert.AreEqual(6, list[2]);
            Assert.AreEqual(7, list[3]);

            list = new ListSorted<int>(new int[] { 5, 4, 8, 7 }, new DecComparer());
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(8, list[0]);
            Assert.AreEqual(7, list[1]);
            Assert.AreEqual(5, list[2]);
            Assert.AreEqual(4, list[3]);

            list.AddSorted(0);
            Assert.AreEqual(0, list[4]);

            list.AddSorted(3);
            Assert.AreEqual(0, list[5]);
            Assert.AreEqual(3, list[4]);

            list.AddSorted(6);
            Assert.AreEqual(0, list[6]);
            Assert.AreEqual(3, list[5]);
            Assert.AreEqual(6, list[2]);

        }

        [Test]
        public void TestRemove() {
            ListSorted<int> list = new ListSorted<int>(new int[] { 5, 4, 7, 6 });
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(4, list[0]);
            Assert.AreEqual(5, list[1]);
            Assert.AreEqual(6, list[2]);
            Assert.AreEqual(7, list[3]);

            list.Remove(6);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(4, list[0]);
            Assert.AreEqual(5, list[1]);
            Assert.AreEqual(7, list[2]);

            list.Remove(4);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(5, list[0]);
            Assert.AreEqual(7, list[1]);

            list.Remove(7);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(5, list[0]);

            list.Remove(5);
            Assert.AreEqual(0, list.Count);
        }

        private class DecComparer : IComparer<int> {
            public int Compare(int a, int b) {
                return -(a.CompareTo(b));
            }
        }

        #endregion

        #region SortedMatrix<T>

        [Test]
        public void TestSortedMatrix()
        {
            SortedMatrix<float> matrix =
                new SortedMatrix<float>(5, 5);
            matrix.Set(2, 0, 0.3f);
            matrix.Set(2, 1, 0.2f);
            matrix.Set(3, 2, 0.25f);
            matrix.Set(4, 0, 0.21f);
            matrix.Set(4, 1, 0.4f);

            Assert.AreEqual(5, matrix.Count);
            Assert.AreEqual(2, matrix.Minimum.Row);
            Assert.AreEqual(1, matrix.Minimum.Column);

            Assert.AreEqual(0.25f, matrix.ElementAt(3,2).Item);
            Assert.AreNotEqual(0.25f, matrix.Minimum.Item);

            matrix.Update(matrix.ElementAt(3, 2), 0.1f);
            Assert.AreEqual(0.1f, matrix.Minimum.Item);

            matrix.RemoveRow(2);
            Assert.AreEqual(3, matrix.Count);

            matrix.RemoveColumn(1);
            Assert.AreEqual(2, matrix.Count);

            IEnumerator<MatrixEntry<float>> enumerator = matrix.Column(2).GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current.Row);
            Assert.IsFalse(enumerator.MoveNext());

            enumerator = matrix.Row(4).GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current.Column);
            Assert.IsFalse(enumerator.MoveNext());

        }

        #endregion

        //[Test]
        //public void TestDoClusteringSingleLink() {
        //    PointSetTest pst = new PointSetTest();
        //    PointSet ps = pst.ComplicatedPointSet;

        //    ExtendedHACM hacm = new ExtendedHACM();
        //    hacm.PointSet = ps;
        //    hacm.Space = new Space(ps.ColumnSet, "");
        //    hacm.Name = "Single-Link-Clustering";

        //    hacm.Mode = (int)ExtendedHACM.MODE.MODE_SINGLE_LINK;
        //    hacm.RelevantColumns = new bool[] { true, true, true, true, true, true };
        //    ClusterSet clustering = (hacm.Start());

        //    foreach (Column column in ps.ColumnSet.Columns) {
        //        Assert.AreEqual(ps.MinMaxMean(ps.ColumnSet)[Result.MEAN][column], (clustering.PointLists[0] as HierarchicalClusterList).RootNode.Cluster[column]);
        //    }

        //    Node baseNode0 = new Node(new Cluster("Base-Cluster", ps[0].Trim(ps.ColumnSet)), 6, null, null);
        //    baseNode0.Cluster.PointSet = new PointSet("", ps.ColumnSet);
        //    baseNode0.Cluster.PointSet.Add(new PointList(ps.ColumnSet));
        //    baseNode0.Cluster.PointSet.PointLists[0].Add(ps[0]);
        //    Node baseNode1 = new Node(new Cluster("Base-Cluster", ps[1].Trim(ps.ColumnSet)), 6, null, null);
        //    baseNode1.Cluster.PointSet = new PointSet("", ps.ColumnSet);
        //    baseNode1.Cluster.PointSet.Add(new PointList(ps.ColumnSet));
        //    baseNode1.Cluster.PointSet.PointLists[0].Add(ps[1]);
        //    Node baseNode2 = new Node(new Cluster("Base-Cluster", ps[2].Trim(ps.ColumnSet)), 6, null, null);
        //    baseNode2.Cluster.PointSet = new PointSet("", ps.ColumnSet);
        //    baseNode2.Cluster.PointSet.Add(new PointList(ps.ColumnSet));
        //    baseNode2.Cluster.PointSet.PointLists[0].Add(ps[2]);
        //    Node baseNode3 = new Node(new Cluster("Base-Cluster", ps[3].Trim(ps.ColumnSet)), 6, null, null);
        //    baseNode3.Cluster.PointSet = new PointSet("", ps.ColumnSet);
        //    baseNode3.Cluster.PointSet.Add(new PointList(ps.ColumnSet));
        //    baseNode3.Cluster.PointSet.PointLists[0].Add(ps[3]);
        //    Node baseNode4 = new Node(new Cluster("Base-Cluster", ps[4].Trim(ps.ColumnSet)), 6, null, null);
        //    baseNode4.Cluster.PointSet = new PointSet("", ps.ColumnSet);
        //    baseNode4.Cluster.PointSet.Add(new PointList(ps.ColumnSet));
        //    baseNode4.Cluster.PointSet.PointLists[0].Add(ps[4]);

        //    //0-3
        //    PointList pl = new PointList(ps.ColumnSet, new Point[] { ps[3], ps[0] });
        //    Node node03 = new Node(new Cluster("", pl.MinMaxMean()[Result.MEAN]), 5, baseNode0, baseNode3);
        //    node03.Cluster.PointSet = new PointSet("", ps.ColumnSet);
        //    node03.Cluster.PointSet.Add(pl);

        //    //1-2
        //    pl = new PointList(ps.ColumnSet, new Point[] { ps[2], ps[1] });
        //    Node node12 = new Node(new Cluster("", pl.MinMaxMean()[Result.MEAN]), 4, baseNode1, baseNode2);
        //    node12.Cluster.PointSet = new PointSet("", ps.ColumnSet);
        //    node12.Cluster.PointSet.Add(pl);

        //    //0-3-4
        //    pl = new PointList(ps.ColumnSet, new Point[] { ps[4], ps[3], ps[0] });
        //    Node node034 = new Node(new Cluster("", pl.MinMaxMean()[Result.MEAN]), 3, node03, baseNode4);
        //    node034.Cluster.PointSet = new PointSet("", ps.ColumnSet);
        //    node034.Cluster.PointSet.Add(pl);

        //    //0-1-2-3-4
        //    pl = new PointList(ps.ColumnSet, new Point[] { ps[4], ps[3], ps[2], ps[1], ps[0] });
        //    Node node01234 = new Node(new Cluster("", pl.MinMaxMean()[Result.MEAN]), 2, node034, node12);
        //    node01234.Cluster.PointSet = new PointSet("", ps.ColumnSet);
        //    node01234.Cluster.PointSet.Add(pl);


        //    Node rootNode = (clustering.PointLists[0] as HierarchicalClusterList).RootNode;

        //    Assert.AreEqual(node01234.Cluster.Values, rootNode.Cluster.Values);
        //    Assert.AreEqual(node01234.SplittingNumber, rootNode.SplittingNumber);

        //    Assert.AreEqual(node12.Cluster.Values, rootNode.LeftSubNode.Cluster.Values);
        //    Assert.AreEqual(node12.SplittingNumber, rootNode.LeftSubNode.SplittingNumber);
        //    Assert.AreEqual(node034.Cluster.Values, rootNode.RightSubNode.Cluster.Values);
        //    Assert.AreEqual(node034.SplittingNumber, rootNode.RightSubNode.SplittingNumber);

        //    Assert.AreEqual(ps[1].Values, rootNode.LeftSubNode.RightSubNode.Cluster.Values);
        //    Assert.AreEqual(ps[2].Values, rootNode.LeftSubNode.LeftSubNode.Cluster.Values);

        //    Assert.AreEqual(ps[4].Values, rootNode.RightSubNode.LeftSubNode.Cluster.Values);
        //    Assert.AreEqual(node03.Cluster.Values, rootNode.RightSubNode.RightSubNode.Cluster.Values);
        //    Assert.AreEqual(node03.SplittingNumber, rootNode.RightSubNode.RightSubNode.SplittingNumber);

        //    Assert.AreEqual(ps[0].Values, rootNode.RightSubNode.RightSubNode.RightSubNode.Cluster.Values);
        //    Assert.AreEqual(ps[3].Values, rootNode.RightSubNode.RightSubNode.LeftSubNode.Cluster.Values);

        //    (clustering.PointLists[0] as HierarchicalClusterList).DefaultClusterCount = 5;
        //    IEnumerator<Point> enu = (clustering.PointLists[0] as HierarchicalClusterList).GetEnumerator();
        //    Assert.IsTrue(enu.MoveNext());
        //    Assert.AreEqual(ps[2].Values, (enu.Current as Cluster).Values);
        //    Assert.IsTrue(enu.MoveNext());
        //    Assert.AreEqual(ps[1].Values, (enu.Current as Cluster).Values);
        //    Assert.IsTrue(enu.MoveNext());
        //    Assert.AreEqual(ps[4].Values, (enu.Current as Cluster).Values);
        //    Assert.IsTrue(enu.MoveNext());
        //    Assert.AreEqual(ps[3].Values, (enu.Current as Cluster).Values);
        //    Assert.IsTrue(enu.MoveNext());
        //    Assert.AreEqual(ps[0].Values, (enu.Current as Cluster).Values);
        //    Assert.IsFalse(enu.MoveNext());

        //    //Other DefaultDistances...

        //    //foreach (Cluster cluster in clustering.Clusters) {
        //    //    Assert.AreEqual(ps[i++].Values, cluster.Values);
        //    //}

        //    //double distance;
        //    //distance = HACM.Distance(node034.Cluster, node12.Cluster, ps.ColumnSets[0],
        //    //    (int)HACM.HACMClusteringArgs.Mode.MODE_SINGLE_LINK, ClusterSet.EuclideanDistance);


        //}
    }
}
