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
    /// ScatterPlot
    /// </summary>
    public class ScatterPlot : Visualization {

        /// <summary>
        /// ToolStrip for the ScatterPlot.
        /// </summary>
        private class ScatterPlotToolStrip : VisualizationStandardToolStrip {

            ToolStripButton resetView;
            ToolStripButton setOrigin;
            ToolStripButton setCP;
            ToolStripButton resetCP;
            ToolStripButton paretoEval;

            ScatterPlot     scatterPlot;

            public ScatterPlotToolStrip(ScatterPlot scatterPlot)
                : base(scatterPlot) {
                this.scatterPlot = scatterPlot;

                InitializeButtons();
                SubscribeToEvents();
                UpdateButtonStates();
            }

            void UpdateButtonStates() {
                if (scatterPlot.VisiblePointsAreSelected()) {
                    setOrigin.Enabled = true;
                    setCP.Enabled     = true;
                } else {
                    setOrigin.Enabled = false;
                    setCP.Enabled     = false;
                }
            }

            void InitializeButtons() {
                resetView                       = new ToolStripButton(Pavel.Properties.Resources.RepeatHS);
                resetView.ImageTransparentColor = System.Drawing.Color.Black;
                resetView.ToolTipText           = "Reset View";
                this.Items.Add(resetView);

                setOrigin                       = new ToolStripButton(Pavel.Properties.Resources.PushpinHS);
                setOrigin.ImageTransparentColor = System.Drawing.Color.Black;
                setOrigin.ToolTipText           = "Set Origin";
                this.Items.Add(setOrigin);

                setCP                           = new ToolStripButton(Pavel.Properties.Resources.zoom);
                setCP.ImageTransparentColor     = System.Drawing.Color.White;
                setCP.ToolTipText               = "Set Column Properties";
                this.Items.Add(setCP);

                resetCP                         = new ToolStripButton(Pavel.Properties.Resources.zoomOut);
                resetCP.ImageTransparentColor   = System.Drawing.Color.White;
                resetCP.ToolTipText             = "Reset Column Properties";
                this.Items.Add(resetCP);

                paretoEval                       = new ToolStripButton(Pavel.Properties.Resources.Pareto);
                paretoEval.ImageTransparentColor = System.Drawing.Color.Red;
                paretoEval.ToolTipText           = "Evaluate the Pareto Front";

                this.Items.Add(paretoEval);
            }

            void SubscribeToEvents() {
                resetView.Click  += this.resetViewHandler;
                setOrigin.Click  += this.setOriginHandler;
                setCP.Click      += this.setCPHandler;
                resetCP.Click    += this.resetCPHandler;
                paretoEval.Click += this.paretoEvalHandler;

                ProjectController.CurrentSelection.SelectionModified += this.SelectionModified;
            }

            void UnsubscribeFromEvents() {
                resetView.Click  -= this.resetViewHandler;
                setOrigin.Click  -= this.setOriginHandler;
                setCP.Click      -= this.setCPHandler;
                resetCP.Click    -= this.resetCPHandler;
                paretoEval.Click -= this.paretoEvalHandler;

                ProjectController.CurrentSelection.SelectionModified -= this.SelectionModified;
            }

            protected override void Dispose(bool disposing) {
                base.Dispose(disposing);
                UnsubscribeFromEvents();
                scatterPlot = null;
            }

            #region Handler
            void  resetViewHandler(object sender, EventArgs e) { scatterPlot.Reset(); }
            void  setOriginHandler(object sender, EventArgs e) { scatterPlot.SetTranslation(); }
            void      setCPHandler(object sender, EventArgs e) { scatterPlot.SetMinMax(); }
            void    resetCPHandler(object sender, EventArgs e) { scatterPlot.ResetCP(); }
            void paretoEvalHandler(object sender, EventArgs e) {
                try {
                    scatterPlot.Control.Cursor = Cursors.WaitCursor;
                    this.Cursor = Cursors.WaitCursor;
                    if ((scatterPlot.AxisX != null) && (scatterPlot.AxisY != null)) {
                        Selection sel;
                        if ((scatterPlot.AxisZ) != null) {
                            sel = ParetoFinder.EvaluateParetoFront(scatterPlot.VisualizationWindow.PointSet,
                                scatterPlot.AxisX, scatterPlot.AxisY, scatterPlot.AxisZ);
                        } else {
                            sel = ParetoFinder.EvaluateParetoFront(scatterPlot.VisualizationWindow.PointSet,
                                scatterPlot.AxisX, scatterPlot.AxisY);
                        }
                        //Deactivate all stored selections
                        foreach (Selection s in ProjectController.Selections){
                            s.Active = false;
                        }
                        //Add pareto-selection and show it
                        sel.Active = true;
                        ProjectController.AddSelection(sel);
                        ProjectController.SetSelectionAsCurrentSelection();
                    } else {
                        PavelMain.LogBook.Error("no valid Column", true);
                    }
                } finally {
                    this.Cursor = Cursors.Default;
                    scatterPlot.Control.Cursor = Cursors.Default;
                }
            }

            void SelectionModified(object sender, EventArgs e) { UpdateButtonStates(); }
            #endregion
        }

        #region Fields
        private ScatterPlotToolStrip toolStrip;
        private ScatterPlotControl   control;

        private Boolean      showCube      = false;
        private bool         showLegend    = true;
        private ScatterLines showLines     = ScatterLines.None;
        private int          scaleCount    = 4;
        private int          decimalDigits = 1;

        //TODO Umstellen auf ColorManagement
        private ColorOGL firstColor     = new ColorOGL(Color.Blue);
        private ColorOGL secondColor    = new ColorOGL(Color.Green);
        private ColorOGL[] colorTable;

        private ColumnProperty axisX, axisY, axisZ, axisC;
        private int   pointsAlpha;
        private float pointSize;
        #endregion

        #region Properties
        #region PropertyWindow Properties
        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("Axis X")]
        [Description("Defines the first axis.")]
        [Editor(typeof(AxesEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ColumnProperty AxisX {
            get { return axisX; }
            set {
                if (null == value) return;
                axisX = value;
                if (control != null) { control.AxisChanged(false); }
            }
        }

        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("Axis Y")]
        [Description("Defines the second axis.")]
        [Editor(typeof(AxesEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ColumnProperty AxisY {
            get { return axisY; }
            set {
                if (null == value) return;
                axisY = value;
                if (control != null) { control.AxisChanged(false); }
            }
        }

        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("Axis Z")]
        [Description("Defines the third axis.")]
        [Editor(typeof(AxesEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ColumnProperty AxisZ {
            get { return axisZ; }
            set {
                bool modeChanged = (null == axisZ && null != value) || (null != axisZ && null == value);
                axisZ = value;
                if (control != null) { control.AxisChanged(modeChanged); }
            }
        }

        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("Axis C")]
        [Description("Defines the fourth axis.")]
        [Editor(typeof(AxesEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ColumnProperty AxisC {
            get { return axisC; }
            set {
                axisC = value;
                if (control != null) { control.AxisChanged(false); }
            }
        }

        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("Legend")]
        [Description("Shows a legend for the fourth axis. Visible only if a fourth axis is selected.")]
        public bool ShowLegend {
            get { return showLegend; }
            set {
                showLegend = value;
                control.Invalidate();
            }
        }

        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("First color")]
        [Description("Defines first color for the fourth axis drawn in color gradation.")]
        public ColorOGL FirstColor {
            get { return firstColor; }
            set {
                firstColor = value;
                firstColor.Description = value.Color.Name;
                UpdateColorTable();
                control.ColorChanged();
            }
        }

        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("Second color")]
        [Description("Defines second color for the fourth axis drawn in color gradation.")]
        public ColorOGL SecondColor {
            get { return secondColor; }
            set {
                secondColor = value;
                secondColor.Description = value.Color.Name;
                UpdateColorTable();
                control.ColorChanged();
            }
        }


        /// <summary>
        /// New version of PointsAlpha. Sets the value to 0 if anything less than 0 is entered,
        /// and to 100 if anything higher than 100 is entered. Allows choosing the alpha value
        /// by using a trackbar. It currently sets the PointsAlpha value to do the real changes.
        /// </summary>
        [ShowInProperties]
        [Category("Points")]
        [DisplayName("Transparency")]
        [Description("Default is 80%.")]
        [Editor(typeof(PropertySliderEditor), typeof(System.Drawing.Design.UITypeEditor)), Range(0, 100)]
        public int PointsAlpha {
            get { return pointsAlpha; }
            set {
                pointsAlpha = PropertySliderEditor.BoundValue(value, 0, 100);
                control.ColorChanged();
            }
        }

        [ShowInProperties]
        [Category("Points")]
        [DisplayName("Size")]
        [Description("Default is 8.0f.")]
        public float PointSize {
            get { return pointSize; }
            set {
                if ( value <= 0.5f ) { pointSize = 0.5f; } else if ( value >= 64.0f ) { pointSize = 64.0f; } else { pointSize = value; }
                control.Invalidate();
            }
        }

        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("Decimal digits")]
        [Description("Defines number of decimal digits for the axes scales. Default is 1.")]
        [Editor(typeof(PropertySliderEditor), typeof(System.Drawing.Design.UITypeEditor)), Range(0, 7)]
        public int DecimalDigits {
            get { return decimalDigits; }
            set {
                decimalDigits = PropertySliderEditor.BoundValue(value, 0, 7);
                control.Invalidate();
            }
        }


        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("Scales")]
        [Description("Defines number of scaling intervals. Default is 4.")]
        [Editor(typeof(PropertySliderEditor), typeof(System.Drawing.Design.UITypeEditor)), Range(2, 30)]
        public int ScaleCount {
            get { return scaleCount; }
            set {
                scaleCount = PropertySliderEditor.BoundValue(value, 2, 30);
                control.Invalidate();
            }
        }

        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("Show cube")]
        [Description("Indicates if a cube displaying bounds is drawn or not.")]
        public Boolean ShowCube {
            get { return showCube; }
            set {
                showCube = value;
                control.Invalidate();
            }
        }

        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("Show lines")]
        [Description("Select a perspective mode from none(3D-mode) and xyAxes, xzAxes, yzAxes (2d-modes).")]
        public ScatterLines ShowLines {
            get { return showLines; }
            set {
                showLines = value;
                control.Invalidate();
            }
        }
        
        [ShowInProperties]
        [Category("Axes")]
        [DisplayName("Keep aspect")]
        [Description("When it is true, the relation between height and width is fixed.")]
        public bool KeepAspect {
            get { return control.keepAspect; }
            set { control.keepAspect = value; control.Invalidate(); }
        }

        [ShowInProperties]
        [Category("Stereo")]
        [DisplayName("Enabled")]
        [Description("Enables or disable Stereo Rendering")]
        public bool Stereo {
            get { return control.StereoMode; }
            set { control.StereoMode = value; control.ColorChanged(); }
        }
        [ShowInProperties]
        [Category("Stereo")]
        [DisplayName("Eye Distance")]
        public float EyeDistance {
            get { return control.EyeDistance; }
            set { control.EyeDistance = value; control.Invalidate(); 
            }
        }
        
        #endregion

        public ColorOGL[] ColorTable {
            get { return this.colorTable; }
            set {
                this.colorTable = value;
                control.Invalidate();
            }
        }

        /// <summary>
        /// A normalized float version of PointsAlpha for use in OpenGL
        /// </summary>
        public float AlphaPoints {
            get { return (float)pointsAlpha / 100; }
        }

        public override VisualizationStandardToolStrip ToolStrip {
            get { return this.toolStrip; }
        }

        public override System.Windows.Forms.Control Control {
            get { return this.control; }
        }

        public bool Mode2D {
            get { return null == axisZ; }
        }

        public static System.Drawing.Bitmap Icon {
            get { return Pavel.Properties.Resources.Scatterplot; }
        }
        #endregion

        public ScatterPlot(VisualizationWindow vw)
            : base(vw) {
            this.InitializeSettings( );
            this.control   = new ScatterPlotControl(this);
            this.toolStrip = new ScatterPlotToolStrip(this);
        }

        /// <summary>
        /// Sets up the axes to be displayed, depending on the availability of third or fourth column in the current Space
        /// </summary>
        private void InitializeAxes( ) {
            ColumnProperty[] cps = this.VisualizationWindow.Space.ColumnProperties;
            this.axisX = cps[0];
            this.axisY = cps[1];
            if (cps.Length > 2) {
                this.axisZ = cps[2];
            }
            this.axisC = null;
           
            if (null != control) control.AxisChanged(false);
        }

        public void InitializeSettings( ) {
            this.UpdateColorTable( );
            this.showCube       = false;
            this.showLines      = ScatterLines.None;
            this.scaleCount     = 4;
            this.decimalDigits  = 1;
            this.firstColor     = new ColorOGL(Color.Blue);
            this.secondColor    = new ColorOGL(Color.Green);
            // alpha value for points
            this.pointsAlpha    = 80;
            this.pointSize      = 8;
            this.showLegend     = true;
            this.InitializeAxes();
        }

        /// <summary>
        /// Initializes the ColorTable for the VisualizationWindow?
        /// </summary>
        private void UpdateColorTable() {
            colorTable = ColorOGL.InterpolationArray(firstColor, secondColor);
        }
        
        /// <summary>
        /// Resets Everything after changes to the Space have been made
        /// </summary>
        public override void UpdateSpace( ) {
            this.InitializeSettings( );
            this.control.Update( );
            control.SpaceChanged( );
        }

        public void Reset() {
            control.ResetView();
        }

        public override Bitmap Screenshot() {
            return control.Screenshot();
        }

        /// <summary>
        /// Adjust the column properties so that the center of all selected points is
        /// translated to the center of the Parallelplot
        /// </summary>
        public void SetTranslation() {
            int X = 0, Y = 1, Z = 2, C = 3;
            double[] newCenter = new double[4];
            double[] oldCenter = new double[4];
            double[] diff      = new double[4];
            

            Selection cs = ProjectController.CurrentSelection;
            foreach (Pavel.Framework.Point p in cs) {
                newCenter[X] += p[axisX.Column];
                newCenter[Y] += p[axisY.Column];
                if (null != axisZ) newCenter[Z] += p[axisZ.Column];
                if (null != axisC) newCenter[C] += p[axisC.Column];
            }

            newCenter[X] = newCenter[X] / cs.Length;
            oldCenter[X] = (axisX.Min + axisX.Max) / 2.0;
            diff[X]      = newCenter[X] - oldCenter[X];
            axisX.SetMinMax(axisX.Min + diff[X], axisX.Max + diff[X]);

            newCenter[Y] = newCenter[Y] / cs.Length;
            oldCenter[Y] = (axisY.Min + axisY.Max) / 2.0;
            diff[Y]      = newCenter[Y] - oldCenter[Y];
            axisY.SetMinMax(axisY.Min + diff[Y], axisY.Max + diff[Y]);

            if (null != axisZ) {
                newCenter[Z] = newCenter[Z] / cs.Length;
                oldCenter[Z] = (axisZ.Min + axisZ.Max) / 2.0;
                diff[Z]      = newCenter[Z] - oldCenter[Z];
                axisZ.SetMinMax(axisZ.Min + diff[Z], axisZ.Max + diff[Z]);
            }
            if (null != axisC) {
                oldCenter[C] = (axisC.Min + axisC.Max) / 2.0;
                newCenter[C] = newCenter[C] / cs.Length;
                diff[C]      = newCenter[C] - oldCenter[C];
                axisC.SetMinMax(axisC.Min + diff[C], axisC.Max + diff[C]);
            }
            
            control.ColumnPropertiesChanged();
        }

        /// <summary>
        /// Adjust the column properties in a way that the currently selected
        /// points define the displayed domain (their Minimum becomes ColumnProperty.Min)
        /// </summary>
        public void SetMinMax() {
            int X = 0, Y = 1, Z = 2, C = 3;
            double[][] mm = new double[4][];
            mm[X] = new double[] { Double.PositiveInfinity, Double.NegativeInfinity };
            mm[Y] = new double[] { Double.PositiveInfinity, Double.NegativeInfinity };
            mm[Z] = new double[] { Double.PositiveInfinity, Double.NegativeInfinity };
            mm[C] = new double[] { Double.PositiveInfinity, Double.NegativeInfinity };

            bool xAscending = AxisX.IsAscendingOrder();
            bool yAscending = AxisY.IsAscendingOrder();
            bool zAscending = true;
            bool cAscending = true;
            if (null != axisZ) zAscending = AxisZ.IsAscendingOrder();
            if (null != axisC) cAscending = AxisC.IsAscendingOrder();

            Selection cs = ProjectController.CurrentSelection;
            foreach (Pavel.Framework.Point p in cs) {
                if (p[axisX.Column] < mm[X][Result.MIN]) mm[X][Result.MIN] = p[axisX.Column];
                if (p[axisX.Column] > mm[X][Result.MAX]) mm[X][Result.MAX] = p[axisX.Column];
                if (p[axisY.Column] < mm[Y][Result.MIN]) mm[Y][Result.MIN] = p[axisY.Column];
                if (p[axisY.Column] > mm[Y][Result.MAX]) mm[Y][Result.MAX] = p[axisY.Column];
                if (null != axisZ) {
                    if (p[axisZ.Column] < mm[Z][Result.MIN]) mm[Z][Result.MIN] = p[axisZ.Column];
                    if (p[axisZ.Column] > mm[Z][Result.MAX]) mm[Z][Result.MAX] = p[axisZ.Column];
                }
                if (null != axisC) {
                    if (p[axisC.Column] < mm[C][Result.MIN]) mm[C][Result.MIN] = p[axisC.Column];
                    if (p[axisC.Column] > mm[C][Result.MAX]) mm[C][Result.MAX] = p[axisC.Column];
                }
			}
            axisX.SetMinMax(mm[X][Result.MIN], mm[X][Result.MAX]);
            axisY.SetMinMax(mm[Y][Result.MIN], mm[Y][Result.MAX]);
            if (null != axisZ) {
                axisZ.SetMinMax(mm[Z][Result.MIN], mm[Z][Result.MAX]);
            }
            if (null != axisC) {
                axisC.SetMinMax(mm[C][Result.MIN], mm[C][Result.MAX]);
            }

            if (!xAscending) axisX.SwitchOrientation();
            if (!yAscending) axisY.SwitchOrientation();
            if (axisZ != null && !zAscending) axisZ.SwitchOrientation();
            if (axisC != null && !cAscending) axisC.SwitchOrientation();

            control.ColumnPropertiesChanged();
        }

        public void ResetCP() { //TODO: Auslagern nach Visualization?
            bool xAscending = axisX.IsAscendingOrder();
            bool yAscending = axisY.IsAscendingOrder();
            bool zAscending = true;
            bool cAscending = true;
            if (null != axisZ) zAscending = axisZ.IsAscendingOrder();
            if (null != axisC) cAscending = axisC.IsAscendingOrder();

            VisualizationWindow.Space.Reset();

            if (!xAscending) axisX.SwitchOrientation();
            if (!yAscending) axisY.SwitchOrientation();
            if (axisZ != null && !zAscending) axisZ.SwitchOrientation();
            if (axisC != null && !cAscending) axisC.SwitchOrientation();

            control.ColumnPropertiesChanged();
        }

        #region EventHandler
        public override void PointSetModified(object sender, EventArgs e) {
            base.PointSetModified(sender, e);
            control.PointSetChanged();
        }

        public override void  SelectionModified(object sender, EventArgs e) {
            control.CurrentSelectionChanged();
        }

        #endregion

    }
    public enum ScatterLines { None, xyAxes, xzAxes, yzAxes }

}
