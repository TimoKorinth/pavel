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
using System.Text;
using System.Windows.Forms;
using Pavel.Framework;

namespace Pavel.GUI.Visualizations {
    /// <summary>
    /// FreeToolStrip containing the elements common to all Visualizations.
    /// </summary>
    public class VisualizationStandardToolStrip : FreeToolStrip {

        #region Fields

        private Visualization visualization;

        private ToolStripButton  resetSpaceButton;
        private ToolStripButton  saveSpaceButton;
        private ToolStripButton  saveSpaceAsButton;
        private ToolStripButton  invertSelectionButton;
        private ToolStripButton  deleteButton;
        private ToolStripButton  undoDeletingPointsButton;
        private ToolStripButton  makeScreenshot;
        private ToolStripButton  filterMode;
        private ToolStripButton  clusterMode;
        private ToolStripLabel   numberOfClusterLabel;
        private ToolStripTextBox numberOfCluster;

        #endregion

        #region Constructor / Dispose

        /// <summary>
        /// Initializes the VisualizationStandardToolStrip.
        /// </summary>
        /// <param name="vis">Visualization this VisualizationStandardToolStrip belongs to</param>
        public VisualizationStandardToolStrip(Visualization vis) {
            visualization = vis;

            InitializeResetSpaceButton();
            InitializeSaveSpaceButton();
            InitializeSaveSpaceAsButton();
            InitializeInvertSelectionButton();
            InitializeDeleteButton();
            InitializeUndoDeletingPointsButton();
            InitializeMakeScreenShotButton();
            InitializeFilterModeButton();
            InitializeClusterModeButton();
            InitializeNumberOfClusterButtonAndLabel();

            Items.Add(resetSpaceButton);
            Items.Add(saveSpaceButton);
            Items.Add(saveSpaceAsButton);
            Items.Add(new ToolStripSeparator());
            Items.Add(invertSelectionButton);
            Items.Add(deleteButton);
            Items.Add(undoDeletingPointsButton);
            Items.Add(makeScreenshot);
            Items.Add(new ToolStripSeparator());
            Items.Add(filterMode);
            Items.Add(clusterMode);
            Items.Add(numberOfClusterLabel);
            Items.Add(numberOfCluster);
            Items.Add(new ToolStripSeparator());

            UpdateToolStrip();
        }

        #endregion

        #region Button Initializers

        private void InitializeResetSpaceButton() {
            resetSpaceButton = new ToolStripButton(Pavel.Properties.Resources.RepeatHS);
            resetSpaceButton.ToolTipText = "Reset Space";
            resetSpaceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            resetSpaceButton.Click += delegate(object sender, EventArgs e) {
                visualization.VisualizationWindow.Space = visualization.VisualizationWindow.Space.Parent;
            };
        }

        private void InitializeSaveSpaceButton() {
            saveSpaceButton = new ToolStripButton(Pavel.Properties.Resources.saveButton);
            saveSpaceButton.ToolTipText = "Save Space";
            saveSpaceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            saveSpaceButton.Click += delegate(object sender, EventArgs e) {
                visualization.VisualizationWindow.Space.WriteBack();
            };
        }

        private void InitializeSaveSpaceAsButton() {
            saveSpaceAsButton = new ToolStripButton(Pavel.Properties.Resources.saveAsButton);
            saveSpaceAsButton.ToolTipText = "Save Space as...";
            saveSpaceAsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            saveSpaceAsButton.Click += delegate(object sender, EventArgs e) {
                SaveSpaceAsDialog saveSpaceAsDialog = new SaveSpaceAsDialog(visualization.VisualizationWindow.Space, false);
                if (saveSpaceAsDialog.ShowDialog() == DialogResult.OK) {
                    visualization.VisualizationWindow.Space = saveSpaceAsDialog.SavedSpace;
                }
            };
        }

        private void InitializeInvertSelectionButton() {
            invertSelectionButton = new ToolStripButton(Pavel.Properties.Resources.InvertSelection);
            invertSelectionButton.ToolTipText = "Invert selection";
            invertSelectionButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            invertSelectionButton.Click += delegate(object sender, EventArgs e) {
                visualization.InvertSelection();
            };
        }

