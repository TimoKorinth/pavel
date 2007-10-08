// Part of PAVEl: PAVEl (Paretoset Analysis Visualization and Evaluation) is a tool for
// interactively displaying and evaluating large sets of highdimensional data.
// Its main intended use is the analysis of result sets from multi-objective evolutionary algorithms.
//
// Copyright (C) 2007  PG500, ISF, University of Dortmund
//      PG500 are: Christoph Begau, Christoph Heuel, Raffael Joliet, Jan Kolanski,
//                 Mandy Kr�ller, Christian Moritz, Daniel Niggemann, Mathias St�ber,
//                 Timo St�nner, Jan Varwig, Dafan Zhai
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
    /// A parser to parse standard files.
    /// Expects at least one StreamReader.
    /// </summary>
    public class DefaultParser : IParser {

        #region Fields

        private char[] separators = { ' ', '	' };
        private string completeRow;
        private string[] row;
        private NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
        private int rowCount;

        #endregion

        #region Properties

        /// <value> Gets a label "Default Parser".</value>
        public String Label { get { return "Default Parser"; } }

        #endregion

        #region Methods

        #region Helpers

        /// <summary>
        /// Reads the header (labels) of a given input file (via stream).
        /// </summary>
        /// <param name="reader">StreamReader for the input file</param>
        /// <returns>List of the Columns of the ColumnSet</returns>
        private List<Column> ReadHeader(StreamReader reader) {
            // For creating the labels
            StringBuilder sb = new StringBuilder();
            List<Column> columns = new List<Column>();

            while (reader.Peek() != -1) {
                completeRow = reader.ReadLine();
                rowCount++;
                if (completeRow.Trim().Length == 0) {
                    if (columns.Count == 0) {
                        columns.Add(new Column("No name - check input file"));
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
        /// Reads the data for one individual in a given input file (via stream).
        /// </summary>
        /// <param name="reader">StreamReader for the input file</param>
        /// <returns>Values of the next Point</returns>
        private List<double> NextPoint(StreamReader reader) {
            List<double> currentValues = new List<double>();

            while ((completeRow.Trim().Length != 0) && (!completeRow.StartsWith("#"))) {
                row = completeRow.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < row.Length; i++) {
                    currentValues.Add(Double.Parse(row[i], NumberStyles.Float, numberFormatInfo));
                }
                if (reader.Peek() == -1) { break; }
                completeRow = reader.ReadLine();
            }
            return currentValues;
        }

        /// <summary>
        /// Extends the given ColumnSet if there are more values than header columns.
        /// </summary>
        /// <param name="columnSet">The ColumnSet to be extended</param>
        /// <param name="length">The length to which the columnSet is going to be extended</param>
        /// <returns>The extended ColumnSet</returns>
        private ColumnSet ExtendColumnSet(ColumnSet columnSet, int length) {
            List<Column> columns = new List<Column>();
            foreach (Column c in columnSet.Columns) {
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
        /// <param name="reader">An array of StreamReaders</param>
        /// <returns>New ParserResult</returns>
        /// <exception cref="InvalidDataException">The parameter file has an invalid format</exception>
        public ParserResult Parse(params StreamReader[] reader) {
            numberFormatInfo.NumberDecimalSeparator = ".";
            String message = "";
            List<ColumnSet> columnSets = new List<ColumnSet>();
            List<PointList> pointLists = new List<PointList>();

            for (int i = 0; i < reader.Length; i++) {

                // Making sure everything is reinitialized
                row = null;
                completeRow = null;
                rowCount = 0;

                // Create ColumnSet
                StreamReader cReader = reader[i];
                List<Column> columns = ReadHeader(cReader);
                ColumnSet columnSet = new ColumnSet(columns);
                message += "ColumnSet " + i + " Column Count: " + columns.Count + "\n";

                // Create PointList
                PointList pointList = new PointList(columnSet);
                pointList.Label = "Point List " + i;

                // Read data
                while (cReader.Peek() != -1) {
                    try {
                        completeRow = cReader.ReadLine().Trim();
                        if (completeRow.Trim().Length != 0 & !completeRow.StartsWith("#")) {
                            List<double> currentValues = NextPoint(cReader);
                            if (columnSet.Dimension != currentValues.Count) {
                                columnSet = ExtendColumnSet(columnSet, currentValues.Count);
                                pointList = new PointList(columnSet);
                                pointList.Label = "Point List " + i;
                            }
                            pointList.Add(new Point(columnSet, currentValues.ToArray()));
                        }

                    }
                    catch (ApplicationException e) {
                        throw new InvalidDataException("Individual does not match axes", e);
                    }
                }

                columnSets.Add(columnSet);
                pointLists.Add(pointList);
                cReader.Close();
            }

            ColumnSet masterColumnSet = columnSets[0];
            for (int i = 1; i < columnSets.Count; i++) { masterColumnSet = ColumnSet.Union(masterColumnSet, columnSets[i]); }
            columnSets.Add(masterColumnSet);

            PointList masterPointList;
            if (pointLists.Count == 1) {
                masterPointList = pointLists[0];
            }
            else {
                PointList[] pointListArray = pointLists.ToArray();
                masterPointList = new PointList(masterColumnSet);

                for (int i = 0; i < pointListArray[0].Count; i++) {
                    List<double> values = new List<double>();
                    for (int j = 0; j < pointListArray.Length; j++) {
                        values.AddRange(pointListArray[j][i].Values);
                    }
                    masterPointList.Add(new Point(masterColumnSet, values.ToArray()));
                }
            }
            masterPointList.Label = "Master Points";

            PointSet masterPointSet = new PointSet("MasterPointSet", masterColumnSet, true);
            masterPointSet.Add(masterPointList);

            //Create default ColumProperties
            ProjectController.CreateMinMaxColumnProperties(masterPointSet);

            //Create Spaces
            List<Space> spaces = new List<Space>();
            if (columnSets.Count > 2) {
                for (int i = 0; i < columnSets.Count - 1; i++) {
                    spaces.Add(new Space(columnSets[i], "Space " + i));
                }
            }
            spaces.Insert(0, new Space(columnSets[columnSets.Count - 1], "Master Space"));

            message += "Master Points Count: " + masterPointSet.Length;

            // Some informational stuff
            if (null != PavelMain.LogBook) {
                PavelMain.LogBook.Message(message, false);
            }

            return new ParserResult(masterPointSet, spaces);
        }

        public void Initialize() { }

        public void Dispose() { }

        #endregion

        #endregion
    }
}
