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
using System.Runtime.Serialization;

namespace Pavel.Framework {
    [Serializable()]
    /// <summary>
    /// A Data Point, consisting of multiple double-values.
    /// </summary>
    public class Point : IEquatable<Point> {

        #region Fields

        protected double[] values;
        protected ColumnSet columnSet;
        protected DataTag  tag;

        #endregion

        #region Properties

        /// <value>Gets the ColumnSet of the Point. Do not make changes, this is a reference, not a copy</value>
        public ColumnSet ColumnSet {
            [CoverageExclude]
            get { return columnSet; }
        }

        /// <value> Gets the tag of the point, or stores the reference to this point </value>
        public DataTag Tag {
            [CoverageExclude]
            get { return tag; }
            [CoverageExclude]
            set { tag = value; }
        }

        /// <value> The values stored in the Point. Do not make changes, this is a reference, not a copy </value>
        public double[] Values {
            [CoverageExclude]
            get { return values; }
        }

        /// <summary>
        /// Used to access the values of the Point if the index of the value for the column
        /// is not known. Performs a binary search and should be avoided if speed is a concern.
        /// </summary>
        /// <param name="col">A Column</param>
        /// <returns>The value for the Column</returns>
        public double this[Column col] {
            get { return values[Array.BinarySearch<Column>(columnSet.Columns, col)]; }
        }

