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
using Pavel.Framework;

namespace Pavel.Framework {
    [CoverageExclude]

    /// <summary>
    /// Helper class for creating individual Columns.
    /// </summary>
    public static class IndividualColumns {

        #region Fields

        // Operators represented by words, with the exception of "root", "nCr" and "nPr".
        private static string[] words = new string[] { "Arsech", "Arcsch", "Arsinh", "Arcosh", "Artanh", "Arcoth",
            "acosec", "asec", "asin", "acos", "atan", "acot",
            "cosech", "sech", "sinh", "cosh", "tanh", "coth",
            "cosec", "sec", "sin", "cos", "tan", "cot",
            "log", "ln", "floor", "ceiling", "sqrt", "round", "abs", "e", "pi" };
        public static double factor = 1.0d;
        // Factors for the angle-modes (DEG, RAD, GRAD)
        private static double[] factors = { Math.PI / 180, 1.0d, Math.PI / 200 };

        #endregion

        #region Methods

        /// <summary>
        /// Create an individual Column by replacing the selected PointSets with an extended one
        /// </summary>
        /// <param name="factorIndex">Index of the factor for evaluation in the list of factors</param>
        /// <param name="formula">Formula for the individual Column</param>
        /// <param name="pointSets">PointSets to be extended</param>
        /// <param name="columnLabel">Label for the new Column</param>
        public static Column CreateColumn(Int32 factorIndex, String formula, IEnumerable<PointSet> pointSets, string columnLabel) {
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            IndividualColumns.factor = factors[factorIndex];

            Column col = new Column(columnLabel);
            ColumnSet newColColumnSet = new ColumnSet(col);
            Dictionary<Point, Point> copiedPoints = new Dictionary<Point, Point>();

            //1. Step: Copy all Points, with extended columnSet and a new column
            foreach (PointSet ps in pointSets) {
                for (int i = 0; i < ps.PointLists.Count; i++) {
                    ColumnSet extendedColumnSet = ColumnSet.Union(ps.PointLists[i].ColumnSet, newColColumnSet);
                    foreach (Point p in ps.PointLists[i]) {
                        if (!copiedPoints.ContainsKey(p)) {
                            double[] copiedValues = new double[p.Values.Length + 1];
                            p.Values.CopyTo(copiedValues, 0);

                            String editedFormula = formula;
                            for (int j = ProjectController.Project.columns.Count - 1; j >= 0; j--) {
                                if (editedFormula.Contains("$" + j)) {
                                    editedFormula = editedFormula.Replace("$" + j, p[ProjectController.Project.columns[j]].ToString());
                                }
                            }
                            double value = Evaluate(editedFormula);

                            if (value > max) { max = value; }
                            if (value < min) { min = value; }

                            copiedValues[copiedValues.Length - 1] = value;
                            Point copiedPoint = new Point(extendedColumnSet, copiedValues);
                            copiedPoint.Tag = p.Tag;
                            copiedPoints.Add(p, copiedPoint);
                        }
                    }
                }
            }

            //Set ColumnProperties min and max
            col.DefaultColumnProperty.SetMinMax(min, max);

            //2. Step: Copy the pointSets and PointLists, extend their columnSets and fill them with the copied points
            List<PointSet> copiedPointSets = new List<PointSet>();
            foreach (PointSet ps in pointSets) {
                List<PointList> copiedPointsList = new List<PointList>();
                for (int i = 0; i < ps.PointLists.Count; i++) {
                    List<Point> points = new List<Point>();
                    ColumnSet extendedColumnSet = ColumnSet.Union(ps.PointLists[i].ColumnSet, newColColumnSet);
                    //Create a new PointList, with the copied Points
                    foreach (Point p in ps.PointLists[i]) {
                        points.Add(copiedPoints[p]);
                    }
                    PointList copiedPointList = new PointList(extendedColumnSet, points);
                    copiedPointsList.Add(copiedPointList);
                }
                PointSet copiedPointSet = new PointSet(ps.Label, ColumnSet.Union(ps.ColumnSet, newColColumnSet));
                foreach (PointList pl in copiedPointsList) {
                    copiedPointSet.Add(pl);
                }
                copiedPointSets.Add(copiedPointSet);
            }
            //3. Step: Delete old PointSets, and store new PointSets
            foreach (PointSet ps in pointSets) {
                ProjectController.Project.pointSets.Remove(ps);
            }
            ProjectController.Project.pointSets.AddRange(copiedPointSets);
            return col;
        }

