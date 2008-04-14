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
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Pavel.Framework;


namespace Pavel.GUI.Visualizations {
    class ParallelPlot : Visualization {

        #region ParallelPlotToolStrip

        private class ParallelPlotToolStrip : VisualizationStandardToolStrip {
            // No own functionality ATM
            public ParallelPlotToolStrip(ParallelPlot pp) : base(pp) {}
        }

        #endregion

        #region Properties
        private int axesScale = 6;

        [ShowInProperties]
        [Category("Axes")]
        [Description("Defines the number of scaling intervals. Default is 6.")]
        [DisplayName("Scales")]
        [Editor(typeof(PropertySliderEditor), typeof(System.Drawing.Design.UITypeEditor)), Range(1, 15)]
        public int AxesScale {
            get { return axesScale; }
            set {
                axesScale = PropertySliderEditor.BoundValue(value, 1, 15);
                control.CreateScaleText( );
                control.Invalidate( );
            }
        }

        private int linesAlpha = 75;

        /// <summary>
        /// New version of PointsAlpha. Sets the value to 0 if anything less than 0 is entered,
        /// and to 100 if anything higher than 100 is entered. Allows choosing the alpha value
        /// by using a trackbar.
        /// </summary>
        [ShowInProperties]
        [Category("Lines")]
        [DisplayName("Transparency")]
        [Description("Defines transparency of the lines. Default 75%.")]
        [Editor(typeof(PropertySliderEditor), typeof(System.Drawing.Design.UITypeEditor)), Range(0, 100)]
        public int LinesAlpha {
            get { return linesAlpha; }
            set {
                linesAlpha = PropertySliderEditor.BoundValue(value, 0, 100);
                control.Invalidate( );
            }
        }

        private float lineWidth = 3.0f;

        [ShowInProperties]
        [Category("Lines")]
        [Description("Defines width of lines. Default is 3.0.")]
        [DisplayName("Width")]
        public float LineWidth {
            get { return lineWidth; }
            set {
                if (value <= 0.5f) { lineWidth = 0.5f; } else if (value >= 10) { lineWidth = 10; } else { lineWidth = value; }
                control.Invalidate( );
            }
        }

        private float clusterLineWidth = 2.0f;

        [ShowInProperties]
        [Category("Lines")]
        [Description("Defines width of lines representing clustering values. Default is 2.0.")]
        [DisplayName("Cluster line width")]
        public float ClusterLineWidth {
            get { return clusterLineWidth; }
            set {
                if (value <= 0.5f) { clusterLineWidth = 0.5f; } else if (value >= 10) { clusterLineWidth = 10; } else { clusterLineWidth = value; }
                control.Invalidate( );
            }
        }

        private int decimalDigits = 2;


        [ShowInProperties]
        [Category("Axes")]
        [Description("Defines number of decimal digits for scale values. Default is 2.")]
        [DisplayName("Decimal digits")]
        [Editor(typeof(PropertySliderEditor), typeof(System.Drawing.Design.UITypeEditor)), Range(0, 7)]
        public int DecimalDigits {
            get { return decimalDigits; }
            set {
                decimalDigits = PropertySliderEditor.BoundValue(value,0,7);
                control.CreateScaleText( );
                control.Invalidate( );
            }
        }

        #endregion

        #region Declarations

        private PictureBox pictureBox = new PictureBox();

        public PictureBox PictureBox {
            get { return pictureBox; }
            set { pictureBox = value; }
        }

        private ParallelPlotControl control;

        public override Control Control {
            get { return this.control; }
        }

        private ParallelPlotToolStrip parallelPlotToolStrip;

        public override VisualizationStandardToolStrip ToolStrip {
            get { return this.parallelPlotToolStrip; }
        }

        public static System.Drawing.Bitmap Icon {
            get { return Pavel.Properties.Resources.ParallelPlot; }
        }


        public ParallelPlot(VisualizationWindow vw)
            : base(vw) {
            control = new ParallelPlotControl(this);
            this.parallelPlotToolStrip = new ParallelPlotToolStrip(this);

            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Dock = DockStyle.Fill;
        }
        #endregion

        #region Methods

        public void ResetProperties( ) {
            this.axesScale = 6;            
            this.linesAlpha = 75;
            this.lineWidth = 3.0f;
            this.clusterLineWidth = 2.0f;
            this.decimalDigits = 2;
        }

        public override void UpdateSpace() {
            this.ResetProperties();
            control.ReInit();
        }

        public override void UpdateColors() {
            this.ResetProperties();
            control.ReInit();
        }

        public override void PointSetModified(object sender, EventArgs e) {
            base.PointSetModified(sender, e);
            control.ReInit();
        }

        public override Bitmap Screenshot() {
            return control.Screenshot();
        }

        /// <summary>Removes the Column at position p from the displayed space</summary>
        internal void RemoveColumn(int p) {
            if (this.VisualizationWindow.Space.Dimension > 2) {
                this.VisualizationWindow.Space.RemoveColumn(p);
                control.ReInit();
            } else {
                MessageBox.Show("At least two columns must remain!");
            }
        }

        #endregion

        #region Eventhandling
        public override void SelectionModified(object sender, EventArgs e) {
            control.CurrentSelectionChanged();
        }

        #endregion
    }
}