        private void InitializeDeleteButton() {
            deleteButton = new ToolStripButton(Pavel.Properties.Resources.Delete);
            deleteButton.ToolTipText = "Delete selected points";
            deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            deleteButton.Click += delegate(object sender, EventArgs e) {
                visualization.DeleteSelectedPoints();
            };
        }

        private void InitializeUndoDeletingPointsButton() {
            undoDeletingPointsButton = new ToolStripButton(Pavel.Properties.Resources.Undo);
            undoDeletingPointsButton.ToolTipText = "Undo Deleting Points";
            undoDeletingPointsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            undoDeletingPointsButton.Click += delegate(object sender, EventArgs e) {
                visualization.VisualizationWindow.PointSet.Undo();
            };
        }

        private void InitializeMakeScreenShotButton() {
            makeScreenshot = new ToolStripButton(Pavel.Properties.Resources.screenshot);
            makeScreenshot.ImageTransparentColor = System.Drawing.Color.Magenta;
            makeScreenshot.ToolTipText = "Make Screenshot";
            makeScreenshot.Click += delegate(object sender, EventArgs e) {
                System.Drawing.Bitmap bitmap = visualization.Screenshot();
                SaveFileDialog sd = new SaveFileDialog();
                System.Drawing.Imaging.ImageFormat imagef = System.Drawing.Imaging.ImageFormat.Bmp;
                sd.Filter = "Bitmap|*.bmp|JPEG|*.jpg|PNG|*.png";
                if (sd.ShowDialog() == DialogResult.OK) {
                    switch (sd.FilterIndex) {
                        case 2:
                            imagef = System.Drawing.Imaging.ImageFormat.Jpeg;
                            break;
                        case 3:
                            imagef = System.Drawing.Imaging.ImageFormat.Png;
                            break;
                        default:
                            imagef = System.Drawing.Imaging.ImageFormat.Bmp;
                            break;
                    }
                    bitmap.Save(sd.FileName, imagef);
                }
            };
        }

        private void InitializeFilterModeButton() {
            filterMode = new ToolStripButton(Pavel.Properties.Resources.Filter);
            filterMode.ImageTransparentColor = System.Drawing.Color.Red;
            filterMode.ToolTipText = "Filter Mode";
            filterMode.CheckOnClick = true;
            filterMode.Click += delegate(object sender, EventArgs e) {
                visualization.VisualizationWindow.FilterMode = filterMode.Checked;
            };
        }

        private void InitializeClusterModeButton() {
            clusterMode = new ToolStripButton(Pavel.Properties.Resources.Cluster);
            clusterMode.ImageTransparentColor = System.Drawing.Color.Red;
            clusterMode.ToolTipText = "Cluster Mode";
            clusterMode.CheckOnClick = true;
            clusterMode.Click += delegate(object sender, EventArgs e) {
                visualization.VisualizationWindow.ClusterMode = clusterMode.Checked;
            };
        }

        private void InitializeNumberOfClusterButtonAndLabel() {
            numberOfClusterLabel = new ToolStripLabel("Number of clusters");
            numberOfCluster = new ToolStripTextBox();
            numberOfCluster.Enabled = true;
            numberOfCluster.TextChanged += delegate(object sender, EventArgs e) {
                Clustering.HierarchicalClusterList clusterList
                    = visualization.VisualizationWindow.PointSet.PointLists[0]
                    as Clustering.HierarchicalClusterList;
                int number = clusterList.DefaultClusterCount;
                if (int.TryParse(numberOfCluster.Text, out number)) {
                    clusterList.DefaultClusterCount = number;
                }
                visualization.VisualizationWindow.CreateDisplayedPointSet();
                visualization.PointSetModified(this, null);
            };
        }

        #endregion

        public void UpdateToolStrip( ) {
            PointSet ps = visualization.VisualizationWindow.PointSet;
            clusterMode.Visible          = ps is Clustering.ClusterSet;
            numberOfCluster.Visible      = ps.PointLists[0] is Clustering.HierarchicalClusterList;
            numberOfClusterLabel.Visible = numberOfCluster.Visible;
            Clustering.HierarchicalClusterList clusterList = ps.PointLists[0] as Clustering.HierarchicalClusterList;

            if ( clusterList != null ) numberOfCluster.Text = clusterList.DefaultClusterCount.ToString();
        }
    }
}
