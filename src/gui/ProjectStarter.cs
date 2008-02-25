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
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace Pavel.GUI {
    /// <summary>
    /// This class is the starting point for creating / opening Projects.
    /// It allows starting a new Project with one of the given useCases or
    /// open an existing Project stored on HD.
    /// </summary>
    public partial class ProjectStarter : Form {

        #region Fields

        private Pavel.Framework.PluginManager pluginManager;
        private Pavel.Framework.IUseCase selectedUseCase = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for this form.
        /// </summary>
        /// <param name="pluginManager">Pavels PluginManager</param>
        public ProjectStarter(Pavel.Framework.PluginManager pluginManager) {
            this.pluginManager = pluginManager;

            InitializeComponent();

            this.openProjectRB.DoubleClick += this.OpenProject_DoubleClick;

            InitializeLists();

            this.CenterToScreen();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the useCaseList and the recentProjectsList.
        /// </summary>
        private void InitializeLists() {
            if (pluginManager.AvailableUseCases.Count == 0) {
                MessageBox.Show("No UseCases were found, please check the plugin directory", "Error", MessageBoxButtons.OK);
            } else {
                foreach (Pavel.Framework.IUseCase useCase in pluginManager.AvailableUseCases) {
                    useCaseList.Items.Add(useCase.Label);
                }
            }

            DirectoryInfo dir = new DirectoryInfo(SpecialDirectories.CurrentUserApplicationData);
            if (dir.Exists) {
                if (File.Exists(dir + @"\recentprojects.prp")) {
                    StreamReader recentProjects = File.OpenText(dir + @"\recentprojects.prp");
                    string line = recentProjects.ReadLine();
                    while (line != null) {
                        if (File.Exists(line)) {
                            recentProjectsList.Items.Add(line);
                        }
                        line = recentProjects.ReadLine();
                    }
                    recentProjects.Dispose();
                }
            }
        }

        /// <summary>
        /// Flips to the next page and closes the ProjectStarter, when the last page is reached.
        /// When the PointSet contains more than 10.000 points, the user is asked to cluster them.
        /// </summary>
        private void NextPage() {
            Pavel.Framework.ProjectController.Project.UseCase = selectedUseCase;
            Control contr = null;
            // Obtains the next ProjectStarter page
#if !DEBUG
            try {
                contr = selectedUseCase.ProjectStarterPages.nextPageControl();
            } catch (Exception e) {
                return;
            }
#else
            contr = selectedUseCase.ProjectStarterPages.nextPageControl();
#endif

            // The last page is reached and this form closed
            if (contr == null) {
                selectedUseCase.ProjectStarterPages.Reset();
                this.Close();

                // If there are more than 10.000 points in the opened files, the user is advised to cluster.
                if (Pavel.Framework.ProjectController.Project.pointSets[0].Length > 10000) {
                    if (MessageBox.Show("The opened data is rather large.\nIt is recommend to cluster the data first.\nDo you want to open the Clustering Assistant?",
                        "Clustering recommend", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        Clustering.ClusteringSelectDialog cSD = new Pavel.Clustering.ClusteringSelectDialog();
                        cSD.ShowDialog();
                    }
                }
            }

            // The next page is set
            this.mainPanel.Controls.Clear();
            this.mainPanel.Controls.Add(contr);
        }

        #endregion

        #region Event Handling Stuff

        #region Button events

        /// <summary>
        /// Clicking the fwdButton either opens an existing Project 
        /// or starts / proceeds with the ProjectStarter for the selected useCase.
        /// </summary>
        /// <param name="sender">fwdButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void FwdButton_Click(object sender, EventArgs e) {
            if (createProjectRB.Checked) {

                // Checks whether the current ProjectStarter page has correct input to proceed
                if (!selectedUseCase.ProjectStarterPages.HasCorrectInput()) {
                    return;
                }

                NextPage();

            } else if (openRecentProjectRB.Checked && (this.recentProjectsList.SelectedIndices.Count > 0)) {
                // An existing project is opened
                try {
                    Pavel.Framework.ProjectController.OpenSavedProject((string)this.recentProjectsList.SelectedItem);
                } catch (Exception exception) {
                    Pavel.Framework.PavelMain.LogBook.Error("Error opening file " + this.recentProjectsList.SelectedItem + ": " + exception.Message, true);
                }
                this.Close();
            } else if (this.openProjectRB.Checked) {
                if (Pavel.Framework.PavelMain.MainWindow.OpenProject()) this.Close();
            }
        }

        // TODO: Sollte dann nicht das Project wieder geschlossen werden?
        /// <summary>
        /// Clicking the cancelButton resets the ProjectStarterPages and closes this form.
        /// </summary>
        /// <param name="sender">cancelButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void CancelButton_Click(object sender, EventArgs e) {
            Pavel.Framework.ProjectController.ResetProject();
            if (selectedUseCase != null) {
                selectedUseCase.ProjectStarterPages.Reset();
            }
            this.Close();
        }

        /// <summary>
        /// Clicking the backButton returns to the previous ProjectStarterPage
        /// </summary>
        /// <param name="sender">backButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void BackButton_Click(object sender, EventArgs e) {
            this.mainPanel.Controls.Clear();

            Control contr = selectedUseCase.ProjectStarterPages.previousPageControl();
            if (contr != null) {
                this.mainPanel.Controls.Add(contr);
            }
            else {
                selectedUseCase.ProjectStarterPages.ResetIndexer();
                this.mainPanel.Controls.Add(standardLayout);
            }
        }

        #endregion

        /// <summary>
        /// Every time a different Control is set, the buttons are updated.
        /// </summary>
        /// <param name="sender">The mainPanel</param>
        /// <param name="e">Standard ControlEventArgs</param>
        private void MainPanel_ControlAdded(object sender, ControlEventArgs e) {
            if (selectedUseCase != null) {
                backButton.Enabled = selectedUseCase.ProjectStarterPages.HasStarted();
                if (selectedUseCase.ProjectStarterPages.IsLastPage()) {
                    fwdButton.Text = "&Finish";
                }
                else {
                    fwdButton.Text = "&Next >";
                }
            }
        }

        /// <summary>
        /// If the checked radio button changes, the corresponding buttons and lists are updated.
        /// </summary>
        /// <param name="sender">openProjectRB</param>
        /// <param name="e">Standard EventArgs</param>
        private void OpenRecentProjectRB_CheckedChanged(object sender, EventArgs e) {
            if (this.openRecentProjectRB.Checked) {
                this.useCaseList.SelectedIndex = -1;
                this.fwdButton.Enabled = false;
            }
        }

        /// <summary>
        /// If the checked radio button changes, the corresponding buttons and lists are updated.
        /// </summary>
        /// <param name="sender">openProjectRB</param>
        /// <param name="e">Standard EventArgs</param>
        private void OpenProjectRB_CheckedChanged(object sender, EventArgs e) {
            if (openProjectRB.Checked) {
                this.recentProjectsList.SelectedIndex = -1;
                this.fwdButton.Enabled = true;
            }
        }

        /// <summary>
        /// If the checked radio button changes, the corresponding buttons and lists are updated
        /// </summary>
        /// <param name="sender">createProjectRB</param>
        /// <param name="e">Standard EventArgs</param>
        private void CreateProjectRB_CheckedChanged(object sender, EventArgs e) {
            if (this.createProjectRB.Checked) {
                this.recentProjectsList.SelectedIndex = -1;
                this.fwdButton.Enabled = false;
            }
        }

        /// <summary>
        /// Enables/disables the next button.
        /// </summary>
        /// <param name="sender">useCaseList</param>
        /// <param name="e">Standard EventArgs</param>
        private void UseCaseList_SelectedIndexChanged(object sender, EventArgs e) {
            fwdButton.Enabled = (useCaseList.SelectedIndices.Count > 0);
            if (useCaseList.SelectedIndices.Count > 0) {
                selectedUseCase = pluginManager.AvailableUseCases[useCaseList.SelectedIndices[0]];
            }
        }

        /// <summary>
        /// Invoked when a double click on the useCaseList occurs.
        /// A new Project is started when the corresponding radio button is checked.
        /// </summary>
        /// <param name="sender">useCaseList</param>
        /// <param name="e">Standard EventArgs</param>
        private void UseCaseList_DoubleClick(object sender, EventArgs e) {
            if (useCaseList.SelectedIndices.Count > 0) {
                NextPage();
            }
        }

        /// <summary>
        /// Invoked when a click on the recentProjectList occurs.
        /// </summary>
        /// <param name="sender">recentProjectsList</param>
        /// <param name="e">Standard EventArgs</param>
        private void RecentProjectsList_Click(object sender, EventArgs e) {
            if (!openRecentProjectRB.Checked) { openRecentProjectRB.Checked = true; }
        }

        /// <summary>
        /// Invoked when a double click on the recentProjectList occurs.
        /// </summary>
        /// <param name="sender">recentProjectList</param>
        /// <param name="e">Standard EventArgs</param>
        private void RecentProjectsList_DoubleClick(object sender, EventArgs e) {
            if (this.recentProjectsList.SelectedIndices.Count > 0) {
                try {
                    this.Cursor = Cursors.WaitCursor;
                    Pavel.Framework.ProjectController.OpenSavedProject((string)this.recentProjectsList.SelectedItem);
                } catch (Exception exception) {
                    this.Cursor = Cursors.Default;
                    Pavel.Framework.PavelMain.LogBook.Error("Error opening file " + this.recentProjectsList.SelectedItem + ": " + exception.Message, true);
                } finally {
                    this.Cursor = Cursors.Default;
                }
                this.Close();
            }
        }

        /// <summary>
        /// Invoked when a click on the useCaseList occurs.
        /// </summary>
        /// <param name="sender">useCaseList</param>
        /// <param name="e">Standard EventArgs</param>
        private void UseCaseList_Click(object sender, EventArgs e) {
            if (!createProjectRB.Checked) { createProjectRB.Checked = true; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenProject_DoubleClick(object sender, EventArgs e) {
            if (Pavel.Framework.PavelMain.MainWindow.OpenProject()) this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecentProjectsList_SelectedIndexChanged(object sender, EventArgs e) {
            if ((recentProjectsList.SelectedIndex >= 0) && (openRecentProjectRB.Checked)) {
                this.fwdButton.Enabled = true;
            }
        }

        #endregion
    }

    #region Helper class
    /// <summary>
    /// This class allows the event DoubleClick.
    /// </summary>
    class DoubleClickableRadioButton : RadioButton {
        public DoubleClickableRadioButton()
            : base() {
            this.SetStyle(ControlStyles.StandardClick, true);
            this.SetStyle(ControlStyles.StandardDoubleClick, true);
        }
    }
    #endregion
}