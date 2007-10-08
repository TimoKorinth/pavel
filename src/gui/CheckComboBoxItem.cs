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
using Pavel.Framework;
using System.Drawing;

namespace Pavel.GUI {

    /// <summary>
    /// An item in a CheckComboBox.
    /// </summary>
    public class CheckComboBoxItem {

        #region Fields

        private bool checkState = false;
        private string text = "";
        private Selection selection = new Selection();
        private Color backColor = new Color();

        #endregion

        #region Properties

        /// <value> Gets the BackColor or sets it </value>
        public Color BackColor {
            get { return backColor; }
            set { backColor = value; }
        }

        /// <value> Gets the checkState or sets it </value>
        public bool CheckState {
            get { return checkState; }
            set { checkState = value; }
        }

        /// <value> Gets the text or sets it </value>
        public string Text {
            get { return text; }
            set { text = value; }
        }

        /// <value> Gets the selection or sets it </value>
        public Selection Selection {
            get { return selection; }
            set { selection = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Sets the text, the checkState and the selection of the CheckComboBoxItem.
        /// </summary>
        /// <param name="text">Text to be displayed</param>
        /// <param name="initialCheckState">Initial checkState</param>
        /// <param name="selection">Initial selection</param>
        public CheckComboBoxItem(string text, bool initialCheckState, Selection selection) {
            this.checkState = initialCheckState;
            this.text = text;
            this.selection = selection;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overrides the ToString()-method to display no text in the
        /// display field of the CheckComboBox.
        /// </summary>
        /// <returns>Empty string</returns>
        public override string ToString() {
            return "";
        }

        #endregion
    }
}
