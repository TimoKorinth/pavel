using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;
using Pavel.Test.Framework;
using Pavel.Clustering;
using Node = Pavel.Clustering.HierarchicalClusterList.Node;

using NUnit.Framework;

namespace Pavel.Test.Clustering {
    [TestFixture]
    public class HACMTest {
        [Test]
        public void TestDoClusteringSingleLink() {
            PointSetTest pst = new PointSetTest();
            PointSet ps = pst.ComplicatedPointSet;

            HACM hacm = new HACM();
            hacm.PointSet  = ps;
            hacm.SpaceView = new SpaceView(ps.Space, "");
            hacm.Name      = "Single-Link-Clustering";

            hacm.Mode = (int)HACM.MODE.MODE_SINGLE_LINK;
            hacm.RelevantColumns = new bool[] { true, true, true, true, true, true };
            ClusterSet clustering = (hacm.Start());

            foreach (Column column in ps.Space.Columns) {
                Assert.AreEqual(ps.MinMaxMean(ps.Space)[Result.MEAN][column], (clustering.PointLists[0] as HierarchicalClusterList).RootNode.Cluster[column]);
            }

            Node baseNode0 = new Node(new Cluster("Base-Cluster", ps[0].Trim(ps.Space)), 6, null, null);
            baseNode0.Cluster.PointSet = new PointSet("", ps.Space);
            baseNode0.Cluster.PointSet.Add(new PointList(ps.Space));
            baseNode0.Cluster.PointSet.PointLists[0].Add(ps[0]);
            Node baseNode1 = new Node(new Cluster("Base-Cluster", ps[1].Trim(ps.Space)), 6, null, null);
            baseNode1.Cluster.PointSet = new PointSet("", ps.Space);
            baseNode1.Cluster.PointSet.Add(new PointList(ps.Space));
            baseNode1.Cluster.PointSet.PointLists[0].Add(ps[1]);
            Node baseNode2 = new Node(new Cluster("Base-Cluster", ps[2].Trim(ps.Space)), 6, null, null);
            baseNode2.Cluster.PointSet = new PointSet("", ps.Space);
            baseNode2.Cluster.PointSet.Add(new PointList(ps.Space));
            baseNode2.Cluster.PointSet.PointLists[0].Add(ps[2]);
            Node baseNode3 = new Node(new Cluster("Base-Cluster", ps[3].Trim(ps.Space)), 6, null, null);
            baseNode3.Cluster.PointSet = new PointSet("", ps.Space);
            baseNode3.Cluster.PointSet.Add(new PointList(ps.Space));
            baseNode3.Cluster.PointSet.PointLists[0].Add(ps[3]);
            Node baseNode4 = new Node(new Cluster("Base-Cluster", ps[4].Trim(ps.Space)), 6, null, null);
            baseNode4.Cluster.PointSet = new PointSet("", ps.Space);
            baseNode4.Cluster.PointSet.Add(new PointList(ps.Space));
            baseNode4.Cluster.PointSet.PointLists[0].Add(ps[4]);
             
            //0-3
            PointList pl = new PointList(ps.Space, new Point[] { ps[3], ps[0] });
            Node node03 = new Node(new Cluster("", pl.MinMaxMean()[Result.MEAN]), 5, baseNode0, baseNode3);
            node03.Cluster.PointSet = new PointSet("", ps.Space);
            node03.Cluster.PointSet.Add(pl);

            //1-2
            pl = new PointList(ps.Space, new Point[] { ps[2], ps[1] });
            Node node12 = new Node(new Cluster("", pl.MinMaxMean()[Result.MEAN]), 4, baseNode1, baseNode2);
            node12.Cluster.PointSet = new PointSet("", ps.Space);
            node12.Cluster.PointSet.Add(pl);

            //0-3-4
            pl = new PointList(ps.Space, new Point[] { ps[4], ps[3], ps[0] });
            Node node034 = new Node(new Cluster("", pl.MinMaxMean()[Result.MEAN]), 3, node03, baseNode4);
            node034.Cluster.PointSet = new PointSet("", ps.Space);
            node034.Cluster.PointSet.Add(pl);

            //0-1-2-3-4
            pl = new PointList(ps.Space, new Point[] { ps[4], ps[3], ps[2], ps[1], ps[0] });
            Node node01234 = new Node(new Cluster("", pl.MinMaxMean()[Result.MEAN]), 2, node034, node12);
            node01234.Cluster.PointSet = new PointSet("", ps.Space);
            node01234.Cluster.PointSet.Add(pl);


            Node rootNode = (clustering.PointLists[0] as HierarchicalClusterList).RootNode;

            Assert.AreEqual(node01234.Cluster.Values, rootNode.Cluster.Values);
            Assert.AreEqual(node01234.SplittingNumber, rootNode.SplittingNumber);

            Assert.AreEqual(node12.Cluster.Values, rootNode.RightSubNode.Cluster.Values);
            Assert.AreEqual(node12.SplittingNumber, rootNode.RightSubNode.SplittingNumber);
            Assert.AreEqual(node034.Cluster.Values, rootNode.LeftSubNode.Cluster.Values);
            Assert.AreEqual(node034.SplittingNumber, rootNode.LeftSubNode.SplittingNumber);

            Assert.AreEqual(ps[1].Values, rootNode.RightSubNode.RightSubNode.Cluster.Values);
            Assert.AreEqual(ps[2].Values, rootNode.RightSubNode.LeftSubNode.Cluster.Values);

            Assert.AreEqual(ps[4].Values, rootNode.LeftSubNode.LeftSubNode.Cluster.Values);
            Assert.AreEqual(node03.Cluster.Values, rootNode.LeftSubNode.RightSubNode.Cluster.Values);
            Assert.AreEqual(node03.SplittingNumber, rootNode.LeftSubNode.RightSubNode.SplittingNumber);

            Assert.AreEqual(ps[0].Values, rootNode.LeftSubNode.RightSubNode.RightSubNode.Cluster.Values);
            Assert.AreEqual(ps[3].Values, rootNode.LeftSubNode.RightSubNode.LeftSubNode.Cluster.Values);

            (clustering.PointLists[0] as HierarchicalClusterList).DefaultClusterCount = 5;
            IEnumerator<Point> enu = (clustering.PointLists[0] as HierarchicalClusterList).GetEnumerator();
            Assert.IsTrue(enu.MoveNext());
            Assert.AreEqual(ps[4].Values, (enu.Current as Cluster).Values);
            Assert.IsTrue(enu.MoveNext());
            Assert.AreEqual(ps[3].Values, (enu.Current as Cluster).Values);
            Assert.IsTrue(enu.MoveNext());
            Assert.AreEqual(ps[0].Values, (enu.Current as Cluster).Values);
            Assert.IsTrue(enu.MoveNext());
            Assert.AreEqual(ps[2].Values, (enu.Current as Cluster).Values);
            Assert.IsTrue(enu.MoveNext());
            Assert.AreEqual(ps[1].Values, (enu.Current as Cluster).Values);
            Assert.IsFalse(enu.MoveNext());

            //Other DefaultDistances...
            
            //foreach (Cluster cluster in clustering.Clusters) {
            //    Assert.AreEqual(ps[i++].Values, cluster.Values);
            //}

            //double distance;
            //distance = HACM.Distance(node034.Cluster, node12.Cluster, ps.Spaces[0],
            //    (int)HACM.HACMClusteringArgs.Mode.MODE_SINGLE_LINK, ClusterSet.EuclideanDistance);
            
            
        }

