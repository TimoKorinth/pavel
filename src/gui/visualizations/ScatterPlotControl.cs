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
using Tao.OpenGl;
using System.Windows.Forms;
using Pavel.Framework;
using Tao.FreeGlut;

namespace Pavel.GUI.Visualizations {
    /// <summary>
    /// Visualization of the ScatterPlot
    /// </summary>
    public class ScatterPlotControl : OpenGLControl {

        #region Fields
        /// <summary>
        /// A reference to the Visualization containing this control
        /// </summary>
        private ScatterPlot vis;

        /// <summary>
        /// An instance of the FlipAxis Context Menu
        /// </summary>
        private FlipAxisMenu flipAxisMenu;

        /// <summary>
        /// Stores the actual displayed pointSet, used for determining changes of
        /// PointSet in the propertyEditor
        /// </summary>
        private PointSet visualizedPointSet = null;

        #region View Parameters
        /// <summary>
        /// The rotational angle in left-right direction.
        /// </summary>
        private float   lrAngle;
        /// <summary>
        /// The rotational angle in up-down direction.
        /// </summary>
        private float   udAngle;
        /// <summary>
        /// The zoom value which is subtracted from the width and height of the glOrtho clipping Area
        /// </summary>
        private float   zoom;
        /// <summary>
        /// During Animations this is used to hold the ACTUAL current rotation
        /// for each step, while lrAngle is the goal of the animation.
        /// </summary>
        private float   lrAngleCurrent;
        /// <summary>
        /// During Animations this is used to hold the ACTUAL current rotation
        /// for each step, while udAngle is the goal of the animation.
        /// </summary>
        private float   udAngleCurrent;
        /// <summary>
        /// Used in the mouseMove Event
        /// </summary>
        private float   lrAngleTemp;
        /// <summary>
        /// Used in the mouseMove Event
        /// </summary>
        private float   udAngleTemp;
        /// <summary>
        /// During Animations this is used to hold the ACTUAL current zoom
        /// for each step, while zoom is the goal of the animation.
        /// </summary>
        private float   zoomCurrent;
        /// <summary>
        /// Scale Factors for each axis 
        /// </summary>
        private VectorF scale;
        /// <summary>
        /// Translation offsets for each axis, applied BEFORE scaling
        /// </summary>
        private VectorF trans;
        /// <summary>
        /// During Animations this is used to hold the ACTUAL current scale factor
        /// for each step, while scale is the goal of the animation.
        /// </summary>
        private VectorF scaleCurrent;
        /// <summary>
        /// During Animations this is used to hold the ACTUAL current translation
        /// for each step, while trans is the goal of the animation.
        /// </summary>
        private VectorF transCurrent;
        /// <summary>
        /// Animation steps
        /// </summary>
        private const int steps = 30;
        #endregion
        
        /// <summary>
        /// Array of point coordinates to draw
        /// </summary>
        private float[] vertexArray;
        /// <summary>
        /// Array of colors for the drawn points.
        /// Is manipulated when some points change color (in a selection for example)
        /// </summary>
        private float[] colorArray;
        /// <summary>
        /// See, colorArray. This one stores the base colors, as a means to restore colors after
        /// points have been deselected
        /// </summary>
        private float[] colorArrayBase;
        /// <summary>
        /// Stores the position at which the mousebutton was held down
        /// </summary>
        private Vector mouseDragStartPoint;

        #endregion

        #region Properties
        public Visualization Visualization {
            get { return vis; }
        }

        public float LRAngleCurrent {
            get { return lrAngleCurrent;        }
            set { 
                lrAngleCurrent = value % 360;
            }
        }

        public float UDAngleCurrent {
            get { return udAngleCurrent;        }
            set {
                float newVal = value % 360;

                if      (newVal >  270) udAngleCurrent =  270;
                else if (newVal >   90) udAngleCurrent =   90;
                else if (newVal < -270) udAngleCurrent = -270;
                else if (newVal <  -90) udAngleCurrent =  -90;
                else                    udAngleCurrent = newVal;
            }
        }

        public float ZoomCurrent {
            get { return zoomCurrent; } //TODO: Currently unused, See ticket #257
            set {
                zoomCurrent = value >= halfHeight ? halfHeight-0.00001f : value;
            }
        }

        #endregion

