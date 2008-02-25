using System;
using System.Collections.Generic;
using System.Text;
using Node = Pavel.Clustering.HierarchicalClusterSet.Node;
using Pavel.Clustering;
using Pavel.Framework;

using NUnit.Framework;

namespace Pavel.Test.Clustering {
    [TestFixture]
    public class HierarchicalClusterSetTest {
        private static ColumnSet anyColumnSet;
        private static Point anyPoint;
        private static Cluster emptyCluster;
        private static Node anyNode;
        private static Node dendogramm;
        private static Node mergedDendogramm;
        
        [SetUp]
        public void Init()
        {
            anyColumnSet = new ColumnSet(new Column[] { new Column(), new Column() });
            anyPoint = new Point(anyColumnSet, 0.0, 0.0);
            emptyCluster = new Cluster("", anyPoint);
            anyNode = new Node(emptyCluster, 0, null, null);

            // Create DendoGramm
            Node node1 = new Node(new Cluster("", anyPoint), 4, null, null);
            node1.Cluster.PointSet = new PointSet("", anyColumnSet);
            node1.Cluster.PointSet.Add(new Point(anyColumnSet, 1.0, 2.0));
            node1.Cluster.SetValues(node1.Cluster.PointSet.MinMaxMean()[Result.MEAN]);
            Node node2 = new Node(new Cluster("", anyPoint), 4, null, null);
            node2.Cluster.PointSet = new PointSet("", anyColumnSet);
            node2.Cluster.PointSet.Add(new Point(anyColumnSet, 1.0, 1.0));
            node2.Cluster.SetValues(node2.Cluster.PointSet.MinMaxMean()[Result.MEAN]);
            Node node3 = new Node(new Cluster("", anyPoint), 4, null, null);
            node3.Cluster.PointSet = new PointSet("", anyColumnSet);
            node3.Cluster.PointSet.Add(new Point(anyColumnSet, -1.0, 1.0));
            node3.Cluster.SetValues(node3.Cluster.PointSet.MinMaxMean()[Result.MEAN]);
            Node node4 = new Node(new Cluster("", anyPoint), 4, null, null);
            node4.Cluster.PointSet = new PointSet("", anyColumnSet);
            node4.Cluster.PointSet.Add(new Point(anyColumnSet, -1.0, -2.0));
            node4.Cluster.SetValues(node4.Cluster.PointSet.MinMaxMean()[Result.MEAN]);
            
            Node node12        = new Node(new Cluster("", anyPoint), 4, node1, node2);
            Node node123       = new Node(new Cluster("", anyPoint), 3, node12, node3);
            Cluster cluster = new Cluster("", anyPoint);
            cluster.PointSet = new PointSet("", anyColumnSet);
            dendogramm = new Node(cluster, 2, node123, node4);
            
            Node mergedNode12  = HierarchicalClusterSet.MergeNodes(node1, node2, 4);
            Node mergedNode123 = HierarchicalClusterSet.MergeNodes(mergedNode12, node3, 3);
            mergedDendogramm   = HierarchicalClusterSet.MergeNodes(mergedNode123, node4, 2);
        }

        #region Node Tests
        [Test]
        public void TestNodeConstructor1() {
            try {
                Node node = new Node(emptyCluster, 0, null, anyNode);
                Assert.Fail();
            } catch (ArgumentException) { }
            try {
                Node node = new Node(emptyCluster, 0, anyNode, null);
                Assert.Fail();
            } catch (ArgumentException) { }
            try {
                // Fail if splittingNumber is too low
                Node node = new Node(emptyCluster, 3, dendogramm.LeftSubNode, dendogramm.LeftSubNode.LeftSubNode);
                Assert.Fail();
            } catch (ArgumentException) { }
        }