        [Test]
        public void TestDoClusteringCompleteLink() {
            PointSetTest pst = new PointSetTest();
            PointSet ps = pst.ComplicatedPointSet;

            HACM km = new HACM();
            km.PointSet  = ps;
            km.SpaceView = new SpaceView(ps.Space, "");
            km.Name      = "Complete-Link-Clustering";
            km.Mode      = (int)HACM.MODE.MODE_COMPLETE_LINK;
            km.RelevantColumns = new bool[] { true, true, true, true, true, true };
            km.Start();
        }

        [Test]
        public void TestDistance() {
            Space space = new Space("Space", new Column(), new Column(), new Column());
            
            Cluster clusterA     = new Cluster("", new Point(space, new double[] { 0.0, 0.0, 0.0 }));
            Cluster clusterB     = new Cluster("", new Point(space, new double[] { 0.0, 0.0, 0.0 }));
            Cluster emptyCluster = new Cluster("", new Point(space, new double[] { 0.0, 0.0, 0.0 }));

            clusterA.PointSet     = new PointSet("", space);
            clusterB.PointSet     = new PointSet("", space);
            emptyCluster.PointSet = new PointSet("", space);

            clusterA.PointSet.Add(new PointList(space));
            clusterB.PointSet.Add(new PointList(space));

            clusterA.PointSet.PointLists[0].Add(new Point(space, new double[] { -2,  8  ,  1   }));
            clusterA.PointSet.PointLists[0].Add(new Point(space, new double[] {  1,  6.5,  2   }));

            clusterB.PointSet.PointLists[0].Add(new Point(space, new double[] {  5,  3  , -2   }));
            clusterB.PointSet.PointLists[0].Add(new Point(space, new double[] {  4, -1  , -2.5 }));

            int[][] superSpaceMap = new int[1][] { space.SuperSpaceMap(space) };

            //SingleLinkNormal
            Assert.AreEqual(44.25, HACM.Distance(clusterA, clusterB, space, superSpaceMap, (int)HACM.MODE.MODE_SINGLE_LINK));

            //CompleteLinkNormal
            Assert.AreEqual(129.25, HACM.Distance(clusterA, clusterB, space, superSpaceMap, (int)HACM.MODE.MODE_COMPLETE_LINK));

            //TODO: Test Errors of Distance-Function
        }

        
    }
}