        #region Evaluation

        /// <summary>
        /// Evaluates the formula given by <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">The formula as a String</param>
        /// <returns>Result of the formula</returns>
        public static double Evaluate(string expression) {
            double value;
            expression = expression.Trim();
            try {
                value = (EvalExpression(ref expression));
                if ((expression.Length != 0) || (Double.IsNaN(value))) { throw new FormatException(); }
                return value;
            } catch (Exception e) { throw e; }
        }

        /// <summary>
        /// Calculates the result of the formula given by <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">Formula as a String</param>
        /// <returns>Calculated result</returns>
        private static double EvalExpression(ref string expression) {
            double op, op2;

            expression = expression.Trim();
            if (expression == "") { return 0; }

            // Parse left side of expression
            op = EvalFactor(ref expression);

            // Parse right side of expression, if it exists
            if (expression != "") {
                if (expression[0] == '+') {
                    expression = expression.Substring(1, expression.Length - 1).Trim();
                    // No operator on right side
                    if (expression.Length == 0) { throw new FormatException(); }
                    op2 = EvalExpression(ref expression);
                    op += op2;
                } else if (expression[0] == '-') {
                    expression = expression.Substring(1, expression.Length - 1).Trim();
                    // No operator on right side
                    if (expression.Length == 0) { throw new FormatException(); }
                    op2 = EvalExpression(ref expression);
                    op -= op2;
                }
            }
            return op;
        }

        /// <summary>
        /// Calculates *, / and % expressions.
        /// </summary>
        /// <param name="expression">Formula as a String</param>
        /// <returns>Result of multiplikation, division or modulo</returns>
        private static double EvalFactor(ref string expression) {
            double op, op2;

            expression = expression.Trim();
            op = EvalExpo(ref expression);
            if (expression != "") {
                if (expression[0] == '*') {
                    expression = expression.Substring(1, expression.Length - 1).Trim();
                    if (expression.Length == 0) { throw new FormatException(); }
                    op2 = EvalFactor(ref expression);
                    op *= op2;
                } else if (expression[0] == '/') {
                    expression = expression.Substring(1, expression.Length - 1).Trim();
                    if (expression.Length == 0) { throw new FormatException(); }
                    op2 = EvalFactor(ref expression);
                    if (op2 == 0) { throw new DivideByZeroException(); }
                    op /= op2;
                } else if (expression[0] == '%') {
                    expression = expression.Substring(1, expression.Length - 1).Trim();
                    if (expression.Length == 0) { throw new FormatException(); }
                    op2 = EvalFactor(ref expression);
                    op = Modulo((int)op,(int)op2);
                }
            }
            return op;
        }

        /// <summary>
        /// Calculates factorial and exponential expressions.
        /// </summary>
        /// <param name="expression">Formula as a String</param>
        /// <returns>Result of factorial or exponential evaluation</returns>
        private static double EvalExpo(ref string expression) {
            double op, op2;

            expression = expression.Trim();
            op = EvalTerm(ref expression);
            if (expression != "") {
                if (expression.StartsWith("root")) {
                    expression = expression.Substring(4, expression.Length - 4).Trim();
                    if (expression.Length == 0) { throw new FormatException(); }
                    op2 = EvalExpo(ref expression);
                    op = Root(op, op2);
                } else if(expression.StartsWith("E")) {
                    expression = expression.Substring(1, expression.Length - 1).Trim();
                    if (expression.Length == 0) { throw new FormatException(); }
                    op2 = EvalExpo(ref expression);
                    op = op * Math.Pow(10,op2);
                } else if (expression.StartsWith("C")) {
                    expression = expression.Substring(1, expression.Length - 1).Trim();
                    if (expression.Length == 0) { throw new FormatException(); }
                    op2 = EvalExpo(ref expression);
                    op = Combination(op, op2);
                } else if (expression.StartsWith("P")) {
                    expression = expression.Substring(1, expression.Length - 1).Trim();
                    if (expression.Length == 0) { throw new FormatException(); }
                    op2 = EvalExpo(ref expression);
                    op = Permutation(op, op2);
                } else if (expression[0] == '!') {
                    expression = expression.Substring(1, expression.Length - 1).Trim();
                    return Fact((int)op);
                } else if (expression[0] == '^') {
                    expression = expression.Substring(1, expression.Length - 1).Trim();
                    if (expression.Length == 0) { throw new FormatException(); }
                    op2 = EvalExpo(ref expression);
                    op = Math.Pow((int)op, (int)op2);
                }
            }
            return op;
        }

