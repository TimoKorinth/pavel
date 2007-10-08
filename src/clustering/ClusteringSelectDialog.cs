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
using System.Reflection;

using Pavel.Framework;

namespace Pavel.Clustering {
    /// <summary>
    /// A dialog for clustering.
    /// </summary>
    public partial class ClusteringSelectDialog : Form {

        #region Fields

        private ClusteringAlgorithm clusteringAlgorithm;
        private int tmpProgress;
        private string tmpProgressMessage;
        private System.Timers.Timer progressBarUpdateTimer;
        private int lastGlobalProgress = 0;
        private bool progressBarChanged = false;

        private System.Threading.Thread workerThread;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the ClusteringSelectDialog.
        /// </summary>
        public ClusteringSelectDialog() {
            InitializeComponent();

            //Set Data-Sources
            algorithmBox.DataSource = PavelMain.PluginManager.AvailableClusteringAlgorithms;
            
            pointSetDropDown.DataSource = ProjectController.Project.pointSets;
            pointSetDropDown.DisplayMember = "Label";
            int pointSetIndex = PavelMain.MainWindow.ActivePointSetIndex();
            if (pointSetIndex >= 0) { pointSetDropDown.SelectedIndex = pointSetIndex; }

            progressBarUpdateTimer = new System.Timers.Timer(1000);
            progressBarUpdateTimer.AutoReset = false;
            progressBarUpdateTimer.Elapsed += UpdateProgressBar;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Fills the dialog with meaningful values.
        /// </summary>
        private void CreateArgumentControls() {
            Type clusteringAlgorithmType = ((ClusteringAlgorithm)algorithmBox.SelectedItem).GetType();

            try {
                clusteringAlgorithm = ((ClusteringAlgorithm)Activator.CreateInstance(clusteringAlgorithmType));
            } catch (Exception e) {
                PavelMain.LogBook.Warning("Creating Clustering-Algorithm was not possible:\n" + e.Message, true);
                this.Close();
            }
            //Set Std-Clustering-Args
            clusteringAlgorithm.PointSet = (PointSet)this.pointSetDropDown.SelectedItem;
            clusteringAlgorithm.Space = this.columnSetDropDown.SelectedItem as Space;

            if (clusteringAlgorithm.Space != null) {
                CreateRelevanceList();
            }

            //Set Std-Name
            this.nameBox.Text = (ClusteringAlgorithm)algorithmBox.SelectedItem + " Cluster-Set " + String.Format("{0:D2}:{1:D2}", DateTime.Now.Hour, DateTime.Now.Minute);
            clusteringAlgorithm.Name = this.nameBox.Text;

            this.clusteringArgsPanel.Controls.Clear();
            
            ClusteringArgumentControl argumentControl = clusteringAlgorithm.ArgumentControl;
            
            argumentControl.AutoSize = true;
            argumentControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            argumentControl.Location = new System.Drawing.Point(0, 0);
            argumentControl.Name = "argumentControl";
            argumentControl.TabIndex = 10;

            this.clusteringArgsPanel.Controls.Add(argumentControl);

            int clusteringArgsPanelHeight = clusteringArgsPanel.Height - 10;
            clusteringParameterGroupBox.Height = 46 + clusteringArgsPanelHeight;
            argsTable.Height = 257 + clusteringArgsPanelHeight;
            progressPanel.Location = new System.Drawing.Point(progressPanel.Location.X, 258 + clusteringArgsPanelHeight);
            //actionButton.Location = new System.Drawing.Point(actionButton.Location.X, 340 + clusteringArgsPanelHeight);
            //cancelButton.Location = new System.Drawing.Point(cancelButton.Location.X, 340 + clusteringArgsPanelHeight);
            Height = 387 + clusteringArgsPanelHeight;
        }

        /// <summary>
        /// Resets the dialog to the start values.
        /// </summary>
        private void ResetDialog() {
            // Switch to Dialogs Thread
            VoidDelegate resetDialog = delegate {
                //Reset Button-Function
                actionButton.Text = "Start Clustering";
                actionButton.Click -= ActionButton_Cancel;
                actionButton.Click += ActionButton_Click;

                //Reset Dialog
                argsTable.Enabled = true;
                argsTable.Enabled = true;
                cancelButton.Enabled = true;

                //Reset Progressbar
                progressBar.Value = 0;
                progressLabel.Text = "";
                percentLabel.Text = "0%";

                //Update PointSets
                pointSetDropDown.DataSource = null;
                pointSetDropDown.DataSource = ProjectController.Project.pointSets;

                CreateArgumentControls();
            };
            this.Invoke(resetDialog);
        }

        /// <summary>
        /// Change the action of the actionButton, so that the clustering is stopped,
        /// when the actionButton is clicked.
        /// </summary>
        private void PrepareDialogForClustering() {
            //Disable Dialog
            this.argsTable.Enabled = false;
            this.cancelButton.Enabled = false;

            //Change Button-Function
            actionButton.Click -= this.ActionButton_Click;
            actionButton.Click += this.ActionButton_Cancel;

            //Change actionButton label
            this.actionButton.Text = "Stop Clustering";
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Aborts the clustering.
        /// </summary>
        /// <param name="sender">actionButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void ActionButton_Cancel(object sender, EventArgs e) {
            DialogResult res = MessageBox.Show("Do you want to cause clustering-algorithm to produce an intermediate result?\n\n"
                + "Select \"Yes\" to ask the clustering-algorithm to come to an end. If the algorithm proceeds try it again and choose \"No\"\n"
                + "Select \"No\" to stop clustering immediately\n"
                + "Select \"Cancel\" to do nothing",
                "Try to get intermediate result?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == DialogResult.No) {
                workerThread.Abort();
            } else if (res == DialogResult.Yes) {
                clusteringAlgorithm.InterruptClustering();
            }
        }

        /// <summary>
        /// Starts the clustering.
        /// </summary>
        /// <param name="sender">actionButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void ActionButton_Click(object sender, EventArgs e) {
            PrepareDialogForClustering();

            SignalProgressEventHandler progressHandler = delegate(int progress, string message) {
                this.progressBarChanged = true;
                this.tmpProgress = progress;
                this.tmpProgressMessage = message;
            };

            clusteringAlgorithm.ProgressChanged += progressHandler;
            Pavel.Clustering.ClusterSet result = null;

            workerThread = new System.Threading.Thread(delegate() {
                long duration = 0L;
                try {
                    progressBarUpdateTimer.Start();
                    PavelMain.MainWindow.StatusBar.StartProgressBar(0, 1000, "Clustering... 0%");
                    lastGlobalProgress = 0;
                    duration = DateTime.Now.Ticks;
                    result = clusteringAlgorithm.Start();
                } finally {
                    duration = DateTime.Now.Ticks - duration;
                    progressBarUpdateTimer.Stop();
                    PavelMain.MainWindow.StatusBar.EndProgressBar();

                    if (clusteringAlgorithm.ErrorMessage == null && result != null) {
                        // Clustering finished properly
                        ProjectController.Project.pointSets.Add(result);
                        if ( duration > 60000000 ) { //60 seconds
                            PavelMain.LogBook.Message("Clustering finished in " + duration / 10000 + "ms", true);
                        } else {
                            PavelMain.LogBook.Message("Clustering finished in " + duration / 10000 + "ms", false);
                        }
                    }
                    else {
                        if (workerThread.ThreadState != System.Threading.ThreadState.AbortRequested) {
                            // Internal error while Clustering
                            PavelMain.LogBook.Error("Error occured while Clustering:\n" + clusteringAlgorithm.ErrorMessage, true);
                        }
                    }

                    clusteringAlgorithm.ProgressChanged -= progressHandler;
                    ResetDialog();
                }
            });

            workerThread.Start();
            ProjectController.SetProjectChanged(true);
        }

