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
using System.Drawing;
using Pavel.Framework;

namespace Pavel.GUI.Visualizations {
    /// <summary>
    /// Manages the colors for the visualizations.
    /// </summary>
    public static class ColorManagement {

        #region Fields

        private static List<ColorOGL> selectionColors;
        private static ColorOGL backgroundColor = new ColorOGL(Color.Black);
        private static ColorOGL axesColor = new ColorOGL(Color.LightGray);
        private static ColorOGL descriptionColor = new ColorOGL(Color.LightGray);

        #endregion

        #region Properties

        /// <value>Gets the color for descriptions</value>
        public static ColorOGL DescriptionColor { get { return ColorManagement.descriptionColor; } }

        /// <value>Gets the color for axes</value>
        public static ColorOGL AxesColor { get { return ColorManagement.axesColor; } }

        /// <value>Gets the color for the background</value>
        public static ColorOGL BackgroundColor { get { return backgroundColor; } }

        /// <value>Gets the color for unselected points</value>
        public static ColorOGL UnselectedColor {
            get {
                if (null == selectionColors) { InitializeSelectionColors(2); }
                return GetColor(0);
            }
        }

        /// <value>Gets the color for currently selected points</value>
        public static ColorOGL CurrentSelectionColor {
            get {
                if (null == selectionColors) { InitializeSelectionColors(2); }
                return GetColor(1);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the selectionColors.
        /// </summary>
        /// <param name="count">Total number of colors</param>
        private static void InitializeSelectionColors(int count) {
            selectionColors = new List<ColorOGL>();
            FillSelectionColors(count);
        }

        /// <summary>
        /// Returns a color at a position. If no color exists at this position
        /// the selectionColors list is expanded.
        /// </summary>
        /// <param name="position">Position of the Color.</param>
        /// <returns>ColorOGL</returns>
        public static ColorOGL GetColor(int position) {
            if ( selectionColors == null ) {
                InitializeSelectionColors(position + 1);
            }
            if (position >= selectionColors.Count) {
                FillSelectionColors(position+1);
            }
            return selectionColors[position];
        }

        /// <summary>
        /// Fills the selectionColors list with colors, which are optimized
        /// for good readability.
        /// </summary>
        /// <param name="count">Total number of colors</param>
        private static void FillSelectionColors(int count) {
            int unselected = 180;
            int selected = 0;
            selectionColors.Clear();
            selectionColors.Add(HSVToRGB(unselected, 1, 0.78f));
            selectionColors.Add(HSVToRGB(selected, 1, 0.78f));
            float h;
            int index = 1;
            while (selectionColors.Count < count) {
                bool s = false;
                for (int i = 1; i<index;i += 2 ) {
                    h = 360/index*i;
                    if (s) {
                        if (h != unselected && h != selected) {
                            selectionColors.Add(HSVToRGB(h, 0.5f, 0.97f));
                            s = !s;
                        }
                    }
                    else {
                        if (h != unselected && h != selected) {
                            selectionColors.Add(HSVToRGB(h, 1, 0.78f));
                            s = !s;
                        }
                    }
                }
                index *= 2;
                if (index == 0) index++;
            }
        }

        /// <summary>
        /// Converts HSV Colors to RGB Colors.
        /// </summary>
        /// <param name="h">Hue (0 to 360)</param>
        /// <param name="s">Saturation (0 to 1)</param>
        /// <param name="v">Value (0 to 1)</param>
        /// <returns>OpenGL Color</returns>
        public static ColorOGL HSVToRGB(float h, float s, float v) {
            ColorOGL color = new ColorOGL();
            int i;
            float f, p, q, t;
            if (s == 0) {
                color.Color = Color.FromArgb(0, (int)(v * 255), (int)(v * 255), (int)(v * 255));
                return color;
            }
            h /= 60;
            i = (int)Math.Floor(h);
            f = h - i;
            p = v * (1 - s);
            q = v * (1 - s * f);
            t = v * (1 - s * (1 - f));
            switch (i) {
                case 0:
                    color.Color = Color.FromArgb(0, (int)(v * 255), (int)(t * 255), (int)(p * 255));
                    break;
                case 1:
                    color.Color = Color.FromArgb(0, (int)(q * 255), (int)(v * 255), (int)(p * 255));
                    break;
                case 2:
                    color.Color = Color.FromArgb(0, (int)(p * 255), (int)(v * 255), (int)(t * 255));
                    break;
                case 3:
                    color.Color = Color.FromArgb(0, (int)(p * 255), (int)(q * 255), (int)(v * 255));
                    break;
                case 4:
                    color.Color = Color.FromArgb(0, (int)(t * 255), (int)(p * 255), (int)(v * 255));
                    break;
                default:
                    color.Color = Color.FromArgb(0, (int)(v * 255), (int)(p * 255), (int)(q * 255));
                    break;
            }
            return color;
        }

        #endregion
    }
}