        /// <summary>
        /// Evaluates a term, that is either a number, an unary operator (-, sin, cos, etc.),
        /// or an expression in brackets.
        /// </summary>
        /// <param name="expression">Formula as a String</param>
        /// <returns>Result of term evaluation</returns>
        private static double EvalTerm(ref string expression) {
            double value = 0;

            expression = expression.Trim();
            if (expression.Length != 0) {
                // Number
                if (char.IsDigit(expression[0])) { return EvalNumber(ref expression); }
                    // Word like sin, cos etc.
                else if (char.IsLetter(expression[0])) { return EvalWord(ref expression); }
                    // Unary - operator
                else if (expression[0] == '-') {
                    expression = expression.Substring(1, expression.Length - 1);
                    return -EvalTerm(ref expression);
                    // Expression in brackets, must have opening and closing bracket
                } else if (expression[0] == '(') {
                    expression = expression.Substring(1, expression.Length - 1);
                    value = EvalExpression(ref expression);
                    if (expression[0] != ')') { throw new FormatException(); }
                    expression = expression.Substring(1, expression.Length - 1);
                } else { throw new FormatException(); }
            } else { throw new FormatException(); }
            return value;
        }

        /// <summary>
        /// Evaluates the unary operations cos, sin, tan, cosh, sinh, tanh, acos, asin, atan,
        /// log, ln, floor, ceil, sqrt, round, abs and the constants e and pi given as words.
        /// </summary>
        /// <param name="expression">Formula as a String</param>
        /// <returns>Result of the unary operation or constant</returns>
        private static double EvalWord(ref string expression) {
            double value = 0;

            expression = expression.Trim();
            if (expression.Length != 0) {
                string funcName = GetFuncName(expression);
                expression = expression.Substring(funcName.Length, expression.Length - funcName.Length);
                switch (funcName) {
                    case "Arcsch":
                        return Arcsch(EvalTerm(ref expression));
                    case "Arsech":
                        return Arsech(EvalTerm(ref expression));
                    case "Arsinh":
                        return Arsinh(EvalTerm(ref expression));
                    case "Arcosh":
                        return Arcosh(EvalTerm(ref expression));
                    case "Artanh":
                        return Artanh(EvalTerm(ref expression));
                    case "Arcoth":
                        return Arcoth(EvalTerm(ref expression));

                    case "cosech":
                        return 1 / Math.Sinh(EvalTerm(ref expression));
                    case "sech":
                        return 1 / Math.Cosh(EvalTerm(ref expression));
                    case "cosh":
                        return Math.Cosh(EvalTerm(ref expression));
                    case "sinh":
                        return Math.Sinh(EvalTerm(ref expression));
                    case "tanh":
                        return Math.Tanh(EvalTerm(ref expression));
                    case "coth":
                        return 1 / Math.Tanh(EvalTerm(ref expression));

                    case "cosec":
                        return 1 / Math.Sin(EvalTerm(ref expression) * factor);
                    case "sec":
                        return 1 / Math.Cos(EvalTerm(ref expression) * factor);
                    case "cos":
                        return Math.Cos(EvalTerm(ref expression) * factor);
                    case "sin":
                        return Math.Sin(EvalTerm(ref expression) * factor);
                    case "tan":
                        return Math.Tan(EvalTerm(ref expression) * factor);
                    case "cot":
                        return 1 / Math.Tan(EvalTerm(ref expression) * factor);

                    case "acosec":
                        return Math.Asin(1 / EvalTerm(ref expression)) / factor;
                    case "asec":
                        return Math.Acos(1 / EvalTerm(ref expression)) / factor;
                    case "acos":
                        return Math.Acos(EvalTerm(ref expression)) / factor;
                    case "asin":
                        return Math.Asin(EvalTerm(ref expression)) / factor;
                    case "atan":
                        return Math.Atan(EvalTerm(ref expression)) / factor;
                    case "acot":
                        return (Math.PI / 2 - Math.Atan(EvalTerm(ref expression))) / factor;

                    case "log":
                        return Math.Log10(EvalTerm(ref expression));
                    case "ln":
                        return Math.Log(EvalTerm(ref expression));

                    case "floor":
                        return Math.Floor(EvalTerm(ref expression));
                    case "ceiling":
                        return Math.Ceiling(EvalTerm(ref expression));
                    case "sqrt":
                        return Math.Sqrt(EvalTerm(ref expression));
                    case "round":
                        return Math.Round(EvalTerm(ref expression));
                    case "abs":
                        return Math.Abs(EvalTerm(ref expression));

                    case "E":
                        return 10 ^ (int)EvalTerm(ref expression);
                    case "e":
                        return Math.E;
                    case "pi":
                        return Math.PI;
                    default:
                        throw new FormatException();
                }
            }
            return value;
        }

