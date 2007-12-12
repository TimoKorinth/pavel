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
using Pavel.Framework;

namespace Pavel.GUI.Visualizations {
    /// <summary>
    /// Class that collects several helper methods needed for single solution visualizations.
    /// </summary>
    static public class DirtyLittleOpenGlHelper {

        #region Methods

        /// <summary>
        /// Calculate array for the normals. Every point from the vertex array needs a normal.
        /// </summary>
        /// <param name="nicerRendering">True to interpolate the colors at the normal vectors correctly</param>
        /// <param name="normArray">Single normal array</param>
        /// <param name="vertexArray">Array of the vertices</param>
        /// <returns>Array cointaining tripled normals</returns>
        public static float[] ExpandNormalArray(bool nicerRendering, float[] normArray, float[] vertexArray) {
            float[] tmpNormalArray = new float[vertexArray.Length];
            if (nicerRendering) {
                Vertex[] vertices = new Vertex[vertexArray.Length / 3];
                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i] = new Vertex(vertexArray[i * 3], vertexArray[i * 3 + 1], vertexArray[i * 3 + 2]);
                }

                //Create a map to find the normals to the vertex after sorting
                int[] map = new int[vertices.Length];
                for (int i = 0; i < map.Length - 1; i++) { map[i] = i; }

                Array.Sort(vertices, map);

                int pos = 0;

                int start = 0; //interval-start
                int end = 0; // interval-end

                bool hard;

                //This loops find any interval with equal vertices and will recalculate their normalvectors
                while (pos < vertices.Length) {

                    hard = false;

                    if (!vertices[pos].Equals(vertices[start])) { //End of interval reached->start normal-calculation
                        // The normal vector for a vertex is the sum of the normal vectors of the adjacent faces.
                        double x = 0, y = 0, z = 0;
                        for (int j = start; j <= end; j++) {
                            int tmp = (map[j] / 3) * 3;
                            if (j + 1 <= end) {
                                int tmp2 = (map[j + 1] / 3) * 3;
                                //if (hard = HardAngle(new float[3] { normArray[tmp], normArray[tmp + 1], normArray[tmp + 2] },
                                //    new float[3] { normArray[tmp2], normArray[tmp2 + 1], normArray[tmp2 + 2] })) {
                                //    break;
                                //}
                            }
                            x += normArray[tmp];
                            y += normArray[tmp + 1];
                            z += normArray[tmp + 2];
                        }

                        if (!hard) {
                            // The normal vector is normalized. Every element is divided by the length of the normal vector (squareroot of the sum of the squares of the elements).
                            double div = Math.Sqrt(x * x + y * y + z * z);
                            x = x / div;
                            y = y / div;
                            z = z / div;

                            // Set the normal vector for all the equivalent vertices.
                            // Remove the equivalent vertices from the list, so they are not looked at again.
                            for (int j = start; j <= end; j++) {
                                tmpNormalArray[map[j] * 3] = (float)x;
                                tmpNormalArray[map[j] * 3 + 1] = (float)y;
                                tmpNormalArray[map[j] * 3 + 2] = (float)z;
                            }
                        } else {
                            for (int j = start; j <= end; j++) {
                                int tmp = (map[j] / 3);
                                tmpNormalArray[map[j] * 3] = normArray[tmp];
                                tmpNormalArray[map[j] * 3 + 1] = normArray[tmp + 1];
                                tmpNormalArray[map[j] * 3 + 2] = normArray[tmp + 2];
                            }
                        }

                        start = pos;
                        end = pos;
                    } else {
                        end++;
                    }
                    pos++;
                }
            } else {
                float x = new float();
                float y = new float();
                float z = new float();
                List<float> nA = new List<float>();

                for (int i = 0; i < normArray.Length; i += 3) {
                    x = normArray[i];
                    y = normArray[i + 1];
                    z = normArray[i + 2];
                    for (int j = 0; j < 3; j++) {
                        nA.Add(x);
                        nA.Add(y);
                        nA.Add(z);
                    }
                }

                tmpNormalArray = nA.ToArray();
            }
            return tmpNormalArray;
        }

        /// <summary>
        /// Calculates the angle between two vectors.
        /// </summary>
        /// <param name="a">First vector as float-array of length 3</param>
        /// <param name="b">Second vector as float-array of length 3</param>
        /// <returns>True if the calculated angle is bigger than 45 degrees</returns>
        private static bool HardAngle(float[] a, float[] b) {
            if ((180 / Math.PI) *
                Math.Acos((a[0] * b[0] + a[1] * b[1] + a[2] * b[2]) / (Length(a) * Length(b))) > 45) {
                return true;
            } else { return false; }
        }

        /// <summary>
        /// Calculates the length of a vector.
        /// </summary>
        /// <param name="vector">Vector as float-array</param>
        /// <returns>Length of the given vector</returns>
        private static double Length(float[] vector) {
            double length = 0;
            for (int i = 0; i < vector.Length; i++) { length += vector[i] * vector[i]; }
            return Math.Sqrt(length);
        }

        #endregion

        #region Vertex-Class
        /// <summary>
        /// Inner Class Vertex. A specialised Container for a vertex of 3 points
        /// implements a comparable and an equal-method
        /// </summary>
        private class Vertex : IComparable<Vertex>, IEquatable<Vertex> {
            private float x1, x2, x3;
            public Vertex(float x1, float x2, float x3) {
                this.x1 = x1;
                this.x2 = x2;
                this.x3 = x3;
            }

            public int CompareTo(Vertex v) {
                if (this.x1 < v.x1) return -1;
                else if (this.x1 > v.x1) return 1;
                if (this.x2 < v.x2) return -1;
                else if (this.x2 > v.x2) return 1;
                if (this.x3 < v.x3) return -1;
                else if (this.x3 > v.x3) return 1;
                return 0;
            }

            public bool Equals(Vertex other) {
                if (this.CompareTo(other) != 0) return false;
                return true;
            }
        }
        #endregion
    }
}