        /// <summary>
        /// Updates the dialog, when a different PointSet is selected.
        /// </summary>
        /// <param name="sender">pointSetBox</param>
        /// <param name="e">Standard EventArgs</param>
        private void PointSetBox_SelectedValueChanged(object sender, EventArgs e) {
            if (pointSetDropDown.SelectedValue == null) { return; }
            clusteringAlgorithm.PointSet = (PointSet)pointSetDropDown.SelectedValue;

            columnSetDropDown.DataSource = Space.AllSpacesForColumnSet(clusteringAlgorithm.PointSet.ColumnSet);
            columnSetDropDown.DisplayMember = "Label";
            int spaceIndex = PavelMain.MainWindow.ActiveSpaceIndex(clusteringAlgorithm.PointSet);
            if (spaceIndex >= 0) { columnSetDropDown.SelectedIndex = spaceIndex; }
            
            clusteringAlgorithm.Space = columnSetDropDown.SelectedItem as Space;
        }

        /// <summary>
        /// Switches the Custering-Arguments.
        /// </summary>
        /// <param name="sender">algorithmBox</param>
        /// <param name="e">Standard EventArgs</param>
        private void AlgorithmBox_SelectedValueChanged(object sender, EventArgs e) {
            CreateArgumentControls();
        }

        /// <summary>
        /// Changes the name of the selected ClusteringAlgorithm.
        /// </summary>
        /// <param name="sender">nameBox</param>
        /// <param name="e">Standard EventArgs</param>
        private void NameBox_TextChanged(object sender, EventArgs e) {
            this.clusteringAlgorithm.Name = this.nameBox.Text;
        }

