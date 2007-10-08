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
    /// A parser to parse mould temperature files.
    /// Expects two files (-obj and -dec) for the objective and the decision space.
    /// A third reader for the sim values can be added.
    /// </summary>
    public class MillingOptimizationParser : IParser {

        #region Fields

        private char[] separators = { ' ', '	' };
        private string completeRow;
        private string[] row;
        private NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
        private int rowCount;
        private List<PointList> pointLists;

        #endregion

        #region Properties

        /// <value> Gets a label "Milling Optimization Parser".</value>
        public String Label { get { return "Milling Optimization Parser"; } }

        #endregion

        #region Methods

        #region Helpers

        /// <summary>
        /// Reads the header (labels) of a given input file (via stream).
        /// </summary>
        /// <param name="cReader">StreamReader for the input file</param>
        /// <returns>List of the Columns of the ColumnSet</returns>
        private List<Column> ReadHeader(StreamReader cReader) {
            // For creating the labels
            StringBuilder sb = new StringBuilder();
            List<Column> columns = new List<Column>();

            while (cReader.Peek() != -1) {
                completeRow = cReader.ReadLine();
                rowCount++;
                if (completeRow.Trim().Length == 0) {
                    if (columns.Count == 0) {
                        columns.Add(new Column("No label"));
                    }
                    return columns;
                }

                if (completeRow.StartsWith("###")) {
                    row = completeRow.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
                    if (row[1].ToLower().CompareTo("column") == 0) {
                        // Remove <### Column xy> at the beginning
                        for (int i = 3; i < row.Length; i++) {
                            sb.Append(row[i] + " ");
                        }
                        columns.Add(new Column(sb.ToString().TrimEnd(' ')));
                        sb.Remove(0, sb.Length);
                    }
                }
            }
            if (columns.Count == 0) {
                columns.Add(new Column("No name - check input file"));
            }
            return columns;
        }

        /// <summary>
        /// Reads the ColumnSet of a given input file (via stream).
        /// </summary>
        /// <param name="cReader">StreamReader for the input file</param>
        /// <returns> ColumnSet of the file given by <paramref name="reader"/></returns>
        private ColumnSet ReadSpace(StreamReader cReader) {
            // Making sure everything is reinitialized
            row = null;
            completeRow = null;
            rowCount = 0;

            // Create ColumnSet
            List<Column> columns = ReadHeader(cReader);
            ColumnSet space = new ColumnSet(columns);

            // Create PointList
            PointList pointList = new PointList(space);
            pointList.Label = "Point List";

            // Read data
            while (cReader.Peek() != -1) {
                try {
                    completeRow = cReader.ReadLine().Trim();
                    if (completeRow.Trim().Length != 0 & !completeRow.StartsWith("#")) {
                        List<double> currentValues = NextPoint(cReader);
                        if (space.Dimension != currentValues.Count) {
                            space = ExtendSpace(space, currentValues.Count);
                            pointList = new PointList(space);
                            pointList.Label = "Point List";
                        }
                        pointList.Add(new Point(space, currentValues.ToArray()));
                    }

                }
                catch (ApplicationException e) {
                    throw new InvalidDataException("Individual does not match axes", e);
                }
            }
            pointLists.Add(pointList);
            cReader.Close();
            return space;
        }

        /// <summary>
        /// Reads the simulation values in the given input file (via stream) and stores them in the Point.Tag.
        /// </summary>
        /// <param name="cReader">StreamReader for the input file</param>
        /// <param name="ps">PointSet to store the values in</param>
        private void ReadSimValues(StreamReader cReader, PointSet ps) {
            // Making sure everything is reinited
            row = null;
            completeRow = null;
            int i = -1;

            // Read data
            while (cReader.Peek() != -1) {
                try {
                    completeRow = cReader.ReadLine().Trim();
                    if (completeRow.Trim().Length != 0 & !completeRow.StartsWith("#")) {
                        List<double> currentValues = NextPoint(cReader);
                        Point curPoint = ps[++i];
                        curPoint.Tag.Data = currentValues.ToArray();
                    }
                }
                catch (ApplicationException e) {
                    throw new InvalidDataException("Sim values do not match point set", e);
                }
            }
            cReader.Close();
        }

        /// <summary>
        /// Reads the data for one individual in a given input file (via stream).
        /// </summary>
        /// <param name="reader">StreamReader for the input file</param>
        /// <returns>Values of the individual</returns>
        private List<double> NextPoint(StreamReader reader) {
            List<double> currentValues = new List<double>();

            while (completeRow.Trim().Length != 0) {
                if (!completeRow.StartsWith("#")) {
                    row = completeRow.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < row.Length; i++) {
                        currentValues.Add(Double.Parse(row[i], NumberStyles.Float, numberFormatInfo));
                    }
                }
                if (reader.Peek() == -1) { break; }
                completeRow = reader.ReadLine();
            }
            return currentValues;
        }

        /// <summary>
        /// Extends the given ColumnSet if there are more values than header columns.
        /// </summary>
        /// <param name="space">The ColumnSet to be extended</param>
        /// <param name="length">The length to which the ColumnSet is going to be extended</param>
        /// <returns>The extended ColumnSet</returns>
        private ColumnSet ExtendSpace(ColumnSet space, int length) {
            List<Column> columns = new List<Column>();
            foreach (Column c in space.Columns) {
                columns.Add(c);
            }

            int j = 0;

            while (columns.Count < length) {
                columns.Add(new Column(columns[j].Label));
                j++;
            }

            return new ColumnSet(columns);
        }

        #endregion

        #region IParser Members

        /// <summary>
        /// Creates a ParserResult object from the given input files.
        /// </summary>
        /// <param name="reader">At least two StreamReaders, first objectiveSpace-File, second decisionSpace-File, last simValues-File</param>
        /// <returns>New ParserResult</returns>
        /// <exception cref="InvalidDataException">The parameter file has an invalid format</exception>
        public ParserResult Parse(params StreamReader[] reader) {
            numberFormatInfo.NumberDecimalSeparator = ".";
            pointLists = new List<PointList>();

            // creating objective space
            ColumnSet objSpace = ReadSpace(reader[0]);

            // creating decision space
            ColumnSet decSpace = ReadSpace(reader[1]);

            // uniting Spaces
            ColumnSet masterSpace = ColumnSet.Union(objSpace, decSpace);
            
            // uniting PointLists
            PointList masterPointList;
            PointList[] pointListArray = pointLists.ToArray();
            masterPointList = new PointList(masterSpace);

            for (int i = 0; i < pointListArray[0].Count; i++) {
                List<double> values = new List<double>();
                for (int j = 0; j < pointListArray.Length; j++) {
                    values.AddRange(pointListArray[j][i].Values);
                }
                masterPointList.Add(new Point(masterSpace, values.ToArray()));
            }
            masterPointList.Label = "Master Points";

            PointSet masterPointSet = new PointSet("MasterPointSet", masterSpace, true);
            masterPointSet.Add(masterPointList);

            if (null != PavelMain.LogBook) {
                PavelMain.LogBook.Message("Decision ColumnSet Column Count: " + decSpace.Columns.Length +
                    "\nObjectiveColumnSete Column Count: " + objSpace.Columns.Length +
                    "\nMaster Point Set Count: " + masterPointSet.Length, false);
            }

            //Create default ColumProperties
            ProjectController.CreateMinMaxColumnProperties(masterPointSet);

            List<Space> spaces = new List<Space>();
            spaces.Add(new Space(masterSpace, "Master Space"));
            spaces.Add(new Space(decSpace, "Decision Space"));
            spaces.Add(new Space(objSpace, "Objective Space"));
            return new ParserResult(masterPointSet, spaces);
        }

        public void Initialize() { }

        public void Dispose() { }

        #endregion

        #endregion
    }
}
