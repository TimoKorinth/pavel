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
using Pavel.Framework;
using System.IO;
using System.Windows.Forms;
using Pavel.GUI;
using System.ComponentModel;
using System.Drawing;
using Pavel.GUI.Visualizations;
using Pavel.GUI.SolutionVisualizations;

namespace Pavel.Plugins {
    /// <summary>
    /// MouldTemperationSolution creates a new control for showing the single
    /// solution for Mould Temperature Drilling.
    /// At first it parses the STL-File.
    /// </summary>
    [Serializable()]
    public class MouldTemperatureSolution : Solution {

        #region Fields

        private MouldTemperatureSolutionControl solutionControl;
        private TableLayoutPanel mainContainer;
        private float[] orginalStlNormalArray;

        //properties of the propertyWindow
        private bool showLegend = true;
        private bool smoothRendering = false;
        private bool absoluteTemperature;
        private bool light = true;
        private double drillRadius = 5.0;
        private float alphaValue = 1.0f;

        private ComparisonMode selectedComparisonMode = ComparisonMode.Add;

        //mode for the calculation of the raw-data: Diff(A-B), AbsDiff(|A-B|), Max(Max(A, B), Add(A+B))
        public enum ComparisonMode { Diff, AbsDiff, Max, Add };

        public event EventHandler<EventArgs> DrillRadiusChanged;

        #endregion

        #region Properties

        /// <value>
        /// Gets selectedComparisonMode or sets it.
        /// </value>
        [ShowInProperties]
        [Category("Modes")]
        [DisplayName("Comparison mode")]
        [Description("You can choose between diff, absDiff, max and add. (A-B, |A-B|, max(A,B), A+B). Choosing absDiff or diff disables AbsoluteTemperature")]
        public ComparisonMode SelectedComparisonMode {
            get { return selectedComparisonMode; }
            set {
                selectedComparisonMode = value;
                if ( selectedComparisonMode == ComparisonMode.AbsDiff || selectedComparisonMode == ComparisonMode.Diff ) {
                    absoluteTemperature = false;
                }
                solutionControl.RefreshArrays();
                solutionControl.Invalidate();
            }
        }

        /// <value>
        /// Gets showLegend or sets it.
        /// Controls the showing of the legend at the left side.
        /// </value>
        [ShowInProperties]
        [Category("Misc")]
        [DisplayName("Show legend")]
        [Description("Shows a legend displaying numerical values of the selected solution.")]
        public bool ShowLegend {
            get { return showLegend; }
            set {
                showLegend = value;
                InitializeMainContainer();
            }
        }

        /// <value>
        /// Enables or disables a smoothed rendering mode by interpolating the normal vectors
        /// </value>
        [ShowInProperties]
        [Category("Misc")]
        [DisplayName("Smooth workpiece")]
        [Description("Smooths the workpiece surface´s.")]
        public bool SmoothRendering {
            get { return smoothRendering; }
            set {
                smoothRendering = value;
                solutionControl.RefreshArrays();
                solutionControl.Invalidate();
            }
        }

        /// <value>
        /// Enables or disables a smoothed rendering mode by interpolating the normal vectors
        /// </value>
        [ShowInProperties]
        [Category("Misc")]
        [DisplayName("Absolute Temperature")]
        [Description("Absolute Temperature scales the temperature of the surface over all selected points, otherwise the temperature is scaled only to one individual.")]
        public bool AbsoluteTemperature {
            get { return absoluteTemperature; }
            set {
                absoluteTemperature = value;
                solutionControl.RefreshArrays();
                solutionControl.Invalidate();
            }
        }

        /// <value> 
        /// Gets the radius of the boreholes or sets it.
        /// </value>
        [ShowInProperties]
        [Category("Misc")]
        [DisplayName("Drill radius")]
        [Description("Defines the radius of the drills. The default is 5.0.")]
        public double DrillRadius {
            get { return drillRadius; }
            set {
                if (value <= 1) { drillRadius = 1; } else if (value >= 50.0) { drillRadius = 50.0; } else { drillRadius = value; }
                if (DrillRadiusChanged != null) {
                    DrillRadiusChanged(this, new EventArgs());
                }
            }
        }

        /// <value>
        /// Gets light or sets it.
        /// </value>
        [ShowInProperties]
        [Category("Misc")]
        [DisplayName("Light")]
        [Description("Switches the light in the scene. Some features of a workpieces may be better visible with or without out light.")]
        public bool Light {
            get { return light; }
            set {
                light = value;
                solutionControl.SetLight();
                solutionControl.Invalidate();
            }
        }

        /// <value>
        /// Gets the stereo or sets it.
        /// </value>
        [ShowInProperties]
        [Category("3D")]
        [DisplayName("Stereo")]
        [Description("Shows the scene in stereo.")]
        public bool Stereo {
            get { return solutionControl.StereoMode; }
            set {
                solutionControl.StereoMode = value;
                solutionControl.Invalidate();
            }
        }

        /// <value>
        /// Gets the eye distance or sets it.
        /// </value>
        [ShowInProperties]
        [Category("3D")]
        [DisplayName("Eye distance")]
        [Description("Defines the distance of the eyes if stereo is turned on.")]
        public float EyeDistance {
            get { return solutionControl.EyeDistance; }
            set {
                solutionControl.EyeDistance = value;
                solutionControl.Invalidate();
            }
        }


