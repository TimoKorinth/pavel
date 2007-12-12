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
    /// Maximum-Linkage-Clustering
    /// </summary>
    public class MLA : KMeans  {
        
        #region Methods

        protected override PointList CreateInitialClusterList() {
            //Report
            SignalProgress(0, "Initialize Clusters by maximum distance");

            PointList clusterList = new PointList(ColumnSet);

            // Remaining Indices
            int[] r = new int[PointSet.Length];
            for (int i = 0; i < r.Length; i++) {
                r[i] = i;
            }

            // Initial Center
            Random rand = new Random(RandomSeed);
            int[] centers = new int[NumberOfClusters];
            centers[0] = rand.Next(0, PointSet.Length);
            r[centers[0]] = -1;

            // Centers
            for (int clusterIndex = 1; clusterIndex < NumberOfClusters; clusterIndex++) {
                // Compare all Points to all previous centers and find
                // for each remaining Point the closest center. From these minimum
                // distances the maximum is taken as next center
                double max = double.MinValue;
                int maxIndex = 0;
                // Iterate over all remaining Points
                for (int remainingIndex = 0; remainingIndex < PointSet.Length; remainingIndex++) {
                    if (r[remainingIndex] != -1) {
                        SignalProgress(clusterIndex / NumberOfClusters * 1000, "Compute centers");
                        double min = double.MaxValue;
                        int minIndex = 0;
                        // All centers
                        for (int index = 0; index < clusterIndex; index++) {
                            double dist = Point.Distance(ScaledFlatData[remainingIndex], ScaledFlatData[index]);
                            if (dist < min) {
                                min = dist;
                                minIndex = remainingIndex;
                            }
                        }
                        // Nearest Cluster found
                        // If this nearest distance is bigger than any other minimum
                        // save as a candidat for next cluster
                        if (min > max) {
                            max = min;
                            maxIndex = minIndex;
                        }
                    }
                }
                centers[clusterIndex] = maxIndex;
                r[maxIndex] = -1;
            }

            foreach (int center in centers) {
                Cluster cluster = new Cluster(center.ToString(), PointSet[center].Trim(ColumnSet));
                clusterList.Add(cluster);
            }

            return clusterList;
        }

        /// <summary>
        /// Overrides the ToString method to return "MLA".
        /// </summary>
        /// <returns>"K-Means"</returns>
        public override string ToString() { return "MLA"; }

        #endregion
    }
}
