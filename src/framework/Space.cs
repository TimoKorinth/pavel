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
    /// Space is the frontend to visualizing the data in the ColumnSets.  TODO: That's not documentation, that's bullshit. Gibberish and wrong.
    /// </summary>
    [Serializable()]
    public class Space : IEnumerable<Column> {

        #region Fields

        private ColumnProperty[] columnProperties = null;
        private string label;
        private Space baseSpace = null;

        /// <value> Event fired if the Space has been modified. </value>
        [field: NonSerializedAttribute()]
        public event EventHandler Changed;

        #endregion

        #region Properties

        /// <value> Gets the Spaces label or sets it </value>
        public string Label {
            get { return label; }
            set {
                label = value;
                if (null != Changed) { Changed(this, EventArgs.Empty); }
            }
        }

        /// <value> Gets the ColumnProperties of this Space or sets them </value>
        public ColumnProperty[] ColumnProperties {
            get { return columnProperties;  }
            set {
                if (value == null || value.Length == 0) {
                    throw new ArgumentException("ColumnProperties are null or empty");
                }
                columnProperties = value;
                if (null != Changed) { Changed(this, EventArgs.Empty); }
            } 

        }

        /// <value> Gets the length of the Columns </value>
        public int Dimension {
            get { return columnProperties.Length; }
        }

        /// <value> Gets the ancestor of the Space, returns null if the Space is the default one </value>
        public Space Parent {
            get { return baseSpace; }
        }

        /// <value> Gets the Column of the ColumnProperty at index i of ColumnProperties</value>
        public Column this[int i] { get { return columnProperties[i].Column; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create new Space.
        /// </summary>
        /// <param name="columnProperties">ColumnProperties for the Space</param>
        /// <param name="label">Label for the Space</param>
        public Space(ColumnProperty[] columnProperties, string label) {
            this.columnProperties = columnProperties;
            this.label = label;
        }

        /// <summary>
        /// Create a new Space.
        /// </summary>
        /// <param name="columnSet">ColumnSet containing the Columns for the Space</param>
        /// <param name="label">Label for the Space</param>
        public Space(ColumnSet columnSet, string label) {
            columnProperties = new ColumnProperty[columnSet.Dimension];
            for ( int i = 0; i < columnProperties.Length; i++ ) {
                columnProperties[i] = new ColumnProperty(columnSet.Columns[i], columnSet.Columns[i].DefaultColumnProperty.Min, columnSet.Columns[i].DefaultColumnProperty.Max);
            }
            this.label = label;
        }

        /// <summary>
        /// Private Constuctor for generating a copy of a Space.
        /// Sets baseSpace to the given Space <paramref name="baseSV"/>.
        /// Each Visualization has its own Space to allow local modifications.
        /// It is possible to write this local modifications back to the global
        /// Space (the parent) to use this settings in other Visualizations.
        /// </summary>
        /// <param name="baseSV">Space to copy</param>
        private Space(Space baseSV) {
            this.baseSpace = baseSV;
            this.label  = baseSV.label;
            this.columnProperties = new ColumnProperty[baseSV.Dimension];
            for ( int i = 0; i < columnProperties.Length; i++ ) {
                columnProperties[i] = new ColumnProperty(baseSV.columnProperties[i].Column, baseSV.columnProperties[i].Min, baseSV.columnProperties[i].Max);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a map to the Columns of a given ColumnSet.
        /// The map is indexed by axes in the Space and yields the indices for the Columns in columnSet.
        /// </summary>
        /// <param name="columnSet">ColumnSet to create the map to</param>
        /// <returns>map to the Space</returns>
        public int[] CalculateMap(ColumnSet columnSet) {
            int[] map = new int[this.columnProperties.Length];
            for ( int i = 0; i < map.Length; i++ ) {
                map[i] = columnSet.IndexOf( columnProperties[i].Column );
                if ( map[i] < 0 )
                    throw new ApplicationException( "Current Space's columnSet is not a subcolumnSet of given columnSet");
            }
            return map;
        }

        /// <summary>
        /// Checks whether this Space displays a subcolumnSet or the ColumnSet given by <paramref name="columnSet"/>.
        /// </summary>
        /// <param name="columnSet">ColumnSet to check</param>
        /// <returns>True if this Space displays a subcolumnSet of the ColumnSet</returns>
        public bool IsViewOfColumnSet(ColumnSet columnSet) {
            foreach ( ColumnProperty cp in columnProperties ) {
                if ( !columnSet.Contains(cp.Column) ) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Returns the ColumnSet displayed in this Space, ordered and without duplicates.
        /// </summary>
        /// <returns>The ColumnSet with all Columns in this Space</returns>
        public ColumnSet ToColumnSet() {
            SortedDictionary<int, Column> newColumns = new SortedDictionary<int, Column>();
            foreach ( ColumnProperty cp in columnProperties ) {
                if (!newColumns.ContainsKey(cp.Column.Index))newColumns.Add(cp.Column.Index, cp.Column);
            }
            return new ColumnSet(newColumns.Values);
        }

        /// <summary>
        /// Creates a copy of a Space to allow local modifications for each Visualization.
        /// The copy has its own Columns and ColumnProperties.
        /// The parent field points to the ancestor Space.
        /// Use WriteBack and Reset to store or to discard any local modifications to the global space.
        /// </summary>
        /// <returns>Copy of Space</returns>
        public Space CreateLocalCopy() {
            return new Space(this);
        }

        /// <summary>
        /// Overwrites Columns of the global default (=parent) Space with the Columns of a child-Space.
        /// </summary>
        public void WriteBack() {
            if ( baseSpace != null ) {
                baseSpace.ColumnProperties = (ColumnProperty[]) this.columnProperties.Clone();
            }
        }

        /// <summary>
        /// Resets all local modifications with the settings of the global default (=parent) Space.
        /// </summary>
        [CoverageExclude]
        public void Reset() {
            for ( int i = 0; i < columnProperties.Length; i++ ) {
                columnProperties[i].SetMinMax(
                    baseSpace.ColumnProperties[i].Min,
                    baseSpace.ColumnProperties[i].Max);
            }
        }

        /// <summary>
        /// Overrides the ToString() method to return the label of this Space.
        /// </summary>
        /// <returns>This Spaces label</returns>
        [CoverageExclude]
        public override String ToString() {
            return label;
        }

        /// <summary>
        /// Returns A List of all known Space compatible to the given ColumnSet.
        /// </summary>
        /// <param name="columnSet">ColumnSet to check</param>
        /// <returns>A List of all known Space compatible to the given ColumnSet</returns>
        public static List<Space> AllSpacesForColumnSet (ColumnSet columnSet){
            List<Space> spaces = new List<Space>();
            foreach (Space space in ProjectController.Project.spaces) {
                if ( space.IsViewOfColumnSet(columnSet) ) { spaces.Add(space); }
            }
            return spaces;
        }

        #region Column Modifications

        /// <summary>
        /// Creates a copy of the Column given by <paramref name="column"/> 
            /// and inserts it at <paramref name="position"/>.
        /// </summary>
        /// <param name="column">Columnindex to copy</param>
        /// <param name="position">New position</param>
        public void CopyColumn(int column, int position) {
            if (position > this.Dimension) throw new ArgumentException();

            List<ColumnProperty> tmp = new List<ColumnProperty>(ColumnProperties);
            tmp.Insert(position, columnProperties[column]);

            this.columnProperties = (ColumnProperty[])tmp.ToArray(); 
        }

        /// <summary>
        /// Moves a Column in the Space to another position.
        /// </summary>
        /// <remarks>New position means the position on which the same column will be accessed after.</remarks>
        /// <param name="column">Columnindex to move</param>
        /// <param name="position">New Position</param>
        public void MoveColumn(int column, int position) {
            if ( column == position ) return;

            List<ColumnProperty> tmp = new List<ColumnProperty>(ColumnProperties);
            if ( column > position ) {
                tmp.Insert(position, columnProperties[column]);
                tmp.RemoveAt(column + 1);
            } else { 
                tmp.Insert(position + 1, columnProperties[column]);
                tmp.RemoveAt(column);
            }
            this.columnProperties = (ColumnProperty[])tmp.ToArray(); 
        }

        /// <summary>
        /// Inserts a new Column at the given position.
        /// </summary>
        /// <param name="column">The column to be inserted</param>
        /// <param name="position">The position, satisfying 0 &lt;= position &lt;= Dimension</param>
        public void InsertColumn(Column column, int position) {
            if (position > Dimension || position < 0)
                throw new ArgumentOutOfRangeException("Position has to be between 0 and Dimension");

            List<ColumnProperty> tmp = new List<ColumnProperty>(ColumnProperties);
            tmp.Insert(position, column.DefaultColumnProperty);
            this.ColumnProperties = tmp.ToArray();
        }

        /// <summary>
        /// Swaps two Columns in the Space.
        /// </summary>
        /// <param name="column1">First Column</param>
        /// <param name="column2">Second Column</param>
        public void SwapColumns( int column1, int column2 ) {
            ColumnProperty tmp = ColumnProperties[column1];
            columnProperties[column1] = columnProperties[column2];
            columnProperties[column2] = tmp;
        }

        /// <summary>
        /// Removes the Column at the given position.
        /// </summary>
        /// <param name="position">Position of the column to delete</param>
        public void RemoveColumn(int position) {
            List<ColumnProperty> tmp = new List<ColumnProperty>(ColumnProperties);
            tmp.RemoveAt(position);
            this.columnProperties = (ColumnProperty[])tmp.ToArray();
        }

        #endregion

        #region Enumeration- & Indeximplementation

        /// <summary>
        /// Returns an Enumerator for each Column for each ColumnProperty in columnProperties.
        /// </summary>
        /// <returns>An Enumerator over all Columns in ColumnSet</returns>
        [CoverageExclude]
        public IEnumerator<Column> GetEnumerator() {
            foreach (ColumnProperty cp in columnProperties) {
                yield return cp.Column;
            }
        }

        /// <summary>
        /// Returns an Enumerator for each Column for each ColumnProperty in columnProperties.
        /// </summary>
        /// <returns>An Enumerator for each Column for each ColumnProperty in columnProperties</returns>
        [CoverageExclude]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            foreach (ColumnProperty cp in columnProperties) {
                yield return cp.Column;
            }
        }

        #endregion

        #endregion
    }
}