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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Pavel.GUI {

    /// <summary>
    /// The Slider used to change Integers in the PropertyGrids.
    /// </summary>
    public partial class PropertySliderForm : Form {

        #region Fields

        /// <value> Controls the DropDown </value>
        public IWindowsFormsEditorService wfes;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the PropertySliderForm.
        /// </summary>
        /// <param name="value">Preset value</param>
        /// <param name="min">Minimum value of the slider</param>
        /// <param name="max">Maximum value of the slider</param>
        public PropertySliderForm(int value, int min, int max ) {
            InitializeComponent( );
            TopLevel = false;
            slider.Value = value;
			button.Text = value.ToString();
            slider.Minimum = min;
            slider.Maximum = max;
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Sets the button.Text, if the value of the slider is changed.
        /// </summary>
        /// <param name="sender">slider</param>
        /// <param name="e">Standard EventArgs</param>
        private void Slider_ValueChanged(object sender, System.EventArgs e) {
            button.Text = slider.Value.ToString();

        }

        /// <summary>
        /// Closes the PropertySliderForm.
        /// </summary>
        /// <param name="sender">The button</param>
        /// <param name="e">Standard EventArgs</param>
        private void Slider_Click(object sender, System.EventArgs e) {
            Close( );
            wfes.CloseDropDown( );
        }

        private void slider_MouseUp(object sender, MouseEventArgs e) {
            Close();
            wfes.CloseDropDown();
        }
        #endregion

        
    }
}