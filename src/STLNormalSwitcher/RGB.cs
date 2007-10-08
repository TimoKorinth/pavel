// Part of STLNormalSwitcher: A program to switch normal vectors in STL-files
//
// Copyright (C) 2007  PG500, ISF, University of Dortmund
//      PG500 are: Christoph Begau, Christoph Heuel, Raffael Joliet, Jan Kolanski,
//                 Mandy Kröller, Christian Moritz, Daniel Niggemann, Mathias Stöber,
//                 Timo Stönner, Jan Varwig, Dafan Zhai
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
// The licence can also be found at: http://www.gnu.org/licenses/old-licenses/gpl-2.0.txt
//
// For more information and contact details look at STLNormalSwitchers website:
//      http://normalswitcher.sourceforge.net/
//
// Check out PAVEl (http://pavel.sourceforge.net/) another great program brought to you by PG500.

using System;
using System.Collections.Generic;
using System.Text;

namespace STLNormalSwitcher {
    /// <summary>
    /// A specialised Container for an RGB color of 3 floats.
    /// Implements a comparable and an equal-method.
    /// </summary>
    public class RGB : IComparable<RGB>, IEquatable<RGB> {

        #region Fields

        private float r, g, b;

        #endregion

        #region Properties

        /// <value>Gets the red value or sets it</value>
        public float R {
            get { return r; }
            set { r = value; }
        }

        /// <value>Gets the green value or sets it</value>
        public float G {
            get { return g; }
            set { g = value; }
        }

        /// <value>Gets the blue value or sets it</value>
        public float B {
            get { return b; }
            set { b = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the RGB with the given values.
        /// </summary>
        /// <param name="r">Red value</param>
        /// <param name="g">Green value</param>
        /// <param name="b">Blue value</param>
        public RGB(float r, float g, float b) {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the RGB to another one.
        /// </summary>
        /// <param name="color">Other RGB</param>
        /// <returns>0 if the RGBs are equal, 1 if <paramref name="color"/> is smaller, -1 if <paramref name="color"/> is bigger </returns>
        public int CompareTo(RGB color) {
            if (this.r < color.r)
                return -1;
            else if (this.r > color.r)
                return 1;
            if (this.g < color.g)
                return -1;
            else if (this.g > color.g)
                return 1;
            if (this.b < color.b)
                return -1;
            else if (this.b > color.b)
                return 1;
            return 0;
        }

        /// <summary>
        /// Returns true if the RGB given by <paramref name="other"/> is the same as this RGB.
        /// </summary>
        /// <param name="other">Other RGB</param>
        /// <returns>True if the RGBs are equal</returns>
        public bool Equals(RGB other) {
            if (this.CompareTo(other) != 0)
                return false;
            return true;
        }

        #endregion
    }
}