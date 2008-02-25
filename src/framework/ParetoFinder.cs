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
                return Sort(pointSet, cp[0], cp[1]);
            } else if (cp.Length == 3) {
                return Sort(pointSet, cp[0], cp[1], cp[2]);
            } else {
                PavelMain.LogBook.Error("Invalid Column-Indices for evaluating the Pareto Front", true);
                return null;
            }
        }
        
        /// <summary>
        /// Performs the paretofinding for two dimensions
        /// Runtime is O(n*log(n))
        /// </summary>
        /// <param name="pointSet">pointset to calculated the paretofront</param>
        /// <param name="cp0">first dimension</param>
        /// <param name="cp1">second dimension</param>
        /// <returns>a selection containing the pareto-optimal points</returns>
        private static Selection Sort(PointSet pointSet, ColumnProperty cp0, ColumnProperty cp1) {

            ParetoPoint[] sorted = new ParetoPoint[pointSet.Length];

            //TODO: Mixing gui-code within the framework is not really nice, replace this by events
            PavelMain.MainWindow.StatusBar.StartProgressBar(0, sorted.Length, "Calculating Paretofront...");

            //Create an array containing the scaled points
            int column0 = pointSet.ColumnSet.IndexOf(cp0.Column);
            int column1 = pointSet.ColumnSet.IndexOf(cp1.Column);
            for (int i = 0; i < pointSet.Length; i++) {
                //TODO: Make Faster by precomputing Scale and Translation
                sorted[i] = new ParetoPoint(new double[] { pointSet[i].ScaledValue(column0, cp0), pointSet[i].ScaledValue(column1, cp1) }, pointSet[i]);
            }

            Array.Sort(sorted, Comparer2D);
            double min = sorted[0].values[1];
            Selection paretoFront = new Selection();
            paretoFront.Active = true;
            paretoFront.Label = "Pareto-Front";

            for (int i = 0; i < sorted.Length; i++) {
                if ( i % 100 == 0 ) PavelMain.MainWindow.StatusBar.IncrementProgressBar(100);
                if (min >= sorted[i].values[1]) {
                    paretoFront.Add(sorted[i].point);
                    min = sorted[i].values[1];
                }
            }
            PavelMain.MainWindow.StatusBar.EndProgressBar();
            return paretoFront;  
        }
        
        /// <summary>
        /// Performs the paretofinding for three dimensions
        /// Worst case runtime is O(n^2), when all points are already pareto-optimal
        /// Typically it is much faster and scales more or less with the size of the pareto-optimal set
        /// </summary>
        /// <param name="pointSet">pointset to calculated the paretofront</param>
        /// <param name="cp0">first dimension</param>
        /// <param name="cp1">second dimension</param>
        /// <param name="cp2">third dimension</param>
        /// <returns>a selection containing the pareto-optimal points</returns>
        private static Selection Sort(PointSet pointSet, ColumnProperty cp0, ColumnProperty cp1, ColumnProperty cp2) {
            ParetoPoint[] points = new ParetoPoint[pointSet.Length];
            //Create an array containing the scaled points
            int column0 = pointSet.ColumnSet.IndexOf(cp0.Column);
            int column1 = pointSet.ColumnSet.IndexOf(cp1.Column);
            int column2 = pointSet.ColumnSet.IndexOf(cp2.Column);
            for (int i = 0; i < pointSet.Length; i++) {
                //TODO: Make Faster by precomputing Scale and Translation
                points[i] = new ParetoPoint(new double[] { pointSet[i].ScaledValue(column0, cp0), pointSet[i].ScaledValue(column1, cp1), pointSet[i].ScaledValue(column2, cp2) }, pointSet[i]);
            }
            Selection paretoFront = new Selection();
            paretoFront.Active = true;
            paretoFront.Label = "Pareto-Front";

            //TODO: Mixing gui-code within the framework is not really nice, replace this by events
            PavelMain.MainWindow.StatusBar.StartProgressBar(0, points.Length, "Calculating Paretofront...");

            //determine the pareto-front
            for (int p1 = 0; p1 < points.Length; p1++) {
                if ( p1 % 100 == 0 ) PavelMain.MainWindow.StatusBar.IncrementProgressBar(100);
                if ( !points[p1].dominated ) {
                    for ( int p2 = p1; p2 < points.Length; p2++ ) {
                        if (!points[p2].dominated){
                            //Test if the the two points are in an dominating relation
                            Domination dom = Dominated3d(points[p1], points[p2]);
                            if ( dom == Domination.Dominated ) {
                                //p1 is dominated by p2, p1 cannot be pareto-optimal, ignore it
                                //and exit the loop, all points that are dominated by p1 must by dominated by p2 too.
                                points[p1].dominated = true;
                                break;
                            } else if ( dom == Domination.Dominating ) {
                                //p2 is domiated by p1, p2 cannot be pareto-optimal
                                //ignore it in the future
                                points[p2].dominated = true;
                            }
                        }
                    }
                }
            }
            //Get all not dominated points from the array
            for ( int p1 = 0; p1 < points.Length; p1++ ) {
                if ( !points[p1].dominated ) {
                    paretoFront.Add(points[p1].point);
                }
            }
            PavelMain.MainWindow.StatusBar.EndProgressBar();
            return paretoFront;
        }

        private enum Domination {Dominated, Dominating, None};

        /// <summary>
        /// Return the domination relation between two points.
        /// Either q is dominated by q or vice versa, or they are non-comparible by the criteria of pareto-optimality
        /// </summary>
        /// <param name="p">first point</param>
        /// <param name="q">second point</param>
        /// <returns>Dominated if q dominates p, Dominating if p dominates q, none otherwise</returns>
        private static Domination Dominated3d(ParetoPoint p, ParetoPoint q) {
            //All values must be smaller or equal, at least one must be smaller to be dominated
            if ( q.values[0] <= p.values[0] && q.values[1] <= p.values[1] && q.values[2] <= p.values[2] &&
                (q.values[0] < p.values[0] || q.values[1] < p.values[1] || q.values[2] < p.values[2]) )
                return Domination.Dominated;
            //All values must be greater or equal, at least one must be greater to dominate
            else if ( q.values[0] >= p.values[0] && q.values[1] >= p.values[1] && q.values[2] >= p.values[2] &&
                    (q.values[0] > p.values[0] || q.values[1] > p.values[1] || q.values[2] > p.values[2]) )
                    return Domination.Dominating;
            return Domination.None;
        }

        #region Comparer

        private static int Comparer2D(ParetoPoint p1, ParetoPoint p2) {
            if (p1.values[0] > p2.values[0]) return 1;
            if (p1.values[0] < p2.values[0]) return -1;
            if (p1.values[1] > p2.values[1]) return 1;
            if (p1.values[1] < p2.values[1]) return -1;
            return 0;
        }

        #endregion

        /// <summary>
        /// Container for storing some informations
        /// </summary>
        private class ParetoPoint {
            public double[] values;
            public Point    point;
            public bool     dominated;
            public ParetoPoint(double[] values, Point point) {
                this.values      = values;
                this.point       = point;
                this.dominated = false;
            }
        }
    }
}