        /// <summary>
        /// Makes the necessary changes, when a different Space is selected.
        /// </summary>
        /// <param name="sender">columnSetDropDown</param>
        /// <param name="e">Standard EventArgs</param>
        private void ColumnSetDropDown_SelectedValueChanged(object sender, EventArgs e) {
            clusteringAlgorithm.Space = columnSetDropDown.SelectedItem as Space;
            CreateRelevanceList();
        }

        /// <summary>
        /// Binds Data to relevanceList (Treeview)
        /// </summary>
        private void CreateRelevanceList() {
            relevanceList.Nodes.Clear();
            List<ColumnProperty> columnProperties = new List<ColumnProperty>(clusteringAlgorithm.Space.ColumnProperties);
            clusteringAlgorithm.RelevantColumns = new bool[columnProperties.Count];

            for (int i = 0; i < columnProperties.Count; i++) {
                TreeNode node = new TreeNode(columnProperties[i].Label);
                clusteringAlgorithm.RelevantColumns[i] = true;
                node.Checked = true;
                relevanceList.Nodes.Add(node);
            }

        }

        /// <summary>
        /// Changes the CheckState when an item in the relevanceList is checked.
        /// </summary>
        /// <param name="sender">relevanceList</param>
        /// <param name="e">TreeViewEventArgs</param>
        private void relevanceList_AfterCheck(object sender, TreeViewEventArgs e) {
            clusteringAlgorithm.RelevantColumns[e.Node.Index] = e.Node.Checked;
        }

        /// <summary>
        /// Updates the ProgressBar.
        /// </summary>
        /// <param name="sender">progressBarUpdateTimer</param>
        /// <param name="e">Standard ElapsedEventArgs</param>
        private void UpdateProgressBar(object sender, System.Timers.ElapsedEventArgs e) {
            // Set Text only if something has changed
            if (progressBarChanged) {
                // Switch to Dialogs Thread
                VoidDelegate update = delegate {
                    progressLabel.Text = tmpProgressMessage;
                    progressBar.Value = tmpProgress;
                    percentLabel.Text = String.Format("{0:F1} %", (float)tmpProgress / 10f);
                };
                this.Invoke(update);

                if (tmpProgress - lastGlobalProgress > 10) {
                    PavelMain.MainWindow.StatusBar.IncrementProgressBar(
                        tmpProgress - lastGlobalProgress,
                        string.Format("Clustering... {0}%", tmpProgress / 10));
                    lastGlobalProgress = tmpProgress;
                }
                progressBarChanged = false;
            }
            // Restart Timer
            progressBarUpdateTimer.Start();
        }

        #endregion
    }
}