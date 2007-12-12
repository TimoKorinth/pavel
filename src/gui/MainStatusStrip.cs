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
using Pavel.Framework;

namespace Pavel.GUI {
    /// <summary>
    /// The StatusStrip of PAVELs MainWindow.
    /// </summary>
    public class MainStatusStrip : StatusStrip {

        #region Fields

        private ToolStripStatusLabel currentUseCaseLabel;
        private ToolStripStatusLabel selectionCount;
        private ToolStripStatusLabel progressBarLabel;
        private ToolStripProgressBar progressBar;

        #endregion

        #region Constructors

        /// <summary>
        /// Inititializes the MainStatusStrip and subsrcibes to events.
        /// </summary>
        public MainStatusStrip()
            : base() {
            InitCurrentUseCaseLabel();
            InitSelectionCount();
            InitProgressBar();
            InitProgressBarLabel();

            Items.Add(currentUseCaseLabel);
            Items.Add(selectionCount);
            Items.Add(progressBarLabel);
            Items.Add(progressBar);

            SubscribeToEvents();
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            UnsubscribeFromEvents();
            base.Dispose(disposing);
        }

        #endregion

        # region Methods

        /// <summary>
        /// Initializes the currentUseCaseLabel.
        /// </summary>
        private void InitCurrentUseCaseLabel() {
            currentUseCaseLabel = new ToolStripStatusLabel();
            currentUseCaseLabel.Spring = true;
            currentUseCaseLabel.Visible = true;
            currentUseCaseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            currentUseCaseLabel.Alignment = ToolStripItemAlignment.Left;
            UpdateUseCaseLabel();
        }

        /// <summary>
        /// Initializes the selectionCount.
        /// </summary>
        private void InitSelectionCount() {
            selectionCount = new ToolStripStatusLabel();
            selectionCount.Visible = false;
            selectionCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        }

        /// <summary>
        /// Initializes the progressBar.
        /// </summary>
        private void InitProgressBar() {
            progressBar = new ToolStripProgressBar();
            progressBar.Visible = false;
        }

        /// <summary>
        /// Initializes the progressBarLabel.
        /// </summary>
        private void InitProgressBarLabel() {
            progressBarLabel = new ToolStripStatusLabel();
            progressBarLabel.Visible = false;
        }

        /// <summary>
        /// Starts the progressBar with the given <paramref name="min"/> as the startvalue
        /// and <paramref name="max"/> as the endvalue and stepsize 1.
        /// The progressBarLabel.Text is set to <paramref name="label"/>.
        /// </summary>
        /// <param name="min">The min Value</param>
        /// <param name="max">The Max Value</param>
        /// <param name="label">The Label for the Progress Bar</param>
        public void StartProgressBar(int min, int max, String label) {
            // Threadsafe-Implementation
            VoidDelegate code = delegate {
                progressBar.Visible = true;
                progressBarLabel.Visible = true;
                progressBarLabel.Text = label;
                progressBar.Minimum = min;
                progressBar.Maximum = max;
                progressBar.Value = 0;
                progressBar.Step = 1;
                this.Refresh();
            };

            if (this.InvokeRequired) {
                this.Invoke(code);
            } else {
                code();
            }
        }

        /// <summary>
        /// Increments the progressBar by <paramref name="inc"/>.
        /// </summary>
        /// <param name="inc">The steps to increment</param>
        public void IncrementProgressBar(int inc) {
            // Threadsafe-Implementation
            VoidDelegate code = delegate {
                progressBar.Increment(inc);
            };

            if (this.InvokeRequired) {
                this.Invoke(code);
            } else {
                code();
            }
        }

        /// <summary>
        /// Increments the progressBar by <paramref name="inc"/>
        /// and changes the label to <paramref name="label"/>.
        /// </summary>
        /// <param name="inc">The steps to increment</param>
        /// <param name="label">The text for the label</param>
        public void IncrementProgressBar(int inc, string label) {
            VoidDelegate code = delegate {
                progressBar.Increment(inc);
                progressBarLabel.Text = label;
            };

            if (this.InvokeRequired) {
                this.Invoke(code);
            } else {
                code();
            }
        }

        /// <summary>
        /// Set the progressBar Invisible.
        /// </summary>
        public void EndProgressBar() {
            VoidDelegate code = delegate {
                progressBar.Visible = false;
                progressBarLabel.Visible = false;
            };

            if (this.InvokeRequired) {
                this.Invoke(code);
            } else {
                code();
            }
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Subscribes to the ProjectController.CurrentSelection.SelectionModified event and
        /// handles it by changing the selectionCount.Text.
        /// </summary>
        private void SubscribeToEvents() {
            ProjectController.CurrentSelection.SelectionModified += CurrentSelection_SelectionModified;
            ProjectController.ProjectNew += ProjectChanged;
            ProjectController.ProjectOpened += ProjectChanged;
            ProjectController.ProjectReset += ProjectChanged;
        }

        /// <summary>
        /// Unsubscribes the Events
        /// </summary>
        private void UnsubscribeFromEvents() {
            ProjectController.ProjectReset -= ProjectChanged;
            ProjectController.ProjectNew -= ProjectChanged;
            ProjectController.ProjectOpened -= ProjectChanged;
            ProjectController.CurrentSelection.SelectionModified -= CurrentSelection_SelectionModified;
        }

        void CurrentSelection_SelectionModified(object sender, EventArgs e) {
            if (ProjectController.CurrentSelection.Length != 0) {
                selectionCount.Text = "Selection Count: " + ProjectController.CurrentSelection.Length;
                selectionCount.Visible = true;
            } else { selectionCount.Visible = false; }
        }

        private void ProjectChanged(object sender, EventArgs e) {
            UpdateUseCaseLabel();
        }

        private void UpdateUseCaseLabel() {
            if (ProjectController.Project.UseCase != null) {
                currentUseCaseLabel.Text = "Use Case: " + ProjectController.Project.UseCase.Label;
            } else {
                currentUseCaseLabel.Text = "Use Case: (none)";
            }
        }

        #endregion
    }
}
