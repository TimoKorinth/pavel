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
    /// A homogenous list of points, all in the same columnSet
    /// </summary>
    [Serializable]
    public class PointList : IEnumerable<Point> {

        #region Fields

        private ColumnSet columnSet;
        protected List<Point> points;
        private string label = "Point List";

        #endregion

        #region Properties

        /// <value> Gets the number of points in the PointList </value>
        [CoverageExclude]
        public int Count { get { return points.Count; } }

        /// <value> Gets the ColumnSet of the PointList or sets it </value>
        public ColumnSet ColumnSet {
            [CoverageExclude]
            get { return columnSet; }
            set { columnSet = value; }
        }

        /// <value> Gets the label of the PointList or sets it </value>
        [CoverageExclude]
        public string Label {
            get { return label; }
            set { label = value; }
        }

        /// <summary>
        /// Accesses a Point at a specific position in the list.
        /// </summary>
        /// <remarks>
        /// WARNING: Although it's unlikely, it's possible that accessing the indexer this way
        /// returns different points for the same index if the PointList was changed in the meantime.
        /// </remarks>
        public Point this[int i] {
            [CoverageExclude]
            get { return points[i]; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new, empty PointList with the given ColumnSet.
        /// </summary>
        /// <param name="columnSet">A ColumnSet. Must not be null!</param>
        /// <exception cref="ArgumentNullException">If the ColumnSet is null.</exception>
        public PointList(ColumnSet columnSet) {
            if (null == columnSet)
                throw new ArgumentNullException("columnSet", "ColumnSet can not be null in a PointList");
            this.columnSet = columnSet;
            this.points = new List<Point>();
        }

        /// <summary>
        /// Creates a new PointList and fills it with the given Points.
        /// </summary>
        /// <param name="columnSet">A ColumnSet. Must not be null</param>
        /// <param name="points">A Point Enumeration</param>
        /// <exception cref="ArgumentNullException">If the ColumnSet is null.</exception>
        public PointList(ColumnSet columnSet, IEnumerable<Point> points)
            : this(columnSet) {
            AddRange(points);
        }

        /// <summary>
        /// Creates a new PointList and fills it with the given Points from another PointList.
        /// Faster than using a generic Point Enumeration.
        /// </summary>
        /// <param name="columnSet">A columnSet. Must not be null</param>
        /// <param name="points">A PointList</param>
        /// <exception cref="ArgumentNullException">If the ColumnSet is null.</exception>
        public PointList(ColumnSet columnSet, PointList points)
            : this(columnSet) {
            AddRange(points);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a Point to the PointList.
        /// </summary>
        /// <param name="point">Point to be added</param>
        /// <exception cref="ArgumentException">If the Points ColumnSet doesn't match the PointLists ColumnSet</exception>
        public void Add(Point point) {
            if (point.ColumnSet.Equals(columnSet))
                points.Add(point);
            else throw new ArgumentException("point.ColumnSet != Pointlist.ColumnSet");
        }

        /// <summary>
        /// Adds an enumeration of Points to the PointList.
        /// </summary>
        /// <param name="points">A Point Enumeration</param>
        /// <exception cref="ArgumentException">If the Points ColumnSet doesn't match the PointLists ColumnSet</exception>
        public void AddRange(IEnumerable<Point> points) {
            if (null == points)
                throw new ArgumentNullException("points", "Can't add null to PointList");
            int additionalPoints = 0;
            foreach (Point p in points) {
                if (!p.ColumnSet.Equals(columnSet))
                    throw new ArgumentException("point.ColumnSet != Pointlist.ColumnSet");
                additionalPoints++;
            }
            int newCapacity = this.points.Count + additionalPoints;
            if (this.points.Capacity < newCapacity)
                this.points.Capacity = newCapacity;
            this.points.AddRange(points);
        }

        /// <summary>
        /// Adds another PointList to this one.
        /// The advantage over adding a Point Enumeration is that the Points don't have to
        /// be validated individually.
        /// </summary>
        /// <param name="points">A PointList to be added</param>
        /// <exception cref="ArgumentException">If the other PointLists ColumnSet doesn't match this PointLists ColumnSet</exception>
        public void AddRange(PointList points) {
            if (null == points)
                throw new ArgumentNullException("points", "Can't add null to PointList");
            if (points.columnSet.Equals(this.columnSet))
                this.points.AddRange(points.points);
            else
                throw new ArgumentException("ColumnSets don't match");
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


        /// <summary>
        /// Calculates an array of three Points with the minimum, maximum and mean values
        /// over the Points in this PointList.
        /// </summary>
        /// <returns>An array with three points containing the min/max and mean values of this PointList</returns>
        public Point[] MinMaxMean() {
            double[] min = new double[columnSet.Dimension];
            double[] max = new double[columnSet.Dimension];
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
                min[i] = columnMin;
                max[i] = columnMax;
                mean[i] = columnSum / points.Count;
            }

            return new Point[] {
                new Point(columnSet, min),
                new Point(columnSet, max),
                new Point(columnSet, mean)
            };
        }

        #region IEnumerable Member

        /// <summary>
        /// Returns the enumerator of the list of Points.
        /// </summary>
        /// <returns>points.GetEnumerator()</returns>
        [CoverageExclude]
        public IEnumerator<Point> GetEnumerator() {
            return points.GetEnumerator();
        }

        /// <summary>
        /// Returns the enumerator of the list of Points.
        /// </summary>
        /// <returns>points.GetEnumerator()</returns>
        [CoverageExclude]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return points.GetEnumerator();
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Identifier for the results of MinMaxMean in PointList an PointSet
    /// </summary>
    public static class Result {
        public const int MIN = 0;
        public const int MAX = 1;
        public const int MEAN = 2;
    }
}
