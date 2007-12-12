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
    /// <summary>
    /// The Milling Optimization IUseCase.
    /// Special properties are a String containing the path of the STL file for the visualization and
    /// nothing else yet...
    /// </summary>
    [Serializable]
    public class MillingOptimizationUseCase : IUseCase {

        #region Fields

        private String stlFile = "";
        private String posFile = "";
        private String toolFile = "";
        private List<int> millingColumns;

        private ColumnSet solutionColumnSet;

        [NonSerialized]
        private ProjectStarterPageHandler projectStarterPages = new ProjectStarterPageHandler();

        #endregion

        # region Properties

        #region IUseCase Members

        /// <value> Gets a label "Milling Optimization". </value>
        public string Label { get { return "Milling Optimization"; } }

        /// <value> Gets the Solution instance of this useCase. </value>
        public Solution SolutionInstance { get { return new MillingOptimizationSolution(); } }

        /// <value> Gets the ProjectStarterPageHandler of this useCase. </value>
        public ProjectStarterPageHandler ProjectStarterPages { get { return projectStarterPages; } }

        #endregion

        /// <value>Gets the path of the STL file or sets it</value>
        public String STLFile {
            get { return stlFile; }
            set { stlFile = value; }
        }

        /// <summary>
        /// Gets the list of Column indices for the milling visualization or sets it.
        /// </summary>
        public List<int> MillingColumns {
            get { return millingColumns; }
            set { millingColumns = value; }
        }

        /// <value>Gets the path of the positon file or sets it</value>
        public String PosFile {
            get { return posFile; }
            set { posFile = value; }
        }

        /// <value>Gets the path of the tool file or sets it</value>
        public String ToolFile {
            get { return toolFile; }
            set { toolFile = value; }
        }

        /// <value> Gets the space a solution must at least have to be visualized or sets it</value>
        public ColumnSet SolutionColumnSet {
            get { return solutionColumnSet; }
            set { solutionColumnSet = value; }
        }

        #endregion

        # region Constructors

        /// <summary>
        /// Adds the ProjectStarterPages of the MillingOptimizationUseCase to projectStarterPages.
        /// </summary>
        public MillingOptimizationUseCase() {
            projectStarterPages.Add(new ProjectStarterPages.FileOpener());
            projectStarterPages.Add(new ProjectStarterPages.ColumnSelector());
        }

        #endregion
    }
}
