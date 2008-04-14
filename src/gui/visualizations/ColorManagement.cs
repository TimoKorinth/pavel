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
using System.Drawing;
using Pavel.Framework;
using System.ComponentModel;

namespace Pavel.GUI.Visualizations {
    /// <summary>
    /// Manages the colors for the visualizations.
    /// </summary>
    public static class ColorManagement {
        /// <value> This event is fired, if color profile was changed./ </value>
        public static event ColorProfileChangedEvent ColorProfileChanged;

        public delegate void ColorProfileChangedEvent(object sender, EventArgs e);

        public class ColorProfile {
            #region Constructors

            public ColorProfile(String name) {
                this.name = name;
            }

            public ColorProfile(
                String name,
                ColorOGL backgroundColor,
                ColorOGL axesColor,
                ColorOGL descriptionColor,
                params ColorOGL[] selectionColors)
            {
                this.name = name;
                this.backgroundColor = backgroundColor;
                this.axesColor = axesColor;
                this.descriptionColor = descriptionColor;

                this.selectionColors = new List<ColorOGL>(selectionColors);
            }

            #endregion

            #region Fields

            internal String name;
            internal List<ColorOGL> selectionColors = new List<ColorOGL>(
                new ColorOGL[] {
                    new ColorOGL(Color.DarkTurquoise),
                    new ColorOGL(Color.Red),
                    new ColorOGL(Color.Yellow),
                    new ColorOGL(Color.Magenta),
                    new ColorOGL(Color.Lime),
                    new ColorOGL(Color.Orange),
                    new ColorOGL(Color.Blue),
                    new ColorOGL(Color.Lavender),
                    new ColorOGL(Color.Green),
                    new ColorOGL(Color.SkyBlue),
                    new ColorOGL(Color.Brown),
                    new ColorOGL(Color.Gold)
                });
            internal ColorOGL backgroundColor = new ColorOGL(Color.Black);
            internal ColorOGL axesColor = new ColorOGL(Color.LightGray);
            internal ColorOGL descriptionColor = new ColorOGL(Color.LightGray);

            #endregion

            #region Properties

            [ShowInProperties]
            [Category("Surroundings")]
            [DisplayName("Description Color")]
            [Description("Defines color of text in visualizations.")]
            /// <value>Gets or sets the color for descriptions</value>
            public ColorOGL DescriptionColor {
                get { return descriptionColor; }
                set {
                    descriptionColor = value;
                    if (null != ColorProfileChanged)
                        ColorProfileChanged(this, EventArgs.Empty);
                }
            }

            [ShowInProperties]
            [Category("Surroundings")]
            [DisplayName("Axes Color")]
            [Description("Defines color of axes.")]
            /// <value>Gets or sets the color for axes</value>
            public ColorOGL AxesColor {
                get { return axesColor; }
                set {
                    axesColor = value;
                    if (null != ColorProfileChanged)
                        ColorProfileChanged(this, EventArgs.Empty);
                }
            }

            [ShowInProperties]
            [Category("Surroundings")]
            [DisplayName("Background Color")]
            [Description("Defines color of background.")]
            /// <value>Gets or sets the color for the background</value>
            public ColorOGL BackgroundColor {
                get { return backgroundColor; }
                set {
                    backgroundColor = value;
                    if (null != ColorProfileChanged)
                        ColorProfileChanged(this, EventArgs.Empty);
                }
            }

            [ShowInProperties]
            [Category("Selections")]
            [DisplayName("Unselected Color")]
            [Description("Defines color of Points that are neither selected nor in any other existing selection.")]
            /// <value>Gets or sets the color for unselected points</value>
            public ColorOGL UnselectedColor {
                get { return GetColor(0); }
                set {
                    selectionColors[0] = value;
                    if (null != ColorProfileChanged)
                        ColorProfileChanged(this, EventArgs.Empty);
                }
            }