        #region Constructor
        public ScatterPlotControl(ScatterPlot vis)
            : base() {
            this.vis = vis;
            this.visualizedPointSet = vis.VisualizationWindow.PointSet;
            ResetViewParameters(true, true);
            StoreViewParametersInCurrent(true, true);
            this.Dock = DockStyle.Fill;
            this.MouseDown  += this.MouseDownEventHandler;
            this.MouseMove  += this.MouseMoveEventHandler;
            this.MouseUp    += this.MouseUpEventHandler;
            this.MouseWheel += this.MouseWheelEventHandler;
            this.CreateArrays();
            this.ColorizeSelection();
            this.flipAxisMenu = new FlipAxisMenu(this);
        }
        #endregion

        #region Setup Functions
        /// <summary>
        /// This creates a vertex array and a color array, based on the visualization window's displayed PointSet.
        /// </summary>
        private void CreateArrays() {
            PointSet ps = vis.VisualizationWindow.DisplayedPointSet;
            Space space = vis.VisualizationWindow.Space;

            float alphaPoints = vis.AlphaPoints;
            int iX = Array.IndexOf(space.ColumnProperties, vis.AxisX);
            int iY = Array.IndexOf(space.ColumnProperties, vis.AxisY);
            int iZ = Array.IndexOf(space.ColumnProperties, vis.AxisZ);
            int iC = Array.IndexOf(space.ColumnProperties, vis.AxisC);
            bool mode3D = (iZ >= 0);

            this.vertexArray = new float[ps.Length * 3];
            this.colorArray  = new float[ps.Length * 4];
            int colorIndex;
            PavelMain.MainWindow.StatusBar.StartProgressBar(0, ps.Length, "Create Vertex Array...");
            int pointIndex = 0;
            foreach (PointList pl in ps.PointLists) {
                int[] map = space.CalculateMap(pl.ColumnSet);
                for (int i = 0; i < pl.Count; i++) {
                    if (pointIndex % 100 == 0) PavelMain.MainWindow.StatusBar.IncrementProgressBar(100);
                    vertexArray[pointIndex * 3 + 0] = (float)pl[i][map[iX]];
                    vertexArray[pointIndex * 3 + 1] = (float)pl[i][map[iY]];
                    if (mode3D) { vertexArray[pointIndex * 3 + 2] = (float)pl[i][map[iZ]]; } else { vertexArray[pointIndex * 3 + 2] = 0f; }
                    if ( this.StereoMode ) {
                        colorArray[pointIndex * 4 + 0] = 1;
                        colorArray[pointIndex * 4 + 1] = 1;
                        colorArray[pointIndex * 4 + 2] = 1;
                    } else if (vis.AxisC != null) {
                        colorIndex = (int)(pl[i].ScaledValue(map[iC], vis.AxisC) * 255);
                        colorArray[pointIndex * 4 + 0] = vis.ColorTable[colorIndex].R;
                        colorArray[pointIndex * 4 + 1] = vis.ColorTable[colorIndex].G;
                        colorArray[pointIndex * 4 + 2] = vis.ColorTable[colorIndex].B;
                    } else {
                        colorArray[pointIndex * 4 + 0] = ColorManagement.UnselectedColor.R;
                        colorArray[pointIndex * 4 + 1] = ColorManagement.UnselectedColor.G;
                        colorArray[pointIndex * 4 + 2] = ColorManagement.UnselectedColor.B;
                    }
                    colorArray[pointIndex * 4 + 3] = alphaPoints;
                    pointIndex++;
                }
            }
            PavelMain.MainWindow.StatusBar.EndProgressBar();
            this.colorArrayBase = (float[])this.colorArray.Clone();
        }

        /// <summary>
        /// Set up the modelview Matrix according to the current rotation
        /// </summary>
        protected override void SetupModelViewMatrixOperations() {
            Gl.glRotatef( (udAngleCurrent), 1.0f, 0.0f, 0.0f);
            Gl.glRotatef(-(lrAngleCurrent), 0.0f, 1.0f, 0.0f);
            //Shift the OGL Coordinate System, so that 0.5, 0.5, 0.5 is the center of rotation
            Gl.glTranslatef(-0.5f, -0.5f, -0.5f);
        }

        /// <summary>
        /// Multiplies the current modelview matrix with a linear transformation corresponding
        /// to transCurrent and scaleCurrent.
        /// </summary>
        /// <remarks>
        /// transCurrent and scaleCurrent are based on the ColumnSet's ColumnProperties, so
        /// point coordinates can be drawn unmodified and are mapped to the unit cube by this
        /// transformation.
        /// Does not change current matrix mode.
        /// </remarks>
        private void TranslateAndScalePoints() {
            Gl.glPushAttrib(Gl.GL_TRANSFORM_BIT);

            //Scale Points to the Normcube
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glScalef(scaleCurrent.X,
                        scaleCurrent.Y,
                        scaleCurrent.Z);
            Gl.glTranslatef(transCurrent.X, transCurrent.Y, transCurrent.Z);

            Gl.glPopAttrib();
        }

