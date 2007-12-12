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
using Pavel.GUI.Visualizations;
using Pavel.GUI.SolutionVisualizations;

namespace Pavel.Plugins {
    [Serializable()]
    /// <summary>
    /// MillingOptimizationSolution creates a new control for showing the single
    /// solution for Milling Optimization.
    /// At first it parses the STL-File.
    /// </summary>
    public class MillingOptimizationSolution : Solution {

        #region Fields

        private MillingOptimizationSolutionControl solutionControl;
        private TableLayoutPanel mainContainer;

        //properties of the propertyWindow
        private bool showLegend = true;
        private bool smoothRendering = false;
        private bool light = true;

        private float[] originalStlNormalArray;

        #endregion

        #region Properties

        /// <value>
        /// Gets showLegend or sets it.
        /// Controls the showing of the legend at the left side.
        /// </value>
        [ShowInProperties]
        [Category("Misc")]
        [DisplayName("Show legend")]
        [Description("Show a legend displaying numerical values of the selected solution.")]
        public bool ShowLegend {
            get {
                return showLegend;
            }
            set {
                showLegend = value;
                InitializeMainContainer();
            }
        }

        /// <value>
        /// Gets smoothRendering or sets it.
        /// Switches between rendering with smooth colors / normal vectors and rough approximations.
        /// </value>
        [ShowInProperties]
        [Category("Misc")]
        [DisplayName("Smooth workpiece")]
        [Description("Smooths the workpiece surfaces.")]
        public bool SmoothRendering {
            get { return smoothRendering; }
            set {
                smoothRendering = value;
                solutionControl.RefreshArrays();
                solutionControl.Invalidate();
            }
        }

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

        [ShowInProperties]
        [Category("3D")]
        public bool Stereo {
            get { return solutionControl.StereoMode; }
            set {
                solutionControl.StereoMode = value;
                solutionControl.Invalidate();
            }
        }

        [ShowInProperties]
        [Category("3D")]
        public float EyeDistance {
            get { return solutionControl.EyeDistance; }
            set {
                solutionControl.EyeDistance = value;
                solutionControl.Invalidate();
            }
        }

        /// <value>Gets the array of normal vectors as defined in the STL file</value>
        public float[] OrginalStlNormalArray { get { return originalStlNormalArray; } }

        /// <value> Gets a label "Milling Optimization Solution".</value>
        public override String Label { get { return "Milling Optimization Solution"; } }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new control with the data from the given Pavel.Framework.Points <paramref name="p"/>.
        /// </summary>
        /// <param name="p">The selected Pavel.Framework.Points</param>
        public override void Initialize(Point[] p) {
            base.Initialize(p);
            this.Text = "Solution Window";
            this.Size = new System.Drawing.Size(600, 600);
            this.points = p;
            currentPointIndex = 0;
            IUseCase useCase = ProjectController.Project.UseCase;
            STLParser stlParser = new STLParser();
            PositionParser posParser = new PositionParser();
            ToolParser tooParser = new ToolParser();

            StreamReader stlFile = File.OpenText((useCase as MillingOptimizationUseCase).STLFile);
            try {
                stlParser.Parse(stlFile);
            }
            finally {
                stlFile.Close();
            }

            StreamReader posFile = File.OpenText((useCase as MillingOptimizationUseCase).PosFile);
            try {
                posParser.ParsePos(posFile);
            }
            finally {
                posFile.Close();
            }

            StreamReader tooFile = File.OpenText((useCase as MillingOptimizationUseCase).ToolFile);
            try {
                tooParser.Parse(tooFile);
            }
            finally {
                tooFile.Close();
            }
            originalStlNormalArray = stlParser.NormalArray;

            //Initialize Windows Forms Elements
            InitializeDataGrid();
            FillDataGrid();
            glyphControl = new GlyphControl(points);
            this.mainContainer = new TableLayoutPanel();
            this.legendContainer = new TableLayoutPanel();
            InitializeLegendContainer();
            compManyControl.ComparisonPointsChanged += this.OnComparisonPointChanged;
            compRefControl.ComparisonPointsChanged += this.OnComparisonPointChanged;

            solutionControl = new MillingOptimizationSolutionControl(this, stlParser.VertexArray, originalStlNormalArray, p, posParser.VertexArray, tooParser.ToolInformation);
            solutionControl.KeyUp += OnKeyUpEvent;

            InitializeGlyphSpace((useCase as MillingOptimizationUseCase).MillingColumns);

            InitializeMainContainer();
            this.Controls.Add(mainContainer);
        }

        /// <summary>
        /// Switches the display to the next or the previous Pavel.Framework.Point
        /// depending on the pressed key.
        /// </summary>
        /// <param name="forwardDirection"> True for changing to the next Point, false for changing to the previous Point </param>
        public override int ChangePoint(Boolean forwardDirection) {
            List<int> ps = new List<int>();
            switch (selectedMode) {
                case Mode.Zapping: {
                        if (forwardDirection) { index++; } else { index--; }
                        index = (index + points.Length) % points.Length;
                        currentPointIndex = index;
                        ps.Add(currentPointIndex);
                        break;
                    }
                case Mode.CompareToMany: {
                        currentPointIndex = compManyControl.HighlightedPoint;
                        ps = compManyControl.SelectedPoints;
                        break;
                    }
                case Mode.CompareToRef: {
                        referencePointIndex = compRefControl.ReferencePoint;
                        currentPointIndex = compRefControl.ComparisonPoint;
                        ps.Add(compRefControl.ReferencePoint);
                        break;
                    }
            }

            solutionControl.ChangePoint(ps);
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
                        compManyControl.HighlightedPoint = currentPointIndex;
                        referencePointIndex = -1;
                        legendContainer.Controls.Add(compManyControl, 0, 2);
                        if (dataGridView.ColumnCount == 3) {
                            dataGridView.Columns.RemoveAt(2);
                        }
                        break;
                    }
                case Mode.CompareToRef: {
                        compRefControl.ReferencePoint = currentPointIndex;
                        referencePointIndex = currentPointIndex;
                        legendContainer.Controls.Add(compRefControl, 0, 2);
                        dataGridView.ColumnCount = 3;
                        dataGridView.Columns[2].Name = "Reference Value";
                        break;
                    }
                default: {
                        currentPointIndex = compManyControl.HighlightedPoint;
                        referencePointIndex = -1;
                        legendContainer.Controls.Add(zapControl, 0, 2);
                        if (dataGridView.ColumnCount == 3) {
                            dataGridView.Columns.RemoveAt(2);
                        }
                        break;
                    }
            }

            solutionControl.Invalidate();
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
            InitializeGlyphSpace((ProjectController.Project.UseCase as MillingOptimizationUseCase).MillingColumns);
            PavelMain.MainWindow.PropertyControl.VisualizationWindow = this;
        }

        /// <summary>
        /// When the ComparisonPointChangedEvent is fired
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">ComparisonEventArgs</param>
        private void OnComparisonPointChanged(object sender, ComparisonEventArgs e) {
            ChangePoint(true);
        }

        #endregion
    }
}