            [ShowInProperties]
            [Category("Selections")]
            [DisplayName("Current Selection Color")]
            [Description("Defines color of Points that are currently selected.")]
            /// <value>Gets or sets the color for currently selected points</value>
            public ColorOGL CurrentSelectionColor {
                get { return GetColor(1); }
                set {
                    selectionColors[1] = value;
                    if (null != ColorProfileChanged)
                        ColorProfileChanged(this, EventArgs.Empty);
                }
            }

            [ShowInProperties]
            [Category("Selections")]
            [DisplayName("Selection Colors")]
            [Description("Defines colors for selections.")]
            /// <value>Gets or sets the color for selections</value>
            public List<ColorOGL> SelectionColors {
                get { return selectionColors; }
                set {
                    selectionColors = value;
                    if (null != ColorProfileChanged)
                        ColorProfileChanged(this, EventArgs.Empty);
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Returns a color at a position.
            /// </summary>
            /// <param name="position">Position of the Color.</param>
            /// <returns>ColorOGL</returns>
            internal ColorOGL GetColor(int position) {
                if (position >= 0 && position < selectionColors.Count) {
                    return selectionColors[position];
                } else {
                    // The colorvalues are taken from HSV-Colorspace, starting with
                    // angels 0° and 180° and iterating by bisecting each partition:
                    // basis: Log_2 of position
                    float basis = (float)(Math.Ceiling(Math.Log((double)position, 2.0)));
                    // sections: 1, 2, 4, 8, 16, 32, ...
                    int sections = (int)Math.Pow(2.0, basis + 1);
                    int index = position - (int)Math.Pow(2.0, basis);
                    float h = (180f / (sections - 1)) + (360f / (sections)) * index * 2;
                    return HSVToRGB((180f + h) % 360f, 1f, 0.78f);
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

            public override String ToString() {
                return name;
            }

            #endregion
        }

        #region Constructor
        
        static ColorManagement() {
            colorProfiles = new List<ColorProfile>();
            colorProfiles.Add(new ColorProfile("Default"));
            ColorProfile profile = new ColorProfile(
                "Print",
                new ColorOGL(Color.White),
                new ColorOGL(Color.Black),
                new ColorOGL(Color.Black),
                new ColorOGL(Color.LightGray),
                new ColorOGL(Color.Black),
                new ColorOGL(Color.DarkGray),
                new ColorOGL(Color.Gray));
            colorProfiles.Add(profile);
            activeProfile = colorProfiles[0];
        }
        
        #endregion

        #region Fields

        private static ColorProfile activeProfile = null;
        private static List<ColorProfile> colorProfiles;
        
        #endregion

        #region Properties

        public static List<ColorProfile> ColorProfiles { get { return colorProfiles; } }

        public static ColorProfile ActiveProfile { get { return activeProfile; } }

        /// <value>Gets the color for descriptions</value>
        public static ColorOGL DescriptionColor { get { return activeProfile.descriptionColor; } }

        /// <value>Gets the color for axes</value>
        public static ColorOGL AxesColor { get { return activeProfile.axesColor; } }

        /// <value>Gets the color for the background</value>
        public static ColorOGL BackgroundColor { get { return activeProfile.backgroundColor; } }

        /// <value>Gets the color for unselected points</value>
        public static ColorOGL UnselectedColor {
            get { return activeProfile.GetColor(0); }
        }

        /// <value>Gets the color for currently selected points</value>
        public static ColorOGL CurrentSelectionColor {
            get { return activeProfile.GetColor(1); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a color at a position. If no color exists at this position
        /// the selectionColors list is expanded.
        /// </summary>
        /// <param name="position">Position of the Color.</param>
        /// <returns>ColorOGL</returns>
        public static ColorOGL GetColor(int position) {
            return activeProfile.GetColor(position);
        }

        /// <summary>
        /// Switches to ColorProfile with <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Index of ColorProfile</param>
        public static void SetActiveProfile(ColorProfile profile) {
            if (!colorProfiles.Contains(profile)) throw new ArgumentException("Nonexisting Color Profile made active!");
            activeProfile = profile;
            if (null != ColorProfileChanged)
                ColorProfileChanged(profile, EventArgs.Empty);
        }

        #endregion
    }
}