        /// <summary>
        /// Change the Color Array according to the current selection
        /// </summary>
        private void ColorizeSelection() {
            for (int i = 0; i < this.colorArray.Length / 4; i++) {
                Point point = vis.VisualizationWindow.DisplayedPointSet[i];
                this.colorArray[i * 4 + 0] = this.colorArrayBase[i * 4];
                this.colorArray[i * 4 + 1] = this.colorArrayBase[i * 4 + 1];
                this.colorArray[i * 4 + 2] = this.colorArrayBase[i * 4 + 2];
                int colorIndex = 2;
                foreach (Selection s in ProjectController.Selections) {
                    if (s.Active) {
                        if (s.Contains(point)) {
                            this.colorArray[i * 4 + 0] = ColorManagement.GetColor(colorIndex).R;
                            this.colorArray[i * 4 + 1] = ColorManagement.GetColor(colorIndex).G;
                            this.colorArray[i * 4 + 2] = ColorManagement.GetColor(colorIndex).B;
                        }
                    }
                    colorIndex++;
                }
                if (ProjectController.CurrentSelection.Contains(point)) {
                    this.colorArray[i * 4 + 0] = ColorManagement.CurrentSelectionColor.R;
                    this.colorArray[i * 4 + 1] = ColorManagement.CurrentSelectionColor.G;
                    this.colorArray[i * 4 + 2] = ColorManagement.CurrentSelectionColor.B;
                }
            }
        }

        /// <summary>
        /// Some OpenGL initialization and global settings
        /// </summary>
        protected override void InitOpenGL() {
            ColorOGL backColor = ColorManagement.BackgroundColor;
            Gl.glClearColor(backColor.R, backColor.G, backColor.B, 0.0f);
            Gl.glShadeModel(Gl.GL_FLAT);
            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);
        }

        #endregion

        #region Scaling
        /// <summary>
        /// Sets trans and scale according to the space's Columnproperties
        /// </summary>
        private void ResetDefaultScaleTrans() {
            this.scale.X = (float)(1 / (vis.AxisX.Max - vis.AxisX.Min));
            this.trans.X = -(float)vis.AxisX.Min;

            this.scale.Y = (float)(1 / (vis.AxisY.Max - vis.AxisY.Min));
            this.trans.Y = -(float)vis.AxisY.Min;
            
            if (null != vis.AxisZ) {
                this.scale.Z = (float)(1 / (vis.AxisZ.Max - vis.AxisZ.Min));
                this.trans.Z = -(float)vis.AxisZ.Min;
            } else {
                this.scale.Z = 1;
                this.trans.Z = 0;
            }
        }

        /// <summary>
        /// Converts a Point's values to OpenGL unit coordinates.
        /// </summary>
        /// <param name="pointValues">Values of a point</param>
        private VectorF ScaleToOpenGL(VectorF pointValues) {
            return (pointValues + transCurrent) * scaleCurrent;
        }

        /// <summary>
        /// Converts OpenGL unit coordinates to point values.
        /// </summary>
        /// <param name="coord">OpenGL unit coordinates</param>
        private VectorF ScaleToPoint(VectorF coord) {
            return coord / scaleCurrent - transCurrent;
        }

        #endregion

        #region View Parameter Handling

        /// <summary>
        /// Sets the view parameters to clean, initial values, derived from the current Mode2D
        /// and ColumnProperties
        /// </summary>
        /// <param name="scaleTrans">Set to true if you want to reset scaling and transformation</param>
        /// <param name="zoomRotation">Set to true if you want to reset zoom and rotation</param>
        protected void ResetViewParameters(bool scaleTrans, bool zoomRotation) {
            if (scaleTrans) ResetDefaultScaleTrans();
            if (zoomRotation) {
                if (!vis.Mode2D) {
                    zoom = 0f;
                    lrAngle = 20;
                    udAngle = 20;
                } else {
                    zoom = 0.2f;
                    lrAngle = 0;
                    udAngle = 0;
                }
            }
        }

        /// <summary>
        /// Resets the current View Parameters to the Default ones.
        /// </summary>
        /// <param name="scaleTrans">Reset Scale and Translation</param>
        /// <param name="zoomRotation">Reset Zoom and rotation</param>
        protected void StoreViewParametersInCurrent(bool scaleTrans, bool zoomRotation) {
            if (scaleTrans) {
                scaleCurrent = scale;
                transCurrent = trans;
            }
            if (zoomRotation) {
                ZoomCurrent = zoom;          //TODO Properties fuer Default statt current, "Default" weglassen
                LRAngleCurrent = lrAngle;
                UDAngleCurrent = udAngle;
            }
        }

