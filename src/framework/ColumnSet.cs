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
    /// A ColumnSet is a sorted set of Columns.
    /// </summary>
    [Serializable]
    public class ColumnSet : IEnumerable<Column>, IEquatable<ColumnSet>, IComparable<ColumnSet> {

        #region Fields

        protected Column[] columns;

        #endregion

        #region Properties

        /// <value> Gets the number of the Columns in ColumnSet </value>
        public int Dimension { get { return columns.Length; } }

        /// <value> Gets an array of all Columns in this ColumnSet </value>
        /// <remarks>WARNING: Do not alter this array!</remarks>
        public Column[] Columns {
            [CoverageExclude]
            get { return this.columns; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a ColumnSet with initial Columns.
        /// </summary>
        /// <param name="columns">Initial Columns</param>
        public ColumnSet(params Column[] columns) {
            this.columns = columns.Clone() as Column[];
            Array.Sort<Column>(this.columns);
        }

        /// <summary>
        /// Creates a ColumnSet with initial Columns.
        /// </summary>
        /// <param name="columns">Initial Columns</param>
        public ColumnSet(IEnumerable<Column> columns)
            : this(new List<Column>(columns).ToArray()){
        }

        #endregion

        #region Methods

        #region Column Management

        /// <summary>
        /// Returns the index of the given Column within the columns-array.
        /// Performs a binary search.
        /// </summary>
        /// <param name="c">A Column</param>
        /// <returns>The index of the Column or -1 if the Column isn't part of the ColumnSet.</returns>
        public int IndexOf(Column c) {
            return Array.BinarySearch<Column>(columns, c);
        }

        /// <summary>
        /// Indicates whether this ColumnSet contains the Column or not.
        /// Performs a binary search.
        /// </summary>
        /// <param name="col">A Column</param>
        /// <returns>Returns true if the ColumnSet contains the Column</returns>
        public bool Contains(Column col) {
            return Array.BinarySearch(columns, col) >= 0;
        }

        #endregion

        #region Set Operations

        /// <summary>
        /// Calculates a union of two ColumnSets, containg Columns that are present in any of the ColumnSets.
        /// </summary>
        /// <param name="columnSetA">First ColumnSet to be united</param>
        /// <param name="columnSetB">Second ColumnSet to be united</param>
        /// <returns>A ColumnSet that is the union of columnSetA and columnSetB</returns>
        public static ColumnSet Union(ColumnSet columnSetA, ColumnSet columnSetB) {
            SortedDictionary<int, Column> newColumns = new SortedDictionary<int, Column>();
            foreach ( Column c in columnSetA.Columns ) newColumns.Add(c.Index, c);
            foreach ( Column c in columnSetB.Columns ) { if ( !newColumns.ContainsKey(c.Index) ) { newColumns.Add(c.Index, c); } }
            return new ColumnSet(newColumns.Values);
        }

        /// <summary>
        /// Calculates an intersection of two ColumnSets, containing only Columns
        /// that are present in both ColumnSets.
        /// </summary>
        /// <param name="columnSetA">First ColumnSet to be intersected</param>
        /// <param name="columnSetB">Second ColumnSet to be intersected</param>
        /// <returns>A ColumnSet that is the intersection of columnSetA and columnSetB, null if the intersection is empty</returns>
        public static ColumnSet Intersect(ColumnSet columnSetA, ColumnSet columnSetB) {
            SortedDictionary<int, Column> newColumns = new SortedDictionary<int, Column>();
            foreach (Column c in columnSetA) {
                if (columnSetB.Contains(c)) newColumns.Add(c.Index, c);
            }
            if ( newColumns.Values.Count == 0 ) return null;
            return new ColumnSet(newColumns.Values);
        }

        /// <summary>
        /// Subtract subtrahend from minuend, calculates a set by taking all Columns in
        /// the minuend and then removes those that are present in the subtrahend.
        /// </summary>
        /// <param name="minuend">The basic ColumnSet</param>
        /// <param name="subtrahend">The ColumnSet containing the Columns to be removed from the basic columnSet minuend</param>
        /// <returns>A ColumnSet containing all the Columns contained in minuend that are not present in subtrahend.</returns>
        public static ColumnSet Subtract(ColumnSet minuend, ColumnSet subtrahend) {
            SortedDictionary<int, Column> newColumns = new SortedDictionary<int, Column>();
            foreach (Column c in minuend) {
                if (!subtrahend.Contains(c)) newColumns.Add(c.Index, c);
            }
            return new ColumnSet(newColumns.Values);
        }

        /// <summary>
        /// Returns true if this ColumnSet is a subset of the <paramref name="other"/> ColumnSet.
        /// </summary>
        /// <param name="other">The ColumnSet suspected of containing this ColumnSet</param>
        /// <returns>True if this ColumnSet is a subset of the <paramref name="other"/> ColumnSet</returns>
        public bool IsSubSetOf(ColumnSet other) {
            // Both columnSets are sorted!
            if (other.Dimension == 0) {
                if (Dimension == 0) return true;
                else                return false;
            }
            int otherIndex = 0;
            for (int i = 0; i < columns.Length; i++) {
                while (columns[i].CompareTo(other.columns[otherIndex]) > 0) {
                    otherIndex++;
                    if (otherIndex >= other.columns.Length) { return false; }
                }
                if (columns[i] != other.columns[otherIndex]) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Calculates a mapping array that maps indices of this columnSet to indices of a superset.
        /// </summary>
        /// <example>
        /// SuperSet might look like this [A,B,D,F,G]
        /// SubSet like this [A,D,G]
        /// The mapping array between these would then be [0,2,4].
        /// 
        /// Why?
        /// 
        /// The Column that sits at index 0 in the subSet sits at 0 in the superSet.
        /// The Column that sits at index 1 in the subSet sits at 2 in the superSet.
        /// The Column that sits at index 2 in the subSet sits at 4 in the superSet.
        /// 
        /// Now, if you want to access the same Colums in two Points, one being in the
        /// subSet, the other in the superSet you would access the values of the
        /// subpoint through normal numbering: 0,1,2, getting the values for columns A,D and G
        /// 
        /// To access those same columns in the superpoint, you have to map your
        /// array indices since the positions of the columns are different:
        /// 
        /// To access the value for column D, you look up the index for D in your map array:
        /// In the subSet, the index is 1, so map[1] == 2 which is precisely the position of
        /// column D in the superSet.
        /// </example>
        /// <param name="superSet">The superSet</param>
        /// <returns>The mapping array</returns>
        public int[] SuperSetMap(ColumnSet superSet) {
            int[] map = new int[Dimension];
            for (int i = 0; i < map.Length; i++) {
                map[i] = superSet.IndexOf(columns[i]);
                if (map[i] < 0)
                    throw new ApplicationException("Supplied ColumnSet this is not a SuperSet");
            }
            return map;
        }

        #endregion

        #region IEnumerator implementation

        /// <summary>
        /// Returns an Enumerator over all Columns in ColumnSet.
        /// </summary>
        /// <returns>An Enumerator over all Columns in ColumnSet</returns>
        public IEnumerator<Column> GetEnumerator() {
            return (columns as IEnumerable<Column>).GetEnumerator();
        }

        /// <summary>
        /// Returns an Enumerator over all Columns in ColumnSet.
        /// </summary>
        /// <returns>An Enumerator over all Columns in ColumnSet</returns>
        [CoverageExclude]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return (columns as IEnumerable<Column>).GetEnumerator();
        }

        #endregion

        #region IEquatable<ColumnSet> Member

        /// <summary>
        /// Compares this ColumnSet to the one given by <paramref name="other"/> and
        /// returns true if both columnSets have exactly the same Columns.
        /// </summary>
        /// <param name="other">A ColumnSet to compare this ColumnSet to</param>
        /// <returns>True if Both ColumnSets have exactly the same Columns</returns>
        public bool Equals(ColumnSet other) {
            if (this.columns.Length != other.columns.Length) return false;
            for (int i = 0; i < this.columns.Length; i++) {
                if (this.columns[i] != other.columns[i]) return false;
            }
            return true;
        }

        #endregion

        #region IComparable<ColumnSet> Members

        /// <summary>
        /// Compares to ColumnSets
        /// </summary>
        /// <param name="other"> other columnSet</param>
        /// <returns>ordering by lexicograhic order</returns>
        public int CompareTo(ColumnSet other) {
            if (this.Dimension <= other.Dimension) {
                for (int i = 0; i < this.Columns.Length; i++) {
                    if (this.columns[i].Index < other.columns[i].Index) return -1;
                    if (this.columns[i].Index > other.columns[i].Index) return 1;
                }
                if (this.Dimension < other.Dimension) return -1;
            } else {
                for (int i = 0; i < other.Columns.Length; i++) {
                    if (this.columns[i].Index > other.columns[i].Index) return -1;
                    if (this.columns[i].Index < other.columns[i].Index) return 1;
                }
                return -1;
            }
            return 0;
        }

        #endregion

        #endregion
    }
}
