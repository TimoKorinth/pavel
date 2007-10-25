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
using System.Globalization;
using Pavel.Framework;

namespace Pavel.Plugins {
    /// <summary>
    /// A parser to parse csv-files.
    /// </summary>
    public class CSVParser {

        #region Fields

        private NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
        private char[] splitter;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a CSVParser with "." as decimal seperator and ";" or "\t" as splitters
        /// </summary>
        public CSVParser() {
            numberFormatInfo.NumberDecimalSeparator = "." ;
            this.splitter = new char[] {';', '\t'};
        }

        /// <summary>
        /// Creates a CSVParser with given decimal seperator and splitters
        /// </summary>
        /// <param name="numberDecimalSeperator">A decimal seperator like "." or ";"</param>
        /// <param name="splitter">An array of characters used to split a line into fields</param>
        public CSVParser(string numberDecimalSeperator, params char[] splitter) {
            numberFormatInfo.NumberDecimalSeparator = numberDecimalSeperator;
            this.splitter = splitter;
        }

        #endregion
        #region Properties

        /// <value> Gets a label "CSV Parser".</value>
        public String Label { get { return "CSV Parser"; } }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a ParserResult object from the given input files.
        /// </summary>
        /// <param name="reader">One Stream</param>
        /// <returns>New ParserResult</returns>
        /// <exception cref="ApplicationException">Wrong number of parameters, must be one</exception>
        /// <exception cref="InvalidDataException">The parameter file has an invalid format</exception>
        public ParserResult Parse(StreamReader reader) {
            int rownum = 0;
            String completeRow = "";

            completeRow = reader.ReadLine().Trim();
            ++rownum;
            while (RowEmpty(completeRow)) {
                completeRow = reader.ReadLine();
                ++rownum;
            }
            
            String[] splittedRow = completeRow.Split(splitter);

            // Throw an exception, if no labels can be found
            // Assuming, there must be two rows, not only one
            if (splittedRow.Length <= 1) {
                throw new InvalidDataException("No labels read while parsing, wrong Dataformat?");
            }

            //Parsing header for Spaces
            List<Column> masterSpaceColumnList = new List<Column>();
            for ( int i = 0; i < splittedRow.Length; i++ ) {
                masterSpaceColumnList.Add(new Column(splittedRow[i]));
            }
            ColumnSet masterSpaceColumnSet = new ColumnSet(masterSpaceColumnList);

            //Create MasterPointList and MasterPointSet
            PointList masterPointList = new PointList(masterSpaceColumnSet);
            PointSet masterPointSet = new PointSet("MasterPointSet", masterSpaceColumnSet, true);
            masterPointSet.Add(masterPointList);

            //Parse Points
            double[] pointValues = new double[masterSpaceColumnSet.Columns.Length];

            while (!reader.EndOfStream) {
                completeRow = reader.ReadLine().Trim();
                ++rownum;
                if (!RowEmpty(completeRow)) {
                    splittedRow = completeRow.Split(splitter);
                    pointValues = new double[splittedRow.Length];
                    if (pointValues.Length != masterSpaceColumnList.Count) {
                        throw new InvalidDataException(masterSpaceColumnList.Count.ToString() + " Columns in Header, " +
                            pointValues.Length + " Columns in line " + rownum);
                    }
                    for (int i = 0; i < splittedRow.Length; i++) {
                        pointValues[i] = Double.Parse(splittedRow[i], NumberStyles.Float, numberFormatInfo);
                    }
                    masterPointList.Add(new Point(masterSpaceColumnSet, pointValues));
                }
            }
          
            //Create default ColumProperties
            ProjectController.CreateMinMaxColumnProperties(masterPointSet);

            List<Space> spaces = new List<Space>();
            spaces.Add(new Space(masterSpaceColumnSet, "Master Space"));
            return new ParserResult(masterPointSet, spaces);
        }

        private static bool RowEmpty(String row) {
            return (row.Length == 0) || (row.StartsWith("#"));
        }

        #endregion
    }
}