        /// <summary>
        /// Resets all current View Parameters to the default ones in an animated process.
        /// </summary>
        /// <param name="animateScaleTrans">Reset Scale and Translation</param>
        /// <param name="animateZoomRotation">Reset Zoom and rotation</param>
        protected void ResetCurrentViewParameters(bool animateScaleTrans, bool animateZoomRotation) {
            StoreViewParametersInCurrent(!animateScaleTrans, !animateZoomRotation);
            if (animateScaleTrans || animateZoomRotation) {
                VectorF scaleStartInv   = VectorF.Unit / scaleCurrent;
                VectorF scaleDefaultInv = VectorF.Unit / scale;
                VectorF transStart  = transCurrent;
                float lrStart       = lrAngleCurrent;
                float udStart       = udAngleCurrent;
                float zoomStart     = zoomCurrent;
                float start, end;
                for (int i = 0; i < steps; i++) {
                    end   = (float)i / steps;
                    start = 1f - end;
                    if (animateZoomRotation) {
                        lrAngleCurrent = lrStart     * start + lrAngle * end;
                        udAngleCurrent = udStart     * start + udAngle * end;
                        zoomCurrent    = zoomStart   * start + zoom    * end;
                    }
                    if (animateScaleTrans) {
                        scaleCurrent   = VectorF.Unit /(scaleStartInv  * start + scaleDefaultInv  * end);
                        transCurrent   = transStart  * start + trans   * end;
                    }
                    Refresh();
                    System.Threading.Thread.Sleep(30);
                }
                StoreViewParametersInCurrent(animateScaleTrans, animateZoomRotation);
            }
            Refresh();
        }

        #endregion

        #region Drawing

        protected override void RenderContent( ) {
            DrawAxes();
            DrawPoints();
            DrawCube();
            DrawPickingRectangle();
            if ( vis.ShowLegend ) { DrawLegend(); }
        }

        /// <summary>
        /// Renders a cube surrounding the plotting area
        /// </summary>
        private void DrawCube() {
            if (vis.ShowCube) {
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glTranslatef(0.5f, 0.5f, 0.5f);
                Gl.glColor4f(0.5f, 0.5f, 0.5f, 0.2f);
                Glut.glutWireCube(1f);
                Gl.glTranslatef(-0.5f, -0.5f, -0.5f);
            }
        }

        /// <summary>
        /// Draws a gradient rectangle for the 4th Dimension in the lower left corner of the control
        /// </summary>
        private void DrawLegend() {
            if ( vis.AxisC == null ) { return; }

            PushMatrices();
            SetupProjectionFlat(true, true, false);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glBegin(Gl.GL_QUADS);
                Gl.glColor4f(vis.ColorTable[0].R, vis.ColorTable[0].G, vis.ColorTable[0].B, vis.AlphaPoints);
                Gl.glVertex2i(10, 10);
                Gl.glColor4f(vis.ColorTable[0].R, vis.ColorTable[0].G, vis.ColorTable[0].B, vis.AlphaPoints);
                Gl.glVertex2i(40, 10);
                Gl.glColor4f(vis.ColorTable[255].R, vis.ColorTable[255].G, vis.ColorTable[255].B, vis.AlphaPoints);
                Gl.glVertex2i(40, 160);
                Gl.glColor4f(vis.ColorTable[255].R, vis.ColorTable[255].G, vis.ColorTable[255].B, vis.AlphaPoints);
                Gl.glVertex2i(10, 160);
            Gl.glEnd();
            Gl.glShadeModel(Gl.GL_FLAT);

            Gl.glColor3f(0.5f, 0.5f, 0.5f);
            Gl.glRasterPos2i(45, 5);
            Glut.glutBitmapString(Glut.GLUT_BITMAP_HELVETICA_10, vis.AxisC.Label);
            double pos;
            double cMin = vis.AxisC.Min;
            double cStep = (vis.AxisC.Max - cMin) / (vis.ScaleCount - 1);
            for ( int i = 0; i < vis.ScaleCount; i++ ) {
                pos = cMin + i * cStep;
                Gl.glRasterPos2i(45, 15 + i * (150 / (vis.ScaleCount - 1)));
                Glut.glutBitmapString(Glut.GLUT_BITMAP_HELVETICA_10, pos.ToString("F" + vis.DecimalDigits));
            }
            PopMatrices();
        }

