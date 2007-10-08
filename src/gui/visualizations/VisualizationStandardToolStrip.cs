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
        private ToolStripButton clusterMode;
        private ToolStripTextBox numberOfCluster;
        private ToolStripLabel numberOfClusterLabel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the VisualizationStandardToolStrip.
        /// </summary>
        /// <param name="vis">Visualization this VisualizationStandardToolStrip belongs to</param>
        public VisualizationStandardToolStrip(Visualization vis) {
            this.visualization = vis;

            ToolStripButton deleteButton = new ToolStripButton(Pavel.Properties.Resources.Delete);
            deleteButton.ToolTipText = "Delete selected points";
            deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            deleteButton.Click += delegate(object sender, EventArgs e) {
                vis.DeleteSelectedPoints();
            };
            this.Items.Add(deleteButton);

            ToolStripButton undoDeletingPointsButton = new ToolStripButton(Pavel.Properties.Resources.Undo);
            undoDeletingPointsButton.ToolTipText = "Undo Deleting Points";
            undoDeletingPointsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            undoDeletingPointsButton.Click += delegate(object sender, EventArgs e) {
                vis.VisualizationWindow.PointSet.Undo();
            };
            this.Items.Add(undoDeletingPointsButton);

            ToolStripButton makeScreenshot = new ToolStripButton(Pavel.Properties.Resources.screenshot);
            makeScreenshot.ImageTransparentColor = System.Drawing.Color.Magenta;
            makeScreenshot.ToolTipText = "Make Screenshot";
            makeScreenshot.Click += delegate(object sender, EventArgs e) {
                System.Drawing.Bitmap bitmap = vis.Screenshot();
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
            this.Items.Add(makeScreenshot);
            this.Items.Add(new ToolStripSeparator());
            
            ToolStripButton filterMode = new ToolStripButton(Pavel.Properties.Resources.Filter);
            filterMode.ImageTransparentColor = System.Drawing.Color.Red;
            filterMode.ToolTipText = "Filter Mode";
            filterMode.CheckOnClick = true;
            filterMode.Click += delegate(object sender, EventArgs e) {
                visualization.VisualizationWindow.FilterMode = filterMode.Checked;
            };
            this.Items.Add(filterMode);

            clusterMode = new ToolStripButton(Pavel.Properties.Resources.Cluster);
            clusterMode.ImageTransparentColor = System.Drawing.Color.Red;
            clusterMode.ToolTipText = "Cluster Mode";
            clusterMode.CheckOnClick = true;
            clusterMode.Click += delegate(object sender, EventArgs e) {
                visualization.VisualizationWindow.ClusterMode = clusterMode.Checked;
            };
            Items.Add(clusterMode);

            numberOfClusterLabel = new ToolStripLabel("Number of clusters");
            this.Items.Add(numberOfClusterLabel);

            numberOfCluster = new ToolStripTextBox();
            numberOfCluster.Enabled = true;
            numberOfCluster.TextChanged += delegate(object sender, EventArgs e) {
                Clustering.HierarchicalClusterList clusterList
                    = visualization.VisualizationWindow.PointSet.PointLists[0]
                    as Clustering.HierarchicalClusterList;
                int number = clusterList.DefaultClusterCount;
                if ( int.TryParse(numberOfCluster.Text, out number) ) {
                    clusterList.DefaultClusterCount = number;
                }
                visualization.VisualizationWindow.CreateDisplayedPointSet();
                visualization.PointSetModified(this, null);
            };
            this.Items.Add(numberOfCluster);

            this.Items.Add(new ToolStripSeparator());

            this.UpdateToolStrip();
        }

        #endregion

        #region Methods
        public void UpdateToolStrip( ) {
            clusterMode.Visible = visualization.VisualizationWindow.PointSet is Clustering.ClusterSet;
            numberOfCluster.Visible =  visualization.VisualizationWindow.PointSet.PointLists[0] is Clustering.HierarchicalClusterList;
            numberOfClusterLabel.Visible = visualization.VisualizationWindow.PointSet.PointLists[0] is Clustering.HierarchicalClusterList;
            Clustering.HierarchicalClusterList clusterList
                   = visualization.VisualizationWindow.PointSet.PointLists[0]
                   as Clustering.HierarchicalClusterList;

            if ( clusterList != null ) numberOfCluster.Text = clusterList.DefaultClusterCount.ToString();
        }
        #endregion
    }
}