        /// <summary>
        /// Used to access the values of the Point if the index of the value for the column
        /// is known. Performs a direct access.
        /// </summary>
        /// <param name="col">A column-index</param>
        /// <returns>The value for the Column</returns>
        public double this[int col] {
            get { return values[col]; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a Point based on an array of double values and a ColumnSet.
        /// The size of values[] must be columnSet.Dimension, the values are assigned to the columns by 
        /// their corresponding position in the array.
        /// </summary>
        /// <param name="columnSet">ColumnSet this Point belongs to</param>
        /// <param name="values">Values of the Point</param>
        /// <exception cref="ApplicationException">If the lengths of the value[] array is invalid.</exception>
        public Point(ColumnSet columnSet, params double[] values) {
            if (columnSet.Dimension != values.Length)
                throw new ApplicationException("values.Length doesn't match columnSet.Dimension");
            this.values = values;
            this.columnSet = columnSet;
            this.tag = new DataTag();
        }

        /// <summary>
        /// Creates a Point based on an array of double values and a ColumnSet.
        /// The size of values[] must be columnSet.Dimension, the values are assigned to the columns by 
        /// their corresponding position in the array.
        /// Additionally a tag as a reference to the origin of the Point is stored.
        /// </summary>
        /// <param name="columnSet">ColumnSet this Point belongs to</param>
        /// <param name="values">Values of the Point</param>
        /// <param name="tag">Reference to the origin of the Point</param>
        /// <exception cref="ApplicationException">If the Lengths of the Value[] Array is invalid.</exception>
        [CoverageExclude]
        public Point(ColumnSet columnSet, DataTag tag, params double[] values) : this(columnSet,values) {
            this.tag = tag;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The value stored in the Point scaled by the given ColumnProperty
        /// </summary>
        /// <param name="position">Position of the value to be scaled in this point</param>
        /// <param name="property">Any ColumnProperty</param>
        /// <remarks>Use of the mapping function for the position of the column is needed if 
        /// points of different pointLists are scaled If performance doesn't matter, e.g. for scaling only a few points, 
        /// use ScaledValue with the column and not the position of the column as a parameter.
        /// </remarks>
        /// <returns>the scaled value and 0.5 if Min == Max</returns>
        public double ScaledValue(int position, ColumnProperty property) {
            if (property.Min == property.Max) return 0.5;
            return (this[position] - property.Min) / (property.Max - property.Min);
        }

        /// <summary>
        /// The value stored in the Point scaled by the given ColumnProperty
        /// </summary>
        /// <param name="col">Column to scale</param>
        /// <param name="property">Any ColumnProperty</param>
        /// <returns>the scaled value and 0.5 if Min == Max</returns>
        public double ScaledValue(Column col, ColumnProperty property) {
            if ( property.Min == property.Max ) return 0.5;
            return (this[col] - property.Min) / (property.Max - property.Min);
        }

        /// <summary>
        /// Values from the Point, ordered, selected and scaled by the Space.
        /// Faster version for known spaceMap (Created once with space.CalculateMap(point.ColumnSet))
        /// </summary>
        /// <param name="space">Space for Column selection, ordering and scaling</param>
        /// <param name="spaceMap">Int-array that holds the corresponding position of each Column in space in the Point's ColumnSet.</param>
        /// <returns>An array of length space.Dimension with scaled values (by space)</returns>
        /// <exception cref="ArgumentException">If spaceMap is too short</exception>
        public double[] ScaledValues(Space space, int[] spaceMap) {
            if (spaceMap.Length < space.Dimension) {
                throw new ArgumentException("Map must be at least of the space's size");
            }
            double[] data = new double[space.Dimension];
            for (int i = 0; i < data.Length; i++) {
                data[i] = this.ScaledValue(spaceMap[i], space.ColumnProperties[i]);
            }
            return data;
        }

        /// <summary>
        /// Values from the Point, ordered, selected and scaled by the Space.
        /// Slower version for unknown spaceMap
        /// </summary>
        /// <param name="space">Space for Column selection, ordering and scaling</param>
        /// <returns>An array of length space.Dimension with scaled Values (by space)</returns>
        public double[] ScaledValues(Space space) {
            int[] spaceMap = space.CalculateMap(this.ColumnSet);
            double[] data = new double[space.Dimension];
            for (int i = 0; i < data.Length; i++) {
                data[i] = this.ScaledValue(spaceMap[i], space.ColumnProperties[i]);
            }
            return data;
        }

        /// <summary>
        /// Creates a new Point that contains only columns which are present in <paramref name="columnSet"/>.
        /// Calculates a SuperColumnSetMap each time. Use Trim(ColumnSet, int[]) if speed is a concern.
        /// Trimming using the same columnSet yields a reference to this point, not a copy.
        /// </summary>
        /// <param name="columnSet">A ColumnSet</param>
        public Point Trim(ColumnSet columnSet) {
            if (columnSet == this.columnSet) return this;

            return Trim(columnSet, columnSet.SuperSetMap(this.columnSet));
        }

        /// <summary>
        /// Faster Version of the Trim Function that can be used if the
        /// SuperColumnSetMap is already known.
        /// Always returns a new Point, even if the map is an identity map
        /// </summary>
        /// <param name="columnSet">A ColumnSet</param>
        /// <param name="superColumnSetMap">The SuperColumnSetMap, mapping indices
        /// of the subcolumnSet to indices of the supercolumnSet</param>
        public Point Trim(ColumnSet columnSet, int[] superColumnSetMap) {
            double[] valNew = new double[superColumnSetMap.Length];

            for (int i = 0; i < valNew.Length; i++) {
                valNew[i] = values[superColumnSetMap[i]];
            }

            return new Point(columnSet, tag, valNew);
        }

        #region static Point Operations

        /// <summary>
        /// Calculates the squared Euclidean distance between two Points ( a^2 + b^2 + ... ).
        /// Both Points have to have the same ColumnSet.
        /// </summary>
        /// <param name="pointA">First Point</param>
        /// <param name="pointB">Second Point</param>
        public static double Distance(Point pointA, Point pointB) {
            double d = 0.0;
            double diff;

            for (int i = 0; i < pointA.columnSet.Dimension; i++) {
                diff = pointA.values[i] - pointB.values[i];
                d += diff * diff;
            }
            return d;
        }

        /// <summary>
        /// Calculates the squared Euclidean distance between two Points ( a^2 + b^2 + ... ).
        /// Both Points are given by their value-arrays and have the same size.
        /// </summary>
        /// <param name="pointA">First Point</param>
        /// <param name="pointB">Second Point</param>
        public static double Distance(double[] pointA, double[] pointB) {
            double d = 0.0;
            double diff;

            for (int i = 0; i < pointA.Length; i++) {
                diff = pointA[i] - pointB[i];
                d += diff * diff;
            }
            return d;
        }


        /// <summary>
        /// Calculates the squared Euclidean distance between two Points ( a^2 + b^2 + ... ).
        /// subPoint has to be in the subcolumnSet of the ColumnSet that contains superPoint.
        /// superColumnSetMap has to be an array mapping indices of the supcolumnSet to indices of the supercolumnSet.
        /// </summary>
        /// <param name="subPoint">First Point (in subcolumnSet of the second point)</param>
        /// <param name="superPoint">Second Point (in supercolumnSet of the first point)</param>
        /// <param name="superColumnSetMap">An array mapping indices of the supcolumnSet to indices of the supercolumnSet</param>
        public static double Distance(Point subPoint, Point superPoint, int[] superColumnSetMap) {
            double d = 0.0;
            double diff;

            for (int i = 0; i < superColumnSetMap.Length; i++) {
                diff = subPoint.values[i] - superPoint.values[superColumnSetMap[i]];
                d += diff * diff;
            }
            return d;
        }

        /// <summary>
        /// Calculates the squared Euclidean distance between two Points ( a^2 + b^2 + ... )
        /// inside a subcolumnSet common to both Points.
        /// a/bColumnSetMap has to be an array mapping indices of the common supcolumnSet to indices
        /// of the respective ColumnSet of the Points.
        /// </summary>
        /// <param name="pointA">First Point</param>
        /// <param name="pointB">Second Point</param>
        /// <param name="aColumnSetMap">An array mapping indices of the common supcolumnSet to indices
        /// of the columnSet of the first Point</param>
        /// <param name="bColumnSetMap">An array mapping indices of the common supcolumnSet to indices
        /// of the columnSet of the second Point</param>
        /// <returns></returns>
        public static double Distance(Point pointA, Point pointB, int[] aColumnSetMap, int[] bColumnSetMap) {
            double d = 0.0;
            double diff;

            for (int i = 0; i < aColumnSetMap.Length; i++) {
                diff = pointA.values[aColumnSetMap[i]] - pointB.values[bColumnSetMap[i]];
                d += diff * diff;
            }
            return d;
        }

        /// <summary>
        /// Calculates the squared Euclidean distance between two scaled Points ( a^2 + b^2 + ... ).
        /// subPoint has to be in the subcolumnSet of the ColumnSet that contains superPoint.
        /// superColumnSetMap has to be an array mapping indices of the supcolumnSet to indices of the supercolumnSet.
        /// </summary>
        /// <param name="subPoint">First Point (in subcolumnSet of the second point)</param>
        /// <param name="superPoint">Second Point (in supercolumnSet of the first point)</param>
        /// <param name="superColumnSetMap">An array mapping indices of the supcolumnSet to indices of the supercolumnSet</param>
        /// <param name="space">The Space the scaling is based upon</param>
        /// <param name="relevantColumns">An array indicating relevant Columns</param>
        public static double ScaledDistance(Point subPoint, Point superPoint, int[] superColumnSetMap, Space space, bool[] relevantColumns) {
            double d = 0.0;
            double diff;

            for (int i = 0; i < superColumnSetMap.Length; i++) {
                if (relevantColumns[i] == true) {
                    diff = subPoint.ScaledValue(i, space.ColumnProperties[i]) - superPoint.ScaledValue(superColumnSetMap[i], space.ColumnProperties[i]);
                    d += diff * diff;
                }
            }
            return d;
        }

        #endregion

        #region IEquatable<Point> Member

        /// <summary>
        /// Returns true if the other Point has the same ColumnSet
        /// and the same values for all Columns
        /// </summary>
        /// <param name="other">A point to compare this one to</param>
        public bool Equals(Point other) {
            if (this.columnSet != other.columnSet) return false;
            for (int i = 0 ; i<values.Length ; i++ ) {
                if (other.values[i] != this.values[i]) return false;
            }
            return true;
        }

        #endregion

        #endregion
    }
}
