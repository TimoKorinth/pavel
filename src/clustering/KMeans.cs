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
    /// K-Means-Clustering
    /// </summary>
    public class KMeans : ClusteringAlgorithm {

        #region Fields

        // Specific K-Means-Clustering-Arguments
        private int numberOfClusters = 10;
        private double convergionBound = 0.0;
        private int maximumIterations = 50;
        private int randomSeed = (int)DateTime.Now.Ticks;
        private bool smartRepair = true;

        #endregion

        #region Properties

        /// <value>Gets the number of clusters or set it</value>
        [Spinner("Number of Clusters",
                "Specifies the number of clusters. Be aware: Runtime is O(#cluster * #points)",
                1.0, 1000000.0, 0, 1)]
        public int NumberOfClusters {
            get { return numberOfClusters; }
            set { numberOfClusters = value; }
        }

        /// <value>Gets convergion bound or set it</value>
        [Spinner("Convergion bound",
                "Specifies a bound for the accumulated changeing of clusterCenters under that the iteration stops",
                0.0, 1000000.0, 2, 1)]
        public double ConvergionBound {
            get { return convergionBound; }
            set { convergionBound = value; }
        }

        /// <value>Gets the maximum number of iterations or set it</value>
        [Spinner("Maximum iterations",
                "Specifies how many iterations are performed while the algorithm has not converged",
                1, 1000000, 0, 1)]
        public int MaximumIterations {
            get { return maximumIterations; }
            set { maximumIterations = value; }
        }

        /// <value>Gets the random seed or set it</value>
        [Spinner("Random Seed",
                "Initializes the Clustering-Algorithm-Random-Generator. Use for repeatable behaviour",
                int.MinValue, int.MaxValue, 0, 1)]
        public int RandomSeed {
            get { return randomSeed; }
            set { randomSeed = value; }
        }

        /// <value>Gets the SmartRepair-Option or sets it</value>
        [CheckBox("Smart Repair", "Avoids empty clusters and thus Number-Of-Cluster mismatches." +
            "On empty Clusters these are moved to Regions where Points are far away from Clustercenters.")]
        public bool SmartRepair {
            get { return smartRepair; }
            set { smartRepair = value; }
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
            //Check:
            if (PointSet.Length < NumberOfClusters) {
                ErrorMessage = "Number of Clusters is greater than the size of the PointSet!";
                return null;
            }

            PointList clusterList = CreateInitialClusterList();

            int iteration = 0;          // iteration counter
            double change = 0.0;        // modification in last iteration
            SortedList<double, Point> mostFarestPoints = new SortedList<double, Point>(numberOfClusters);
                
            do {
                //Clear all Clusters
                foreach (Cluster cluster in clusterList) {
                    // New, empty PointSet that is structured like the original PointSet
                    cluster.PointSet = new PointSet(cluster.Label, PointSet.ColumnSet);
                    foreach (PointList pl in PointSet.PointLists) {
                        PointList newPointList = new PointList(pl.ColumnSet);
                        cluster.PointSet.Add(newPointList);
                    }
                }
                mostFarestPoints.Clear();

                //Assign Points to centers
                int progress = 0;

                int[] map = RelevantSpace.CalculateMap(ColumnSet);
                for (int listIndex = 0; listIndex < PointSet.PointLists.Count; listIndex++) { // Lists
                    for (int i = 0; i < PointSet.PointLists[listIndex].Count; i++) { // Point in Lists
                        double min = double.PositiveInfinity;
                        Cluster minCluster = null;  // local closest Cluster
                        
                        for (int j = 0; j < clusterList.Count; j++) {
                            double distance = Point.Distance(ScaledData[listIndex][i], clusterList[j].ScaledValues(RelevantSpace, map));
                            if (distance < min) {
                                min = distance;
                                minCluster = clusterList[j] as Cluster;
                            }
                        }
                        minCluster.PointSet.PointLists[listIndex].Add(PointSet.PointLists[listIndex][i]);
                        // check for most far-out Point
                        if (smartRepair && (mostFarestPoints.Count < numberOfClusters || min > mostFarestPoints.Keys[0])) {
                            // delete closest
                            if (mostFarestPoints.Count >= 10) { mostFarestPoints.RemoveAt(0); }
                            mostFarestPoints[min] = PointSet.PointLists[listIndex][i];
                        }

                        progress++;
                        if (progress % 1000 == 0) {
                            //Report
                            SignalProgress((int)((1000 / MaximumIterations) * (iteration + ((double)progress / (double)PointSet.Length))),
                                "Last Change: " + (float)change + " - Assign Points to Clusters. Iteration: " + iteration + " Points: " + progress);
                        }
                    }
                }

                //Recomputer Centers
                change = 0.0;
                for (int i = 0; i < clusterList.Count; i++) {
                    // Empty Cluster?
                    if (((Cluster)clusterList[i]).PointSet.Length == 0) {
                        if (smartRepair && mostFarestPoints.Count > 0) {
                            (clusterList[i] as Cluster).SetValues(mostFarestPoints.Values[mostFarestPoints.Count - 1]);
                            mostFarestPoints.RemoveAt(mostFarestPoints.Count - 1);
                        }
                        continue;
                    }
                    Point mean = (clusterList[i] as Cluster).PointSet.MinMaxMean(ColumnSet)[Result.MEAN];

                    change += Point.Distance(mean, clusterList[i]);
                    //write center
                    (clusterList[i] as Cluster).SetValues(mean);
                }

                iteration++;
            } while (iteration < MaximumIterations && change > ConvergionBound && !SaveAbortRequested);

            return clusterList;
        }

        #endregion

        /// <summary>
        /// Creates with the given global Clusteringarguments random initial
        /// Clusters
        /// </summary>
        /// <returns>A ClusterList with random initial Clusters</returns>
        protected virtual PointList CreateInitialClusterList() {
            //Report
            SignalProgress(0, "Initialize Clusters by random");

            PointList clusterList = new PointList(ColumnSet);

            //Create Clusters
            Random r = new Random(RandomSeed);
            int[] randoms = CreateDifferentRandoms(r, NumberOfClusters, 0, PointSet.Length);
            Array.Sort(randoms);
            int pointIndex = 0;
            int randomIndex = 0;
            foreach (Point point in PointSet) {
                if (pointIndex == randoms[randomIndex]) {
                    Cluster c = new Cluster(randomIndex.ToString(), point.Trim(ColumnSet));
                    clusterList.Add(c);
                    randomIndex++;
                    if (randomIndex == randoms.Length)
                        break;
                }
                pointIndex++;
            }

            return clusterList;
        }
        
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
        public override string ToString() { return "K-Means"; }

        #endregion
    }
}
