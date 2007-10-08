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

namespace Pavel.Framework {
    /// <summary>
    /// A set of PointLists that share a common subcolumnSet
    /// </summary>
    [Serializable]
    public class PointSet : IEnumerable<Point> {

        #region Fields

        protected String label;
        protected ColumnSet  columnSet;
        protected List<PointList> pointLists = new List<PointList>();
        private bool locked;

        [NonSerialized]
        private Stack<Dictionary<PointList,Point[]>> undoSteps = new Stack<Dictionary<PointList,Point[]>>();

        /// <value> Event fired if the PointSet has been modified. </value>
        [field: NonSerializedAttribute()]
        public event PointSetModifiedEventHandler pointSetModified;

        #endregion

        #region Properties

        /// <value> Gets the value of locked </value>
        public bool Locked { get { return locked; } }

        /// <value> Gets the label of the PointSet or sets it </value>
        public String Label {
            [CoverageExclude]
            get { return label; }
            [CoverageExclude]
            set { label = value; }
        }

        /// <value> Gets the subcolumnSet common to all PointLists in this PointSet</value>
        public ColumnSet ColumnSet {
            [CoverageExclude]
            get { return columnSet; }
        }

        /// <value> Gets the number of Points in the PointSet, accumulated from all PointLists </value>
        public int Length {
            get {
                int length = 0;
                foreach (PointList pl in pointLists)
                    length += pl.Count;
                return length;
            }
        }

        /// <value> Gets the PointLists contained in this PointSet. </value>
        /// <remarks>
        /// WARNING: Do not manipulate the returned list of PointLists directly!
        /// Use PointSet.Add(PointList) etc.
        /// 
        /// This property might become obsolete in the future.
        /// </remarks>
        public List<PointList> PointLists {
            [CoverageExclude]
            get { return this.pointLists; }
        }

        /// <value> Gets a Point in the PointSet by index </value>
        /// <remarks>
        /// WARNING: Although it's unlikely, it's possible that accessing the indexer this way
        /// returns different points for the same index if the Pointlists were changed in the meantime.
        /// The PointSetModified event is fired if this happens
        /// </remarks>
        public Point this[int index] {
            get {
                if (index >= Length)
                    throw new IndexOutOfRangeException();

                int currentPointList = 0;
                foreach (PointList pl in pointLists) {
                    if (index - pl.Count < 0)
                        break;
                    else {
                        index -= pl.Count;
                        currentPointList++;
                    }
                }
                return pointLists[currentPointList][index];
            }
        }

