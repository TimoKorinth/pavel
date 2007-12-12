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

/// <summary>
/// The namespace containing PAVEls framework-classes.
/// </summary>
namespace Pavel.Framework {
    [Serializable()]
    /// <summary>
    /// A Column in the Data / A Dimension in a ColumnSet
    /// </summary>
    public class Column: IComparable<Column> {

        #region Fields

        private String label;
        private int index;
        private ColumnProperty defaultColumnProperty;

        #endregion

        #region Properties

        /// <value>Gets the Columns columnProperty or sets it.</value>
        public ColumnProperty DefaultColumnProperty { 
            get { return defaultColumnProperty; }
            set { defaultColumnProperty = value; }
        }

        /// <value>Gets the Columns index or sets it.</value>
        public int Index {
            [CoverageExclude]
            get { return index; }
        }

        /// <value>Gets the Columns label or sets it.</value>
        public String Label {
            [CoverageExclude]
            get { return label; }
            [CoverageExclude]
            set { label = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Adds a new Column to Project.columns as the last Column.
        /// defaultColumnProperty is set to a standard ColumnProperty with min=0 and max=1.
        /// </summary>
        public Column() {
            this.index = ProjectController.Project.columns.Count;
            ProjectController.Project.columns.Add(this);
            this.defaultColumnProperty = new ColumnProperty(this, 0, 1);
        }
        
        /// <summary>
        /// Adds a new Column to Project.columns as the last Column
        /// with the given label.
        /// </summary>
        /// <param name="label">Label for the Column</param>
        public Column(String label) : this() {
            this.label = label;
        }

        /// <summary>
        /// Adds a new Column to Project.columns as the last Column.
        /// The Column gets the given ColumnProperty.
        /// </summary>
        /// <param name="columnProperty">ColumnProperty for the created Column</param>
        public Column(ColumnProperty columnProperty): this() {
            this.defaultColumnProperty = columnProperty;
        }

        /// <summary>
        /// Adds a new Column to Project.columns as the last Column
        /// with the given label. The Column gets the given ColumnProperty.
        /// </summary>
        /// <param name="label">Label for the Column</param>
        /// <param name="columnProperty">ColumnProperty for the created Column</param>
        public Column(String label, ColumnProperty columnProperty): this(label) {
            this.defaultColumnProperty = columnProperty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the label as a string.
        /// </summary>
        /// <returns>The Columns label</returns>
        [CoverageExclude]
        public override string ToString() {
            return label;
        }

        #region IComparable<Column> Member

        /// <summary>
        /// Compares the Column index to that of the given <paramref name="other"/> Column.
        /// </summary>
        /// <param name="other">The Column to which this Column is compared</param>
        /// <returns>Difference between the index of this Column and that of the Column given by <paramref name="other"/></returns>
        public int CompareTo(Column other) {
            return this.index - other.index;
        }

        #endregion

        #endregion
    }
}
