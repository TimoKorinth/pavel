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
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using Pavel.Framework;

namespace Pavel.GUI {
    /// <summary>
    /// A ComboBox with CheckBoxes.
    /// </summary>
    public class CheckComboBox : ComboBox {

        #region Fields

        /// <value> Event fired, if the CheckState is changed. </value>
        public event EventHandler CheckStateChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor that binds the neccessary events.
        /// </summary>
        public CheckComboBox() {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += CheckComboBox_DrawItem;
            this.SelectionChangeCommitted += CheckComboBox_SelectedIndexChanged;
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Changes the CheckState of the selected item and fires the CheckStateChanged event.
        /// </summary>
        /// <param name="sender">Selected item in CheckedComboBox</param>
        /// <param name="e">Standard EventArgs</param>
        private void CheckComboBox_SelectedIndexChanged( object sender, EventArgs e ) {
            if ((SelectedItem is CheckComboBoxItem)) {
                CheckComboBoxItem item = (CheckComboBoxItem)SelectedItem;
                item.CheckState = !item.CheckState;
                if (CheckStateChanged != null) { CheckStateChanged(item, e); }
            } else if ((SelectedItem is CheckComboBoxString)) {
                CheckComboBoxString selectedItem = (CheckComboBoxString)SelectedItem;
                if (selectedItem.Operation == CheckComboBoxString.Operations.SelectAll) {
                    for ( int i = 0; i < Items.Count; i++ ) {
                        CheckComboBoxItem item = this.Items[i] as CheckComboBoxItem;
                        if ( item != null ) item.CheckState = true;
                    }
                }
                if ( selectedItem.Operation == CheckComboBoxString.Operations.DeselectAll ) {
                    for (int i = 0; i < Items.Count; i++) {
                        CheckComboBoxItem item = this.Items[i] as CheckComboBoxItem;
                        if ( item != null ) item.CheckState = false;
                    }
                }
                if ( selectedItem.Operation == CheckComboBoxString.Operations.RemoveSelected) {
                    List<Selection> selections = new List<Selection>();
                    for (int i = 0; i < Items.Count; i++) {
                        CheckComboBoxItem item = this.Items[i] as CheckComboBoxItem;
                        if ( item != null && item.Selection.Active ) selections.Add(item.Selection);
                    }
                    ProjectController.RemoveSelections(selections);
                }
                if ( selectedItem.Operation == CheckComboBoxString.Operations.SetToCurrent) {
                    ProjectController.SetSelectionAsCurrentSelection();
                }
                if (CheckStateChanged != null) { CheckStateChanged(SelectedItem, e); }
            }
            this.Text = "";
        }

        /// <summary>
        /// Draws the CheckComboBoxes.
        /// </summary>
        /// <param name="sender">DrawItem</param>
        /// <param name="e">Standard EventArgs</param>
        private void CheckComboBox_DrawItem(object sender, DrawItemEventArgs e) {
            if (e.Index == -1) {
                return;
            }

            if (!(Items[e.Index] is CheckComboBoxItem)) {
                e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
                e.Graphics.DrawString(
                    Items[e.Index].ToString(),
                    this.Font,
                    Brushes.Black,
                    new System.Drawing.Point(e.Bounds.X, e.Bounds.Y));
                return;
            }

            CheckComboBoxItem box = (CheckComboBoxItem)Items[e.Index];

            CheckBoxRenderer.RenderMatchingApplicationState = true;
            e.Graphics.FillRectangle(new SolidBrush(box.BackColor), e.Bounds);
            CheckBoxRenderer.DrawCheckBox(
                e.Graphics,
                new System.Drawing.Point(e.Bounds.X, e.Bounds.Y),
                e.Bounds,
                box.Text,
                this.Font,
                false,
                box.CheckState ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);
        }

        #endregion
    }

    /// <summary>
    /// Container to store a string and an operation which is executed if this item is selected
    /// </summary>
    public class CheckComboBoxString {
        public enum Operations { SelectAll, DeselectAll, RemoveSelected, SetToCurrent };

        protected String s;
        protected Operations operation;

        /// <summary>
        /// Create a new Item for a checkBox with a displayed string and an operation
        /// </summary>
        /// <param name="s">String displayed in the comboBox</param>
        /// <param name="operation">Operation which is executed if item is selected</param>
        public CheckComboBoxString(String s, Operations operation) {
            this.s = s;
            this.operation = operation;
        }

        public Operations Operation {
            get { return operation; }
        }

        public override String ToString() {
            return s;
        }
    }
}
