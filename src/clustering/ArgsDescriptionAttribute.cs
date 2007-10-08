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

/// <summary>
/// Namespace for clustering-classes.
/// </summary>
namespace Pavel.Clustering {

    #region ArgsDescriptionAttribute

    /// <summary>
    /// Special Attribute to describe Properties of ClusteringAlgorithm for default.
    /// ClusteringArgumentControl
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class ArgsDescriptionAttribute : System.Attribute {

        #region Fields

        private string name;
        private string help;

        #endregion

        #region Properties

        /// <value> Gets the name of the argument </value>
        public string Name { get { return name; } }

        /// <value> Gets the description of the argument </value>
        public string Description { get { return help; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Superclass for ArgumentDescriptionAttributes
        /// </summary>
        /// <param name="argumentName">Name for the argument</param>
        /// <param name="argumentDescription">Description for the argument</param>
        public ArgsDescriptionAttribute(string argumentName, string argumentDescription) {
            this.name = argumentName;
            this.help = argumentDescription;
        }

        #endregion
    }

    #endregion

    #region SpinnerAttribute

    /// <summary>
    /// DescriptionAttribute for Properties of type int, double, short that should be displayed as a spinner.
    /// </summary>
    public class SpinnerAttribute : ArgsDescriptionAttribute {

        #region Fields

        private double minimum;
        private double maximum;
        private int decimalPlaces;
        private int increment;

        #endregion

        #region Properties

        /// <value> Gets the lowest value of spinner </value>
        public double Minimum { get { return this.minimum; } }

        /// <value> Gets the highest value for spinner </value>
        public double Maximum { get { return this.maximum; } }

        /// <value> Gets the number of decimal places behind the comma </value>
        public int DecimalPlaces { get { return this.decimalPlaces; } }

        /// <value> Gets the default-increment for Spinner-Click </value>
        public int Increment { get { return this.increment; } }

        #endregion

        #region Constructors

        /// <summary>
        /// New Descriptor for a Spinner.
        /// </summary>
        /// <param name="argumentName">Display-Name</param>
        /// <param name="argumentDescription">Long description</param>
        /// <param name="minimum">Lowest value for spinner</param>
        /// <param name="maximum">Highest value for spinner</param>
        /// <param name="decimalPlaces">Number of decimal places behind the comma</param>
        /// <param name="increment">Default-increment for Spinner-Click</param>
        public SpinnerAttribute(
            string argumentName, string argumentDescription,
            double minimum, double maximum, int decimalPlaces, int increment)
            : base(argumentName, argumentDescription) {

            this.minimum = minimum;
            this.maximum = maximum;
            this.decimalPlaces = decimalPlaces;
            this.increment = increment;
        }

        #endregion
    }

    #endregion

    #region CheckBoxAttribute

    /// <summary>
    /// DescriptionAttribute for Properties of type bool that should be displayed as a Check-Box
    /// </summary>
    public class CheckBoxAttribute : ArgsDescriptionAttribute {
        /// <summary>
        /// New Descriptor for a Check-Box.
        /// </summary>
        /// <param name="argumentName">Display-Name</param>
        /// <param name="argumentDescription">Long description</param>
        public CheckBoxAttribute(string argumentName, string argumentDescription)
            : base(argumentName, argumentDescription) { }
    }

    #endregion

    #region ComboBoxAttribute

    /// <summary>
    /// DescriptionAttribute for Properties of type String-array that should be displayed as a Combo-Box
    /// </summary>
    public class ComboBoxAttribute : ArgsDescriptionAttribute {

        #region Fields

        private string[] items;

        #endregion

        #region Properties

        /// <value> Gets the items to be displayed </value>
        public string[] Items {
            get { return items.Clone() as string[]; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// New Descriptor for a Combo-Box.
        /// </summary>
        /// <param name="argumentName">Display-Name</param>
        /// <param name="argumentDescription">Long-Description</param>
        /// <param name="items">Items to be displayed</param>
        public ComboBoxAttribute(string argumentName, string argumentDescription,
            string[] items)
            : base(argumentName, argumentDescription) {
            this.items = items;
        }

        #endregion
    }

    #endregion
}