        /// <summary>
        /// Draws the coordinate axes
        /// </summary>
        private void DrawAxes() {
            if (!vis.Mode2D) DrawAxis(ColorManagement.AxesColor, 2);
                             DrawAxis(ColorManagement.AxesColor, 1);
                             DrawAxis(ColorManagement.AxesColor, 0);
        }

        /// <summary>
        /// Draws a single coordinate axis
        /// </summary>
        /// <param name="color">The color in which to draw the axis in</param>
        /// <param name="axis">0 for X, 1 for Y, 2 for Z</param>
        private void DrawAxis(ColorOGL color, int axis) {
            if (null == color)        throw new ArgumentNullException("color is null");
            ColumnProperty axisCp;
            //axisBase is the base vector for the axis
            //scaleDisp is the displacement perpendicular to the axis
            VectorF axisBase, labelPos, scaleDisp;
            switch (axis) {
                case 0:
                    axisCp    = vis.AxisX;
                    axisBase  = new VectorF(1f, 0f, 0f);
                    scaleDisp = new VectorF(0, -0.05f, 0);
                    labelPos  = new VectorF(1.05f, -0.05f, 0f);
                    break;
                case 1:
                    axisCp    = vis.AxisY;
                    axisBase  = new VectorF(0f, 1f, 0f);
                    scaleDisp = new VectorF(-0.05f, 0, 0);
                    labelPos  = new VectorF(-0.05f, 1.05f, 0);
                    break;
                case 2:
                    axisCp    = vis.AxisZ;
                    axisBase  = new VectorF(0f, 0f, 1f);
                    scaleDisp = new VectorF(0, -0.05f, 0);
                    labelPos  = new VectorF(0, 0.05f, 1.05f);
                    break;
                default:
                    throw new ArgumentException("axis not in (0,1,2)");
            }
            float step = 1 / ((float) vis.ScaleCount - 1);

            // Axis
            Gl.glColor3fv(color.RGB);
            Gl.glBegin(Gl.GL_LINES);
                Gl.glVertex3f(0, 0, 0);
                Gl.glVertex3f(axisBase.X, axisBase.Y, axisBase.Z);
            Gl.glEnd();

            //Scale
            float pos;
            VectorF PointValue;
            Gl.glColor3fv(ColorManagement.DescriptionColor.RGB);
            for (int i = 0; i < vis.ScaleCount; i++) {
                pos = 0 + i * step;
                Gl.glRasterPos3fv((scaleDisp + pos * axisBase).XYZ);
                PointValue = ScaleToPoint(scaleDisp + axisBase * pos);
                Glut.glutBitmapString(Glut.GLUT_BITMAP_HELVETICA_10, (axisBase.X * PointValue.X + axisBase.Y * PointValue.Y + axisBase.Z * PointValue.Z).ToString("F" + vis.DecimalDigits));
            }
            //Label
            Gl.glRasterPos3f(labelPos.X, labelPos.Y, labelPos.Z);
            Glut.glutBitmapString(Glut.GLUT_BITMAP_HELVETICA_10, axisCp.Label);
        }

        /// <summary>
        /// Draw the Points and optional Lines to them
        /// </summary>
        private void DrawPoints() {
            int[] mode = new int[1];
            Gl.glGetIntegerv(Gl.GL_RENDER_MODE, mode);
            Gl.glPointSize(vis.PointSize);
            Gl.glVertexPointer(3, Gl.GL_FLOAT, 0, this.vertexArray);
            Gl.glColorPointer(4, Gl.GL_FLOAT, 0, this.colorArray);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            TranslateAndScalePoints();

            if (mode[0] == Gl.GL_SELECT) {
                //Draw only Points, each point individually
                Gl.glPushName(0);
                for (int i = 0; i < vis.VisualizationWindow.DisplayedPointSet.Length; i++) {
                    Gl.glLoadName(i);
                    Gl.glBegin(Gl.GL_POINTS);
                    Gl.glArrayElement(i);
                    Gl.glEnd();
                }
                Gl.glPopName();
            } else {
                Gl.glDrawArrays(Gl.GL_POINTS, 0, vis.VisualizationWindow.DisplayedPointSet.Length);
                if (vis.ShowLines != ScatterLines.None) {
                    Gl.glColor3f(0.5f, 0.5f, 0.5f);
                    for (int i = 0; i < vis.VisualizationWindow.DisplayedPointSet.Length; i++) {
                        Gl.glBegin(Gl.GL_LINES);
                        Gl.glVertex3f(vertexArray[i * 3], vertexArray[i * 3 + 1], vertexArray[i * 3 + 2]);
                        if (vis.ShowLines == ScatterLines.xzAxes) {
                            Gl.glVertex3f(vertexArray[i * 3], (float)vis.AxisY.Min, vertexArray[i * 3 + 2]);
                        }
                        if (vis.ShowLines == ScatterLines.xyAxes) {
                            Gl.glVertex3f(vertexArray[i * 3], vertexArray[i * 3 + 1], (float)vis.AxisZ.Min);
                        }
                        if (vis.ShowLines == ScatterLines.yzAxes) {
                            Gl.glVertex3f((float)vis.AxisX.Min, vertexArray[i * 3 + 1], vertexArray[i * 3 + 2]);
                        }
                        Gl.glEnd();
                    }
                }
            }
            Gl.glPopMatrix();
        }

