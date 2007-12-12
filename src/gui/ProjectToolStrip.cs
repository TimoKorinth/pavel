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
    /// ToolStrip containing Controls related to opening Projects and/or Files.
    /// </summary>
    public class ProjectToolStrip : ToolStrip {

        #region Fields

        private ToolStripButton startProjectButton;
        private ToolStripButton closeButton;
        private ToolStripButton saveButton;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the ToolStripButtons and subscribes to events.
        /// </summary>
        public ProjectToolStrip()
            : base() {
            InitializeStartProjectButton();
            InitializeCloseButton();
            InitializeSaveButton();

            Items.Add(startProjectButton);
            Items.Add(closeButton);
            Items.Add(saveButton);

            SubscribeToEvents();
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            UnSubscribeToEvents();
            base.Dispose(disposing);
        }

        #endregion

        #region Methods

        #region Initialize Buttons

        /// <summary>
        /// Initializes the startProjectButton.
        /// </summary>
        private void InitializeStartProjectButton() {
            startProjectButton = new ToolStripButton();
            startProjectButton.Image = Pavel.Properties.Resources.NewDocumentHS;
            startProjectButton.Text = "Start/Open Project";
            startProjectButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            startProjectButton.ImageTransparentColor = System.Drawing.Color.Black;
            startProjectButton.Click += delegate(object sender, EventArgs e) {
                PavelMain.MainWindow.StartProject();
            };
        }

        /// <summary>
        /// Initializes the closeButton.
        /// </summary>
        private void InitializeCloseButton() {
            closeButton = new ToolStripButton();
            closeButton.Text = "Close Project";
            closeButton.Image = Pavel.Properties.Resources.CloseProject;
            closeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            closeButton.ImageTransparentColor = System.Drawing.Color.Black;
            closeButton.Enabled = false;

            closeButton.Click += delegate(object sender, EventArgs e) {
                PavelMain.MainWindow.CloseProject();
            };
        }

        /// <summary>
        /// Initializes the saveButton.
        /// </summary>
        private void InitializeSaveButton() {
            saveButton = new ToolStripButton();
            saveButton.Text = "Save Project";
            saveButton.Image = Pavel.Properties.Resources.saveButton;
            saveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveButton.ImageTransparentColor = System.Drawing.Color.Black;
            saveButton.Enabled = false;

            saveButton.Click += delegate(object sender, EventArgs e) {
                PavelMain.MainWindow.SaveProject();
            };
        }

        #endregion

        /// <summary>
        /// Subscribes to the events to enable or disable the ToolStripButtons.
        /// </summary>
        private void SubscribeToEvents() {
            ProjectController.ProjectOpened += ProjectController_ProjectOpened;
            ProjectController.ProjectReset += ProjectController_ProjectReset;
            ProjectController.ProjectExport += ProjectController_ProjectSaved;
            ProjectController.ProjectChanged += ProjectController_ProjectChanged;
        }

        /// <summary>
        /// Unsubscribes to the events to enable or disable the ToolStripButtons.
        /// </summary>
        private void UnSubscribeToEvents() {
            ProjectController.ProjectOpened -= ProjectController_ProjectOpened;
            ProjectController.ProjectReset -= ProjectController_ProjectReset;
            ProjectController.ProjectChanged -= ProjectController_ProjectChanged;
            ProjectController.ProjectExport -= ProjectController_ProjectSaved;
        }

        void ProjectController_ProjectOpened(object sender, EventArgs e) {
            saveButton.Enabled = false;
            closeButton.Enabled = true;
        }
        
        void ProjectController_ProjectReset(object sender, EventArgs e) {
            saveButton.Enabled = false;
            closeButton.Enabled = false;
        }

        void ProjectController_ProjectSaved(object sender, EventArgs e) {
            saveButton.Enabled = false;
        }

        void ProjectController_ProjectChanged(object sender, EventArgs e) {
            saveButton.Enabled = true;
        }
        
        #endregion
    }
}
