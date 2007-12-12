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

namespace Pavel.GUI {
    /// <summary>
    /// Shows a given logo for a given time.
    /// </summary>
    public class SplashScreen : Form {

        #region Fields

        private Timer timer;
        private TimeSpan time;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the SplashScreen with the given TimeSpan <paramref name="time"/>
        /// and the Bitmap <paramref name="image"/> and shows it.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="image"></param>
        public SplashScreen(TimeSpan time, Bitmap image) {
            timer = new Timer();
            this.time = time;
            this.InitializeComponent();
            this.BackgroundImage = image;
            this.ClientSize = image.Size;
            timer.Interval = (int)time.TotalMilliseconds;
            timer.Start();
            timer.Tick += CloseSplashScreen;
            this.Show();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the SplashScreen.
        /// </summary>
        private void InitializeComponent() {
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new Size(0, 0);
            this.ControlBox = false;
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Opacity = 1;
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Closes the SplashScreen
        /// </summary>
        /// <param name="sender">This SplashScreens Timer</param>
        /// <param name="e">Standard EventArgs</param>
        private void CloseSplashScreen(object sender, EventArgs e) {
            this.Close();
        }

        #endregion
    }
}
