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
using System.IO;
using System.Windows.Forms;
using Pavel.GUI;
using System.ComponentModel;
using System.Drawing;
using Pavel.GUI.Visualizations;
using Pavel.GUI.SolutionVisualizations;

namespace Pavel.Plugins {
    /// <summary>
    /// The Solution for the default useCase.
    /// </summary>
    [Serializable()]
    public class DefaultSolution : Solution {

        #region Fields

        private DefaultSolutionControl solutionControl;

        #endregion

        #region Properties

        /// <value> Gets a label "Text Display".</value>
        public override String Label { get { return "Text Display"; } }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new control with the data from the given Pavel.Framework.Points <paramref name="p"/>.
        /// </summary>
        /// <param name="p">The selected Pavel.Framework.Points</param>
        public override void Initialize(Pavel.Framework.Point[] p) {
            base.Initialize(p);
            this.Text = "Solution Window";
            this.Size = new System.Drawing.Size(235, 700);
            this.points = p;
            currentPointIndex = 0;

            //Initialize Windows Forms Elements
            InitializeDataGrid();
            FillDataGrid();
            glyphControl = new GlyphControl(points);
            this.legendContainer = new TableLayoutPanel();
            InitializeLegendContainer();
            compManyControl.ComparisonPointsChanged += this.OnComparisonPointChanged;
            compRefControl.ComparisonPointsChanged += this.OnComparisonPointChanged;

            solutionControl = new DefaultSolutionControl(this, p);
            solutionControl.KeyUp += OnKeyUpEvent;

            InitializeGlyphSpace(null);

            this.Controls.Add(legendContainer);
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

            SetWidth();
            GlyphControl.UpdateGlyphs(solutionControl.GetGlyphPoints());
        }

        /// <summary>
        /// Sets the width of the SolutionWindow to the width of the DataGridView.
        /// </summary>
        private void SetWidth() {
            int width = 0;
            for (int i = 0; i < dataGridView.Columns.Count; i++) {
                width += dataGridView.Columns[i].Width + 5;
            }
            this.Width = width + 25;
        }

        #endregion

        #region EventHandling

        protected override void GlyphSpaceChanged(object sender, EventArgs e) {
            InitializeGlyphSpace(null);
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

