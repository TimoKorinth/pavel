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
using Pavel.GUI;
using Pavel.GUI.Visualizations;
using Pavel.GUI.SolutionVisualizations;

namespace Pavel.Plugins {
    [Serializable]
    public class MillingSimulationUseCase : IUseCase {

        /// <summary>
        /// During the execution of the ProjectStarterPages this is used to pass
        /// the histogram PointSets from the HistogramOpener to the ProfileOpener
        /// </summary>
        [NonSerialized]
        internal static PointSet[] openingHistogramPointSets;

        /// <summary>
        /// During the execution of the ProjectStarterPages this is used to pass
        /// the simulationPointSet from the HistogramOpener to the simProfileOpener
        /// </summary>
        [NonSerialized]
        internal static PointSet simulationPointSet;

        [NonSerialized]
        private ProjectStarterPageHandler projectStarterPages = new ProjectStarterPageHandler();

        /// <value> Gets a label "Milling Optimization". </value>
        public string Label { get { return "Milling Simulation"; } }

        /// <value> Gets a new Solution instance of this useCase. </value>
        public Solution SolutionInstance { get { return new MillingSimulationSolution(); } }

        /// <value> Gets the ProjectStarterPageHandler of this useCase. </value>
        public ProjectStarterPageHandler ProjectStarterPages { get { return projectStarterPages; } }

        public ColumnSet SolutionColumnSet {
            get { return null; }
            set { }
        }
        
        public MillingSimulationUseCase() {
            projectStarterPages.Add(new ProjectStarterPages.HistogramOpener());
            projectStarterPages.Add(new ProjectStarterPages.ProfileOpener());
            projectStarterPages.Add(new ProjectStarterPages.SimProfileOpener());
        }
    }
}