        /// <summary>
        /// Draws the picking rectangle based on the values of pickingStart and PickingEnd
        /// </summary>
        private void DrawPickingRectangle() {
            if (null != pickingStart && null != pickingEnd) {
                PushMatrices();
                SetupProjectionFlat(true, true, true);
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glLoadIdentity();
                //Inner gray area
                Gl.glColor4f(0.7f, 0.7f, 0.7f, 0.4f);
                Gl.glRecti(pickingStart.X, pickingStart.Y, pickingEnd.X, pickingEnd.Y);
                //Outer border
                Gl.glColor4f(0.9f, 0.1f, 0.1f, 0.7f);
                Gl.glLineWidth(1f);
                Gl.glDisable(Gl.GL_LINE_SMOOTH);
                Gl.glBegin(Gl.GL_LINE_STRIP);
                    Gl.glVertex2i(pickingStart.X, pickingStart.Y);
                    Gl.glVertex2i(pickingStart.X,   pickingEnd.Y);
                    Gl.glVertex2i(  pickingEnd.X,   pickingEnd.Y);
                    Gl.glVertex2i(  pickingEnd.X, pickingStart.Y);
                    Gl.glVertex2i(pickingStart.X, pickingStart.Y);
                Gl.glEnd();
                Gl.glFlush();
                //Restore previous modelview matrix
                PopMatrices();
            }
        }

        #endregion

        #region Manual Event Handling (communication with ScatterPlot3D)
        /// <summary>
        /// Used by ScatterPlot to inform the ScatterPlotControl about
        /// changed Axes, triggering necessary redraws.
        /// </summary>
        /// <param name="mode2dChanged">Set to true if the Axis change is a change from 2D-Mode to 3D-Mode or vice versa.</param>
        public void AxisChanged(bool mode2dChanged) {
            flipAxisMenu.ReGenerateEntries();
            CreateArrays();
            ColorizeSelection();
            ResetViewParameters(true, mode2dChanged);
            ResetCurrentViewParameters(false, mode2dChanged);
        }

        /// <summary>
        /// Used by ScatterPlot to inform the ScatterPlotControl about
        /// changed ColumnProperties, triggering necessary redraws.
        /// </summary>
        public void ColumnPropertiesChanged() {
            ResetViewParameters(true, false);
            ResetCurrentViewParameters(true, false);
        }

        /// <summary>
        /// Used by ScatterPlot to tell the ScatterPlotcontrol to reset its zoom and rotation
        /// view parameters
        /// </summary>
        public void ResetView() {
            ResetViewParameters(false, true);
            ResetCurrentViewParameters(false, true);
        }

        /// <summary>
        /// Used by ScatterPlot to inform the ScatterPlotControl about
        /// a changed selection, triggering necessary redraws.
        /// </summary>
        public void CurrentSelectionChanged() {
            ColorizeSelection();
            Invalidate();
        }

        /// <summary>
        /// Used by ScatterPlot to inform the ScatterPlotControl about
        /// changed colors, triggering necessary redraws.
        /// </summary>
        public void ColorChanged() {
            CreateArrays();
            ColorizeSelection();
            Invalidate();
        }

        /// <summary>
        /// Used by ScatterPlot to inform the ScatterPlotControl about
        /// a changed pointset, triggering necessary redraws.
        /// </summary>
        public void PointSetChanged() {
            CreateArrays();
            ColorizeSelection();
            //Reset perspective if another PointSet has been selected
            //If only the displayed PointSet has been changed e.g. by filtering or deleting points
            //just Rerender the scene
            this.MakeCurrentContext();
            if ( visualizedPointSet != vis.VisualizationWindow.PointSet ) {
                ResetViewParameters(true, true);
                ResetCurrentViewParameters(false, false);
                visualizedPointSet = vis.VisualizationWindow.PointSet;
            } else {
                Refresh();
            }
        }