        /// <summary>
        /// Converts a number from string to double.
        /// </summary>
        /// <param name="expression">Number as string</param>
        /// <returns>Number as double</returns>
        private static double EvalNumber(ref string expression) {
            bool foundSeparator = false;
            string temp = "";
            int i = 0;
            expression = expression.Trim();

            // build a string with the number
            while (i != expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.')) {
                // Allow only one decimal separator
                if (expression[i] == '.') {
                    if (!foundSeparator) { foundSeparator = true; } else { throw new FormatException(); }
                }
                temp += expression[i++];
            }
            // First or last symbol may not be the decimal separator
            if (temp[0] == '.' || temp[temp.Length - 1] == '.') { throw new FormatException(); }

            expression = expression.Substring(i, expression.Length - i).Trim();
            return Double.Parse(temp);
        }

        #region Helpers

        /// <summary>
        /// Returns the function or constant that is represented by a word
        /// the string <paramref name="expression"/> starts with.
        /// </summary>
        /// <param name="expression">Formula as string</param>
        /// <returns>Function the formula starts with</returns>
        private static string GetFuncName(string expression) {
            string funcName = "";
            for (int i = 0; i < words.Length; i++) {
                if (expression.StartsWith(words[i])) { return funcName = words[i]; }
            }
            if (funcName == "") { throw new System.FormatException(); }
            return funcName;
        }

        #endregion

        #region Math

        /// <summary>
        /// Calculates the mathematically correct modulo-function
        /// <paramref name="op1"/> modulo <paramref name="op2"/>.
        /// The % operator in C# does not function correctly for
        /// negative <paramref name="op1"/>.
        /// </summary>
        /// <param name="op1">Dividend</param>
        /// <param name="op2">Divisor</param>
        /// <returns><paramref name="op1"/> modulo <paramref name="op2"/></returns>
        private static int Modulo(int op1, int op2) {
            if (op2<=0) { throw new FormatException(); }
            return op1 - op2 * (int)Math.Floor((double)op1 / (double)op2);
        }

        /// <summary>
        /// Calculates factorial <paramref name="n"/>.
        /// </summary>
        /// <param name="n">Value to calculate factorial of</param>
        /// <returns>Factorial of <paramref name="n"/></returns>
        private static int Fact(int n) {
            return n == 0 ? 1 : n * Fact(n - 1);
        }

        /// <summary>
        /// Calculates the <paramref name="x"/>-th root of <paramref name="y"/>.
        /// </summary>
        /// <param name="x">Root-exponent</param>
        /// <param name="y">Radicand</param>
        /// <returns><paramref name="x"/>-th root of <paramref name="y"/></returns>
        private static double Root(double x, double y) {
            double root = Math.Sqrt(y);
            double oldRoot;
            if ((int)x == 2) { return root; }

            do {
                oldRoot = root;
                root = 1.0d / (int)x * (((int)x - 1) * oldRoot + y / Math.Pow(oldRoot, (int)x - 1));
            } while (Math.Abs(oldRoot - root) > 0.0000000000001);
            return root;
        }

