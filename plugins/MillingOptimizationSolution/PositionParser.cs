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
using System.IO;
using System.Globalization;
using Pavel.Framework;

namespace Pavel.Plugins {
    /// <summary>
    /// A parser to parse standard pos-files.
    /// </summary>
    public class PositionParser {

        #region Fields

        private float[] vertexArray;

        #endregion

        #region Properties

        /// <value> Gets the array of positions or sets it.</value>
        public float[] VertexArray {
            get { return vertexArray; }
            set { vertexArray = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses the position file <paramref name="file"/> for the positions.
        /// </summary>
        /// <param name="file">Path of the position file</param>
        public void ParsePos(StreamReader file) {
            String input;
            List<float> vertexList = new List<float>();
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
            while ((input = file.ReadLine()) != null) {
                input = input.Trim();
                String[] v = input.Split(' ');
                if ((v[0] != "")&&(!v[0].StartsWith("#"))) {
                    vertexList.Add(float.Parse(v[0], NumberStyles.Float, numberFormatInfo));
                    vertexList.Add(float.Parse(v[1], NumberStyles.Float, numberFormatInfo));
                    vertexList.Add(float.Parse(v[2], NumberStyles.Float, numberFormatInfo));
                }
            }
            this.vertexArray = vertexList.ToArray();
            file.Close();
        }

        #endregion
    }
}
