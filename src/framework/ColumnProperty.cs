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
    [Serializable()]
    /// <summary>
    /// Represents the minimum and the maximum displayed values of a Column.
    /// </summary>
    public class ColumnProperty {

        #region Fields

        private double min=0;
        private double max=1;
        private Column column;

        #endregion

        #region Properties

        /// <value> Gets the minimum of the Column. </value>
        /// <remarks> Note: min can be greater than max, to realize an inverted order </remarks>
        public double Min {
            get { return min; }
        }

        /// <value> Gets the maximum of the Column. </value>
        /// <remarks> Note: max can be smaller than min, to realize an inverted order </remarks>
        public double Max {
            get { return max; }
        }

        /// <value> Gets the Column this ColumnProperty belongs to </value>
        public Column Column { get { return column; } }

        /// <value> Gets the Label of the Column this ColumnProperty belongs to </value>
        public String Label { get { return column.Label; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new ColumnProperty.
        /// </summary>
        /// <param name="col">The Column this ColumnProperty belongs to</param>
        /// <param name="min">Displayed max for this Column</param>
        /// <param name="max">Displayed min for this Column</param>
        public ColumnProperty(Column col, double min, double max) {
            if ( col == null ) { throw new ArgumentNullException("col has to be not null"); }
            SetMinMax(min, max);
            this.column = col;
        }

        #endregion

        #region Methods

        /// <summary> Sets the maximum and minimum of the Column. </summary>
        /// If both are equal, min is set to 0, max to 1.
        /// <remarks> Note: max can be smaller than min, to realize an inverted order </remarks>
        /// <param name="min">The chosen minimum</param>
        /// <param name="max">The chosen maximum</param>
        public void SetMinMax(double min, double max) {
            if (min != max) {
                this.min = min;
                this.max = max;
            } else {
                this.min = 0.0;
                this.max = 2*max;
                if (max == 0.0) {
                    this.min = -1.0;
                    this.max = 1.0; 
                }
            }
        }

        /// <summary>
        /// Switches the orientation of the Column, by swapping min and max.
        /// </summary>
        public void SwitchOrientation() {
            SetMinMax(max, min);
        }

        /// <summary>
        /// Returns the orientation of the Column.
        /// </summary>
        /// <returns>true if ascending order, false otherwise</returns>
        public bool IsAscendingOrder() {
            if (min<max) {return true;}
            return false;
        }

        /// <summary>
        /// Overrides the standard ToString() method to return the label of the Column.
        /// </summary>
        /// <returns>The Label of the Column this ColumnProperty belongs to</returns>
        public override String ToString() {
            return column.Label;
        }

        /// <summary>
        /// Clones the ColumnProperty.
        /// </summary>
        /// <returns>A copy of this ColumnProperty</returns>
        public ColumnProperty Clone() {
            return new ColumnProperty(this.column, this.min, this.max);
        }

        /// <summary>
        /// Saves this Column Properties' settings in the default ColumnProperty of the attached Column. 
        /// </summary>
        public void Save() {
            column.DefaultColumnProperty.SetMinMax(this.min, this.max);
        }

        #endregion
    }
}
