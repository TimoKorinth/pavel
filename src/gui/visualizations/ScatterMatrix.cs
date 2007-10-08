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
using Pavel.Framework;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Pavel.GUI.Visualizations {
    /// <summary>
    /// The starting class for the ScatterMatrix, that contains the properties
    /// relevant for the user.
    /// </summary>
    public class ScatterMatrix : Visualization {

        #region ToolStrip

        /// <summary>
        /// ToolStrip for the ScatterMatrix.
        /// </summary>
        private class ScatterMatrixToolStrip : VisualizationStandardToolStrip {
            public ScatterMatrixToolStrip(ScatterMatrixControl control)
                : base(control.Visualization) {
                ToolStripButton resView = new ToolStripButton(Pavel.Properties.Resources.RepeatHS);
                resView.ImageTransparentColor = System.Drawing.Color.Black;
                resView.ToolTipText = "Reset View";
                resView.Click += delegate(object sender, EventArgs e) {
                    control.ResetView();
                };
                this.Items.Add(resView);

                ToolStripButton zoomIn = new ToolStripButton(Pavel.Properties.Resources.zoom);
                zoomIn.ImageTransparentColor = System.Drawing.Color.White;
                zoomIn.ToolTipText = "Zoom In";
                zoomIn.Click += delegate(object sender, EventArgs e) {
                    control.ZoomIn();
                };
                this.Items.Add(zoomIn);

                ToolStripButton zoomOut = new ToolStripButton(Pavel.Properties.Resources.zoomOut);
                zoomOut.ImageTransparentColor = System.Drawing.Color.White;
                zoomOut.ToolTipText = "ZoomOut";
                zoomOut.Click += delegate(object sender, EventArgs e) {
                    control.ZoomOut();
                };
                this.Items.Add(zoomOut);

                ToolStripButton selected = new ToolStripButton(Pavel.Properties.Resources.zoomNew);
                selected.ImageTransparentColor = System.Drawing.Color.White;
                selected.ToolTipText = "Show Selected in New Window";
                selected.Click += delegate(object sender, EventArgs e) {
                    control.ShowSelected();
                };
                this.Items.Add(selected);

                ToolStripButton completeLegend = new ToolStripButton(Pavel.Properties.Resources.TileWindowsVerticallyHS);
                completeLegend.ImageTransparentColor = System.Drawing.Color.White;
                completeLegend.ToolTipText = "Show Complete Legend";
                completeLegend.Click += delegate(object sender, EventArgs e) {
                    (control.Visualization as ScatterMatrix).Legend.ShowMe(new System.Drawing.Point(0, 30), 3000);
                };
                this.Items.Add(completeLegend);
            }
        }

        #endregion

        #region Fields

        private ScatterMatrixControl control;
        private ScatterMatrixToolStrip toolStrip;
        private Legend legend;
        private float pointSize = 3.0f;
        // alpha value for points
        private int pointsAlpha = 80;
        private float alphaPoints = 0.8f;

        public enum ScattermatrixPickModes { Points, Plots }
        private ScattermatrixPickModes pickMode = ScattermatrixPickModes.Plots;

        private ColorOGL selectionColor = new ColorOGL(Color.Azure, Color.Azure.Name);

        // picturebox to contain the resize-bitmap
        private PictureBox pictureBox = new PictureBox();

        #endregion

        #region Properties

        /// <value> The size of the points as a float between 0.5f and 64.0f.</value>
        [ShowInProperties]
        [Category("Points")]
        [Browsable(true)]
        [DisplayName("point size")]
        [Description("Defines the size of the points. It should not be more than 10, because you will not see the single points separated any more. The default is 3.0.")]
        public float PointSize {
            get { return pointSize; }
            set {
                if (value <= 0.5f) { pointSize = 0.5f; } else if (value >= 64.0f) { pointSize = 64.0f; } else { pointSize = value; }
                control.Invalidate();
            }
        }

        /// <value> The alpha value of the points as an int between 0 and 100.</value>
        [ShowInProperties]
        [Category("Points")]
        [DisplayName("point transparency")]
        [Description("Defines the transparency of the point. The default value is 80%.")]
        [Editor(typeof(PropertySliderEditor), typeof(System.Drawing.Design.UITypeEditor)), Range(0, 100)]
        public int PointsAlpha {
            get { return pointsAlpha; }
            set {
                pointsAlpha = PropertySliderEditor.BoundValue(value, 0, 100);
                alphaPoints = (float)pointsAlpha / 100;
                control.Invalidate();
            }
        }

        /// <value> Used to determine, whether the user wants to pick points or plots. </value>
        [ShowInProperties]
        [Category("Points")]
        [DisplayName("Picking")]
        [Description("Choose between picking plots or points")]
        public ScattermatrixPickModes PickMode {
            get { return pickMode; }
            set { pickMode = value; }
        }

        /// <value> Gets the color of the selected ScatterPlots or sets it. </value>
        [ShowInProperties]
        [Category("Color")]
        [DisplayName("selection color")]
        [Description("Defines the color of the selected scatterplots.")]
        public ColorOGL SelectionColor {
            get { return selectionColor; }
            set {
                selectionColor = value;
                selectionColor.Description = value.Color.Name;
                control.Invalidate();
            }
        }

        /// <value>Gets The alpha value of the points as a float between 0 and 1. </value>
        public float AlphaPoints {
            get { return alphaPoints; }
        }

        /// <value>Gets the PictureBox that contains the screenshot used for resizing or sets it.</value>
        public PictureBox PictureBox {
            get { return pictureBox; }
            set { pictureBox = value; }
        }

        /// <value>Gets the legend explaining the axes of the ScatterPlots in the ScatterMatrix.</value>
        public Legend Legend { get { return legend; } }

        /// <value> Gets the icon for the ScatterMatrix. </value>
        public static System.Drawing.Bitmap Icon { get { return Pavel.Properties.Resources.Matrix; } }

        /// <value> Gets the special VisualizationStandardToolStrip for the ScatterMatrix.</value>
        public override VisualizationStandardToolStrip ToolStrip { get { return this.toolStrip; } }

        /// <value>Gets the ScatterMatrixControl. </value>
        public override System.Windows.Forms.Control Control { get { return this.control; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor that initializes the ScatterMatrix.
        /// </summary>
        /// <param name="vw">The VisualizationWindow containing this visualization</param>
        public ScatterMatrix(VisualizationWindow vw) : base(vw) {
            legend = new Legend(vw);
            SetToolTipText();
            this.control = new ScatterMatrixControl(this);
            control.AdaptPointSize();
            this.toolStrip = new ScatterMatrixToolStrip(this.control);
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Dock = DockStyle.Fill;
        }

        #endregion

        #region Methods

        #region Refresh

        /// <summary>
        /// Resets the properties
        /// </summary>
        public void ResetProperties() {
            this.PointSize = 3.0f;
            // alpha value for points
            this.PointsAlpha = 80;
            this.PickMode = ScattermatrixPickModes.Plots;
            this.SelectionColor = new ColorOGL(Color.Azure, Color.Azure.Name);

            control.MakeCurrentContext();
        }

        /// <summary>
        /// Updates the legend with the Columns of the current Space.
        /// </summary>
        /// <remarks>This method is used to create the complete legend.</remarks>
        public void SetToolTipText() {
            this.legend.ToolTipText.BeginUpdate();
            this.legend.ToolTipText.Items.Clear();
            int numOfAxes = this.VisualizationWindow.Space.Dimension;
            for (int i = 0; i <= numOfAxes - 1; i++) {
                String item = i + " = " + this.VisualizationWindow.Space.ColumnProperties[i].ToString();
                this.legend.ToolTipText.Items.Add(item);
            }
            this.legend.ToolTipText.EndUpdate();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Updates the Settings, when the Space is changed.
        /// </summary>
        public override void UpdateSpace() {
            this.ResetProperties();
            SetToolTipText();
            this.control.UpdateSpace();
        }

        /// <summary>
        /// Returns the screenshot for this Visualization.
        /// </summary>
        /// <returns>Screenshot of the ScatterMatrix</returns>
        public override Bitmap Screenshot() {
            return control.Screenshot();
        }

        /// <summary>
        /// Eventhandler to deal with a change of the PointSet.
        /// </summary>
        /// <param name="sender">The displayed PointSet</param>
        /// <param name="e">Standard EventArgs</param>
        public override void PointSetModified(object sender, EventArgs e) {
            base.PointSetModified(sender, e);
            this.control.UpdatePointSet();
        }

        /// <summary>
        /// Redraws the ScatterMatrix, when the selection is modified.
        /// </summary>
        /// <param name="sender">Curren selection</param>
        /// <param name="e">Standard EventArgs</param>
        public override void SelectionModified(object sender, EventArgs e) {
            control.Invalidate();
        }

        #endregion

        #endregion
    }
}
