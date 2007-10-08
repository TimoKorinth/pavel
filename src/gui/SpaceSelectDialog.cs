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
    /// This class provides a dialog/form to select a PointSet and a Space.
    /// </summary>
    public partial class SpaceSelectDialog : Form {

        #region Fields

        private Space selectedSpace;
        private PointSet selectedPointSet;

        #endregion

        #region Properties

        /// <value> Gets the selectedSpace </value>
        public Space SelectedSpace { get { return selectedSpace; } }

        /// <value> Gets the selectedPointSet </value>
        public PointSet SelectedPointSet { get { return selectedPointSet; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the SpaceSelectDialog.
        /// </summary>
        /// <param name="pointSets">List of the PointSets to be shown.</param>
        public SpaceSelectDialog(List<PointSet> pointSets) {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // Set the DataSource to be shown
            this.pointSetList.DataSource = pointSets;
            // Which field should be shown? Label
            this.pointSetList.DisplayMember = "Label";
            this.selectedPointSet = (PointSet)this.pointSetList.SelectedValue;

            // Set the DataSource to be shown
            this.spaceList.DataSource = Space.AllSpacesForColumnSet(selectedPointSet.ColumnSet);
            // Which field should be shown? Label
            this.spaceList.DisplayMember = "Label";
            this.selectedSpace = (Space)this.spaceList.SelectedValue;
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Sets the selectedSpace, when the selected value in the spaceList is changed.
        /// </summary>
        /// <param name="sender">The spaceList</param>
        /// <param name="e">Standard EventArgs</param>
        private void SpaceList_SelectedValueChanged(object sender, EventArgs e) {
            this.selectedSpace = (Space) this.spaceList.SelectedValue;
        }

        /// <summary>
        /// Sets the selectedPointSet, when the selected value in the pointSetList is changed.
        /// Changes the Spaces displayed in the spaceList to match the selectedPointSet.
        /// </summary>
        /// <param name="sender">The pointSetList</param>
        /// <param name="e">Standard EventArgs</param>
        private void PointSetList_SelectedValueChanged(object sender, EventArgs e) {
            this.selectedPointSet = (PointSet)this.pointSetList.SelectedValue;

            // Set the DataSource to be shown
            this.spaceList.DataSource = Space.AllSpacesForColumnSet(selectedPointSet.ColumnSet);
            // Which field should be shown? Label
            this.spaceList.DisplayMember = "Label";
            this.selectedSpace = (Space)this.spaceList.SelectedValue;           
        }

        /// <summary>
        /// Closes the dialog, when the user double clicks the spaceList.
        /// </summary>
        /// <param name="sender">The spaceList</param>
        /// <param name="e">Standard EventArgs</param>
        private void SpaceList_DoubleClick(object sender, EventArgs e) {
            if (spaceList.SelectedIndices.Count > 0) {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        #endregion
    }
}