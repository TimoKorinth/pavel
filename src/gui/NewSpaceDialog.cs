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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Pavel.Framework;

namespace Pavel.GUI {
    /// <summary>
    /// A dialog to create a new Space.
    /// </summary>
    public partial class NewSpaceDialog :Form {

        #region Fields

        private Space newSpace;

        #endregion

        #region Properties

        /// <value> Gets the newSpace </value>
        public Space NewSpace { get { return newSpace; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the Form.
        /// </summary>
        public NewSpaceDialog() {
            InitializeComponent();
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Allows the user to select a Space to copy if the copyRadioButton is checked.
        /// </summary>
        /// <param name="sender">The copyRadioButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void CopyRadioButton_CheckedChanged(object sender, EventArgs e) {
            if ( copyRadioButton.Checked ) {
                spaceComboBox.Enabled = true;
                spaceComboBox.DataSource = ProjectController.Project.spaces;
            }
            else { 
                spaceComboBox.Enabled = false;
                spaceComboBox.DataSource = null;
            }
        }

        /// <summary>
        /// Creates the new Space.
        /// </summary>
        /// <param name="sender">The okButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void OkButton_Click(object sender, EventArgs e) {
            bool uniqueName = true;
            foreach (Space space in ProjectController.Project.spaces) {
                if ( nameTextBox.Text.Trim() == space.Label ) uniqueName = false;
            }
            if ( uniqueName && nameTextBox.Text.Trim() != "" ){
                if (emptyRadioButton.Checked) {
                    newSpace = new Space(new ColumnProperty[] { }, nameTextBox.Text.Trim());
                    this.DialogResult = DialogResult.OK;
                } else {
                    Space oldSpace = spaceComboBox.SelectedItem as Space;
                    if (oldSpace != null) {
                        newSpace = new Space(oldSpace.ColumnProperties, nameTextBox.Text.Trim());
                        this.DialogResult = DialogResult.OK;
                    } else {
                        newSpace = new Space(new ColumnProperty[] { }, nameTextBox.Text.Trim());
                        this.DialogResult = DialogResult.OK;
                    }
                }
            }
            else { MessageBox.Show("Please enter a valid & unique name!","Error"); }
        }

        #endregion
    }
}