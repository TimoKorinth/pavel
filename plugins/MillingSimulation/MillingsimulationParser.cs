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
    public class MillingSimulationParser {

        #region Fields

        private NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
        private char[] splitter;

        /// <summary>
        /// Stores the singleton instance of the profileColumnSet
        /// </summary>
        private static ColumnSet profileColumnSet;
        /// <summary>
        /// Stores the singleton instance of the masterColumnSet
        /// </summary>
        private static ColumnSet masterColumnSet;
        /// <summary>
        /// Stores the singleton instance of the masterSpace
        /// </summary>
        private static Space masterSpace;
        #endregion

        #region Constructor

        /// <summary>
        /// Creates a MillingSimulationParser with given decimal seperator and ";" or "\t" as splitters
        /// <param name="numberFormat">numberDecimalSeperator, "," and "." are allowed</param>
        /// </summary>
        public MillingSimulationParser(string numberFormat) {
            if (!numberFormat.Equals(".") && (!numberFormat.Equals(",")) ){
                throw new ArgumentException("Only \".\" and \",\" are allowed as decimal seperators");
            }
            numberFormatInfo.NumberDecimalSeparator = numberFormat ;
            this.splitter = new char[] {';', '\t'};
        }

        /// <summary>
        /// Creates a MillingSimulationParser with "." as decimal seperator and ";" or "\t" as splitters
        /// </summary>
        public MillingSimulationParser() {
            numberFormatInfo.NumberDecimalSeparator = "." ;
            this.splitter = new char[] {';', '\t'};
        }

        #endregion

        #region Properties

        public String Label { get { return "Milling Simulation Parser"; } }

        internal static ColumnSet ProfileColumnSet { //Singleton
            get {
                if (null == profileColumnSet)
                    profileColumnSet = new ColumnSet(
                        new Column("Kraft X"),
                        new Column("Kraft Y"),
                        new Column("Kraft Z"),
                        new Column("X-Auslenkung Pkt 1"),
                        new Column("Y-Auslenkung Pkt 1"),
                        new Column("Z-Auslenkung Pkt 1"),
                        new Column("X-Auslenkung Pkt 2"),
                        new Column("Y-Auslenkung Pkt 2"),
                        new Column("Z-Auslenkung Pkt 2")
                    );
                return profileColumnSet;
            }
        }

        internal static ColumnSet MasterColumnSet { //Singleton
            get {
                if (null == masterColumnSet)
                    masterColumnSet = new ColumnSet(
                        new Column("Frequenz"),
                        new Column("Amplitude 1"),
                        new Column("Hoehe"),
                        new Column("Amplitude 2")
                    );
                return masterColumnSet;
            }
        }

        internal static Space MasterSpace { //Singleton
            get {
                if (null == masterSpace)
                    masterSpace = new Space(MasterColumnSet, "Master Space");
                return masterSpace;
            }
        }

        #endregion

        #region Methods

        public ExtendedParserResult ParseHistogram(StreamReader reader) {
            PointSet  masterPointSet  = new PointSet("Histogram", MasterColumnSet);
            Dictionary<int, PointSet> histogramPointSets = new Dictionary<int, PointSet>();

            int rownum = 0;
            foreach (double[] rowValues in rowsAsDoubleArrays(reader)) {
#if DEBUG
                if (rownum++ == 1) { rownum = 0; continue; }
#endif
                for (int height = 0; height < (rowValues.Length-1)/2; height++) {
                    if (Double.IsNaN(rowValues[1 + 2 * height + 0])) continue;
                    HeightPoint heightPoint = new HeightPoint(MasterColumnSet, height, rowValues[0], rowValues[1 + 2 * height + 0], height, rowValues[1 + 2 * height + 1]);
                    //Add to master PointSet
                    masterPointSet.Add(heightPoint);
                    //Add to histogramPointSet
                    if (!histogramPointSets.ContainsKey(height)) {
                        histogramPointSets.Add(height, new PointSet("Histogram Height " + height, MasterColumnSet));
                    }
                    histogramPointSets[height].Add(heightPoint);
                }                
            }
          
            // Create default ColumProperties
            ProjectController.CreateMinMaxColumnProperties(masterPointSet);

            // Create Spaces
            List<Space> spaces = new List<Space>();
            spaces.Add(MasterSpace);

            //Convert histogramPointSets to array
            PointSet[] hps = new PointSet[histogramPointSets.Count];
            histogramPointSets.Values.CopyTo(hps, 0);

            return new ExtendedParserResult(masterPointSet, spaces, hps);
        }

        public PointSet ParseProfile(StreamReader reader) {
            PointSet  profilePointSet  = new PointSet("Profile", ProfileColumnSet);

            int rownum = 0;

            foreach (double[] rowValues in rowsAsDoubleArrays(reader)) {
                ++rownum;
                profilePointSet.Add(new Point(ProfileColumnSet, rowValues));
            }

            return profilePointSet;
        }

        private static bool RowEmpty(String row) {
            return (row.Length == 0) || (row.StartsWith("#"));
        }

        private double[] doublesFromRow(String row) {
            String[] splittedRow = row.Split(splitter);
            double[] rowValues = new double[splittedRow.Length];
            for (int i = 0; i < splittedRow.Length; i++) {
                if (splittedRow[i].Length != 0)
                    rowValues[i] = Double.Parse(splittedRow[i], NumberStyles.Float, numberFormatInfo);
                else
                    rowValues[i] = 0d;
            }
            return rowValues;
        }

        private IEnumerable<double[]> rowsAsDoubleArrays(StreamReader reader) {
            String completeRow;
            while (!reader.EndOfStream) {
                completeRow = reader.ReadLine().Trim();
                if (!RowEmpty(completeRow)) yield return doublesFromRow(completeRow);
            }

        }

        #endregion

    }

}
