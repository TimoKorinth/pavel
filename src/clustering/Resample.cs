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
    [Serializable()]
    /// <summary>
    /// Resampling
    /// TODO: Complete summary
    /// </summary>
    public class Resample : ClusteringAlgorithm {

        private int numberOfClusters = 1000;

        /// <value>Gets the number of clusters or set it</value>
        [Spinner("Resolution",
                "Specifies the resolution for each dimension.",
                1.0, 1000000.0, 0, 10)]
        public int NumberOfClusters {
            get { return numberOfClusters; }
            set { numberOfClusters = value; }
        }

        protected override PointList DoClustering() {
            //TODO: Behandlung der mehrdimensionalitaet
            //TODO: Mehrdimensional, mit verschiedener Dichte pro Dimension
            //TODO: Bei Petras Datensatz verschwinden obere und untere Zeilen

            ColumnProperty[] cp = RelevantSpace.ColumnProperties;
            double stepsize_x = (cp[0].Max - cp[0].Min) / NumberOfClusters;
            double stepsize_y = (cp[1].Max - cp[1].Min) / NumberOfClusters;

            PointList[] pointLists = new PointList[NumberOfClusters*NumberOfClusters];
            for (int i = 0; i < pointLists.Length; i++) {
                pointLists[i] = new PointList(ColumnSet);
            }

            //TODO: Slow and buggy, replace with double loops
            foreach (Point p in PointSet) {
                int place_x = (int)((p[cp[0].Column] - cp[0].Min) / stepsize_x);
                int place_y = (int)((p[cp[1].Column] - cp[1].Min) / stepsize_y);
                if (0 < place_x && 0 < place_y && place_x < NumberOfClusters && place_y < NumberOfClusters) {
                    pointLists[place_x + NumberOfClusters*place_y].Add(p);
                }
            }

            PointList clusters = new PointList(ColumnSet);
            for (int i = 0; i < NumberOfClusters*NumberOfClusters; i++) {
                if (0 == pointLists[i].Count) continue;
                Cluster c = new Cluster("Cluster" + i.ToString(), pointLists[i].MinMaxMean()[Result.MEAN]);
                c.PointSet = new PointSet("Cluster " + i.ToString() + "PointSet", ColumnSet,false);
                c.PointSet.Add(pointLists[i]);
                clusters.Add(c);
            }

            return clusters;

        }

        public override string ToString() { return "Resample"; }

    }
}
