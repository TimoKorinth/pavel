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
    /// Eventhandler for signal progress
    /// </summary>
    /// <param name="progress">The progress of the signal</param>
    /// <param name="message">The displayed message</param>
    public delegate void SignalProgressEventHandler(int progress, string message);
        
    /// <summary>
    /// Abstract Class for all Clustering-Algorithms.
    /// </summary>
    [Serializable]
    public abstract class ClusteringAlgorithm {

        #region Fields

        /// <value> Event for messages while clustering </value>
        public event SignalProgressEventHandler ProgressChanged;

        private string errorMessage = null;
        private bool saveAbortRequest;

        // Generated Data:
        private ColumnSet columnSet;
        private Space relevantSpace;
        private double[][][] scaledData;
        private double[][] scaledFlatData;


        // Clustering Args:
        private PointSet pointSet;
        private Space space;
        private bool[] relevantColumns;
        private String name;

        #endregion

        #region Properties

        /// <value>
        /// Gets the errorMessage or sets it.
        /// </value>
        /// <remarks>
        /// Only ClusteringAlgorithms can set an Error-Message and should exit their
        /// DoClustering-Method with a return value of "null".
        /// Anyone can get the ErrorMessage.
        /// </remarks>
        public string ErrorMessage {
            get { return this.errorMessage; }
            protected set { this.errorMessage = value; }
        }


        /// <value>
        /// Gets the saveAbortRequest or sets it.
        /// </value>
        /// <remarks>
        /// Is true if Application asks Clustering-Tread to finish current run with
        /// meaningful result.
        /// </remarks>
        protected bool SaveAbortRequested {
            get { return saveAbortRequest; }
            private set { saveAbortRequest = value; }
        }

        /// <value>
        /// Derived classes can override the get-method for ArgumentControl and return a valid Control.
        /// This way it should be possible to manipulate any algorithm-specific Clustering-Arguments.
        /// </value>
        public ClusteringArgumentControl ArgumentControl {
            get { return new ClusteringArgumentControl(this); }
        }

        /// <value>
        /// Gets the ColumnSet of the given Space. Will be set in Start() and contain space.ToColumnSet()
        /// </value>
        protected ColumnSet ColumnSet {
            get { return columnSet; }
            private set { columnSet = value; }
        }

        /// <value> Gets a copy of the Space in which all nonrelavant Columns are removed or sets it </value>
        protected Space RelevantSpace {
            get { return relevantSpace; }
            private set { relevantSpace = value; }
        }

        /// <value>
        /// A PointSet-like array that holds in the first dimension the PointLists, 
        /// in the second dimension the Points and in the third dimension the values. Thus it
        /// can be used as an alternative to the original PointSet with the difference that
        /// it holds scaled values (in the context of the given Space) and all Columns of
        /// the Space.
        /// </value>
        protected double[][][] ScaledData {
            get { return scaledData; }
            private set { scaledData = value; }
        }

        /// <value>
        /// A PointSet-like array that holds in the first dimension the Points and in the second dimension the values. Thus it
        /// can be used as an alternative to the original PointSet with the difference that
        /// it holds scaled values (in the context of the given Space) and all Columns of
        /// the Space.
        /// </value>
        protected double[][] ScaledFlatData {
            get { return scaledFlatData; }
            private set { scaledFlatData = value; }
        }

        /// <value> Gets the basic PointSet or sets it </value>
        public PointSet PointSet {
            get { return pointSet; }
            set { pointSet = value; }
        }

        /// <value> Gets the Space for clustering or sets it. </value>
        public Space Space {
            get { return space; }
            set { space = value; }
        }

        /// <value>Gets a selection of relevant Columns that will be used for distance-computation or sets it</value>
        public bool[] RelevantColumns {
            get { return relevantColumns; }
            set { relevantColumns = value; }
        }

        /// <value> Gets the name of the resulting ClusterSet or sets it </value>
        public String Name {
            get { return name; }
            set { name = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Displayname for the ClusteringAlgorithm
        /// </summary>
        public abstract override string ToString();

        /// <summary>
        /// Notifies listeners of the ProgressChanged event.
        /// </summary>
        /// <param name="progress">A value between 0 and 1000 (one-tenth of a percent)</param>
        /// <param name="message">Any message</param>
        /// <exception cref="ArgumentOutOfRangeException">If progress is not in range</exception>
        protected void SignalProgress(int progress, string message) {
            if (progress < 0 || progress > 1000) {
                throw new ArgumentOutOfRangeException("Progress must be between 0 and 1000");
            }
            if (ProgressChanged != null) ProgressChanged(progress, message);
        }

        /// <summary>
        /// Abstract-Clustering-Engine
        /// Return null if Clustering was canceled
        /// </summary>
        /// <returns>The resulting PointList</returns>
        protected abstract PointList DoClustering();

        /// <summary>
        /// Startingpoint for external starting of clustering.
        /// Preprocesses and checks data before clustering it.
        /// </summary>
        /// <returns>A valid ClusterSet if successful or null for abort. Read ErrorMessage
        /// for reason</returns>
        public ClusterSet Start() {
            ColumnSet = space.ToColumnSet();
            RelevantSpace = space.CreateLocalCopy();
            // Remove unrelevant Columns
            for (int i = space.Dimension - 1; i >= 0; i--) {
                if (RelevantColumns[i] == false) {
                    RelevantSpace.RemoveColumn(i);
                }
            }
            if (RelevantSpace.Dimension == 0) {
                ErrorMessage = "No relevant Column for clustering found";
                return null;
            }

            SignalProgress(0, "Preprocessing Data...");
            try {
                ScaledData = CreateScaledAndShrinkedData(PointSet, RelevantSpace);
                ScaledFlatData = CopyToFlatData(ScaledData);
            } catch (OutOfMemoryException e) {
                ErrorMessage = "Not enough memory for clustering. " + e.Message;
                return null;
            }
            
            // The actual clustering
            PointList clusterList = DoClustering();
            if (clusterList != null) {
                ClusterSet clusterSet = new ClusterSet(this);
                clusterSet.Add(clusterList);
                return clusterSet;
            } else {
                // Any Failure occured
                return null;
            }
        }

        /// <summary>
        /// Registers wish to abort Clustering with meaningful intermediate-result.
        /// Time until termination can not be guaranteed.
        /// </summary>
        public void InterruptClustering() {
            SaveAbortRequested = true;
        }

        /// <summary>
        /// Creates a PointSet-like array that holds in the first dimension the PointLists, 
        /// in the second dimension the Points and in the third dimension the values. Thus it
        /// can be used as an alternative to the original PointSet with the difference that
        /// it holds scaled values (in the context of the given space) and all Columns of
        /// the <paramref name="space"/>.
        /// </summary>
        /// <param name="sourcePointSet">A PointSet as data-source</param>
        /// <param name="space">A Space with only relevant Columns and meaningful
        /// Column-scaling</param>
        /// <returns>A three-dimensional array with scaled and shrinked values. Its first dimension
        /// corresponds to the Pointlists. The second holds the Points and the third the values.</returns>
        private double[][][] CreateScaledAndShrinkedData(PointSet sourcePointSet, Space space) {
            double[][][] data = new double[sourcePointSet.PointLists.Count][][];

            for (int plIndex = 0; plIndex < sourcePointSet.PointLists.Count; plIndex++) {
                int[] map = space.CalculateMap(sourcePointSet.PointLists[plIndex].ColumnSet);
                data[plIndex] = new double[sourcePointSet.PointLists[plIndex].Count][];
                for (int index = 0; index < sourcePointSet.PointLists[plIndex].Count; index++) {
                    data[plIndex][index] = new double[space.Dimension];
                    for (int column = 0; column < space.Dimension; column++) {
                        data[plIndex][index][column] = sourcePointSet.PointLists[plIndex][index].ScaledValue(map[column], space.ColumnProperties[column]);
                    }
                }
            }
            return data;
        }

        private double[][] CopyToFlatData(double[][][] scaledData) {
            // Remapping of the precomputed (scaled) distances without PointList-Structure
            double[][] precomputedValues;
            precomputedValues = new double[PointSet.Length][];
            int vIndex = 0;
            for (int plIndex = 0; plIndex < PointSet.PointLists.Count; plIndex++) {
                for (int index = 0; index < PointSet.PointLists[plIndex].Count; index++) {
                    // Copy precompute (scaled) Values
                    precomputedValues[vIndex] = scaledData[plIndex][index];
                    vIndex++;
                }
            }
            return precomputedValues;
        }

        #endregion
    }
}