        /// <summary>
        /// Calculates the binomial coefficient <paramref name="n"/> over <paramref name="r"/>.
        /// Floors the given doubles first.
        /// </summary>
        /// <param name="n">Top value</param>
        /// <param name="r">Bottom value</param>
        /// <returns>Binomial coefficient</returns>
        private static double Combination(double n, double r) {
            if (n < r) { throw new FormatException(); }
            return Fact((int)n) / (Fact((int)n - (int)r) * Fact((int)r));
        }

        /// <summary>
        /// Calculates the number of permutations of a sequence.
        /// Floors the given doubles first.
        /// </summary>
        /// <param name="n">Size of sequence from which elements are permuted</param>
        /// <param name="r">Size of permutation</param>
        /// <returns>Binomial coefficient</returns>
        private static double Permutation(double n, double r) {
            if (n < r) { throw new FormatException(); }
            return Fact((int)n) / Fact((int)n - (int)r);
        }

        /// <summary>
        /// Calculates the Inverse Hyperbolic Sine.
        /// </summary>
        /// <param name="x">Value to calculate Inverse Hyperbolic Sine of</param>
        /// <returns>Inverse Hyperbolic Sine of <paramref name="x"/></returns>
        private static double Arsinh(double x) {
            return Math.Log(x + Math.Sqrt(x * x + 1));
        }

        /// <summary>
        /// Calculates the Inverse Hyperbolic Cosine.
        /// </summary>
        /// <param name="x">Value to calculate Inverse Hyperbolic Cosine of</param>
        /// <returns>Inverse Hyperbolic Cosine of <paramref name="x"/></returns>
        private static double Arcosh(double x) {
            if (x < 1) { throw new FormatException(); }
            return Math.Log(x + Math.Sqrt(x * x - 1));
        }

        /// <summary>
        /// Calculates the Inverse Hyperbolic Tangent.
        /// </summary>
        /// <param name="x">Value to calculate Inverse Hyperbolic Tangent of</param>
        /// <returns>Inverse Hyperbolic Tangent of <paramref name="x"/></returns>
        private static double Artanh(double x) {
            if ((x >= 1) || (x <= -1)) { throw new FormatException(); }
            return 0.5 * Math.Log((1 + x) / (1 - x));
        }

        /// <summary>
        /// Calculates the Inverse Hyperbolic Cotangent.
        /// </summary>
        /// <param name="x">Value to calculate Inverse Hyperbolic Cotangent of</param>
        /// <returns>Inverse Hyperbolic Cotangent of <paramref name="x"/></returns>
        private static double Arcoth(double x) {
            if ((x >= -1) && (x <= 1)) { throw new FormatException(); }
            return 0.5 * Math.Log((x + 1) / (x - 1));
        }

        /// <summary>
        /// Calculates the Inverse Hyperbolic Secant.
        /// </summary>
        /// <param name="x">Value to calculate Inverse Hyperbolic Secant of</param>
        /// <returns>Inverse Hyperbolic Secant of <paramref name="x"/></returns>
        private static double Arsech(double x) {
            if ((x <= 0) || (x > 1)) { throw new FormatException(); }
            return Math.Log((1 + Math.Sqrt(1 - x * x)) / x);
        }

        /// <summary>
        /// Calculates the Inverse Hyperbolic Cosecant.
        /// </summary>
        /// <param name="x">Value to calculate Inverse Hyperbolic Cosecant of</param>
        /// <returns>Inverse Hyperbolic Cosecant of <paramref name="x"/></returns>
        private static double Arcsch(double x) {
            if (x == 0) { throw new FormatException(); } else if (x < 0) { return Math.Log((1 - Math.Sqrt(1 + x * x)) / x); } else { return Math.Log((1 + Math.Sqrt(1 + x * x)) / x); }
        }

        #endregion

        #endregion

        #endregion
    }
}
