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
using System.IO;

namespace Pavel.Framework {
    /// <summary>
    /// The interface that all parsers must implement.
    /// </summary>
    public interface IParser {

        #region Properties

        /// <value>Gets the label of the plugin</value>
        String Label { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the plugin.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Clean up.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Parses the files determined by the StreamReaders contained in <paramref name="reader"/>
        /// and returns the ParserResult.
        /// </summary>
        /// <param name="reader">Array of StreamReaders</param>
        /// <returns>The ParserResult</returns>
        ParserResult Parse(params StreamReader[] reader);

        #endregion
    }
}
