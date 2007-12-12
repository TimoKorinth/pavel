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
using System.Runtime.Serialization;

namespace Pavel.Clustering {
    [Serializable()]
    /// <summary>
    /// One group of Points in a ClusterSet.
    /// </summary>
    [CoverageExclude]
    public class Cluster : Point {

        #region Fields

        protected String label;
        protected PointSet pointSet;

        #endregion

        #region Properties

        /// <value> Gets the label of this Cluster or sets it </value>
        public String Label {
            get { return this.label; }
            set { this.label = value; }
        }

        /// <value> Gets the PointSet that holds the connected Points of the Cluster or sets it </value>
        public PointSet PointSet {
            get { return this.pointSet; }
            set { this.pointSet = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Cluster.
        /// Usually the DoClustering Method of any ClusteringAlgorithm-Instance creates the ClusterSet
        /// with all Clusters
        /// </summary>
        /// <param name="label">A label for this Cluster</param>
        /// <param name="columnSet">ColumnSet for the pointSet</param>
        /// <param name="values">Values of the Points</param>
        public Cluster(String label, ColumnSet columnSet, double[] values)
            : base(columnSet, values) {
            this.label = label;
        }

        /// <summary>
        /// Creates a new Cluster from a Point.
        /// </summary>
        /// <param name="label">A label for this Cluster</param>
        /// <param name="p">A single Point</param>
        public Cluster(String label, Point p)
            : base(p.ColumnSet, p.Values) {
            this.label = label;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reassignes the values of the Clustercenter.
        /// </summary>
        /// <param name="p">Point containing the new values</param>
        public void SetValues(Point p) {
            this.values = p.Trim(this.columnSet).Values.Clone() as double[];
        }

        /// <summary>
        /// Returns a manipulated label.
        /// </summary>
        /// <returns>Manipulated label</returns>
        public override string ToString() {
            string values = "";
            foreach (double d in this.values) {
                values += d.ToString() + ", ";
            }
            return this.label.ToString() + " " + values + " PointSet-Length: " + this.PointSet.Length
                + " PointLists-Length: " + this.PointSet.PointLists.Count
                + " PointLists[0].Length: " + this.PointSet.PointLists[0].Count;
        }

        #endregion
    }
}
