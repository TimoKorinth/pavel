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
// For more information and contact details visit http://www.sourceforge.net/projects/pavel

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
    public class MLA : ClusteringAlgorithm  {
        #region Fields

        // Specific MLA-Clustering-Arguments
        private int numberOfClusters = 10;
        private int randomSeed = (int)DateTime.Now.Ticks;

        #endregion

        #region Properties

        /// <value>Gets the number of clusters or set it</value>
        [Spinner("Number of Clusters",
                "Specifies the number of clusters.",
                1.0, 1000000.0, 0, 1)]
        public int NumberOfClusters {
            get { return numberOfClusters; }
            set { numberOfClusters = value; }
        }

        /// <value>Gets the random seed or set it</value>
        [Spinner("Random Seed",
                "Initializes the Clustering-Algorithm-Random-Generator. Use for repeatable behaviour",
                int.MinValue, int.MaxValue, 0, 1)]
        public int RandomSeed {
            get { return randomSeed; }
            set { randomSeed = value; }
        }

        #endregion

        #region Methods

        #region DoClustering

        /// <summary>
        /// k-means runns iteratively untile some aborting criterion is met.
        /// It assignes all points to the nearest initial cluster (number is given)
        /// and recomputes cluster-centers. Then the procedure iterates.
        /// </summary>
        /// <returns>The PointList with Clusters</returns>
        protected override PointList DoClustering() {
            PointList clusterList = new PointList(ColumnSet);

            //Check:
            if (PointSet.Length < NumberOfClusters) {
                ErrorMessage = "Number of Clusters is greater than the size of the PointSet!";
                return null;
            }

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
            for (int i = 1; i < NumberOfClusters; i++) {
                // Compare all Points to all previous centers
                double max = double.MinValue;
                int maxIndex = 0;
                for (int k = 0; k < PointSet.Length; k++) {
                    if (r[k] != -1) {
                        SignalProgress(i / NumberOfClusters * 1000, "Compute centers");
                        double min = double.MaxValue;
                        int minIndex = 0;
                        // All centers
                        for (int j = 0; j < i; j++) {
                            double dist = Point.Distance(ScaledFlatData[k], ScaledFlatData[j]);
                            if (dist < min) {
                                min = dist;
                                minIndex = k;
                            }
                        }
                        if (min > max) {
                            max = min;
                            maxIndex = minIndex;
                        }
                    }
                }
                centers[i] = maxIndex;
                r[centers[i]] = -1;
            }

            //foreach (int center in centers) {
            //    PointSet ps = 
            //    Cluster cluster = new Cluster("", null);
            //    clusterList.Add(cluster);
            //}

            return clusterList;
        }

        #endregion

        /// <summary>
        /// Creates a List of random values.
        /// </summary>
        /// <param name="r">Random-Generator</param>
        /// <param name="count">Count of randoms</param>
        /// <param name="min">Minimum Value (included)</param>
        /// <param name="max">Maximum Value (included)</param>
        /// <returns></returns>
        public static int[] CreateDifferentRandoms(Random r, int count, int min, int max) {
            if (count < 1 || min < 0 || max < min) throw new ArgumentException("IllegalArguments: Count=" + count + " Min=" + min + " Max=" + max);
            if (count > (max - min + 1)) throw new ArgumentException("Not enought range for unique Randoms");

            int[] nums = new int[count];
            bool ok;
            for (int i = 0; i < count; i++) {
                do {
                    ok = true;
                    nums[i] = r.Next(min, max);
                    for (int j = 0; j < i; j++) {
                        if (nums[j] == nums[i]) { ok = false; }
                    }
                } while (!ok);
            }
            return nums;
        }

        /// <summary>
        /// Overrides the ToString method to return "K-Means".
        /// </summary>
        /// <returns>"K-Means"</returns>
        public override string ToString() { return "MLA"; }

        #endregion
    }
}
