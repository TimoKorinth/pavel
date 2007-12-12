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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using Tao.OpenGl;
using Pavel.GUI;
using Pavel.GUI.Visualizations;
using Pavel.Framework;
using Tao.FreeGlut;
using System.Collections.Generic;
using Pavel.Plugins;

namespace Pavel.Plugins {
    /// <summary>
    /// SingleSolutionControl: This shows a 3D-view of the single solution
    /// of a MouldTemperatureSolution. The workpiece has to be in STL, 
    /// the boreholes, are taken from the DrillingColumns in the MouldTemperatureUseCase.
    /// </summary>
    public class MouldTemperatureSolutionControl : OpenGLControl {

        #region Fields

        private float[] vertexArray;
        private float[] normalArray;
        private float[] baseNormals;
        private float[] colorArray;
        private float xRot, yRot, xDiff, yDiff;
        private float zoom = 0.6f;
        private bool mouseMoved;
        private double minValue = 0.0d;
        private double maxValue = 1.0d;
        private double absMinValue = 0.0d;
        private double absMaxValue = 1.0d;
        private Pavel.Framework.Point diffPoint;
        private int currentPointList;
        private int currentPointExtList;
        private int referencePointList;
        private int referencePointExtList;
        private MouldTemperatureSolution solution;

        private bool picking = false;
        private float[] pickingColors;
        private int cX, cY;
        private ToolTip pickingToolTip = new ToolTip();
        
        #endregion

        #region Properties

        /// <value>Gets the vertexArray</value>
        public float[] VertexArray { get { return vertexArray; } }

        /// <value>Gets the colorArray or sets it</value>
        public float[] ColorArray {
            get { return colorArray; }
            set { colorArray = value; }
        }

        /// <value>Gets the solution or sets it</value>
        public MouldTemperatureSolution Solution { get { return solution; } }