        /// <summary>
        /// Used by ScatterPlot to inform the ScatterPlotControl about
        /// a changed space, triggering necessary redraws.
        /// </summary>
        public void SpaceChanged(){
            this.MakeCurrentContext();
            ResetViewParameters(true, true);
            ResetCurrentViewParameters(false, false);
            visualizedPointSet = vis.VisualizationWindow.PointSet;           
        }

        #endregion

        #region EventHandler

        private void MouseWheelEventHandler(object sender, MouseEventArgs ev) {
            if (!vis.Mode2D) {
                zoom += (float)ev.Delta / 1000;
                StoreViewParametersInCurrent(false, true);
                Invalidate();
            }
        }

        private void MouseDownEventHandler(object sender, MouseEventArgs ev) {
            MakeCurrentContext();
            mouseDragStartPoint = new Vector(ev.X, ev.Y);
            if (ev.Button == MouseButtons.Right) {
                udAngleTemp = udAngle;
                lrAngleTemp = lrAngle;
            } else if (ev.Button == MouseButtons.Left) {
                PickingBegin(new Vector(ev.X, ev.Y));
            }
        }

        private void MouseUpEventHandler(object sender, MouseEventArgs ev) {
            if (ev.Button == MouseButtons.Left) {
                PickingEnd(new Vector( ev.X, ev.Y));
            } else if (ev.Button == MouseButtons.Right
                && ev.X == mouseDragStartPoint.X
                && ev.Y == mouseDragStartPoint.Y) {
                flipAxisMenu.Show(this, ev.X, ev.Y);
            }

            mouseDragStartPoint = null;
        }

        private void MouseMoveEventHandler(object sender, MouseEventArgs ev) {
            if (ev.Button == MouseButtons.Right && mouseDragStartPoint!=null && !vis.Mode2D) {
                lrAngle = lrAngleTemp + (mouseDragStartPoint.X - ev.X);
                udAngle = udAngleTemp + (mouseDragStartPoint.Y - ev.Y);
                StoreViewParametersInCurrent(false, true);
                Invalidate();
            }
            if (ev.Button == MouseButtons.Left) {
                PickingUpdate(new Vector(ev.X, ev.Y));
            }
        }

        #endregion

        #region Picking

        /// <summary>
        /// (X,Y) tuples storing the start- and endpoints of a selection rectangle
        /// </summary>
        private Vector pickingStart, pickingEnd;

        /// <summary>
        /// Starts the picking process
        /// </summary>
        /// <param name="pickingStart">Coordinates where the picking started, window based (0,0) is left-top)</param>
        private void PickingBegin(Vector pickingStart) {
            this.pickingStart = pickingStart;
            this.pickingEnd   = pickingStart;
        }

        /// <summary>
        /// Ends the picking process, performs the actual OpenGL picking routine
        /// </summary>
        /// <param name="pickingEnd">Coordinates where the picking ended, window based (0,0) is left-top)</param>
        private void PickingEnd(Vector pickingEnd) {
            if (null != pickingStart) {
                this.Cursor = Cursors.WaitCursor;
                this.pickingEnd = pickingEnd;

                Point[] pickedPoints;

                if (pickingEnd.Equals(pickingStart)) {
                    //just clicked once
                    pickedPoints = PerformPicking(pickingStart.X, pickingStart.Y, 5, 5);
                } else {
                    //drew a rectangle
                    int w = Math.Abs(pickingStart.X - pickingEnd.X);
                    int h = Math.Abs(pickingStart.Y - pickingEnd.Y);
                    //Check for minimum selection area (5x5)
                    if (w < 5) { w = 5; }
                    if (h < 5) { h = 5; }
                    pickedPoints = PerformPicking((pickingStart.X + pickingEnd.X) / 2, (pickingStart.Y + pickingEnd.Y) / 2, w, h);
                }
                if (Control.ModifierKeys == Keys.Shift) {
                    ProjectController.CurrentSelection.AddRange(pickedPoints);
                } else if ( Control.ModifierKeys == Keys.Control ) {
                    ProjectController.CurrentSelection.RemovePoints(pickedPoints);
                } else {
                    ProjectController.CurrentSelection.ClearAndAddRange(pickedPoints);
                }
                ColorizeSelection();
                this.Cursor = Cursors.Default;
            }
            pickingStart = null;
            pickingEnd   = null;
            Invalidate();
        }