        [Test]
        public void TestNodeConstructor2() {
            Node node = new Node(emptyCluster, 2, anyNode, anyNode);
            Assert.AreEqual(emptyCluster, node.Cluster);
            Assert.AreEqual(2, node.SplittingNumber);
            Assert.AreEqual(node.LeftSubNode, anyNode);
            Assert.AreEqual(node.RightSubNode, anyNode);

            // Ignore SplittingNumber if Subnodes are null
            Assert.AreEqual(int.MaxValue, dendogramm.RightSubNode.SplittingNumber);
        }

        [Test]
        public void TestGetClusters() {
            Assert.AreEqual(1, Count(dendogramm.GetClusters(0)));
            Assert.AreEqual(1, Count(dendogramm.GetClusters(1)));
            Assert.AreEqual(2, Count(dendogramm.GetClusters(2)));
            Assert.AreEqual(3, Count(dendogramm.GetClusters(3)));
            Assert.AreEqual(4, Count(dendogramm.GetClusters(4)));
            Assert.AreEqual(4, Count(dendogramm.GetClusters(5)));

        }
        #endregion

        #region SetTests

        [Test]
        public void TestConstructor1() {
            // Dynamic functionality
            KMeans km = new KMeans();
            km.PointSet = dendogramm.Cluster.PointSet;
            km.Space = new Space(km.PointSet.ColumnSet, "");
            km.Name      = "K-Means-Clustering";

            HierarchicalClusterSet hcs = new HierarchicalClusterSet(km, dendogramm, 3);
            Assert.AreEqual(3, hcs.Length);
            hcs.DefaultClusterCount = 2;
            Assert.AreEqual(2, hcs.Length);
        }
        #endregion

        [Test]
        public void TestMergeNodes() {
            Assert.AreEqual(0, mergedDendogramm.Cluster.Values[0]);
            Assert.AreEqual(0.5, mergedDendogramm.Cluster.Values[1]);

            Node node1 = new Node(new Cluster("", anyPoint), 4, null, null);
            node1.Cluster.PointSet = new PointSet("", anyColumnSet);
            node1.Cluster.PointSet.Add(new Point(anyColumnSet, 1.0, 2.0));
            node1.Cluster.SetValues(node1.Cluster.PointSet.MinMaxMean()[Result.MEAN]);
            Node node2 = new Node(new Cluster("", anyPoint), 4, null, null);
            node2.Cluster.PointSet = new PointSet("", anyColumnSet);
            node2.Cluster.PointSet.Add(new Point(anyColumnSet, 0.5, 1.0));
            node2.Cluster.SetValues(node2.Cluster.PointSet.MinMaxMean()[Result.MEAN]);

            Node node12 = HierarchicalClusterSet.MergeNodes(node1, node2, 4);
            Assert.AreEqual(node2.Cluster, node12.RightSubNode.Cluster);
            Assert.AreEqual(node1.Cluster, node12.LeftSubNode.Cluster);
            Assert.AreEqual(0.75, node12.Cluster.Values[0]);
            Assert.AreEqual(1.5, node12.Cluster.Values[1]);

            Node node3 = new Node(new Cluster("", anyPoint), 4, null, null);
            node3.Cluster.PointSet = new PointSet("", anyColumnSet);
            node3.Cluster.PointSet.Add(new Point(anyColumnSet, 2.0, 1.5));
            node3.Cluster.SetValues(node2.Cluster.PointSet.MinMaxMean()[Result.MEAN]);

            Node node4 = HierarchicalClusterSet.MergeNodes(node12, node3, 3);
            
            PointList pl = new PointList(anyColumnSet, new Point[] {node1.Cluster, node2.Cluster, node3.Cluster});
            Assert.AreEqual(pl.MinMaxMean()[Result.MEAN].Values, node4.Cluster.Values);

        }

        #region Helper Methods
        private int Count(System.Collections.IEnumerable enumerable) {
            int count = 0;
            System.Collections.IEnumerator enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext()) {
                count++;
            }
            return count;
        }
        #endregion
    }
}
