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
    public partial class ParetoFinderWindow : Form {


        public ParetoFinderWindow() {
            InitializeComponent();
        }

        /// <summary>
        /// Sets Parameters for the Progress Bar in the ParetoFinderWindow
        /// </summary>
        /// <param param name="maximum">Highest Value of the Counter for the progress bar</param>
        /// <param param name="stepsize">Size of the steps in the progress bar</param>
        public void setParetoFinderWindow(int maximum, int stepsize) {
            this.progressBar.Maximum = maximum;
            this.progressBar.Step = stepsize;
        }

        /// <summary>
        /// Refreshes the ProgressBar
        /// </summary>
        public void progressBarStep() {
            this.progressBar.PerformStep();
        }

        private void ParetoFinderWindow_Load(object sender, EventArgs e) {

        }






    }
}