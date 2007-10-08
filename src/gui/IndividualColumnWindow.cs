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
using Pavel.GUI.Visualizations;

namespace Pavel.GUI {
    /// <summary>
    /// The Form helping the user to create individual Columns.
    /// </summary>
    public partial class IndividualColumnWindow : Form {

        #region Fields

        // Index of the factor to calculate in the correct angle-mode
        // [0]: DEG, [1]: RAD, [2]: GRAD
        private Int16 factorIndex;
        // The position of the cursor in the newColumnFormula TextBox
        private int lastSelectionStart = 0;
        private List<PointSet> selectedPointSets = new List<PointSet>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the IndividualColumnWindow and subscribes to the events.
        /// </summary>
        public IndividualColumnWindow() {
            InitializeComponent();

            this.pointSetListBox.DataSource = ProjectController.Project.pointSets;
            SetColumnListBox();

            radRadioButton.Checked = true;

            //bind events
            this.newColumnFormula.Click += delegate(object sender, EventArgs e) {
                this.lastSelectionStart = newColumnFormula.SelectionStart;
            };
            this.columnListBox.MouseClick += ColumnListBox_MouseClick;
            this.newColumnFormula.KeyUp += NewColumnFormula_KeyUp;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Fills the columnListBox with the Columns common to all the selected PointSets.
        /// </summary>
        private void SetColumnListBox() {
            this.columnListBox.Items.Clear();
            if ( pointSetListBox.SelectedItems.Count > 0 ) {
                ColumnSet commonColumnSet = selectedPointSets[0].ColumnSet;
                for ( int i = 1; i < pointSetListBox.SelectedItems.Count; i++ ) {
                    commonColumnSet = ColumnSet.Intersect(commonColumnSet, selectedPointSets[i].ColumnSet);
                    //If the intersection is empty stop
                    if ( commonColumnSet == null ) return;
                }
                for (int i = 0; i <= commonColumnSet.Dimension - 1; i++) {
                    Column col = commonColumnSet.Columns[i];
                    String item = "$" + col.Index + " = " + col.ToString();
                    this.columnListBox.Items.Add(item);
                }
            }
        }

        private void CreateColumn() {
            if (newColumnName.Text.Trim() != "") {
                try {
                    Cursor.Current = Cursors.WaitCursor;
                    List<PointSet> pointSets = new List<PointSet>();
                    foreach (Object ps in pointSetListBox.SelectedItems) {
                        pointSets.Add((PointSet)ps);
                    }
                    Column newCol = IndividualColumns.CreateColumn(factorIndex, newColumnFormula.Text, pointSets, this.newColumnName.Text);
                    Cursor.Current = Cursors.Default;

                    ColumnPropertyDialog cpd = new ColumnPropertyDialog(newCol.DefaultColumnProperty);
                    cpd.StartPosition = FormStartPosition.CenterParent;
                    cpd.ShowDialog();

                    this.DialogResult = DialogResult.OK;
                } catch {
                    ProjectController.Project.columns.RemoveAt(ProjectController.Project.columns.Count - 1);
                    MessageBox.Show("Enter a valid formula.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } finally {
                    Cursor.Current = Cursors.Default;
                }
            } else {
                MessageBox.Show("Enter a name for the new Column!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Event Handling Stuff

        #region Operator Buttons

        /// <summary>
        /// Sets the factorIndex to the value stored in the Tag of the selected RadioButton.
        /// 0 for DEG, 1 for RAD, 2 for GRAD.
        /// </summary>
        /// <param name="sender">One of the angle mode RadioButtons (degRadioButton, radRadioButton, gradRadioButton)</param>
        /// <param name="e">Standard EventArgs</param>
        private void AngleModeChanged(object sender, EventArgs e) {
            if (((RadioButton)sender).Checked == true) {
                factorIndex = Convert.ToInt16(((RadioButton)sender).Tag.ToString());
            }
        }

        /// <summary>
        /// Adds the Tag of the pressed button in the operatorsGroupBox at the cursor-position
        /// in the newColumnFormula TextBox, or replaces selected text with it.
        /// </summary>
        /// <param name="sender">Button in the operatorsGroupBox, with the exception of the backButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void OperatorButton_Click(object sender, EventArgs e) {
            if (newColumnFormula.SelectedText != "") {
                newColumnFormula.SelectedText = ((Button)sender).Tag.ToString();
                lastSelectionStart = newColumnFormula.SelectionStart;
            } else {
                newColumnFormula.Text = newColumnFormula.Text.Insert(lastSelectionStart, ((Button)sender).Tag.ToString());
                newColumnFormula.SelectionStart = lastSelectionStart = lastSelectionStart + ((Button)sender).Tag.ToString().Length;
            }

            newColumnFormula.Focus();
        }

        /// <summary>
        /// Erases the symbol before the cursor-position or the selected text in the newColumnFormula TextBox.
        /// </summary>
        /// <param name="sender">backButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void BackButton_Click(object sender, EventArgs e) {
            if (newColumnFormula.SelectedText != "") {
                newColumnFormula.SelectedText = "";
            } else if (lastSelectionStart > 0) {
                String text = newColumnFormula.Text.Substring(0, lastSelectionStart - 1) + newColumnFormula.Text.Substring(lastSelectionStart, newColumnFormula.Text.Length - lastSelectionStart);
                newColumnFormula.Text = text;
                newColumnFormula.SelectionStart = lastSelectionStart = lastSelectionStart - 1;
            }

            newColumnFormula.Focus();
        }

        #endregion

        /// <summary>
        /// Updates the selectedPointSets and the columnListBox,
        /// when the selection in the pointSetListBox is changed.
        /// </summary>
        /// <param name="sender">pointSetListBox</param>
        /// <param name="e">Standard EventArgs</param>
        private void PointSetListBox_SelectedValueChanged(object sender, EventArgs e) {
            selectedPointSets.Clear();
            foreach (Object ps in pointSetListBox.SelectedItems) {
                selectedPointSets.Add((PointSet)ps);
            }
            SetColumnListBox();
        }

        /// <summary>
        /// Sets the lastSelectionStart, when the user presses a key in the newColumnFormula TextBox manually.
        /// </summary>
        /// <param name="sender">newColumnFormula TextBox</param>
        /// <param name="e">Standard KeyEventArgs</param>
        private void NewColumnFormula_KeyUp(object sender, KeyEventArgs e) {
            lastSelectionStart = newColumnFormula.SelectionStart;
            if (e.KeyCode == Keys.Enter) { this.CreateColumn(); }
        }

        /// <summary>
        /// Adds the shortcut for the SelectedItem in the columnListBox to the newColumnFomula.Text
        /// or replaces the selected text with it.
        /// </summary>
        /// <param name="sender">MouseClick in columnListBox</param>
        /// <param name="e">Standard MouseEvenArgs</param>
        private void ColumnListBox_MouseClick(object sender, MouseEventArgs e) {
            String text = columnListBox.SelectedItem.ToString();
            if (newColumnFormula.SelectedText != "") {
                newColumnFormula.SelectedText = text.Substring(0, text.IndexOf('=') - 1);
                lastSelectionStart = newColumnFormula.SelectionStart;
            } else {
                newColumnFormula.Text = newColumnFormula.Text.Insert(lastSelectionStart, text.Substring(0, text.IndexOf('=') - 1));
                newColumnFormula.SelectionStart = lastSelectionStart = lastSelectionStart + text.IndexOf('=') - 1;
            }
            newColumnFormula.Focus();
        }

        /// <summary>
        /// Creates the new Column with the given name and formula and adds it to the selected PointSets.
        /// </summary>
        /// <param name="sender">createButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void CreateColumnButton_Click(object sender, EventArgs e) {
            this.CreateColumn();
        }

        #endregion
    }
}