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

namespace Pavel.Framework {
    [Serializable()]
    /// <summary>
    /// This class should be used for holding the data tag. It extends a tag with a index.
    /// </summary>
    public class DataTag {
        private static int autoIndexer = 1;
        private int index;
        private object data;

        public int Index {
            get { return index; }
            set { index = value; }
        }

        public object Data {
            get { return data; }
            set { data = value; }
        }

        public DataTag() {
            index = autoIndexer++;
            data = null;
        }

        public DataTag(int index, object data) {
            this.index = index;
            this.data = data;
        }

        public override string ToString() {
            return "Index: " + index;
        }
    }
}
