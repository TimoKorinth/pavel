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
using System.Globalization;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Pavel.Framework {
    /// <summary>
    /// Contains Algorithms for computing the pareto front of a given List of Points.
    /// </summary>
    public static class ParetoFinder {

        /// <summary>
        /// Gives a Selection with the Points of the Pareto-Front. Those Points are not dominated by other
        /// points.
        /// </summary>
        /// <param param name="pointSet">Input PointSet</param>
        /// <param param name="cp">This array defines the ColumnProperties for which the Pareto-Front
        /// will be evaluated. For example: If you visualize columns 3 and 4, you should evaluate the
        /// Pareto-Front with {3,4} (in the 2D-Case).</param>
        public static Selection EvaluateParetoFront(PointSet pointSet, params ColumnProperty[] cp) {
            if (cp.Length == 2) {
                return Sort(pointSet, true, cp[0], cp[1]);
            } else if (cp.Length == 3) {
                return Sort(pointSet, true, cp[0], cp[1], cp[2]);
            } else {
                PavelMain.LogBook.Error("Invalid Column-Indices for evaluating the Pareto Front", true);
                return null;
            }
        }
        
        //------------------------2D: works and is fast-------------------------------------------------
        private static Selection Sort(PointSet pointSet, bool onlyPareto, ColumnProperty cp0, ColumnProperty cp1) {
            SortablePoint[] sorted = new SortablePoint[pointSet.Length];
            int index = 0;
            foreach (PointList pl in pointSet.PointLists) {
                int column0 = pl.ColumnSet.IndexOf(cp0.Column);
                int column1 = pl.ColumnSet.IndexOf(cp1.Column);
                for (int i = 0; i < pl.Count; i++) {
                    sorted[index] = new SortablePoint(new double[] { pl[i].ScaledValue(column0,cp0), pl[i].ScaledValue(column1,cp1) }, pl[i], 1);
                    index++;
                }
            }
            Array.Sort(sorted, Comparer2D);
            double min = sorted[0].Values[1];
            Selection paretoFront = new Selection();
            paretoFront.Active = true;
            paretoFront.Label = "Pareto-Front";

            for (int i = 0; i < sorted.Length; i++) {
                if (min >= sorted[i].Values[1]) {
                    paretoFront.Add(sorted[i].Point);
                    min = sorted[i].Values[1];
                }
            }
            return paretoFront;  
        }

        //------------------ 3D: works, but is slow for many points----------------------------------------------------------
        private static Selection Sort(PointSet pointSet, bool onlyPareto, ColumnProperty cp0, ColumnProperty cp1, ColumnProperty cp2) {
            SortablePoint[] sorted = new SortablePoint[pointSet.Length];
            int index = 0;
            foreach (PointList pl in pointSet.PointLists) {
                int column0 = pl.ColumnSet.IndexOf(cp0.Column);
                int column1 = pl.ColumnSet.IndexOf(cp1.Column);
                int column2 = pl.ColumnSet.IndexOf(cp2.Column);
                for (int i = 0; i < pl.Count; i++) {
                    //TODO ScaledValue is slow
                    sorted[index] = new SortablePoint(new double[] { pl[i].ScaledValue(column0,cp0), pl[i].ScaledValue(column1,cp1), pl[i].ScaledValue(column2,cp2) }, pl[i], 1);
                    index++;
                }
            }
            Selection paretoFront = new Selection();
            paretoFront.Active = true;
            paretoFront.Label = "Pareto-Front";
            int zaehler;

            for (int p1 = 0; p1 < sorted.Length; p1++) {
                zaehler = 0;
                for (int p2 = 0; p2 < sorted.Length; p2++) {
                    if (Dominated3d(sorted[p1], sorted[p2])) // true if i dominated by j // TODO: i? j?
                        break; 
                    else
                        zaehler++;
                }
                if (zaehler == sorted.Length) { paretoFront.Add(sorted[p1].Point); }
            }

            return paretoFront;
        }


        // true wenn p durch q dominiert wird
        private static bool Dominated3d(SortablePoint p, SortablePoint q) {
            if  (    ((q.Values[0] <= p.Values[0]) && (q.Values[1] <  p.Values[1]) && (q.Values[2] <  p.Values[2]))
                  || ((q.Values[0] <  p.Values[0]) && (q.Values[1] <= p.Values[1]) && (q.Values[2] <  p.Values[2]))
                  || ((q.Values[0] <  p.Values[0]) && (q.Values[1] <  p.Values[1]) && (q.Values[2] <= p.Values[2]))
                  && (!(q.Equals(p))) ) { //TODO: Operator precedence correct?
                return true;
            } else {
                return false;
            }
        }

        private static void SetMaximumFrontNumber(SortablePoint p1, SortablePoint p2) {
            p1.FrontNumber = Maximum(p1.FrontNumber, (p2.FrontNumber +1));
        }

        #region Helper Methods

        private static double Median(List<SortablePoint> s, int dimension) {
            double temp = 0;
            foreach (SortablePoint p in s) {
                temp = temp + p.Values[dimension];
            }
            temp = (temp / s.Count);
            return temp;
        }

        private static int Maximum(int m, int n) {
            if (m < n) return n;
            if (m > n) return m; // TODO unneccessary line (unnecessary method)
            else       return m;
        }

        private static double Maximum(List<SortablePoint> s, int dimension) {
            double temp = s[0].Values[dimension];

            foreach (SortablePoint p in s) {
                if (p.Values[dimension] > temp) { temp = p.Values[dimension]; }
            }
            return temp;
        }

        private static double Minimum(List<SortablePoint> s, int dimension) {
            double temp = s[0].Values[dimension];

            foreach (SortablePoint p in s) {
                if (p.Values[dimension] < temp) { temp = p.Values[dimension]; }
            }
            return temp;
        }

        #endregion

        #region Comparer

        private static int Comparer2D(SortablePoint p1, SortablePoint p2) {
            if (p1.Values[0] > p2.Values[0]) return 1;
            if (p1.Values[0] < p2.Values[0]) return -1;
            if (p1.Values[1] > p2.Values[1]) return 1;
            if (p1.Values[1] < p2.Values[1]) return -1;
            return 0;
        }

        private static int Comparer3D(SortablePoint p1, SortablePoint p2) {
            if (p1.Values[0] > p2.Values[0]) return 1;
            if (p1.Values[0] < p2.Values[0]) return -1;
            if (p1.Values[1] > p2.Values[1]) return 1;
            if (p1.Values[1] < p2.Values[1]) return -1;
            if (p1.Values[2] > p2.Values[2]) return 1;
            if (p1.Values[2] < p2.Values[2]) return -1;
            return 0;
        }

        #endregion

        struct SortablePoint {
            public double[] Values;
            public Point    Point;
            public int      FrontNumber;
            public SortablePoint(double[] values, Point point, int frontnumber) {
                this.Values      = values;
                this.Point       = point;
                this.FrontNumber = frontnumber;
            }
        }
    }
}
