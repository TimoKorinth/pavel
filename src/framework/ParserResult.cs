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

namespace Pavel.Framework {
    [Serializable()]
    /// <summary>
    /// A container for the results of parsing datafiles
    /// </summary>
    [CoverageExclude]
    public class ParserResult {

        #region Fields

        private PointSet masterPointSet;
        private List<Space> spaces;

        #endregion

        #region Properties

        /// <value> Gets the masterPointSet </value>
        public PointSet MasterPointSet { get { return masterPointSet; } }

        /// <value> Gets the list of Spaces </value>
        public List<Space> Spaces { get { return spaces; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new ParserResult, with the spaces contained in <paramref name="spaces"/>
        /// </summary>
        /// <param name="masterPointSet">The unique masterPointSet</param>
        /// <param name="spaces">List containing the needed spaces</param>
        public ParserResult(PointSet masterPointSet, List<Space> spaces) {
            this.masterPointSet = masterPointSet;
            this.spaces = spaces;
        }

        #endregion
    }
}
