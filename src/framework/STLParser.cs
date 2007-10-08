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
using System.IO;
using System.Globalization;

namespace Pavel.Framework {
    /// <summary>
    /// A parser to parse standard stl-files.
    /// </summary>
    public class STLParser {

        #region Fields

        private float[] vertexArray;
        private float[] normalArray;

        #endregion

        #region Properties

        /// <value>Gets an array of vertices or sets it</value>
        public float[] VertexArray {
            get { return vertexArray; }
            set { vertexArray = value; }
        }

        /// <value>Gets an array of normal vectors for the faces or sets it</value>
        public float[] NormalArray {
            get { return normalArray; }
            set { normalArray = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parse the given Stream.
        /// </summary>
        /// <param name="file">Stream to parse</param>
        public void Parse(StreamReader file) {
            // First test, if the stream is ASCII or binary
            if (TestIfASCII(file)) {
                ParseASCII(file);
            } else {
                ParseBinary(file);
            }
        }

        /// <summary>
        /// Parses a binary STL file.
        /// </summary>
        /// <remarks>
        /// Because ASCII STL files can become very large, a binary version of STL exists. 
        /// A binary STL file has an 80 character header (which is generally ignored - but 
        /// which should never begin with 'solid' because that will lead most software to assume 
        /// that this is an ASCII STL file). Following the header is a 4 byte unsigned 
        /// integer indicating the number of triangular facets in the file. Following that 
        /// is data describing each triangle in turn. The file simply ends after the last triangle.
        /// Each triangle is described by twelve floating point numbers: three for the normal 
        /// and then three for the X/Y/Z coordinate of each vertex - just as with the ASCII version 
        /// of STL. After the twelve floats there is a two byte unsigned 'short' integer that 
        /// is the 'attribute byte count' - in the standard format, this should be zero because most 
        /// software does not understand anything else.( http://en.wikipedia.org/wiki/STL_(file_format) )
        /// </remarks>
        /// <param name="file">Stream to parse</param>
        private void ParseBinary(StreamReader file) {
            BinaryReader binReader = new BinaryReader(file.BaseStream);

            // Set stream back to null
            binReader.BaseStream.Position = 0;
            List<float> normalList = new List<float>();
            List<float> vertexList = new List<float>();
            char[] charBuf = new char[80];

            // The first 80 bytes are trash
            binReader.Read(charBuf, 0, 80);

            // Next 4 bytes contain the count of the normal/3D-vertexes record
            int count = (int)binReader.ReadUInt32();

            // Throw InvalidDataException if size does not fit
            // 84 Byte for header+count, count * (size of data = 50 Bytes)
            if (binReader.BaseStream.Length != (84+count*50)) {
                throw new InvalidDataException();
            }

            try {
                // Read the records
                for (int i = 0; i < count; i++) {

                    // Normal/vertexes
                    for (int j = 0; j < 3; j++) {
                        // First one is the normal
                        normalList.Add(binReader.ReadSingle());
                    }

                    // Next three are vertexes
                    for (int k = 0; k < 9; k++) {
                        vertexList.Add(binReader.ReadSingle());
                    }

                    // Last two bytes are only to fill up to 50 bytes
                    binReader.Read(charBuf, 0, 2);
                }

            } catch (Exception e) {
                if (PavelMain.LogBook != null) {
                    PavelMain.LogBook.Error("Error reading file! " + e.Message, true);
                }
            } finally {
                binReader.Close();
            }

            if (PavelMain.LogBook != null) {
                PavelMain.LogBook.Message("Reading STL-File\nNumber of Vertexes: " + vertexList.Count + "\nNumer of Normals: " + normalList.Count, false);
            }

            if ((vertexList.Count == 0) || (normalList.Count == 0)) {
                throw new InvalidDataException();
            } else {
                this.vertexArray = vertexList.ToArray();
                this.normalArray = normalList.ToArray();
            }

        }

        /// <summary>
        /// Parse STL-ASCII-Format.
        /// </summary>
        /// <param name="file">Stream to parse</param>
        private void ParseASCII(StreamReader file) {
            String input = null;
            int lineCount = 1;
            List<float> vertexList = new List<float>();
            List<float> normalList = new List<float>();

            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";

            file.BaseStream.Position = 0;
            StreamReader sr = new StreamReader(file.BaseStream);

            try {
                while ((input = sr.ReadLine()) != null) {
                    input = input.Trim();

                    // RemoveEmptyEntities remove empty entities, resulting from more than one whitespace
                    String[] v = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (v.Length > 0) {
                        if (v[0].ToLower() == "vertex") {
                            // Parse string, NumberStyles.Float secures that different formats can be parsed
                            // such as: "-2.23454e-001" (exponential format)
                            vertexList.Add(float.Parse(v[1], NumberStyles.Float, numberFormatInfo));
                            vertexList.Add(float.Parse(v[2], NumberStyles.Float, numberFormatInfo));
                            vertexList.Add(float.Parse(v[3], NumberStyles.Float, numberFormatInfo));
                        } else if (v[0].ToLower() == "facet") {
                            float x, y, z;
                            x = float.Parse(v[2], NumberStyles.Float, numberFormatInfo);
                            y = float.Parse(v[3], NumberStyles.Float, numberFormatInfo);
                            z = float.Parse(v[4], NumberStyles.Float, numberFormatInfo);

                            normalList.Add(x);
                            normalList.Add(y);
                            normalList.Add(z);
                        }
                    }
                    lineCount++;
                }
            } catch (Exception e) {
                if (PavelMain.LogBook != null) {
                    PavelMain.LogBook.Error("Error at line " + lineCount + ": " + e.Message + " Inputline: " + input, true);
                }
            } finally {
                sr.Close();
            }

            if (PavelMain.LogBook != null) {
                PavelMain.LogBook.Message("Reading STL-File\nNumber of Vertexes: " + vertexList.Count + "\nNumer of Normals: " + normalList.Count, false);
            }

            if ((vertexList.Count == 0) || (normalList.Count == 0)) {
                throw new InvalidDataException();
            } else {
                this.vertexArray = vertexList.ToArray();
                this.normalArray = normalList.ToArray();
            }
        }

        /// <summary>
        /// Test, if the file is ASCII. This is true, if the file contains "vertex" and "normal".
        /// </summary>
        /// <param name="file">file to be tested</param>
        /// <returns>true, if it is ASCII. Otherwise false.</returns>
        private bool TestIfASCII(StreamReader file) {
            bool foundVertex = false;
            bool foundNormal = false;
            String input = "";
            while ((input = file.ReadLine()) != null) {
                input = input.Trim();
                String[] v = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (0 != v.Length) {
                    if ("vertex" == v[0].ToLower()) {
                        foundVertex = true;
                    } else if ("facet" == v[0].ToLower()) {
                        foundNormal = true;
                    }
                }
                if (foundNormal && foundVertex) { return true; }
            }
            return (foundVertex && foundNormal);
        }

        #endregion
    }
}
