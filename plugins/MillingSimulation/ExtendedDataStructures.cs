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
using Pavel;
using Pavel.Framework;

namespace Pavel.Plugins {

    /// <summary>
    /// A Point for the MillingSimulation UseCase that stores its height and an assciated
    /// profile PointSet
    /// </summary>
    [Serializable]
    public class HeightPoint : Point {
        internal int height;
        internal PointSet profile;

        public HeightPoint(ColumnSet columnSet, int height, params double[] values)
            : base(columnSet, values) {
            this.height = height;
        }
    }

    /// <summary>
    /// Extends the Pavel ParserResult by the ability to store several more PointSets
    /// in addition to the masterPointSet
    /// </summary>
    public class ExtendedParserResult : Pavel.Framework.ParserResult {
        internal PointSet[] additionalPointSets;
        public ExtendedParserResult(PointSet masterPointSet, List<Space> spaces, params PointSet[] additionalPointSets)
            : base(masterPointSet, spaces) {
            this.additionalPointSets = additionalPointSets;
        }
    }
}
