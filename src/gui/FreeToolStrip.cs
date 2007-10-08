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

namespace Pavel.GUI {

    /// <summary>
    /// This special ToolStrip can be freed from its panel to float freely.
    /// </summary>
    public class FreeToolStrip : ToolStrip {

        #region Fields

        private FreeToolStripForm toolStripForm;
        // Flag that stores, whether the FreeToolStrip is already undocked,
        // so is not undocked again and again and again ...
        private Boolean undocked;
        // The first time toolStripForm is opened, the location has to be set after "Show".
        // After that, it can be set before, so it doesn't flicker.
        private Boolean first;

        #endregion

        #region Properties

        /// <value>
        /// Sets the undocked variable. True, if the FreeToolStrip is not contained
        /// in one of the MainWindows ToolStripPanels. False, if it is.
        /// </value>
        public Boolean Undocked {
            set { undocked = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// The constructor creates the FreeToolStripForm and hides it.
        /// undocked is initialized with the value false;
        /// </summary>
        public FreeToolStrip() {
            toolStripForm = new FreeToolStripForm();
            toolStripForm.Hide();
            undocked = false;
            first = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// If the FreeToolStrip is undocked (contained in a FreeToolStripForm),
        /// the FreeToolStripForm is hidden, when it is dragged out of the form.
        /// undocked is set to false.
        /// </summary>
        /// <param name="e">Standard EventArgs</param>
        protected override void OnBeginDrag(EventArgs e) {
            if (undocked) {
                toolStripForm.HideForm();
            }
            base.OnBeginDrag(e);
        }

        /// <summary>
        /// This method creates a ToolStripForm at the Cursor.Position and adds the
        /// FreeToolStrip to it, when the FreeToolStrip is dragged out of the ToolStripPanel.
        /// If the FreeToolStrip is dragged into an other ToolStripPanel, it is added to that
        /// in the usual way.
        /// </summary>
        /// <param name="e">Standard EventArgs</param>
        protected override void OnEndDrag(EventArgs e) {
            // Checks, whether the FreeToolStrip isn't already undocked and the ToolStrip is dropped
            // outside of the ToolStripPanel.
            if ((!(RectangleToScreen((this.Parent as ToolStripPanel).ClientRectangle).Contains(Cursor.Position))) && (!undocked)) {
                base.OnEndDrag(e);

                undocked = true;
                toolStripForm.Owner = this.FindForm();
                toolStripForm.Text = this.Text;

                // The location for the toolStripForm is created.
                System.Drawing.Point pt = Cursor.Position;
                pt.Offset(-10, -30);
                if (!first) { toolStripForm.Location = pt; }

                // Adds the FreeToolStrip to the FreeToolStripForm.
                toolStripForm.AddToolStrip(this);
                toolStripForm.Show();

                // When the FreeToolStrip is dragged from the MainWindow ToolStripPanel for the first time,
                // the location has to be set after "Show".
                if (first) {
                    first = false;
                    toolStripForm.Location = pt;
                }
            } else {
                base.OnEndDrag(e);
            }
        }

        #endregion
    }
}