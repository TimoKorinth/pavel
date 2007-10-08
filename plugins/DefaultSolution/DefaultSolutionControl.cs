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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Pavel.Framework;

namespace Pavel.Plugins {
    /// <summary>
    /// SingleSolutionControl: This shows a DataGridView of a single solution
    /// of a default useCase Project.
    /// </summary>
    public partial class DefaultSolutionControl : UserControl {

        #region Fields

        private DefaultSolution solution;
        private Pavel.Framework.Point[] points;
        private List<int> selectedPoints = new List<int>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the DefaultSolutionControl with the given <paramref name="points"/>.
        /// </summary>
        /// <param name="solution"> The matching solution</param>
        /// <param name="points">The Pavel.Framework.Points to be displayed</param>
        public DefaultSolutionControl(DefaultSolution solution, Pavel.Framework.Point[] points) {
            InitializeComponent();
            this.solution = solution;
            this.points = points;
            solution.GlyphControl.UpdateGlyphs(GetGlyphPoints());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Replaces the current Pavel.Framework.Point with the list of <paramref name="selected"/>.
        /// </summary>
        /// <param name="selected">List of new Pavel.Framework.Points</param>
        public void ChangePoint(List<int> selected) {
            selectedPoints.Clear();
            selectedPoints.AddRange(selected);
            solution.GlyphControl.UpdateGlyphs(GetGlyphPoints());
            this.Invalidate();
        }

        public List<Pavel.Framework.Point> GetGlyphPoints() {
            List<Pavel.Framework.Point> glyphPoints = new List<Pavel.Framework.Point>();

            switch (solution.SelectedMode) {
                case DefaultSolution.Mode.Zapping: {
                        glyphPoints.Add(solution.CurrentPoint);
                        break;
                    }
                case DefaultSolution.Mode.CompareToRef: {
                        glyphPoints.Add(solution.CurrentPoint);
                        if (solution.ReferencePoint != null) {
                            glyphPoints.Add(solution.ReferencePoint);
                        }
                        break;
                    }
                case DefaultSolution.Mode.CompareToMany: {
                        for (int i = 0; i < selectedPoints.Count; i++) {
                            glyphPoints.Add(points[selectedPoints[i]]);
                        }
                        break;
                    }

            }

            return glyphPoints;
        }

        #endregion

    }
}
