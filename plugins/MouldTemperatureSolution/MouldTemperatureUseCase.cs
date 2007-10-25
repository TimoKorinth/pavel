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
using Pavel.GUI;
using Pavel.GUI.Visualizations;
using Pavel.GUI.SolutionVisualizations;

namespace Pavel.Plugins {
    /// <summary>
    /// The Mould Temperature IUseCase.
    /// Special properties are a String containing the path of the STL file for visualization and
    /// a list of Column indices used for visualizing the drillings.
    /// </summary>
    [Serializable]
    public class MouldTemperatureUseCase : IUseCase {

        #region Fields

        private String stlFile = "";
        private List<int> drillingColumns;

        private ColumnSet solutionColumnSet;

        [NonSerialized]
        private ProjectStarterPageHandler projectStarterPages = new ProjectStarterPageHandler();
        
        #endregion

        #region Properties

        #region IUseCase Members

        /// <value> Gets a label "Mould Temperature Control Drilling". </value>
        public string Label { get { return "Mould Temperature Control Drilling"; } }

        /// <value> Gets the Solution instance of this useCase. </value>
        public Solution SolutionInstance { get { return new MouldTemperatureSolution(); } }

        /// <value> Gets the ProjectStarterPageHandler of this useCase. </value>
        public ProjectStarterPageHandler ProjectStarterPages { get { return projectStarterPages; } }

        #endregion

        /// <value>Gets the path of the STL file or sets it</value>
        public String STLFile {
            get { return stlFile; }
            set { stlFile = value; }
        }

        /// <value> Gets the list of column indices for the drilling visualization or sets it </value>
        public List<int> DrillingColumns {
            get { return drillingColumns; }
            set { drillingColumns = value; }
        }

        /// <value> Gets the space a solution must at least have to be visualized or sets it</value>
        public ColumnSet SolutionColumnSet {
            get { return solutionColumnSet; }
            set { solutionColumnSet = value; }
        }

        #endregion

        # region Constructors

        /// <summary>
        /// Adds the ProjectStarterPages of the MouldTemperatureUseCase to projectStarterPages.
        /// </summary>
        public MouldTemperatureUseCase() {
            projectStarterPages.Add(new ProjectStarterPages.FileOpener());
            projectStarterPages.Add(new ProjectStarterPages.ColumnSelector());
        }

        #endregion
    }
}
