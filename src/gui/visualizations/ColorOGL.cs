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
using System.ComponentModel;
using System.Text;
using System.Drawing;

namespace Pavel.GUI.Visualizations {

    #region ColorOGL

    /// <summary>
    /// This class covers the System.Drawing.Color-struct.
    /// It caches the float value of the color, so it doesn't have to be calculated
    /// </summary>
    [Editor(typeof(ColorOGLEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter(typeof(ColorOGLConverter))]
    public class ColorOGL {

        #region Fields

        private Color color;
        private float[] rgbaFloat;
        private String description;

        #endregion

        #region Properties

        /// <value>Gets the alpha value of the ColorOGL as a float or sets it</value>
        [Browsable(false)]
        public float A {
            get { return rgbaFloat[3]; }
            private set { rgbaFloat[3] = value; color = colorFromFloat(rgbaFloat); }
        }

        /// <value>Gets the red value of the ColorOGL as a float or sets it</value>
        [Browsable(false)]
        public float R {
            get { return rgbaFloat[0]; }
            private set { rgbaFloat[0] = value; color = colorFromFloat(rgbaFloat); }
        }

        /// <value>Gets the green value of the ColorOGL as a float or sets it</value>
        [Browsable(false)]
        public float G {
            get { return rgbaFloat[1]; }
            private set { rgbaFloat[1] = value; color = colorFromFloat(rgbaFloat); }
        }

        /// <value>Gets the blue value of the ColorOGL as a float or sets it</value>
        [Browsable(false)]
        public float B {
            get { return rgbaFloat[2]; }
            private set { rgbaFloat[2] = value; color = colorFromFloat(rgbaFloat); }
        }

        /// <value>Gets a float-array of the RGB values of the ColorOGL</value>
        [Browsable(false)]
        public float[] RGB {
            get { return new float[] { rgbaFloat[0], rgbaFloat[1], rgbaFloat[2] }; }
        }

        /// <value>Gets a float-array of the RGBA values of the ColorOGL</value>
        [Browsable(false)]
        public float[] RGBA { get { return rgbaFloat; } }

        /// <summary>
        /// Gets a float-array of the RGB values of the ColorOGL, with an alpha value mixed in on-the-fly
        /// </summary>
        /// <param name="alpha">The desired alpha-value</param>
        /// <returns>A float-array of the RGB values of the ColorOGL, with an alpha value mixed in on-the-fly</returns>
        public float[] RGBwithA(float alpha) {
            float[] retval = new float[4];
            rgbaFloat.CopyTo(retval,0);
            retval[3] = alpha;
            return retval;
        }

        /// <value>Gets the ColorOGL as Color or sets it</value>
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        public Color Color {
            get { return color; }
            set {
                color = value;
                rgbaFloat = floatFromColor(color);
            }
        }

        /// <value>Gets a description of the ColorOGL or sets it</value>
        public String Description {
            get { return description; }
            set { description = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Sets the color to white and the description to "(none)".
        /// </summary>
        public ColorOGL() {
            Color = Color.White;
            Description = "(none)";
        }

        /// <summary>
        /// Sets the color to <paramref name="color"/> and the description to the Name of <paramref name="color"/>.
        /// </summary>
        /// <param name="color">Color to be set</param>
        public ColorOGL(Color color) {
            Color = color;
            Description = color.Name;
        }

        /// <summary>
        /// Sets the color to <paramref name="color"/> and the description to <paramref name="desc"/>.
        /// </summary>
        /// <param name="color">Color to be set</param>
        /// <param name="desc">Description for the color</param>
        public ColorOGL(Color color, String desc) {
            Color = color;
            Description = desc;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the float-values of the given Color.
        /// </summary>
        /// <param name="color">Given Color</param>
        /// <returns>Float-array of the transformed RGBA-values</returns>
        private static float[] floatFromColor(Color color) {
            float[] rgbaFloat = new float[4];
            rgbaFloat[0] = color.R / 255.0f;
            rgbaFloat[1] = color.G / 255.0f;
            rgbaFloat[2] = color.B / 255.0f;
            rgbaFloat[3] = color.A / 255.0f;
            return rgbaFloat;
        }

        /// <summary>
        /// Calculates the Color from the given float-values.
        /// </summary>
        /// <param name="colors">Float-array of RGBA-values</param>
        /// <returns>The transformed Color</returns>
        private static Color colorFromFloat(float[] colors) {
            byte r = (byte) (colors[0] * 255);
            byte g = (byte) (colors[1] * 255);
            byte b = (byte) (colors[2] * 255);
            byte a = (byte) (colors[3] * 255);
            return Color.FromArgb(a,r,g,b);
        }

        /// <summary>
        /// Returns the color converted to System.Drawing.Color.
        /// </summary>
        /// <returns>The color as System.Drawing.Color</returns>
        public Color ToColor() {
            return Color.FromArgb((int)(rgbaFloat[0] * 255), (int)(rgbaFloat[1] * 255), (int)(rgbaFloat[2] * 255));
        }

        /// <summary>
        /// Calculates the linear interpolation between two Colors,
        /// this color being dist=0, the other being dist=1
        /// </summary>
        /// <param name="other">Other Color</param>
        /// <param name="dist">Keep this between 0 and 1 please</param>
        /// <returns>Interpolated Color</returns>
        public ColorOGL Interpolate(ColorOGL other, float dist) {
            ColorOGL c = new ColorOGL();
            for (int i = 0; i < 4; i++)
                c.rgbaFloat[i] = this.rgbaFloat[i] + (other.rgbaFloat[i] - this.rgbaFloat[i]) * dist;
            c.color = colorFromFloat(c.rgbaFloat);
            return c;
        }

        /// <summary>
        /// Calculates a color table with values interpolated between
        /// <paramref name="first"/> and <paramref name="second"/>
        /// </summary>
        /// <param name="first">Start-ColorOGL</param>
        /// <param name="second">End-ColorOGL</param>
        /// <returns>An array of interpolated ColorOGLs</returns>
        public static ColorOGL[] InterpolationArray(ColorOGL first, ColorOGL second) {
            ColorOGL[] colorTable = new ColorOGL[byte.MaxValue + 1];
            for (int i = 0; i <= byte.MaxValue; i++) {
                colorTable[i] = first.Interpolate(second, (float)i / byte.MaxValue);
            }
            return colorTable;
        }

        /// <summary>
        /// Calculates a color table with values interpolated between
        /// <paramref name="first"/> and <paramref name="second"/>
        /// </summary>
        /// <param name="first">Start-System.Drawing.Color</param>
        /// <param name="second">End-System.Drawing.Color</param>
        /// <returns>An array of interpolated ColorOGLs</returns>
        public static ColorOGL[] InterpolationArray(Color first, Color second) {
            ColorOGL firstOGL  = new ColorOGL(first);
            ColorOGL secondOGL = new ColorOGL(second);
            ColorOGL[] colorTable = new ColorOGL[byte.MaxValue + 1];
            for (int i = 0; i <= byte.MaxValue; i++) {
                colorTable[i] = firstOGL.Interpolate(secondOGL, (float)i / byte.MaxValue);
            }
            return colorTable;
        }

        /// <summary>
        /// Calculates a color table with values interpolated between
        /// <paramref name="first"/>, <paramref name="second"/> and <paramref name="third"/>.
        /// </summary>
        /// <param name="first">First System.Drawing.Color</param>
        /// <param name="second">Second System.Drawing.Color</param>
        /// <param name="third">Third System.Drawing.Color</param>
        /// <returns>An array of interpolated ColorOGLs</returns>
        public static ColorOGL[] InterpolationArray(Color first, Color second, Color third) {
            ColorOGL firstOGL = new ColorOGL(first);
            ColorOGL secondOGL = new ColorOGL(second);
            ColorOGL thirdOGL = new ColorOGL(third);
            ColorOGL[] colorTable = new ColorOGL[short.MaxValue + 1];
            for (int i = 0; i <= short.MaxValue/2; i++) {
                colorTable[i] = firstOGL.Interpolate(secondOGL, (float)(i*2) / short.MaxValue);
            }
            for (int i = short.MaxValue / 2; i <= short.MaxValue ; i++) {
                colorTable[i] = secondOGL.Interpolate(thirdOGL, (float)((i*2-short.MaxValue*0.5) / short.MaxValue));
            }
            return colorTable;
        }

        /// <summary>
        /// Overrides the ToString method to return the description of this ColorOGL.
        /// </summary>
        /// <returns>Description of this ColorOGL</returns>
        public override string ToString() {
            return Description;
        }

        #endregion
    }

    #endregion

    #region ColorOGLConverter

    /// <summary>
    /// A converter for colors.
    /// </summary>
    class ColorOGLConverter : TypeConverter {

        /// <summary>
        /// Returns true, if a value can be converted to <paramref name="destinationType"/>.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="destinationType">The Type to be converted to</param>
        /// <returns>True, if a value can be converted to <paramref name="destinationType"/></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if ((destinationType == typeof(string)) && (destinationType == typeof(Color)) && (destinationType == typeof(ColorOGL))) {
                return true;
            } else {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <summary>
        /// Converts a color value to a given type.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="culture">The CultureInfo</param>
        /// <param name="value">The value to be converted</param>
        /// <param name="destinationType">The Type to be converted to</param>
        /// <returns>Converted value</returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)) {
                return ((ColorOGL)value).Description;
            } else if (destinationType == typeof(Color)) {
                return ((ColorOGL)value).Color;
            } else if (destinationType == typeof(ColorOGL)) {
                return (ColorOGL)value;
            } else {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        /// <summary>
        /// Returns true, if if a value can be converted from <paramref name="sourceType"/>.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="sourceType">The Type to be converted from</param>
        /// <returns>True, if if a value can be converted from <paramref name="sourceType"/></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if ((sourceType == typeof(Color)) || (sourceType == typeof(String)) || (sourceType == typeof(ColorOGL))) {
                return true;
            } else {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        /// <summary>
        /// Converts a color value from a given type.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="culture">The CultureInfo</param>
        /// <param name="value">The value to be converted</param>
        /// <returns>Converted value</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
            return base.ConvertFrom(context, culture, value);
        }

    }

    #endregion

    #region ColorOGLEditor

    /// <summary>
    /// An editor for ColorOGLs.
    /// </summary>
    class ColorOGLEditor : System.Drawing.Design.ColorEditor {

        /// <summary>
        /// Edits a given value.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="provider">The service provider</param>
        /// <param name="value">The value to be edited</param>
        /// <returns>The edited value</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            ColorOGL retV = new ColorOGL((Color)base.EditValue(context, provider, (value as ColorOGL).Color), (value as ColorOGL).Description);
            return retV;
        }

        /// <summary>
        /// Paints a value.
        /// </summary>
        /// <param name="e">Standard System.Drawing.Design.PaintValueEventArgs</param>
        public override void PaintValue(System.Drawing.Design.PaintValueEventArgs e) {
            if (e.Context != null) {
                System.Drawing.Design.PaintValueEventArgs pa = new System.Drawing.Design.PaintValueEventArgs(e.Context, (e.Value as ColorOGL).Color, e.Graphics, e.Bounds);
                base.PaintValue(pa);
            } else {
                base.PaintValue(e);
            }
        }

        /// <summary>
        /// Return true.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>true</returns>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
            //return base.GetPaintValueSupported(context);
            return true;
        }
    }

    #endregion
}
