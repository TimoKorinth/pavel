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
    /// This is a special Form for the FreeToolStrip,
    /// that sends the FreeToolStrip back to the MainWindow upon been closed.
    /// </summary>
    public partial class FreeToolStripForm : Form {

        #region Fields

        FreeToolStrip toolStrip;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the FreeToolStripForm.
        /// </summary>
        public FreeToolStripForm() {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the FreeToolStrip <paramref name="fts"/> to the Form.
        /// </summary>
        /// <param name="fts">The FreeToolStrip to be added.</param>
        public void AddToolStrip(FreeToolStrip fts) {
            this.ClientSize = new Size(500, 25);
            toolStrip = fts;
            this.toolStripPanel.Controls.Add(fts);
            this.ClientSize = new Size(toolStrip.Width + 1, toolStrip.Height);
        }

        /// <summary>
        /// Sends toolStrip back to the MainWindow,
        /// sets toolStrips undocked flag to false and hides the form.
        /// </summary>
        public void HideForm() {
            Pavel.Framework.PavelMain.MainWindow.AddToolStrip(toolStrip);
            toolStrip.Undocked = false;
            this.Hide();
        }

        /// <summary>
        /// The Closing is stopped.
        /// The FreeToolStrip is sent back to the MainWindow
        /// and the FreeToolStripForm is hidden.
        /// </summary>
        /// <param name="e">Standard CancelEventArgs</param>
        protected override void OnClosing(CancelEventArgs e) {
            e.Cancel = true;
            HideForm();
        }

        #endregion
    }
}