        /// <value> Gets the number of possible Undos for deleting Points </value>
        public int UndoSteps {
            get {
                if (undoSteps == null) { undoSteps = new Stack<Dictionary<PointList, Point[]>>(); }
                return undoSteps.Count;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new PointSet with a given label and an initial ColumnSet.
        /// </summary>
        /// <param name="label">Label for the PointSet</param>
        /// <param name="columnSet">ColumnSet for the PointSet</param>
        public PointSet(String label, ColumnSet columnSet) {
            this.label = label;
            this.columnSet = columnSet;
            this.locked = false;
        }

        /// <summary>
        /// Creates a new PointSet with a given Label and an initial ColumnSet.
        /// <paramref name="locked"/> can be used to prevent manipulation.
        /// </summary>
        /// <param name="label">Label for the PointSet</param>
        /// <param name="columnSet">ColumnSet for the PointSet</param>
        /// <param name="locked">locked flag is set to this value</param>
        public PointSet(String label, ColumnSet columnSet, bool locked) {
            this.label = label;
            this.columnSet = columnSet;
            this.locked = locked;
        }

        #endregion

        #region Methods

        #region PointList Manipulation

        /// <summary>
        /// Adds a new PointList to this PointSet.
        /// </summary>
        /// <param name="pointList">The PointList to insert</param>
        /// <exception cref="ApplicationException">If the Pointlist doesn't have a supercolumnSet of this PointSets ColumnSet</exception>
        public void Add(PointList pointList) {
            if (columnSet.IsSubSetOf(pointList.ColumnSet))
                pointLists.Add(pointList);
            else
                throw new ApplicationException("Trying to insert Pointlist that doesn't have a supercolumnSet of the PointSet");
            if ( null != pointSetModified ) { pointSetModified(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Removes a PointList from this PointSet.
        /// </summary>
        /// <param name="pointList">PointList to be removed</param>
        /// <returns>true if removed, false if pointList was not in PointSet</returns>
        public bool Remove(PointList pointList) {
            bool removed = pointLists.Remove(pointList);
            if ( removed && null != pointSetModified ) { pointSetModified(this, EventArgs.Empty); }
            return removed;
        }

        #endregion

        #region Point Manipulation

        /// <summary>
        /// Deletes all selected Points from the PointSet.
        /// Stores deleted points as an UndoStep.
        /// </summary>
        public void DeleteSelectedPoints() {
            //Stores which point was deleted from which PointList
            Dictionary<PointList, Point[]> removed = new Dictionary<PointList, Point[]>();
            foreach ( PointList pl in this.pointLists ) {
                List<Point> removedPointsInPointList = new List<Point>();
                for ( int i = pl.Count - 1; i >= 0; i-- ) {
                    //The last point in a pointset cannot be removed -> cause to many problems
                    if ( this.Length == 1 ) { break; }
                    if ( ProjectController.CurrentSelection.Contains(pl[i]) ) {
                        removedPointsInPointList.Add(pl[i]);
                        pl.RemoveAt(i);
                    }
                }
                if ( removedPointsInPointList.Count > 0 ) {
                    removed.Add(pl, removedPointsInPointList.ToArray());
                }
            }
            if ( removed.Count > 0 ) {
                if ( undoSteps == null ) { undoSteps = new Stack<Dictionary<PointList, Point[]>>(); }
                undoSteps.Push(removed);
                if ( this.pointSetModified != null ) { this.pointSetModified(this, new EventArgs()); }
                ProjectController.CurrentSelection.Clear();
            }
            ProjectController.SetProjectChanged(true);
        }

        /// <summary>
        /// Undos the last deleting operation if there is any.
        /// </summary>
        public void Undo() {
            if ( undoSteps == null ) { undoSteps = new Stack<Dictionary<PointList, Point[]>>(); }
            if ( undoSteps.Count > 0 ) {
                //Add all previously deleted points back into their pointList
                //The Pop-operation removes the undostep directly
                foreach ( KeyValuePair<PointList, Point[]> plKeyValuePair in undoSteps.Pop() ) {
                    plKeyValuePair.Key.AddRange(plKeyValuePair.Value);
                }

                if ( this.pointSetModified != null ) { this.pointSetModified(this, new EventArgs()); }
            }
            ProjectController.SetProjectChanged(true);
        }

        #endregion

        #region MinMaxMean

        /// <summary>
        /// Calculates Min/Max/Mean for all Points in this PointSet.
        /// </summary>
        /// <returns>An array of Points in this PointSets ColumnSet, containing the min/max/mean values for each Column.</returns>
        [CoverageExclude]
        public Point[] MinMaxMean() {
            return MinMaxMean(this.columnSet);
        }

        /// <summary>
        /// Calculates Min/Max/Mean for all Points in this PointSet and trims
        /// the resulting Min/Max/Mean Points to the given ColumnSet.
        /// </summary>
        /// <param name="columnSet">ColumnSet to trim the result to</param>
        /// <returns>An array of Points in the given ColumnSet, containing the min/max/mean values for each Column over this PointSet.</returns>
        public Point[] MinMaxMean(ColumnSet columnSet) {
            //Stores the Min/Mean/Max Values of every PointList, trimmed to this PointSets
            //columnSet and converted to an array
            double[][] pointlist_min   = new double[pointLists.Count][];
            double[][] pointlist_max   = new double[pointLists.Count][];
            double[][] pointlist_mean  = new double[pointLists.Count][];
            //Stores the number of Points in each PointList
            int[]      pointlist_count = new int[pointLists.Count];

            //Fill the pointlist _min, _max and _mean
            for (int pli = 0; pli < pointLists.Count; pli++ ) {
                //Calculate Min/Max/Mean for the PointList
                Point[] minMaxMean  = pointLists[pli].MinMaxMean();

                //Calculate the ColumnSetMap mapping the PointLists ColumnSet to this PointSets columnSet
                int[]   columnSetMap = columnSet.SuperSetMap(pointLists[pli].ColumnSet);

                //Trim and store the Min/Max/Mean Points
                pointlist_min[pli]  = minMaxMean[Result.MIN] .Trim(columnSet, columnSetMap).Values;
                pointlist_max[pli]  = minMaxMean[Result.MAX] .Trim(columnSet, columnSetMap).Values;
                pointlist_mean[pli] = minMaxMean[Result.MEAN].Trim(columnSet, columnSetMap).Values;
                
                //Save the number of points in the PointList, needed for calculating the means
                pointlist_count[pli] = pointLists[pli].Count;
            }

            // NOW CALCULATE THE TOTAL MIN/MAX/MEAN FOR THIS POINTSET

            //These will be holding the total min/max/mean values
            double[] total_min  = new double[columnSet.Dimension];
            double[] total_max  = new double[columnSet.Dimension];
            double[] total_mean = new double[columnSet.Dimension];
            //The total number of points in this PointSet
            int total_count = this.Length;

            //Iterate over all Columns in this.columnSet
            for (int ci = 0; ci < total_min.Length; ci++) {
                total_min[ci]  = double.PositiveInfinity;
                total_max[ci]  = double.NegativeInfinity;
                total_mean[ci] = 0;
                //Iterate over the results from each PointList
                for (int pli = 0; pli < pointlist_min.Length; pli++) {
                    if (0 == pointlist_count[pli]) continue;
                    total_min[ci]   = Math.Min(total_min[ci], pointlist_min[pli][ci]);
                    total_max[ci]   = Math.Max(total_max[ci], pointlist_max[pli][ci]);
                    total_mean[ci] += pointlist_mean[pli][ci] * pointlist_count[pli];
                }
                total_mean[ci] = total_mean[ci] / total_count;

            }
            return new Point[] {
                new Point(columnSet, total_min),
                new Point(columnSet, total_max),
                new Point(columnSet, total_mean)
            };
        }
        #endregion

        /// <summary>
        /// Creates a copy of this PointSet which contains only the Points 
        /// whose Columns are all bounded inside the boundaries of the ColumnProperties of the given Space.
        /// </summary>
        /// <param name="space">The Space containing the ColumnProperties</param>
        /// <returns>PointSet without Points outside the ColumnProperties of the Space</returns>
        public PointSet CreateFilteredPointSet(Space space) {
            ColumnSet columnSet = space.ToColumnSet();
            if ( !columnSet.IsSubSetOf(this.columnSet) ) { throw new ApplicationException("Space is not a subcolumnSet of this PointSet"); }

            PointSet pointSet = new PointSet("Filtered PointSet", columnSet);

            for ( int plIndex = 0; plIndex < pointLists.Count; plIndex++ ) {
                int[] map = space.CalculateMap(pointLists[plIndex].ColumnSet);
                PointList pointList = new PointList((pointLists[plIndex]).ColumnSet);
                for ( int index = 0; index < pointLists[plIndex].Count; index++ ) {
                    Point p = pointLists[plIndex][index];
                    bool pointValid = true;
                    //scaledValue normalizes all point according to a space's ColumnProperties
                    //If a scaledvlaue is smaller than 0 or bigger than 1, then it is otside the boundaries
                    //and thus must no be copied int o the new filtered Pointset
                    double[] scaledValues = p.ScaledValues(space);
                    for ( int i = 0; i < scaledValues.Length; i++ ) {
                        if ( scaledValues[i] < 0 || scaledValues[i] > 1 ) {
                            pointValid = false;
                            break;
                        }
                    }
                    if ( pointValid ) { pointList.Add(p); }
                }
                pointSet.Add(pointList);
            }
            return pointSet;
        }

        /// <summary>
        /// Overrides the ToString() method to return the label of this PointSet.
        /// </summary>
        /// <returns>This PointSets label</returns>
        [CoverageExclude]
        public override String ToString() {
            return label;
        }

        #region Enumerators

        /// <summary>
        /// An Enumerator over the Points contained in the PointSet.
        /// </summary>
        /// <remarks>
        /// WARNING: This might be pretty slow and should not be used where performance is needed
        /// Prefer iterating over the PointLists in the PointSet.
        /// </remarks>
        public IEnumerator<Point> GetEnumerator() {
            foreach (PointList pl in pointLists) {
                for (int i = 0; i < pl.Count; i++) {
                    yield return pl[i];
                }
            }
        }

        /// <summary>
        /// Returns an Enumerator over the Points contained in the PointSet.
        /// </summary>
        /// <returns>An Enumerator over the Points contained in the PointSet</returns>
        [CoverageExclude]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// EventHandler for PointSet modification.
        /// </summary>
        /// <param name="sender">This PointSet</param>
        /// <param name="e">Standard EventArgs</param>
        public delegate void PointSetModifiedEventHandler(object sender, EventArgs e);

        #endregion
    }
}
