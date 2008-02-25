// Part of PAVEl: PAVEl (Paretoset Analysis Visualization and Evaluation) is a tool for
// interactively displaying and evaluating large sets of highdimensional data.
// Its main intended use is the analysis of result sets from multi-objective evolutionary algorithms.
//
// Copyright (C) 2007  PG500, ISF, University of Dortmund
//      PG500 are: Christoph Begau, Christoph Heuel, Raffael Joliet, Jan Kolanski,
//                 Mandy Kröller, Christian Moritz, Daniel Niggemann, Mathias Stöber,
//                 Timo Stönner, Jan Varwig, Dafan Zhai
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>.
//
// For more information and contact details visit http://pavel.googlecode.com

using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;



namespace Pavel.Clustering {
    /// <summary>
    /// Special PointList to represent the result of an hierarchical Clustering-Algorithm.
    /// If HierarchicalClusterList is treated like a PointList, it behaves fully like a normal PointList.
    /// Behind the normal structure, HierarchicalClusterList is a dendogramm (binary tree of clusters).
    /// A horizontal cut throught the tree results in a PointList
    /// </summary>
    [Serializable]
    public class HierarchicalClusterSet : ClusterSet {

        #region Fields

        private int defaultClusterCount;
        private Node rootNode;

        #endregion

        #region Properties

        /// <value>Gets an indicator, how the dendogramm is mapped to a PointList or sets it</value>
        public int DefaultClusterCount {
            get { return defaultClusterCount; }
            set {
                defaultClusterCount = value;
                RecomputeClusters();
            }
        }

        /// <value>Gets the root-node of the dendogramm or sets it</value>
        public Node RootNode {
            get { return rootNode; }
            set {
                this.rootNode = value;
                RecomputeClusters();
            }
        }

        #endregion

        #region Contructors

