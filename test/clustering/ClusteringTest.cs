using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;
using Pavel.Test.Framework;
using Pavel.Clustering;

using NUnit.Framework;

namespace Pavel.Test.Clustering {

    [TestFixture]
    public class ClusteringTest {
        
        [Test]
        public void TestConstructorAndClusters() {
            PointSetTest pst = new PointSetTest();
            List<Point> ps = new List<Point>(pst.ComplicatedPointSet);

            KMeans km = new KMeans();
            km.PointSet = pst.ComplicatedPointSet;
            km.SpaceView = new SpaceView(pst.ComplicatedPointSet.Space, "");
            km.Name = "";
            ClusterSet clusterSet = new ClusterSet(km);
            clusterSet.Add(new PointList(pst.ComplicatedPointSet.Space));
            clusterSet.PointLists[0].Add(ps[0]);
            clusterSet.PointLists[0].Add(ps[1]);
            clusterSet.PointLists[0].Add(new Cluster("Cluster 1", ps[2]));
            clusterSet.PointLists[0].Add(new Cluster("Cluster 2", ps[3]));
            clusterSet.PointLists[0].Add(new Cluster("Cluster 3", ps[4]));
            Assert.AreEqual(5, clusterSet.Length);

            IEnumerator<Point> clusters = clusterSet.PointLists[0].GetEnumerator();
            
            Assert.IsTrue(clusters.MoveNext());
            foreach (Column c in pst.ComplicatedPointSet.Space) {
                Assert.AreEqual(ps[2][c], clusters.Current[c]);
            }
            Assert.IsTrue(clusters.MoveNext());
            Assert.IsInstanceOfType(typeof(Cluster), clusters.Current);
            foreach (Column c in pst.ComplicatedPointSet.Space) {
                Assert.AreEqual(ps[3][c], clusters.Current[c]);
            }
            Assert.IsTrue(clusters.MoveNext());
            Assert.IsInstanceOfType(typeof(Cluster), clusters.Current);
            foreach (Column c in pst.ComplicatedPointSet.Space) {
                Assert.AreEqual(ps[4][c], clusters.Current[c]);
            }
            Assert.IsFalse(clusters.MoveNext());
        }
    }
}
