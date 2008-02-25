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
using System.ComponentModel;
using Pavel.Framework;

namespace Pavel.Clustering {
    /// <summary>
    /// A ClusterSet is a special PointSet with Clusters instead of Points
    /// </summary>
    [Serializable()]
    [CoverageExclude]
    public class ClusterSet : PointSet {

        #region Fields

        private ClusteringAlgorithm clusteringAlgorithm;
        private PointSet            basicPointSet;

        #endregion

        #region Properties

        /// <value> Gets the ClusteringArgs that have been used to create this clustering </value>
        public ClusteringAlgorithm ClusteringAlgorithm { get { return clusteringAlgorithm; } }

        /// <value> Gets the PointSet that has been clustered </value>
        public PointSet BasicPointSet { get { return this.basicPointSet; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new ClusterSet.
        /// Usually Clusterings are not created directly but by calling the DoClustering method
        /// of any ClusteringAlgorithm instance;
        /// </summary>
        /// <param name="ca">The Arguments that have been used to create the ClusterSet</param>
        public ClusterSet(ClusteringAlgorithm ca) : base(ca.Name, ca.PointSet.ColumnSet) {
            this.basicPointSet = ca.PointSet;
            this.clusteringAlgorithm = ca;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new PointSet containing the points from which clusters are created
        /// </summary>
        /// <param name="name">name of new PointSet</param>
        /// <returns>PointSet of the point from which custers are created</returns>
        public PointSet PointSetFromClusters(String name) {
            ColumnSet cs = this.basicPointSet.ColumnSet;
            PointSet clusterPointSet = new PointSet(name, cs);

            for (int i = 0; i < this.Length; i++) {
                PointSet ps_in_cluster = (this[i] as Cluster).PointSet;
                int[] map = cs.SuperSetMap(ps_in_cluster.ColumnSet); //TODO: Unclear wether the map needs to be reconstructed for every ps in the cluster. Probably not.
                for (int j = 0; j < ps_in_cluster.Length; j++) {
                    clusterPointSet.Add(ps_in_cluster[j].Trim(cs, map));
                }
            }
			
            return clusterPointSet;
        }

        #endregion
    }
}