        /// <value>Gets the baseNormals or sets them</value>
        public float[] BaseNormals {
            get { return baseNormals; }
            set { baseNormals = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// The control visualizing the MillingOptimizationSolution.
        /// </summary>
        /// <param name="solution"> The matching solution </param>
        /// <param name="vertexArray"> Array of the vertices of the workpiece </param>
        /// <param name="normalArray"> Array of the normal vectors of the workpiece </param>
        public MouldTemperatureSolutionControl(MouldTemperatureSolution solution, float[] vertexArray, float[] normalArray) {
            this.vertexArray = vertexArray;
            this.solution = solution;
            this.normalArray = normalArray;
            this.baseNormals = new float[normalArray.Length];
            this.normalArray.CopyTo(baseNormals, 0);
            this.MouseWheel += this.MouseWheelEvent;
            this.MouseDown += this.MouseDownEvent;
            this.MouseMove += this.MouseMoveEvent;
            this.MouseClick += this.MouseClickEvent;
            this.solution.DrillRadiusChanged += OnDrillRadiusChanged;
            this.Dock = DockStyle.Fill;
            this.keepAspect = true;
            InitializePicking(vertexArray.Length);
            solution.GlyphControl.UpdateGlyphs(GetGlyphPoints());
            RefreshArrays();
            SetLight();
            CreateDrills(solution.CurrentPoint, currentPointList);
            CreateExtension(solution.CurrentPoint, currentPointExtList);

        }

        #endregion

        #region Methods

        #region Drawing

        // OpenGLControl Member
        protected override void SetupModelViewMatrixOperations() {
            Gl.glScalef(zoom / 150f, zoom / 150f, zoom / 150f);
            Gl.glRotatef(-this.xRot, 1.0f, 0.0f, 0.0f);
            Gl.glRotatef(this.yRot, 0.0f, 0.0f, 1.0f);
            Gl.glTranslatef(0, 0, -(2f * halfHeight));
        }

        /// <summary>
        /// Initialize the properties of OpenGL.
        /// </summary>
        protected override void InitOpenGL() {
            Gl.glClearColor(ColorManagement.BackgroundColor.R, ColorManagement.BackgroundColor.G, ColorManagement.BackgroundColor.B, 1.0f);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_NORMAL_ARRAY);
            Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);

            //Light settings
            float[] ambientLight = { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] diffuseLight = { 0.8f, 0.8f, 0.8f, 1.0f };
            float[] matSpecular = { 0.6f, 0.6f, 0.6f, 1f };
            float[] matShininess = { 40.0f };
            float[] lightPosition = { 0.0f, 400.0f, 20.0f, 1.0f };
            float[] lmodelAmbient = { 0.2f, 0.2f, 0.2f, 1.0f };
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, matSpecular);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, matShininess);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambientLight);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, diffuseLight);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPosition);
            Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, lmodelAmbient);
            currentPointList = Gl.glGenLists(1);
            currentPointExtList = Gl.glGenLists(1);
            referencePointList = Gl.glGenLists(1);
            referencePointExtList = Gl.glGenLists(1);
        }

        internal void SetLight() {
            if (solution.Light) {
                Gl.glEnable(Gl.GL_LIGHTING);
                Gl.glEnable(Gl.GL_LIGHT0);
            } else {
                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glDisable(Gl.GL_LIGHT0);
            }
        }

        protected override void RenderContent( ) {
            PushMatrices();
            this.DrawSTL();
            this.DrawGroundPlate();
            this.DrawDrills();
            this.DrawScale();
            PopMatrices();
        }

        /// <summary>
        /// Draws the colored workpiece.
        /// </summary>
        private void DrawSTL() {
            Gl.glLineWidth(1.0f);
            Gl.glVertexPointer(3, Gl.GL_FLOAT, 0, this.vertexArray);
            Gl.glNormalPointer(Gl.GL_FLOAT, 0, this.normalArray);
            if (picking) {
                Gl.glColorPointer(3, Gl.GL_FLOAT, 0, this.pickingColors);
            }
            else {
                Gl.glColorPointer(3, Gl.GL_FLOAT, 0, this.colorArray);
            }
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, this.vertexArray.Length / 3);
        }

        /// <summary>
        /// Draws a plate, on which the workpiece ist placed.
        /// </summary>
        private void DrawGroundPlate() {
            Gl.glColor3fv(new ColorOGL(Color.White).RGB);
            Gl.glPushMatrix();
            Gl.glScaled(1f, 1f, 0.01f);
            Glut.glutSolidCube(300f);
            Gl.glPopMatrix();
        }

        /// <summary>
        /// Draws the scale for the range of displayed simulation values.
        /// </summary>
        private void DrawScale() {
            this.PushMatrices();
            this.SetupProjectionFlat(true, false, false);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glDisable(Gl.GL_DEPTH_TEST);

            Gl.glColor3fv(new ColorOGL(Color.LightGray).RGB);
            if ( solution.AbsoluteTemperature ) {
                this.PrintText(0.027f, 0.24f, 0.0f, 0.024f, absMaxValue.ToString("F" + 3));
            }
            else this.PrintText(0.027f, 0.24f, 0.0f, 0.024f, maxValue.ToString("F" + 3));

            // draws two polygons for a nicer color gradient
            Gl.glBegin(Gl.GL_POLYGON);
            Gl.glColor3fv(new ColorOGL(Color.Red).RGB);
            Gl.glVertex2f(0.03f, 0.05f);
            Gl.glVertex2f(0.07f, 0.05f);
            Gl.glColor3fv(new ColorOGL(Color.Orange).RGB);
            Gl.glVertex2f(0.07f, 0.135f);
            Gl.glVertex2f(0.03f, 0.135f);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_POLYGON);
            Gl.glColor3fv(new ColorOGL(Color.Orange).RGB);
            Gl.glVertex2f(0.03f, 0.135f);
            Gl.glVertex2f(0.07f, 0.135f);
            Gl.glColor3fv(new ColorOGL(Color.Blue).RGB);
            Gl.glVertex2f(0.07f, 0.22f);
            Gl.glVertex2f(0.03f, 0.22f);
            Gl.glEnd();

            Gl.glColor3fv(new ColorOGL(Color.LightGray).RGB);
            if ( solution.AbsoluteTemperature ) {
                this.PrintText(0.027f, 0.017f, 0.0f, 0.024f, absMinValue.ToString("F" + 3));
            }
            else this.PrintText(0.027f, 0.017f, 0.0f, 0.024f, minValue.ToString("F" + 3));

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            
            this.PopMatrices();
        }

        /// <summary>
        /// Draws the drills.
        /// </summary>
        private void DrawDrills() {
            Gl.glEnable(Gl.GL_CULL_FACE);
            Gl.glCullFace(Gl.GL_FRONT);

            // Change the draw to prevent leaving a cavity in the drill with greater alpha
            if (solution.AlphaValue > 0.5f) {
                DrawDrillsComPoint();
                DrawDrillsRefPoint();
            } else {
                DrawDrillsRefPoint();
                DrawDrillsComPoint();
            }

            Gl.glDisable(Gl.GL_CULL_FACE);
        }

        /// <summary>
        /// Creates the drills for the current point.
        /// </summary>
        private void DrawDrillsComPoint() {
            if (solution.AlphaValue > 0.0f) {
                Gl.glColor4f(0.0f, 0.0f, 1.0f, solution.AlphaValue);
                Gl.glCallList(currentPointList);
                Gl.glColor4f(0.9f, 0.9f, 0.9f, solution.AlphaValue);
                Gl.glCallList(currentPointExtList);
            }
        }

        /// <summary>
        /// Creates the drills for the current reference point.
        /// </summary>
        private void DrawDrillsRefPoint() {
            if ((solution.SelectedMode == MouldTemperatureSolution.Mode.CompareToRef) && (null != solution.ReferencePoint)) {
                if (solution.AlphaValue < 1.0f) {
                    Gl.glColor4f(0.0f, 1.0f, 1.0f, 1.0f - solution.AlphaValue);
                    Gl.glCallList(referencePointList);
                    Gl.glColor4f(0.9f, 0.9f, 0.9f, 1.0f - solution.AlphaValue);
                    Gl.glCallList(referencePointExtList);
                }
            }
        }

        /// <summary>
        /// Indicates the direction of the borehole by extending the borehole-cylinder in the
        /// direction of the starting point of the borehole in a different color.
        /// </summary>
        /// <param name="point">The Point currently displayed</param>
        /// <param name="list">Number of the display list</param>
        private void CreateExtension(Pavel.Framework.Point point, int list) {
            List<int> visColumns =
                (Pavel.Framework.ProjectController.Project.UseCase as MouldTemperatureUseCase).DrillingColumns;

            double factor = 180 / Math.PI;
            double theta, phi, length;
            Gl.glNewList(list, Gl.GL_COMPILE);

            Glu.GLUquadric quadobj = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(quadobj, Glu.GLU_FILL);
            for (int i = 0; i < visColumns.Count - 6; i = i + 4) {

                double x = point.Values[visColumns[i + 4]] - point.Values[visColumns[i]];
                double y = point.Values[visColumns[i + 5]] - point.Values[visColumns[i + 1]];
                double z = point.Values[visColumns[i + 6]] - point.Values[visColumns[i + 2]];

                double s = Math.Sqrt(x * x + y * y);
                length = Math.Sqrt(x * x + y * y + z * z);
                theta = Math.Acos(z / length) * factor;
                if (x >= 0) {
                    phi = Math.Asin(y / s) * factor;
                } else {
                    phi = 180 - Math.Asin(y / s) * factor;
                }

                Gl.glPushMatrix();
                if (point.Values[visColumns[i + 3]] == 1) {
                    Gl.glTranslated(point.Values[visColumns[i]],
                        point.Values[visColumns[i + 1]],
                        point.Values[visColumns[i + 2]]);
                    Gl.glRotated(phi, 0.0, 0.0, 1.0);
                    Gl.glRotated(theta, 0.0, 1.0, 0.0);
                    Gl.glRotated(180, 1.0, 0.0, 0.0);
                } else {
                    Gl.glTranslated(point.Values[visColumns[i + 4]],
                        point.Values[visColumns[i + 5]],
                        point.Values[visColumns[i + 6]]);
                    Gl.glRotated(phi, 0.0, 0.0, 1.0);
                    Gl.glRotated(theta, 0.0, 1.0, 0.0);
                }
                Glu.gluCylinder(quadobj, solution.DrillRadius * 0.89d, solution.DrillRadius * 0.89d, 50, 10, 10);
                Gl.glTranslated(0.0, 0.0, 50.0);
                Glu.gluSphere(quadobj, solution.DrillRadius * 0.89d, 10, 10);
                Gl.glPopMatrix();
            }
            Glu.gluDeleteQuadric(quadobj);
            Gl.glEndList();
        }

        private void CreateDrills(Pavel.Framework.Point point, int list) {
            Gl.glNewList(list, Gl.GL_COMPILE);
            List<int> visColumns =
                (Pavel.Framework.ProjectController.Project.UseCase as MouldTemperatureUseCase).DrillingColumns;

            double factor = 180 / Math.PI;
            double theta, phi, length;

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Glu.GLUquadric quadobj = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(quadobj, Glu.GLU_FILL);
            for (int i = 0; i < visColumns.Count - 6; i = i + 4) {

                double x = point.Values[visColumns[i + 4]] - point.Values[visColumns[i]];
                double y = point.Values[visColumns[i + 5]] - point.Values[visColumns[i + 1]];
                double z = point.Values[visColumns[i + 6]] - point.Values[visColumns[i + 2]];

                double s = Math.Sqrt(x * x + y * y);
                length = Math.Sqrt(x * x + y * y + z * z);
                theta = Math.Acos(z / length) * factor;

                if (x >= 0) {
                    phi = Math.Asin(y / s) * factor;
                } else {
                    phi = 180 - Math.Asin(y / s) * factor;
                }

                Gl.glPushMatrix();

                Gl.glTranslated(point.Values[visColumns[i]], point.Values[visColumns[i + 1]], point.Values[visColumns[i + 2]]);
                Gl.glRotated(phi, 0.0, 0.0, 1.0);
                Gl.glRotated(theta, 0.0, 1.0, 0.0);
                Gl.glNormal3f(0.0f, 1.0f, 0.0f);
                Glu.gluSphere(quadobj, solution.DrillRadius, 10, 10);
                Glu.gluCylinder(quadobj, solution.DrillRadius, solution.DrillRadius, length, 10, 10);

                Gl.glPopMatrix();
            }

            Gl.glPushMatrix();

            Gl.glTranslated(point.Values[visColumns[visColumns.Count - 4]],
                point.Values[visColumns[visColumns.Count - 3]],
                point.Values[visColumns[visColumns.Count - 2]]);
            Glu.gluSphere(quadobj, solution.DrillRadius, 10, 10);

            Gl.glPopMatrix();

            Glu.gluDeleteQuadric(quadobj);
            Gl.glEndList();
        }

        #endregion

        #region Picking

        /// <summary>
        /// Initializes a color array with <paramref name="vertexCount"/> entries for the picking mode.
        /// </summary>
        /// <param name="vertexCount">Number of vertices of displayed object</param>
        private void InitializePicking(int vertexCount) {
            ColorOGL color;
            this.pickingColors = new float[vertexCount];

            for ( int i = 0; i < vertexCount / 9; i++ ) {
                color = GetRGBFromInt(i);
                pickingColors[i * 9] = pickingColors[i * 9 + 3] = pickingColors[i * 9 + 6] = color.R;
                pickingColors[i * 9 + 1] = pickingColors[i * 9 + 4] = pickingColors[i * 9 + 7] = color.G;
                pickingColors[i * 9 + 2] = pickingColors[i * 9 + 5] = pickingColors[i * 9 + 8] = color.B;
            }
        }

        /// <summary>
        /// Converts <param name="color"></param> to an integer value. 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private int GetIntFromRGB(ColorOGL color) {
            return (int)(Math.Ceiling((1.0f - color.B) * 255) +
                Math.Ceiling((1.0f - color.G) * 255) + Math.Ceiling((1.0f - color.R) * 255));
        }

        /// <summary>
        /// Converts <param name="index"></param> to a color.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private ColorOGL GetRGBFromInt(int index) {
            float r = (float)(255 - index % 256) / 255;
            float g = (float)(255 - (int)((index - 256 * 256 * (int)(index / (256 * 256))) / 256)) / 255;
            float b = (float)(255 - (int)(index / (256 * 256))) / 255;
            return new ColorOGL(Color.FromArgb(255, (int)(255 * r), (int)(255 * g), (int)(255 * b)));
        }

        /// <summary>
        /// Gets an array representing the area between (x1,y1) and (x2,y2).
        /// </summary>
        /// <param name="x1">x value of the first corner</param>
        /// <param name="y1">y value of the first corner</param>
        /// <param name="x2">x value of the second corner</param>
        /// <param name="y2">y value of the second corner</param>
        /// <returns></returns>
        private int[] GetPickingRectangle(int x1, int y1, int x2, int y2) {
            int[] rect = new int[4];

            if (x1 <= x2) { rect[0] = x1; } else { rect[0] = x2; }
            if (y1 <= y2) { rect[1] = y1; } else { rect[1] = y2; }
            rect[2] = Math.Abs(x1 - x2) + 1; //width
            rect[3] = Math.Abs(y1 - y2) + 1; //height

            return rect;
        }

        /// <summary>
        /// The picked color is translated to the index of the corresponding triangle.
        /// </summary>
        /// <param name="pickedColor">the picked color</param>
        /// <param name="max">number of existing vertices</param>
        /// <returns></returns>
        public List<int> UniqueSelection(float[] pickedColor, int max) {
            List<int> unique = new List<int>();
            int index;
            for (int i = 0; i < pickedColor.Length / 3; i++) {
                index = GetIntFromRGB(new ColorOGL(Color.FromArgb(255, (int)(255 * pickedColor[i * 3]), (int)(255 * pickedColor[i * 3 + 1]), (int)(255 * pickedColor[i * 3 + 2]))));
                if ((index < max) && (!unique.Contains(index))) {
                    unique.Add(index);
                }
            }
            return unique;
        }

        /// <summary>
        /// Picks the triangle at the coordinates given by <param name="rect"></param>.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private float[] PickTriangle(int[] rect) {
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_LIGHT0);
            this.picking = true;

            float[] color = new float[3 * rect[2] * rect[3]];

            RenderScene();
            Gl.glReadBuffer(Gl.GL_BACK);
            Gl.glReadPixels(rect[0], rect[1], rect[2], rect[3], Gl.GL_RGB, Gl.GL_FLOAT, color);

            this.SetLight();
            this.picking = false;

            return color;
        }

        #endregion

        /// <summary>
        /// Calculates the colorArray and the normal vectors for the vertices from those for the faces,
        /// depending on the value of solution.NicerRendering.
        /// Additionally the extrema (min and max) of the simulation values for the displayed points are calculated. 
        /// </summary>
        internal void RefreshArrays() {
            Pavel.Framework.Point pointToDisplay = null;
            switch (solution.SelectedMode) {
                case MouldTemperatureSolution.Mode.CompareToMany:
                case MouldTemperatureSolution.Mode.CompareToRef:
                    if (null != solution.ReferencePoint && solution.ReferencePoint != solution.CurrentPoint) {
                        diffPoint = CreateDiffPoint(solution.CurrentPoint, solution.ReferencePoint);
                        pointToDisplay = diffPoint;
                    } else {
                        pointToDisplay = solution.CurrentPoint;
                    }
                    break;
                case MouldTemperatureSolution.Mode.Zapping:
                    pointToDisplay = solution.CurrentPoint;
                    break;
            }
            if ( !solution.AbsoluteTemperature ) {
                CalculateExtrema(pointToDisplay);
                colorArray = this.CreateColorArray(solution.SmoothRendering, vertexArray, pointToDisplay, minValue, maxValue);
            } else {
                colorArray = this.CreateColorArray(solution.SmoothRendering, vertexArray, pointToDisplay, absMinValue, absMaxValue);
            }
            normalArray = DirtyLittleOpenGlHelper.ExpandNormalArray(solution.SmoothRendering, this.baseNormals, vertexArray);
        }

        /// <summary>
        /// Calculate the min- and max value with a given point
        /// </summary>
        /// <param name="point">Point to calculate min/maxValue</param>
        private void CalculateExtrema(Pavel.Framework.Point point) {
            maxValue = double.NegativeInfinity;
            minValue = double.PositiveInfinity;
            double[] simValues = (double[])point.Tag.Data;
            if ( simValues == null ) {
                minValue = 0;
                maxValue = 0;
            } else {
                for ( int i = 0; i < simValues.Length; i++ ) {
                    minValue = minValue > simValues[i] ? simValues[i] : minValue;
                    maxValue = maxValue < simValues[i] ? simValues[i] : maxValue;
                }
            }
        }

        /// <summary>
        /// Calculates the extrema of all Pavel.Framework.Points available for display.
        /// </summary>
        /// <param name="points">Array of all Pavel.Framework.Points available for display</param>
        public void CalculateAbsoluteExtrema( Pavel.Framework.Point[] points) {
            absMaxValue = 0.0d;
            absMinValue = double.PositiveInfinity;
            foreach ( Pavel.Framework.Point p in points ) {
                double[] simValues = (double[])p.Tag.Data;
                if (simValues == null) continue;

                for ( int i = 0; i < simValues.Length; i++ ) {
                    absMinValue = absMinValue > simValues[i] ? simValues[i] : absMinValue;
                    absMaxValue = absMaxValue < simValues[i] ? simValues[i] : absMaxValue;
                }
            }
        }

        /// <summary>
        /// Creates a new fake point which contains the differences / maxima of the given simulation values
        /// for each triangle in the data tag. This point can be used to create the color array for the comparison.
        /// </summary>
        /// <param name="curPoint">The currently selected point in the visualization.</param>
        /// <param name="refPoint">The reference point which is set.</param>
        /// <returns>A new fake point which contains the differences / maxima of the given simulation values
        /// for each triangle in the data tag</returns>
        private Pavel.Framework.Point CreateDiffPoint(Pavel.Framework.Point curPoint, Pavel.Framework.Point refPoint) {
            double[] cTag = (double[])curPoint.Tag.Data;
            double[] rTag = (double[])refPoint.Tag.Data;

            if (cTag.Length != rTag.Length) {
                throw new ApplicationException("Different count of simulation values - Calculation of compared values not possible");
            }
            double[] diffTag = new double[cTag.Length];
            maxValue = 0.0d;
            minValue = double.PositiveInfinity;

            for (int i = 0; i < diffTag.Length; i++) {
                switch (this.solution.SelectedComparisonMode) {
                    case MouldTemperatureSolution.ComparisonMode.Diff: {
                            diffTag[i] = ((1 - solution.AlphaValue) * rTag[i]) - (solution.AlphaValue * cTag[i]);
                            minValue = minValue > diffTag[i] ? diffTag[i] : minValue;
                            maxValue = maxValue < diffTag[i] ? diffTag[i] : maxValue;
                            break;
                        }
                    case MouldTemperatureSolution.ComparisonMode.AbsDiff: {
                            diffTag[i] = Math.Abs(((1 - solution.AlphaValue) * rTag[i]) - (solution.AlphaValue * cTag[i]));
                            minValue = minValue > diffTag[i] ? diffTag[i] : minValue;
                            maxValue = maxValue < diffTag[i] ? diffTag[i] : maxValue;
                            break;
                        }
                    case MouldTemperatureSolution.ComparisonMode.Max: {
                            diffTag[i] = Math.Max(rTag[i], cTag[i]);
                            minValue = minValue > diffTag[i] ? diffTag[i] : minValue;
                            maxValue = maxValue < diffTag[i] ? diffTag[i] : maxValue;
                            break;
                        }
                    case MouldTemperatureSolution.ComparisonMode.Add: {
                            diffTag[i] = ((1 - solution.AlphaValue) * rTag[i]) + (solution.AlphaValue * cTag[i]);
                            minValue = minValue > diffTag[i] ? diffTag[i] : minValue;
                            maxValue = maxValue < diffTag[i] ? diffTag[i] : maxValue;
                            break;
                        }
                    default: {
                            diffTag[i] = 0;
                            break;
                        }
                }
            }

            return new Pavel.Framework.Point(curPoint.ColumnSet, new DataTag(-1, diffTag), new double[curPoint.ColumnSet.Dimension]);
        }

        /// <summary>
        /// Replaces the current Pavel.Framework.Point
        /// </summary>
        public void ChangePoint() {
            CreateDrills(solution.CurrentPoint, currentPointList);
            CreateExtension(solution.CurrentPoint, currentPointExtList);
            if ((solution.SelectedMode == MouldTemperatureSolution.Mode.CompareToRef) && (null != solution.ReferencePoint)) {
                CreateDrills(solution.ReferencePoint, referencePointList);
                CreateExtension(solution.ReferencePoint, referencePointExtList);
            }

            solution.GlyphControl.UpdateGlyphs(GetGlyphPoints());
            this.RefreshArrays();
            this.Invalidate();
        }

        public List<Pavel.Framework.Point> GetGlyphPoints() {
            List<Pavel.Framework.Point> glyphPoints = new List<Pavel.Framework.Point>();

            switch (solution.SelectedMode) {
                case MouldTemperatureSolution.Mode.Zapping: {
                        glyphPoints.Add(solution.CurrentPoint);
                        break;
                    }
                case MouldTemperatureSolution.Mode.CompareToRef: {
                        glyphPoints.Add(solution.CurrentPoint);
                        if (solution.ReferencePoint != null) {
                            glyphPoints.Add(solution.ReferencePoint);
                        }
                        break;
                    }
                case MouldTemperatureSolution.Mode.CompareToMany: {
                        glyphPoints.Add(solution.CurrentPoint);
                        if (solution.ReferencePoint != null) {
                            glyphPoints.Add(solution.ReferencePoint);
                        }
                        break;
                    }

            }

            return glyphPoints;
        }

        #region CreateColorArray

        /// <summary>
        /// Creates the colorArray. Fills the array with colors for the temperature values according to a given scale.
        /// </summary>
        /// <param name="nicerRendering">True to interpolate the colors at the vertices correctly</param>
        /// <param name="vertexArray">Array of the vertices</param>
        /// <param name="p">The Pavel.Framework.Point to be displayed</param>
        /// <param name="min">The minimal temperature of the scale</param>
        /// <param name="max">The maximum temperature of the scale</param>
        /// <returns>The Colors for the temperature values</returns>
        private float[] CreateColorArray(bool nicerRendering, float[] vertexArray, Pavel.Framework.Point p, double min, double max) {
            float[] colorArray;
            if ( p.Tag.Data != null ) {
                ColorOGL[] colorTable = ColorOGL.InterpolationArray(Color.Red, Color.Orange, Color.Blue);
                double[] tag = (double[])p.Tag.Data;
                if ( nicerRendering ) {
                    colorArray = new float[vertexArray.Length * 3];
                    List<int> vertices = new List<int>();
                    for ( int i = 0; i < vertexArray.Length / 3; i++ ) { vertices.Add(i); }

                    // Look only once at every vertex
                    while ( vertices.Count > 0 ) {

                        // List of equivalent vertices
                        List<int> equi = new List<int>();
                        equi.Add(vertices[0]);
                        for ( int count = 1; count < vertices.Count; count++ ) {
                            if ( (vertexArray[vertices[0] * 3] == vertexArray[vertices[count] * 3]) &&
                                (vertexArray[vertices[0] * 3 + 1] == vertexArray[vertices[count] * 3 + 1]) &&
                                (vertexArray[vertices[0] * 3 + 2] == vertexArray[vertices[count] * 3 + 2]) ) {
                                equi.Add(vertices[count]);
                            }
                        }

                        float[] smoothColor = new float[3];
                        for ( int j = 0; j < equi.Count; j++ ) {
                            double scale = (tag[equi[j] / 3] - min) / (max - min);
                            int num = (int)(scale * (colorTable.Length - 1));
                            smoothColor[0] += colorTable[num].R;
                            smoothColor[1] += colorTable[num].G;
                            smoothColor[2] += colorTable[num].B;
                        }

                        for ( int k = 0; k < 3; k++ ) { smoothColor[k] = smoothColor[k] / equi.Count; }

                        for ( int l = 0; l < equi.Count; l++ ) {
                            colorArray[equi[l] * 3] = smoothColor[0];
                            colorArray[equi[l] * 3 + 1] = smoothColor[1];
                            colorArray[equi[l] * 3 + 2] = smoothColor[2];
                            vertices.Remove(equi[l]);
                        }
                    }
                } else { // not so nice Rendering
                    colorArray = new float[vertexArray.Length];
                    for ( int i = 0; i < colorArray.Length / 3; i++ ) {
                        double scale = (tag[i / 3] - min) / (max - min);
                        int num = (int)(scale * (colorTable.Length - 1));
                        colorArray[i * 3] = colorTable[num].R;
                        colorArray[i * 3 + 1] = colorTable[num].G;
                        colorArray[i * 3 + 2] = colorTable[num].B;
                    }
                }
            } else { // There is no Raw-Data
                colorArray = new float[vertexArray.Length * 3];
                for ( int i = 0; i < colorArray.Length; i += 3 ) {
                    colorArray[i] = ColorManagement.UnselectedColor.R;
                    colorArray[i + 1] = ColorManagement.UnselectedColor.G;
                    colorArray[i + 2] = ColorManagement.UnselectedColor.B;
                }
            }
            return colorArray;
        }
        #endregion

        #endregion

        #region Event Handling Stuff

        private void OnDrillRadiusChanged(object sender, EventArgs ev) {
            CreateDrills(solution.CurrentPoint, currentPointList);
            CreateExtension(solution.CurrentPoint, currentPointExtList);
            if ((solution.SelectedMode == MouldTemperatureSolution.Mode.CompareToRef) && (null != solution.ReferencePoint)) {
                CreateDrills(solution.ReferencePoint, referencePointList);
                CreateExtension(solution.ReferencePoint, referencePointExtList);
            }
            this.Invalidate();
        }

        /// <summary>
        /// When a button of the mouse is pressed, the values for the rotation are initialized.
        /// </summary>
        /// <param name="sender"> The mouse </param>
        /// <param name="ev"> Standard MouseEventArgs </param>
        private void MouseDownEvent(object sender, MouseEventArgs ev) {
            MakeCurrentContext();
            this.xDiff = ev.X - this.yRot;
            this.yDiff = ev.Y + this.xRot;
            mouseMoved = false;
        }

        /// <summary>
        /// When the mouse is moved with the left or the right mouse button pressed, a rotation will be conducted.
        /// </summary>
        /// <param name="sender"> The mouse </param>
        /// <param name="ev"> Standard MouseEventArgs </param>
        private void MouseMoveEvent(object sender, MouseEventArgs ev) {
            if ((ev.X - cX != 0) || (ev.Y - cY != 0)) {
                cX = ev.X;
                cY = ev.Y;
            }

            if (ev.Button == MouseButtons.Left | ev.Button == MouseButtons.Right) {
                this.yRot = ev.X - this.xDiff;
                this.xRot = -ev.Y + this.yDiff;
                this.Invalidate();
            }
            mouseMoved = true;
        }

        /// <summary>
        /// When the mouse wheel is moved, the view is zoomed in or out. 
        /// </summary>
        /// <param name="sender"> The mouse wheel </param>
        /// <param name="ev"> Standard MouseEventArgs </param>
        private void MouseWheelEvent(object sender, MouseEventArgs ev) {
            if ( zoom - ev.Delta / 1000f > 0.3f && zoom - ev.Delta / 1000f < 2f ) {
                this.zoom -= ev.Delta / 1000f;
            }
            this.Invalidate();
        }

        private void MouseClickEvent(object sender, MouseEventArgs ev) {
            if (mouseMoved || ev.Button!=MouseButtons.Left) return;           

            int[] pickingRectangle = this.GetPickingRectangle(cX, this.Height - cY, cX, this.Height - cY);
            float[] pickedColor = this.PickTriangle(pickingRectangle);

            List<int> uniqueL = UniqueSelection(pickedColor, baseNormals.Length / 3);
            if ( uniqueL.Count > 0 ) {
                double[] simValues;
                if ( (solution.SelectedMode == MouldTemperatureSolution.Mode.CompareToRef) && (diffPoint != null) ) {
                    simValues = (double[])diffPoint.Tag.Data;
                } else {
                    simValues = (double[])solution.CurrentPoint.Tag.Data;
                }
                System.Drawing.Point point = Cursor.Position;
                point = this.PointToClient(point);
                point.Y -= 20;
                pickingToolTip.Show(simValues[uniqueL[0]].ToString(), this, point, 2000);
            }
        }

        #endregion
    }
}