        /// <value>Gets the array of normal vectors as defined in the STL file</value>
        public float[] OrginalStlNormalArray {
            get { return orginalStlNormalArray; }
            set { orginalStlNormalArray = value; }
        }

        internal float AlphaValue {
            get { return alphaValue; }
        }

        /// <value> Gets a label "Mould Temperature Solution". </value>
        public override String Label { get { return "Mould Temperature Solution"; } }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new control with the data from the given Pavel.Framework.Points <paramref name="p"/>.
        /// </summary>
        /// <param name="p">The selected Pavel.Framework.Points</param>
        public override void Initialize(Pavel.Framework.Point[] p) {
            base.Initialize(p);
            IUseCase useCase = ProjectController.Project.UseCase;
            STLParser stlParser = new STLParser();
            this.points = p;
            this.Text = "Solution Window";
            this.Size = new System.Drawing.Size(600, 600);
            currentPointIndex = 0;

            StreamReader stlFile = File.OpenText((useCase as MouldTemperatureUseCase).STLFile);
            try {
                stlParser.Parse(stlFile);
            } finally {
                stlFile.Close();
            }

            //Initialize Windows Forms Elements
            InitializeDataGrid();
            FillDataGrid();
            glyphControl = new GlyphControl(points);
            this.legendContainer = new TableLayoutPanel();
            InitializeLegendContainer();
            compManyControl.ComparisonPointsChanged += this.OnComparisonPointChanged;
            compRefControl.ComparisonPointsChanged += this.OnComparisonPointChanged;
            
            orginalStlNormalArray = stlParser.NormalArray;
            solutionControl = new MouldTemperatureSolutionControl(this, stlParser.VertexArray, orginalStlNormalArray);
            solutionControl.CalculateAbsoluteExtrema(points);
            solutionControl.KeyUp += OnKeyUpEvent;

            AbsoluteTemperature = false;

            InitializeGlyphSpace((useCase as MouldTemperatureUseCase).DrillingColumns);

            this.mainContainer = new TableLayoutPanel();
            InitializeMainContainer();
            this.Controls.Add(mainContainer);
        }

        /// <summary>
        /// Switches the display to the next or the last Pavel.Framework.Point
        /// depending on the pressed key.
        /// </summary>
        /// <param name="forwardDirection"> True for changing to the next Point, false for changing to the previous Point </param>
        public override int ChangePoint(Boolean forwardDirection) {
            if (forwardDirection) { index++; } else { index--; }
            index = (index + points.Length) % points.Length;
            currentPointIndex = index;
            this.solutionControl.ChangePoint();
            FillDataGrid();
            return index;
        }

        /// <summary>
        /// Fills the legend container and reset the current or reference point, when the mode is changed.
        /// </summary>
        protected override void ChangeMode() {
            legendContainer.Controls.RemoveAt(2);
            switch (selectedMode) {
                case Mode.CompareToMany: {
                        MessageBox.Show("CompareToMany is not available in this UseCase.\nCompareToRef will be selected instead.");
                        selectedMode = Mode.CompareToRef;
                        goto case Mode.CompareToRef;
                    }
                case Mode.CompareToRef: {
                        legendContainer.Controls.Add(compRefControl, 0, 2);
                        dataGridView.ColumnCount = 3;
                        dataGridView.Columns[2].Name = "Reference Value";
                        alphaValue = 0.5f;
                        break;
                    }
                default: {
                        legendContainer.Controls.Add(zapControl, 0, 2);
                        if (dataGridView.ColumnCount == 3) {
                            dataGridView.Columns.RemoveAt(2);
                        }
                        alphaValue = 1.0f;
                        break;
                    }
            }

            GlyphControl.UpdateGlyphs(solutionControl.GetGlyphPoints());
        }


        /// <summary>
        /// Initialize the mainContainer.
        /// </summary>
        private void InitializeMainContainer() {
            mainContainer.Controls.Clear();
            if (showLegend) {
                mainContainer.Dock = DockStyle.Fill;
                mainContainer.AutoSize = true;
                mainContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                mainContainer.ColumnCount = 2;
                mainContainer.RowCount = 1;
                mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                mainContainer.Controls.Add(legendContainer);
                mainContainer.Controls.Add(solutionControl);
            } else {
                mainContainer.ColumnCount = 1;
                mainContainer.RowCount = 1;
                mainContainer.Controls.Add(solutionControl);
            }
        }

        #endregion

        #region EventHandling

        protected override void GlyphSpaceChanged(object sender, EventArgs e) {
            InitializeGlyphSpace((ProjectController.Project.UseCase as MouldTemperatureUseCase).DrillingColumns);
            PavelMain.MainWindow.PropertyControl.VisualizationWindow = this;
        }

        /// <summary>
        /// When the ComparisonPointChangedEvent is fired
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">ComparisonEventArgs</param>
        private void OnComparisonPointChanged(object sender, ComparisonEventArgs e) {
            if ((currentPointIndex != e.SelectedPoint) || (referencePointIndex != e.ReferencePoint)) {
                currentPointIndex = e.SelectedPoint;
                referencePointIndex = e.ReferencePoint;
                solutionControl.ChangePoint();
                FillDataGrid();
            }
            if (alphaValue != e.AlphaValue) {
                alphaValue = e.AlphaValue;
                solutionControl.ChangePoint();
                solutionControl.Invalidate();
            }
        }

        #endregion
    }
}
