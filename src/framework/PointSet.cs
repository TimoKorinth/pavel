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

namespace Pavel.Framework {
    /// <summary>
    /// A set of Points that share a common subcolumnSet
    /// </summary>
    [Serializable]
    public class PointSet : IEnumerable<Point> {

        #region Fields

        protected String label;
        protected ColumnSet  columnSet;
        protected List<Point> points;
        private bool locked;

        [NonSerialized]
        private Stack<Point[]> undoSteps = new Stack<Point[]>();

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

        /// <value> Gets the subcolumnSet common to all Points in this PointSet</value>
        public ColumnSet ColumnSet {
            [CoverageExclude]
            get { return columnSet; }
        }

        /// <value> Gets the number of Points in the PointSet</value>
        public int Length {
            get { return points.Count; }
        }

        /// <value> Gets a Point in the PointSet by index </value>
        public Point this[int index] {
            get { return points[index]; }
        }

        /// <value> Gets the number of possible Undos for deleting Points </value>
        public int UndoSteps {
            get {
                if (undoSteps == null) { undoSteps = new Stack<Point[]>(); }
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
            this.points = new List<Point>();
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
            this.points = new List<Point>();
        }

        #endregion

        #region Methods

        #region Point Manipulation

        /// <summary>
        /// Deletes all selected Points from the PointSet.
        /// Stores deleted points as an UndoStep.
        /// </summary>
        public void DeleteSelectedPoints() {
            List<Point> removedPoints = new List<Point>();
            for (int i = points.Count - 1; i >= 0; i--) {
                //The last point in a pointset cannot be removed -> cause to many problems
                if (this.Length == 1) { return; }
                if (ProjectController.CurrentSelection.Contains(points[i])) {
                    removedPoints.Add(points[i]);
                    points.RemoveAt(i);
                }
            }
            if ( removedPoints.Count > 0 ) {
                if ( undoSteps == null ) { undoSteps = new Stack<Point[]>(); }
                undoSteps.Push(removedPoints.ToArray());
                if ( this.pointSetModified != null ) { this.pointSetModified(this, new EventArgs()); }
                ProjectController.CurrentSelection.Clear();
            }
            ProjectController.SetProjectChanged(true);
        }

        /// <summary>
        /// Undos the last deleting operation if there is any.
        /// </summary>
        public void Undo() {
            if ( undoSteps == null ) { undoSteps = new Stack<Point[]>(); }
            if ( undoSteps.Count > 0 ) {
                AddRange(undoSteps.Pop());
                if ( this.pointSetModified != null ) { this.pointSetModified(this, new EventArgs()); }
            }
            ProjectController.SetProjectChanged(true);
        }

        #endregion

        #region Neue Point Manipulation
        /// <summary>
        /// Adds a Point to the PointSet.
        /// </summary>
        /// <param name="point">Point to be added</param>
        /// <exception cref="ArgumentException">If the Points ColumnSet doesn't match the PointSets ColumnSet</exception>
        public void Add(Point point) {
            if (point.ColumnSet.Equals(columnSet))
                points.Add(point);
            else throw new ArgumentException("point.ColumnSet != PointSet.ColumnSet");
            //TODO: Ueberall korrekt events werfen
        }

        /// <summary>
        /// Adds an enumeration of Points to the PointSet.
        /// </summary>
        /// <param name="points">A Point Enumeration</param>
        public void AddRange(IEnumerable<Point> points) {
            if (null == points)
                throw new ArgumentNullException("points", "Can't add null to PointList");
            this.points.AddRange(points);
        }

        /// <summary>
        /// Adds another PointList to this one.
        /// The advantage over adding a Point Enumeration is that the Points don't have to
        /// be validated individually.
        /// </summary>
        /// <param name="points">A PointList to be added</param>
        /// <exception cref="ArgumentException">If the other PointLists ColumnSet doesn't match this PointLists ColumnSet</exception>
        public void AddRange(PointSet other) {
            if (null == other)
                throw new ArgumentNullException("points", "Can't add null to PointList");
            if (other.columnSet.Equals(this.columnSet))
                this.points.AddRange(other);
            else
                throw new ArgumentException("ColumnSets don't match"); //
        }

        /// <summary>
        /// Removes a single Point from the PointList.
        /// </summary>
        /// <param name="point">Point to be removed from the list</param>
        public void Remove(Point point) {
            points.Remove(point);
        }

        /// <summary>
        /// Removes a single Point from the list at the given index.
        /// </summary>
        /// <param name="index">Index of the Point to be removed</param>
        public void RemoveAt(int index) {
            points.RemoveAt(index);
        }

        /// <summary>
        /// Removes all Points from given indices.
        /// </summary>
        /// <param name="indices">Indices of Points to be removed</param>
        public void RemoveAtRange(int[] indices) {
            Array.Sort(indices);
            //Run backwards througth the sorted list
            //Remove starting at the end of the list
            for (int i = indices.Length - 1; i >= 0; i--) {
                this.points.RemoveAt(indices[i]);
            }
        }

        /// <summary>
        /// Removes all Points.
        /// If possible use RemoveAtRange because it is faster.
        /// </summary>
        /// <param name="points">A Point Enumeration</param>
        public void RemoveRange(IEnumerable<Point> points) {
            foreach (Point p in points) {
                this.points.Remove(p);
            }
        }

        #endregion

        #region MinMaxMean

        /// <summary>
        /// Calculates Min/Max/Mean for all Points in this PointSet.
        /// </summary>
        /// <returns>An array of Points in this PointSets ColumnSet, containing the min/max/mean values for each Column.</returns>
        public Point[] MinMaxMean() {
            double[] min  = new double[columnSet.Dimension];
            double[] max  = new double[columnSet.Dimension];
            double[] mean = new double[columnSet.Dimension];

            double columnMin, columnMax, columnSum, pointValue;

            for (int i = 0; i < columnSet.Dimension; i++) {
                columnMin = Double.PositiveInfinity;
                columnMax = Double.NegativeInfinity;
                columnSum = 0;
                for (int j = 0; j < points.Count; j++) {
                    pointValue = points[j].Values[i];
                    if (pointValue < columnMin) { columnMin = pointValue; }
                    if (pointValue > columnMax) { columnMax = pointValue; }
                    columnSum += pointValue;
                }
                min[i]  = columnMin;
                max[i]  = columnMax;
                mean[i] = columnSum / points.Count;
            }

            return new Point[] {
                new Point(columnSet, min),
                new Point(columnSet, max),
                new Point(columnSet, mean)
            };
        }

        /// <summary>
        /// Calculates Min/Max/Mean for all Points in this PointSet and trims
        /// the resulting Min/Max/Mean Points to the given ColumnSet.
        /// </summary>
        /// <param name="columnSet">ColumnSet to trim the result to</param>
        /// <returns>An array of Points in the given ColumnSet, containing the min/max/mean values for each Column over this PointSet.</returns>
        public Point[] MinMaxMean(ColumnSet columnSet) {
            return Array.ConvertAll<Point, Point>(MinMaxMean(), delegate(Point a) { return a.Trim(columnSet); });
        }
        #endregion

        /// <summary>
        /// Creates a copy of this PointSet which contains only the Points 
        /// whose Columns are all bounded inside the boundaries of the ColumnProperties of the given Space.
        /// </summary>
        /// <param name="space">The Space containing the ColumnProperties</param>
        /// <returns>PointSet without Points outside the ColumnProperties of the Space</returns>
        public PointSet CreateFilteredPointSet(Space space) {
            PointSet filteredPointSet = new PointSet("Filtered PointSet", this.ColumnSet);

            int[] spaceMap = space.CalculateMap(this.ColumnSet);
            for ( int pointIndex = 0; pointIndex < points.Count; pointIndex++ ) {
                Point p = points[pointIndex];
                for (int dim = 0; dim < space.Dimension; dim++) {
                    if (p[spaceMap[dim]] < space.ColumnProperties[dim].Min || space.ColumnProperties[dim].Max < p[spaceMap[dim]]) {
                        goto Abort;
                    }
                }
                filteredPointSet.Add(p);
            Abort:
                ;//We jump here if we don't want to add to the pointSet
            }
            return filteredPointSet;
        }

        /// <summary>
        /// Exports this PointSet to a StreamWriter
        /// </summary>
        /// <param name="space">The space to export</param>
        /// <param name="writer">A StreamWriter that actually writes the output somewhere</param>
        public void ExportCSV(Space space, System.IO.StreamWriter writer) {
            ColumnSet columnSet = space.ToColumnSet();
            if (!columnSet.IsSubSetOf(this.columnSet))
                throw new ApplicationException("Space is not a subcolumnSet of this PointSet");

            for (int colIndex = 0; colIndex < columnSet.Dimension; colIndex++) {
                writer.Write(columnSet.Columns[colIndex].Label);
                writer.Write(";");
            }
            writer.Write("\n");
            writer.Flush();

            int[] map = columnSet.SuperSetMap(this.ColumnSet);
            for (int pointIndex = 0; pointIndex < points.Count; pointIndex++) {
                for (int colIndex = 0; colIndex < map.Length; colIndex++) {
                    writer.Write(points[pointIndex][map[colIndex]]);
                    writer.Write(";");
                }
                writer.Write("\n");
            }
            writer.Flush();
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
        public IEnumerator<Point> GetEnumerator() {
            return points.GetEnumerator();
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

    /// <summary>
    /// Identifier for the results of MinMaxMean in PointList an PointSet
    /// </summary>
    public static class Result {
        public const int MIN  = 0;
        public const int MAX  = 1;
        public const int MEAN = 2;
    }
}
