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
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace STLNormalSwitcher {
    /// <summary>
    /// Class that assembles several methods to help the STLNormalSwitcher.
    /// </summary>
    public static class SwitchersHelpers {

        #region Methods

        /// <summary>
        /// Expands the given <paramref name="normalArray"/> for the facets
        /// to an array of normal vectors three times as big for the vertices.
        /// </summary>
        /// <param name="normalArray">Array of normal vectors for the facets</param>
        /// <returns>Expanded array of normal vectors for the vertices</returns>
        public static float[] ExpandNormalArray(float[] normalArray) {
            float x = new float();
            float y = new float();
            float z = new float();
            List<float> nA = new List<float>();

            for (int i = 0; i < normalArray.Length; i += 3) {
                x = normalArray[i];
                y = normalArray[i + 1];
                z = normalArray[i + 2];
                for (int j = 0; j < 3; j++) {
                    nA.Add(x);
                    nA.Add(y);
                    nA.Add(z);
                }
            }

            return nA.ToArray();
        }

        /// <summary>
        /// Normalizes the array of vertices given by <paramref name="vertexArray"/>.
        /// The displayed object is scaled and centered at the origin.
        /// </summary>
        /// <param name="vertexArray">Array of vertices</param>
        /// <param name="min">Minima in the three dimensions</param>
        /// <param name="scale">Factor to scale the object by</param>
        /// <returns>Normalized array of vertices</returns>
        public static float[] NormalizeVertexArray(float[] vertexArray, float[] min, float scale) {
            float[] temp = new float[vertexArray.Length];
            for (int i = 0; i < vertexArray.Length; i++) {
                temp[i] = scale * (((vertexArray[i] - min[i % 3]) / (scale)) - 0.5f);
            }
            return temp;
        }

        /// <summary>
        /// Switches all values in the given array.
        /// </summary>
        /// <param name="normalArray">Array of normal vectors</param>
        /// <returns>Array of negated normal vectors</returns>
        public static float[] SwitchAll(float[] normalArray) {
            float[] tmp = new float[normalArray.Length];
            for (int i = 0; i < normalArray.Length; i++) {
                tmp[i] = -normalArray[i];
            }
            return tmp;
        }

        /// <summary>
        /// Switches the normal vectors in <paramref name="normalArray"/> that are
        /// contained in the <paramref name="selection"/>.
        /// </summary>
        /// <param name="normalArray">Array of normal vectors</param>
        /// <param name="selection">List of indices of selected normal vectors</param>
        /// <returns>Array with the selected normal vectors negated</returns>
        public static float[] SwitchSelected(float[] normalArray, List<int> selection) {
            float[] tmp = new float[normalArray.Length];
            normalArray.CopyTo(tmp, 0);
            for (int i = 0; i < selection.Count; i++) {
                for (int j = 0; j < 3; j++) {
                    tmp[selection[i] * 3 + j] = -normalArray[selection[i] * 3 + j];
                }
            }
            return tmp;
        }

        /// <summary>
        /// Writes the data (normal vectors and vertices of the triangles) to the chosen file in ASCII-mode.
        /// </summary>
        /// <param name="filename">Path of the file to be saved</param>
        /// <param name="normalArray">The normal vectors</param>
        /// <param name="vertexArray">The vertices</param>
        public static void WriteToASCII(string filename, float[] normalArray, float[] vertexArray) {
            StreamWriter sw = new StreamWriter(filename);
            try {
                sw.WriteLine("solid ");
                for (int i = 0; i < normalArray.Length; i += 3) {
                    sw.WriteLine("  facet normal " + normalArray[i] + " " + normalArray[i + 1] + " " + normalArray[i + 2] + " ");
                    sw.WriteLine("    outer loop");
                    for (int j = 0; j < 3; j++) {
                        sw.WriteLine("      vertex " + vertexArray[(i + j) * 3] + " " + vertexArray[(i + j) * 3 + 1] + " " + vertexArray[(i + j) * 3 + 2] + " ");
                    }
                    sw.WriteLine("    endloop");
                    sw.WriteLine("  endfacet");
                }
                sw.WriteLine("endsolid ");
            } catch (Exception exception) {
                MessageBox.Show(exception.Message, "Error");
            } finally {
                sw.Close();
            }
        }

        /// <summary>
        /// Writes the data (normal vectors and vertices of the triangles) to the chosen file in binary-mode.
        /// </summary>
        /// <param name="filename">Path of the file to be saved</param>
        /// <param name="normalArray">The normal vectors</param>
        /// <param name="vertexArray">The vertices</param>
        public static void WriteToBinary(string filename, float[] normalArray, float[] vertexArray) {
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            try {
                byte abc = 0;
                byte[] headerArr = new byte[80];
                GetHeader().CopyTo(headerArr, 0);

                for (int c = 0; c < 80; c++) { bw.Write(headerArr[c]); }

                bw.Write((UInt32)(normalArray.Length / 3));

                for (int i = 0; i < normalArray.Length; i += 3) {

                    // Normal/vertices
                    for (int j = 0; j < 3; j++) {
                        // First one is the normal
                        bw.Write(normalArray[i + j]);
                    }

                    // Next three are vertices
                    for (int k = 0; k < 3; k++) {
                        bw.Write(vertexArray[(i + k) * 3]);
                        bw.Write(vertexArray[(i + k) * 3 + 1]);
                        bw.Write(vertexArray[(i + k) * 3 + 2]);
                    }

                    // Last two bytes are only to fill up to 50 bytes
                    bw.Write(abc);
                    bw.Write(abc);
                }

            } catch (Exception exception) {
                MessageBox.Show(exception.Message, "Error");
            } finally {
                fs.Close();
                bw.Close();
            }
        }

        /// <summary>
        /// Transforms a string to a byte-array.
        /// </summary>
        /// <returns>Header in a byte-array</returns>
        public static byte[] GetHeader() {
            String header = "This file was generated by the STLNormalSwitcher!";
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(header);
        }

        /// <summary>
        /// Converts an Integer into a unique float RGB color.
        /// </summary>
        /// <param name="index">Integer to be converted</param>
        /// <returns>RGB color</returns>
        public static RGB GetRGBFromInt(int index) {
            float r = (float)(255 - index % 256) / 255;
            float g = (float)(255 - (int)((index - 256 * 256 * (int)(index / (256 * 256))) / 256)) / 255;
            float b = (float)(255 - (int)(index / (256 * 256))) / 255;
            RGB rgb = new RGB(r, g, b);

            return rgb;
        }

        /// <summary>
        /// Converts a float RGB color into a unique Integer.
        /// </summary>
        /// <param name="rgb">RGB color</param>
        /// <returns>Corresponding Integer</returns>
        public static int GetIntFromRGB(RGB rgb) {
            int index;
            index = (int)(Math.Ceiling((1.0f - rgb.B) * 255) * 256 * 256 +
                Math.Ceiling((1.0f - rgb.G) * 255) * 256 + Math.Ceiling((1.0f - rgb.R) * 255));
            return index;
        }

        /// <summary>
        /// Takes a float-array of RGB-colors and returns a list of the corresponding triangles.
        /// Every picked triangle is added to the list only once.
        /// </summary>
        /// <param name="selected">Array of the picked triangles</param>
        /// <param name="colorDist">Distance between the colors</param>
        /// <param name="max">Maximum possible index</param>
        /// <returns>Unique list of selected triangles</returns>
        public static List<int> UniqueSelection(float[] selected, int colorDist, int max) {
            List<int> unique = new List<int>();
            int index;
            for (int i = 0; i < selected.Length / 3; i++) {
                RGB color = new RGB(selected[i * 3], selected[i * 3 + 1], selected[i * 3 + 2]);
                index = GetIntFromRGB(color) / colorDist;
                if ((index < max) && (!unique.Contains(index))) {
                    unique.Add(index);
                }
            }

            return unique;
        }

        /// <summary>
        /// Calculates the bottom left corner and the width and height of a rectangle
        /// given by two opposing corners (<paramref name="x1"/>, <paramref name="y1"/>)
        /// and (<paramref name="x2"/>, <paramref name="y2"/>).
        /// </summary>
        /// <param name="x1">X-coordinate of the first corner</param>
        /// <param name="y1">Y-coordinate of the first corner</param>
        /// <param name="x2">X-coordinate of the second corner</param>
        /// <param name="y2">Y-coordinate of the second corner</param>
        /// <returns>[0]: X-coordinate of bottom left corner
        /// [1]: Y-coordinate of bottom left corner
        /// [2]: Width of picking rectangle
        /// [3]: Height of picking rectangle</returns>
        public static int[] GetPickingRectangle(int x1, int y1, int x2, int y2) {
            int[] rect = new int[4];

            if (x1 <= x2) { rect[0] = x1; } else { rect[0] = x2; }
            if (y1 <= y2) { rect[1] = y1; } else { rect[1] = y2; }
            rect[2] = Math.Abs(x1 - x2) + 1; //width
            rect[3] = Math.Abs(y1 - y2) + 1; //height

            return rect;
        }

        #endregion
    }
}
