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
using Pavel.GUI.Visualizations;

namespace Pavel.GUI {
    /// <summary>
    /// Allows the user to create new PointSets and erase existing PointSets.
    /// </summary>
    public partial class PointSetManager : Form {

        #region Fields

        private PointSet selectedPointSet;
        private bool inUse;

        #endregion

        #region Constructors

        /// <summary>
        /// Standard-Constructor. PointSets can be erased, but not created.
        /// </summary>
        public PointSetManager() {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // Set the DataSource to be shown
            this.pointSetList.DataSource = ProjectController.Project.pointSets;
            // Which field should be shown? Label
            this.pointSetList.DisplayMember = "Label";
            this.pointSetList.SelectedIndex = 0;
        }

        /// <summary>
        /// Used to open the PointSetManager when a VisualizationWindow is already open.
        /// This presets the pointSetNameBox with a name "Part of <paramref name="currentPointSet"/>"
        /// and allows creating new PointSets.
        /// </summary>
        /// <param name="selectedPointSet">
        /// The PointSet displayed in the VisualizationWindow active, when the PointSetManager was opened
        /// </param>
        public PointSetManager(PointSet selectedPointSet) : base() {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // Set the DataSource to be shown
            this.pointSetList.DataSource = ProjectController.Project.pointSets;
            // Which field should be shown? Label
            this.pointSetList.DisplayMember = "Label";
            this.pointSetList.SelectedIndex = this.pointSetList.Items.IndexOf(selectedPointSet);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks, whether a PointSet is currently in use and sets the inUse flag.
        /// </summary>
        private void CheckPointSetUse() {
            this.inUse = false;
            foreach (Form f in PavelMain.MainWindow.MdiChildren) {
                VisualizationWindow vw = f as VisualizationWindow;
                if (vw!=null && vw.PointSet == this.selectedPointSet) {
                    this.inUse = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the list of PointSets.
        /// </summary>
        private void UpdateList() {
            this.pointSetList.DataSource = null;
            this.pointSetList.DataSource = ProjectController.Project.pointSets;
        }

        /// <summary>
        /// Checks, whether the name in the pointSetNameBox is unique.
        /// </summary>
        /// <returns>bool indicating, whether the entered name is unique</returns>
        private bool UniqueName() {
            bool uniqueName = true;
            foreach (PointSet ps in ProjectController.Project.pointSets) {
                if (this.pointSetNameBox.Text.Trim() == ps.Label) uniqueName = false;
            }
            return uniqueName;
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Makes the necessary adjustments, when the user selects a value in the pointSetList.
        /// </summary>
        /// <param name="sender">pointSetList</param>
        /// <param name="e">Standard EventArgs</param>
        private void SelectedPointSetChanged(object sender, EventArgs e) {
            this.selectedPointSet = (PointSet)this.pointSetList.SelectedValue;
            if (this.selectedPointSet != null) {
                this.pointSetNameBox.Text = this.selectedPointSet.Label;
                this.CheckPointSetUse();
            }

            if ( this.selectedPointSet is Clustering.ClusterSet ) {
                pointSetFromClusterButton.Enabled = true;
            } else {
                pointSetFromClusterButton.Enabled = false;
            }
        }

        /// <summary>
        /// Changes the name of a PointSet, when the renameButton is clicked
        /// and the entered name is valid and unique.
        /// </summary>
        /// <param name="sender">renameButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void RenameButton_Click(object sender, EventArgs e) {
            if (this.pointSetNameBox.Text != selectedPointSet.Label) {
                if ((!this.UniqueName()) || (this.pointSetNameBox.Text == "")) {
                    MessageBox.Show("Please enter a valid & unique name!", "Error");
                } else if (this.selectedPointSet != null) {
                    if (MessageBox.Show("Do you really want to rename this PointSet?", "Rename?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3) == DialogResult.Yes) {
                        this.selectedPointSet.Label = this.pointSetNameBox.Text;
                        this.UpdateList();
                    }
                    ProjectController.SetProjectChanged(true);
                }
            }
        }

        /// <summary>
        /// Copies the selected pointSet
        /// </summary>
        /// <param name="sender">createButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void CopyPointSetButton_Click(object sender, EventArgs e) {
            if (( this.pointSetNameBox.Text.Trim() != "" ) && ( this.UniqueName() )) {
                //Create new PointSet
                PointSet newPointSet = new PointSet(this.pointSetNameBox.Text, this.selectedPointSet.ColumnSet);
                newPointSet.AddRange(this.selectedPointSet); //TODO: Kopierfunktion in PointSet.cs
                ProjectController.Project.pointSets.Add(newPointSet);
                this.UpdateList();
                this.pointSetList.SelectedIndex = this.pointSetList.Items.IndexOf(newPointSet);
                ProjectController.SetProjectChanged(true);
            } else { MessageBox.Show("Please enter a valid & unique name!", "Error"); }
        }

        /// <summary>
        /// Erases a PointSet, if it isn't locked.
        /// </summary>
        /// <param name="sender">eraseButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void EraseButton_Click(object sender, EventArgs e) {
            if (!(this.selectedPointSet.Locked)) {
                this.CheckPointSetUse();
                if (inUse) {
                    MessageBox.Show("This PointSet is currently in use. Close the visualizations using it, before erasing the PointSet.");
                }else if (MessageBox.Show("Do you really want to erase this PointSet?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3) == DialogResult.Yes) {
                    ProjectController.Project.pointSets.Remove(this.selectedPointSet);
                    this.UpdateList();
                    this.pointSetList.SelectedIndex = 0;
                    ProjectController.SetProjectChanged(true);
                }
            } else { MessageBox.Show("You can't erase this PointSet!"); }
        }

        /// <summary>
        /// Closes the PointSetManager
        /// </summary>
        /// <param name="sender">exitButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void ExitButtonClick(object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// Creates a PointSet from a ClusterSet by extracting all Points which created the cluster
        /// </summary>
        /// <param name="sender">pointsetFromClusterButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void PointSetFromClusterButton_Click(object sender, EventArgs e) {
            if ( (this.pointSetNameBox.Text != "") && (this.UniqueName()) ) {
                Clustering.ClusterSet clusterSet = this.selectedPointSet as Clustering.ClusterSet;

                PointSet ps = clusterSet.PointSetFromClusters(this.pointSetNameBox.Text);

                if (ps.Length != 0) {
                    ProjectController.Project.pointSets.Add(ps);
                    this.UpdateList();
                    this.pointSetList.SelectedIndex = this.pointSetList.Items.IndexOf(ps);
                    ProjectController.SetProjectChanged(true);
                } else { MessageBox.Show("The PointSet you tried to create is empty.\nIt will not be created."); }
            }
            else { MessageBox.Show("Please enter a valid & unique name!", "Error"); }
        }

        #endregion
    }
}