        /// <summary>
        /// Creates a new HierarchicalClusterList and attatches the given dendogramm (rootNode)
        /// and defines a default number of Clusters.
        /// </summary>
        /// <param name="columnSet">A ColumnSet. Must not be null</param>
        /// <param name="rootNode">The Root-Node of the dendogramm</param>
        /// <param name="defaultClusterCount">The default number of Clusters</param>
        public HierarchicalClusterSet(ClusteringAlgorithm ca, Node rootNode, int defaultClusterCount)
            : base(ca) {
            this.rootNode = rootNode;
            this.defaultClusterCount = defaultClusterCount;
            RecomputeClusters();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Combines two Nodes to one. Input-Nodes are attached as children.
        /// New Cluster is computed by the weighted sum of the two clusters.
        /// Ensure that both Nodes contain Clusters with PointSets with the same number of PointLists
        /// and with the same ColumnSets.
        /// The structure (PointLists) of the Points is adopted for the resulting Node.
        /// </summary>
        /// <param name="left">Left Node</param>
        /// <param name="right">Right Node</param>
        /// <param name="splittingNumber">The splitting number defines, how the dendogramm
        /// will be linearized later. While searching for a ClusterList with size t, the current Node
        /// will be split up if t is greater or equal than the current splittingNumber.
        /// Take Care: the root should get a SplittingNumber of 2</param>
        /// <returns></returns>
        public static Node MergeNodes(Node left, Node right, int splittingNumber) {
            PointSet pointSet = new PointSet("" /*left.ToString() + " & " + right.ToString()*/, left.Cluster.PointSet.ColumnSet);
            
            // Check for equality of clusterSets will be perfomed in PointList.AddRange()
			pointSet.AddRange(left.Cluster.PointSet);
            pointSet.AddRange(right.Cluster.PointSet);
			
            // create Cluster with mean
            double[] center = new double[left.Cluster.Values.Length];
            double leftCount   = left.Cluster.PointSet.Length;
            double rightCount  = right.Cluster.PointSet.Length;
            double sumCount = leftCount + rightCount;
            for (int i = 0; i < center.Length; i++) {
                center[i] = (left.Cluster.Values[i] * leftCount + right.Cluster.Values[i] * rightCount) / sumCount;
            }
            
            Cluster cluster = new Cluster(pointSet.Label, new Point(left.Cluster.ColumnSet, center));
            
            // Set connected points
            cluster.PointSet = pointSet;

            //Merge PointIndices
            if (left.PointIndices != null && right.PointIndices != null) {
                int[] pointIndices = new int[left.PointIndices.Length + right.PointIndices.Length];
                Array.Copy(left.PointIndices, pointIndices, left.PointIndices.Length);
                Array.Copy(right.PointIndices, 0, pointIndices, left.PointIndices.Length, right.PointIndices.Length);
                // return new node
                return new Node(cluster, splittingNumber, left, right, pointIndices);
            } else {
                // return new node
                return new Node(cluster, splittingNumber, left, right);
            }
        }

        /// <summary>
        /// Traces the dendogramm with defaultClusterCount as a bound how deep it will be traced.
        /// If dendogramm is created correctly, the PointList contains defaultClusterCount Clusters.
        /// </summary>
        private void RecomputeClusters() {
            List<Point> tmpClusters = new List<Point>();
            foreach (Cluster cluster in rootNode.GetClusters(this.defaultClusterCount)) {
                tmpClusters.Add(cluster);
            }
            // TODO: Check if Clear-Method would be better
            int[] allIndices = new int[this.Length];
            for (int i = 0; i < this.Length; i++) { allIndices[i] = i; } 
            this.RemoveAtRange(allIndices);
            this.AddRange(tmpClusters);
        }

        #endregion

        #region Inner class Node

        /// <summary>
        /// A Node in a dendogramm for a HierarchicalClusterList
        /// </summary>
        [Serializable]
        public class Node {

            #region Fields

            private Cluster cluster;
            private int splittingNumber;
            private Node leftSubNode, rightSubNode;
            private int[] pointIndices;

            #endregion

            #region Properties

            /// <value>Gets the Cluster in the Node</value>
            public Cluster Cluster { get { return cluster; } }

            /// <value>
            /// Gets the splitting number.
            /// The splitting number defines, how the dendogramm
            /// will be linearized later. While searching for a ClusterList with size t, the current Node
            /// will be split up if t is greater or equal than the current splittingNumber.
            /// </value>
            public int SplittingNumber { get { return splittingNumber; } }

            /// <value>Gets the left Sub-Node in dendogramm </value>
            public Node LeftSubNode { get { return leftSubNode; } }

            /// <value>Gets the right Sub-Node in dendogramm</value>
            public Node RightSubNode { get { return rightSubNode; } }

            /// <value>Gets/Sets the indices of contained Point. Optional Values for computation</value>
            public int[] PointIndices {
                get { return pointIndices; }
                set { pointIndices = value; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Creates a new Node with the given parameters. Either both or none of the sub-nodes must be null.
            /// </summary>
            /// <param name="cluster">A Cluster for this Node</param>
            /// <param name="splittingNumber">The splittingNumber defines, how the dendogramm
            /// will be linearized later. While searching for a ClusterList with size t, the current Node
            /// will be split up if t is greater or equal than the current splittingNumber.
            /// If Node is leaf (both subNodes are null) splittingNumber is ignored and set to int.MaxValue.
            /// Take Care: the root should get the a SplittingNumber of 2</param>
            /// <param name="leftSubNode">A valid Node or null for a leaf</param>
            /// <param name="rightSubNode">A valid Node or null for a leaf</param>
            /// <exception cref="ArgumentException">If splittingNumber is not greater than the
            /// splittingNumber of both Sub-Nodes</exception>
            /// <exception cref="ArgumentException">If excactly one subNode is null</exception>
            public Node(Cluster cluster, int splittingNumber, Node leftSubNode, Node rightSubNode) {
                if (((leftSubNode == null) ^ (rightSubNode == null)) == true) { throw new ArgumentException("Either both or none subNode can be null"); }
                this.cluster = cluster;

                if (leftSubNode == null) {
                    // Node is leaf
                    this.splittingNumber = int.MaxValue;
                } else {
                    if (leftSubNode.SplittingNumber > splittingNumber
                        && rightSubNode.SplittingNumber > splittingNumber) {
                        this.splittingNumber = splittingNumber;
                    } else {
                        // Erroneous dendogramm. Will not result in partial ordered tree
                        throw new ArgumentException("SplittingNumber of both SubNodes must be greater than current splittingNumber");
                    }
                }

                this.leftSubNode = leftSubNode;
                this.rightSubNode = rightSubNode;
            }

            /// <summary>
            /// Creates a new Node with the given parameters. Either both or none of the sub-nodes must be null.
            /// </summary>
            /// <param name="cluster">A Cluster for this Node</param>
            /// <param name="splittingNumber">The splittingNumber defines, how the dendogramm
            /// will be linearized later. While searching for a ClusterList with size t, the current Node
            /// will be split up if t is greater or equal than the current splittingNumber.
            /// If Node is leaf (both subNodes are null) splittingNumber is ignored and set to int.MaxValue.
            /// Take Care: the root should get the a SplittingNumber of 2</param>
            /// <param name="leftSubNode">A valid Node or null for a leaf</param>
            /// <param name="rightSubNode">A valid Node or null for a leaf</param>
            /// <param name="pointIndices">Array with indices. If Two nodes are combined, Array of indices is
            /// also combined</param>
            /// <exception cref="ArgumentException">If splittingNumber is not greater than the
            /// splittingNumber of both Sub-Nodes</exception>
            /// <exception cref="ArgumentException">If excactly one subNode is null</exception>
            public Node(Cluster cluster, int splittingNumber, Node leftSubNode, Node rightSubNode, int[] pointIndices)
                : this (cluster, splittingNumber, leftSubNode, rightSubNode) {
                this.pointIndices = pointIndices;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Gives access to the variable number of clusters resulting from a horizontal cut
            /// through the dendogramm.
            /// </summary>
            /// <param name="currentSplittingNumber">The current splitting number</param>
            /// <returns>An IEnumerable of Cluster that contains all Clusters of an horizontal cut
            /// through the dendogramm of the given Node. <paramref name="currentSplittingNumber"/>
            /// determines how deep the Tree will be traced to receive Clusters</returns>
            public IEnumerable<Cluster> GetClusters(int currentSplittingNumber) {
                if ((currentSplittingNumber >= this.splittingNumber) && (leftSubNode != null) && (rightSubNode != null)) {
                    foreach (Cluster cluster in leftSubNode.GetClusters(currentSplittingNumber)) {
                        yield return cluster;
                    }
                    foreach (Cluster cluster in rightSubNode.GetClusters(currentSplittingNumber)) {
                        yield return cluster;
                    }
                } else {
                    yield return cluster;
                }
            }

            /// <summary>
            /// Overrides the ToString method to return the Clusters label.
            /// </summary>
            /// <returns>Label of the Cluster in this Node</returns>
            public override String ToString() {
                return cluster.Label;
            }

            #endregion
        }

        #endregion
    }
}
