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
    /// A Form with the log-messages.
    /// </summary>
    public partial class LogWindow : TabableForm {

        #region Constructors

        /// <summary>
        /// Subscribes to the LogChanged event and updates the LogWindow.
        /// </summary>
        public LogWindow() {
            InitializeComponent();
            logGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            PavelMain.LogBook.LogChanged += this.LogChanged;
            UpdateLogView();
            this.TabPag.Text = "Logbook";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the Table
        /// </summary>
        private void UpdateLogView() {
            LogLevelEnum lle= new LogLevelEnum();
            if (errorButton.Checked) {lle |= LogLevelEnum.Error; }
            if (warningButton.Checked) { lle |= LogLevelEnum.Warning; }
            if (messageButton.Checked) { lle |= LogLevelEnum.Message; }
            // disconnect and reconnect to populate changes
            VoidDelegate del = delegate {
                logGrid.DataSource = null;
                logGrid.DataSource = PavelMain.LogBook.GetLogs(lle);
            };
            if (this.InvokeRequired) {
                this.Invoke(del);
            } else { del(); }
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Called when a log is added or the state of one of the buttons (error, warning, message) is changed.
        /// </summary>
        /// <param name="sender">PavelMain.LogBook, this.errorButton, this.warningButton, this.messageButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void LogChanged(object sender, EventArgs e) {
            UpdateLogView();
        }

        #endregion
    }
}