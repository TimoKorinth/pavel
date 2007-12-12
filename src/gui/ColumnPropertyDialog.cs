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
using Pavel.Framework;

namespace Pavel.GUI {
    /// <summary>
    /// Form to alter ColumnProperty of a Column.
    /// </summary>
    public partial class ColumnPropertyDialog : Form {

        #region Fields

        private ColumnProperty cp;
        private bool advanced = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the Form with the values from <paramref name="cp"/>
        /// and sets the Form-header to <paramref name="label"/>.
        /// </summary>
        /// <param name="cp">Selected ColumnProperty</param>
        public ColumnPropertyDialog(ColumnProperty cp) {
            InitializeComponent();
            this.Width = 250;
            advancedButton.Text = "Advanced >>";
            this.Text = "Column Property: "+cp.Label;
            minTextBox.Text = cp.Min.ToString();
            maxTextBox.Text = cp.Max.ToString();
            this.cp = cp;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Alters the ColumnProperty.
        /// </summary>
        /// <param name="sender">okButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void OkButton_Click(object sender, EventArgs e) {
            try {
                cp.SetMinMax(Double.Parse(minTextBox.Text), Double.Parse(maxTextBox.Text));
                this.DialogResult = DialogResult.OK;
            }
            catch ( Exception ) {
                MessageBox.Show("Please enter value","Error");
            }

        }

        private void AutoButton_Click(object sender, EventArgs e) {
            double min = Double.PositiveInfinity;
            double max = Double.NegativeInfinity;
            ColumnSet cs = new ColumnSet(cp.Column);
            if ( !advanced ) {
                foreach ( PointSet ps in ProjectController.Project.pointSets ) {
                    if ( ps.ColumnSet.Contains(cp.Column) ) {
                        Pavel.Framework.Point[] p = ps.MinMaxMean(cs);
                        if ( p[0][0] < min ) { min = p[0][0]; }
                        if ( p[1][0] > max ) { max = p[1][0]; }
                    }
                }
            } else {
                if ( pointSetCheckedListBox.CheckedItems.Count == 0 ) {
                    MessageBox.Show("Please select a least one Point Set", "Error");
                }
                foreach ( object item in pointSetCheckedListBox.CheckedItems ) {
                    PointSet ps = (PointSet)item;
                    if ( ps.ColumnSet.Contains(cp.Column) ) {
                        Pavel.Framework.Point[] p = ps.MinMaxMean(cs);
                        if ( p[0][0] < min ) { min = p[0][0]; }
                        if ( p[1][0] > max ) { max = p[1][0]; }
                    }
                }
            }

            minTextBox.Text = min.ToString();
            maxTextBox.Text = max.ToString();
        }

        #endregion

        private void Advanced_Click(object sender, EventArgs e) {
            advanced = !advanced;
            if ( advanced ) { 
                this.Width = 420;
                advancedButton.Text = "Advanced <<";
                pointSetCheckedListBox.Items.Clear();
                foreach ( PointSet ps in ProjectController.Project.pointSets ) {
                    if ( ps.ColumnSet.Contains(cp.Column) ) {
                        pointSetCheckedListBox.Items.Add(ps, true);
                    }
                }
            } else { 
                this.Width = 250;
                advancedButton.Text = "Advanced >>";
            }
        }

        
    }
}