        /// <summary>
        /// Updates pickingEnd and invalidates control so the picking rectangle can be redrawn accordingly
        /// </summary>
        /// <param name="newEnd">New coordinates of pickingEnd</param>
        private void PickingUpdate(Vector newEnd) {
            pickingEnd = newEnd;
            Invalidate();
        }

        /// <summary>
        /// Picks the Points in the given Rectangle
        /// </summary>
        /// <param name="x">x-Coordinate (Window based, left is 0) of the picking region's center</param>
        /// <param name="y">y-Coordinate (Window based, top is 0) of the picking region's center</param>
        /// <param name="w">Width of the Picking Rectangle</param>
        /// <param name="h">Height of the Picking Rectangle</param>
        private Point[] PerformPicking(int x, int y, int w, int h) {
            PushMatrices();
            int[] selectBuffer = new int[vis.VisualizationWindow.DisplayedPointSet.Length * 4];
            int[] viewport     = new int[4];
            int hits;

            //Extract viewport
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

            //Designate SelectBuffer and switch to Select mode
            Gl.glSelectBuffer(selectBuffer.Length, selectBuffer);
            Gl.glRenderMode(Gl.GL_SELECT);

            //Clear Namestack
            Gl.glInitNames();

            //Initialize Picking Matrix
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            // create picking region near cursor location
            Glu.gluPickMatrix(x, (viewport[3]-y), w, h, viewport);

            if ( !this.stereoMode ) {
                SetupProjection(false);
            } else {
                SetupProjectionStereo(StereoEye.Picking);
            }

            SetupModelView(true);

            DrawPoints();

            //Switch Back to Render Mode
            hits = Gl.glRenderMode(Gl.GL_RENDER);

            PopMatrices();

            //Calculate actual Points and return them
            Point[] selectedPointsBuffer = new Point[hits];
            for (int i = 0; i < hits; i++) {
                selectedPointsBuffer[i] = vis.VisualizationWindow.DisplayedPointSet[selectBuffer[i * 4 + 3]];
            }
            return selectedPointsBuffer;
        }
        #endregion

        /// <summary>
        /// Menu that allows the user to change the orientation of an Axis
        /// </summary>
        private class FlipAxisMenu : ContextMenuStrip {
            /// <summary>
            /// The ScatterPlotControl this Menu belongs to
            /// </summary>
            private ScatterPlotControl spcontrol;

            public FlipAxisMenu(ScatterPlotControl spcontrol) {
                this.spcontrol = spcontrol;
                ReGenerateEntries();
            }

            protected override void Dispose(bool disposing) {
                base.Dispose(disposing);
                spcontrol = null;
                Items.Clear();
            }

            private void FlipColumn(object sender, EventArgs e) {
                ((sender as ToolStripMenuItem).Tag as ColumnProperty).SwitchOrientation();
                spcontrol.AxisChanged(false);
            }

            /// <summary>
            /// Recreates the menu.
            /// Needs to be called whenever the axes in the Scatterplot change
            /// </summary>
            public void ReGenerateEntries() {
                Items.Clear();
                ToolStripLabel label = new ToolStripLabel("Flip Axis");
                label.Font = new System.Drawing.Font(label.Font, System.Drawing.FontStyle.Bold);
                this.Items.Add(label);
                this.Items.Add(new ToolStripSeparator());

                //X
                ToolStripMenuItem xItem = new ToolStripMenuItem("Flip X (" + spcontrol.vis.AxisX.Label + ")");
                xItem.Tag = spcontrol.vis.AxisX;
                xItem.Click += FlipColumn;
                Items.Add(xItem);

                //Y
                ToolStripMenuItem yItem = new ToolStripMenuItem("Flip Y (" + spcontrol.vis.AxisY.Label + ")");
                yItem.Tag = spcontrol.vis.AxisY;
                yItem.Click += FlipColumn;
                Items.Add(yItem);

                //Z
                if (null != spcontrol.vis.AxisZ) {
                    ToolStripMenuItem zItem = new ToolStripMenuItem("Flip Z (" + spcontrol.vis.AxisZ.Label + ")");
                    zItem.Tag = spcontrol.vis.AxisZ;
                    zItem.Click += FlipColumn;
                    Items.Add(zItem);
                }

                //C
                if (null != spcontrol.vis.AxisC) {
                    ToolStripMenuItem cItem = new ToolStripMenuItem("Flip C (" + spcontrol.vis.AxisC.Label + ")");
                    cItem.Tag = spcontrol.vis.AxisC;
                    cItem.Click += FlipColumn;
                    Items.Add(cItem);
                }
            }

        }
    }
}
