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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Pavel.Framework;
using Tao.OpenGl;
using Tao.Platform.Windows;
using Tao.FreeGlut;

namespace Pavel.GUI.Visualizations {
    class ParallelPlotControl : OpenGLControl {

        #region Configuration
        /// <summary>
        /// The distance in pixels from the control's top border to the beginning of the 
        /// main area for drawing the lines
        /// </summary>
        protected int topSpace    = 40;
        /// <summary>
        /// The distance in pixels from the control's bottom border to the beginning of the 
        /// main area for drawing the lines
        /// </summary>
        protected int bottomSpace = 100;
        /// <summary>
        /// The distance in pixels from the control's left and right border to the beginning of the 
        /// main area for drawing the lines
        /// </summary>
        protected int lrSpace = 60;
        #endregion

        #region Fields
        /// <summary>
        /// Stores a referece to the ParallelPlot instance containing this control
        /// </summary>
        private ParallelPlot vis;
        /// <summary>
        /// Stores the position at which the mousebutton was held down
        /// </summary>
        private Vector mouseDragStartPoint;
        /// <summary>
        /// Stores the labels for the scales
        /// </summary>
        private List<String>[] scaleText;
        /// <summary>
        /// A reference to the Displaylist for the lines
        /// </summary>
        private int linesDisplayList;
        /// <summary>
        /// Context menu for adding Columns
        /// </summary>
        private AddColumnMenuStrip addColumnMenuStrip;
        #endregion

        #region Properties
        /// <summary>
        /// The upper position of the main drawing area for the lines in relative OpenGL coordinates.
        /// </summary>
        /// <remarks>0 is left/bottom, 1 is right/top</remarks>
        private float TopPos    { get { return (this.Height - (float)this.topSpace) / this.Height; } }
        /// <summary>
        /// The bottom position of the main drawing area for the lines in relative OpenGL coordinates.
        /// </summary>
        /// <remarks>0 is left/bottom, 1 is right/top</remarks>
        private float BottomPos { get { return ((float)this.bottomSpace) / this.Height; } }
        /// <summary>
        /// The left limit of the main drawing area for the lines in relative OpenGL coordinates.
        /// </summary>
        /// <remarks>0 is left/bottom, 1 is right/top</remarks>
        private float LeftPos { get { return ((float)this.lrSpace) / this.Width; } }
        /// <summary>
        /// The right limit of the main drawing area for the lines in relative OpenGL coordinates.
        /// </summary>
        /// <remarks>0 is left/bottom, 1 is right/top</remarks>
        private float RightPos { get { return (this.Width - (float)this.lrSpace) / this.Width; } }
        #endregion

        #region Constructor / Destructor
        public ParallelPlotControl(ParallelPlot vis)
            : base() {
            //Events
            this.vis = vis;
            this.MouseDown         += this.MouseDownEventHandler;
            this.MouseUp           += this.MouseUpEventHandler;
            this.MouseMove         += this.MouseMoveEventHandler;
            this.PickRegionChanged += this.PickRegionChangedHandler;

            addColumnMenuStrip = new AddColumnMenuStrip(this);

            keepAspect = false;
            this.Dock = DockStyle.Fill;
            ReInit();
        }

        ~ParallelPlotControl() {
            MakeCurrentContext();
            Gl.glDeleteLists(this.linesDisplayList, 1);
        }

        //TODO Dispose Funktion

        #endregion

        #region Setup
        protected override void InitOpenGL() {
            Gl.glClearColor(ColorManagement.BackgroundColor.R, ColorManagement.BackgroundColor.G, ColorManagement.BackgroundColor.B, 0.0f);
            Gl.glShadeModel(Gl.GL_FLAT);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            this.linesDisplayList = Gl.glGenLists(1);
        }

        protected override void SetupModelViewMatrixOperations() {
            Gl.glLoadIdentity();
        }

        /// <summary>
        /// Creates the Displaylist that renders the Lines to a 1x1 square.
        /// Setup the Modelview Matrix to stretch the lines over the axes.
        /// </summary>
        private void CreateLineDisplayList() {
            MakeCurrentContext();
            PointSet  ps = vis.VisualizationWindow.DisplayedPointSet;
            Space  space = vis.VisualizationWindow.Space;
            Selection cs = ProjectController.CurrentSelection;
            int dimension = space.Dimension;
            //Calculate Translations and Scaling values
            float[] tx = new float[dimension];
            float[] ty = new float[dimension];
            float[] sy = new float[dimension];
            float normalizedAxisSpacing = 1f / (dimension - 1);
            int i = 0;
            foreach (ColumnProperty cp in space.ColumnProperties) {
                tx[i] =  (float)i * normalizedAxisSpacing;
                ty[i] = -(float)cp.Min;
                sy[i] =  (float)(1 / (cp.Max - cp.Min));
                i++;
            }

            //Generate Displaylist

            List<int>      selectedPointNames  = new List<int>(cs.Length);
            List<float[,]> selectedPointValues = new List<float[,]>(cs.Length);

            int pointName = 0;
            int[] map;
            Gl.glNewList(this.linesDisplayList, Gl.GL_COMPILE);
            Gl.glInitNames();
            Gl.glPushName(0);
            foreach (PointList pl in ps.PointLists) {
                map = space.CalculateMap(pl.ColumnSet);
                for (int pointIndex = 0; pointIndex < pl.Count; pointIndex++) {
                    if (cs.Contains(pl[pointIndex])) {
                        selectedPointNames.Add(pointName++);
                        float[,] x = new float[dimension,2];
                        for (int colIndex = 0; colIndex < dimension; colIndex++) {
                            x[colIndex, 0] = tx[colIndex];
                            x[colIndex, 1] = sy[colIndex] * ((float)pl[pointIndex][map[colIndex]] + ty[colIndex]);
                        }
                        selectedPointValues.Add(x);
                    } else {
                        //Set the color of the line with a current alpha-value
                        Gl.glColor4fv(ProjectController.GetSelectionColor(pl[pointIndex]).RGBwithA(vis.LinesAlpha * 0.01f));
                        Gl.glLoadName(pointName++);
                        Gl.glBegin(Gl.GL_LINE_STRIP);
                        for (int colIndex = 0; colIndex < dimension; colIndex++) {
                            Gl.glVertex3f(
                                tx[colIndex],
                                sy[colIndex] * ((float)pl[pointIndex][map[colIndex]] + ty[colIndex]),
                                0f);
                        }
                        Gl.glEnd();
                    }

                }
            }
            //Draw Current Selection over normal lines
            Gl.glColor4fv(ColorManagement.CurrentSelectionColor.RGBwithA(vis.LinesAlpha * 0.01f));
            for (int pointIndex=0; pointIndex < selectedPointValues.Count; ++pointIndex) {
                Gl.glLoadName(selectedPointNames[pointIndex]);
                Gl.glBegin(Gl.GL_LINE_STRIP);
                for (int colIndex = 0; colIndex < dimension; colIndex++) {
                    Gl.glVertex3f(
                        selectedPointValues[pointIndex][colIndex, 0],
                        selectedPointValues[pointIndex][colIndex, 1],
                        1f);
                }
                Gl.glEnd();
            }
            Gl.glEndList();
        }

        /// <summary>
        /// This creates the String representatives of the scale values. It is possible to set the number 
        /// of decimal digits displayed via the DecimalDigits Property of the ParallelPlot.
        /// </summary>
        public void CreateScaleText() {
            //TODO Saubermachen, pruefen ob korrekt
            this.scaleText = new List<string>[this.vis.VisualizationWindow.Space.Dimension];
            float scaleStep = 0.0F;
            float desc = 0.0F;
            float decimalDigits = (float)Math.Pow(10.0, (double)vis.DecimalDigits);
            int tmp = 0;
            int i = 0;
            Space space = vis.VisualizationWindow.Space;
            for (int c = 0; c < space.Dimension; c++) {
                List<String> tmpScaleList = new List<string>();
                scaleStep = (float)((space.ColumnProperties[c].Max - space.ColumnProperties[c].Min) / this.vis.AxesScale);
                for (int j = 0; j <= this.vis.AxesScale; j++) {
                    tmp = (int)((scaleStep * j + space.ColumnProperties[c].Min) * decimalDigits);
                    desc = ((float)tmp) / decimalDigits;
                    tmpScaleList.Add(desc.ToString());
                }
                this.scaleText[i] = tmpScaleList;
                i++;
            }
        }

        #endregion

        #region Calculations

        /// <summary>
        /// The distance between two axes in relative OpenGL coordinates
        /// </summary>
        private float AxisSpacing() { //TODO als Property
            return (RightPos - LeftPos) / (this.vis.VisualizationWindow.Space.Dimension - 1);
        }

        /// <summary>
        /// Calculates the column in vicinity to x.
        /// </summary>
        /// <param name="x">x-position in absolute, windows-based coordinates </param>
        /// <returns>Returns the index of the column in the current Space, -1 if no column (mouse pointer between columns)</returns>
        private int ColumnAtPosition(int x) {
            int dimension = vis.VisualizationWindow.Space.Dimension;
            float relX        = (float)x  / Width;
            float maxdistance = (float)10 / Width;
            float axisSpacing = AxisSpacing();

            if (maxdistance > axisSpacing / 2) maxdistance = axisSpacing / 2;

            float colMin, colMax;
            for (int col = 0; col < dimension; col++) {
                colMin = LeftPos + col * axisSpacing - maxdistance;
                colMax = LeftPos + col * axisSpacing + maxdistance;
                if (colMin <= relX && relX < colMax) return col;
            }
            return -1;
        }

        #endregion

        #region Drawing
        protected override void RenderScene() {
            PushMatrices();

            SetupProjectionFlat(true, false, false);
            SetupModelView(true);
            
            Gl.glDrawBuffer(Gl.GL_BACK);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            DrawAxes();
            DrawAxisLabels();
            DrawScales();
            DrawLines();
            DrawDrempels();

            PopMatrices();
        }

        /// <summary>
        /// Draw the vertical axes
        /// </summary>
        private void DrawAxes() {
            Gl.glPushAttrib(Gl.GL_LINE_BIT);
            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glLineWidth(0.5f);
            Gl.glColor3fv(ColorManagement.AxesColor.RGB);

            float axisSpacing = AxisSpacing();

            float aX  = LeftPos;
            float aBY = BottomPos;
            float aEY = TopPos;

            for (int i = 0; i < vis.VisualizationWindow.Space.Dimension; i++) {
                Gl.glBegin(Gl.GL_LINES);
                    Gl.glVertex2f(aX, aBY);
                    Gl.glVertex2f(aX, aEY);
                Gl.glEnd();
                aX += axisSpacing;                
            }
            Gl.glPopAttrib();
        }

        /// <summary>
        /// Draw the labels for the axes
        /// </summary>
        private void DrawAxisLabels() {
            Gl.glColor3fv(ColorManagement.DescriptionColor.RGB);
            float axisSpacing = AxisSpacing();
            float yStep = 11f / this.Height;
            float x = LeftPos-(10f/Width);
            float y = BottomPos-yStep;
            int i = 0;
            foreach (Column c in vis.VisualizationWindow.Space) {
                Gl.glRasterPos2f(x, y-(i%4)*yStep);
                Glut.glutBitmapString(Glut.GLUT_BITMAP_HELVETICA_10, c.Label);
                x += axisSpacing;
                i++;
            }
        }

        //private void DrawMovingColumn(int x) {
        //    Gl.glDisable(Gl.GL_LINE_SMOOTH);
        //    Gl.glLineWidth(5.0f);
        //    Gl.glColor3f(10.0f, 0.0f, 0.0f);
        //    float aBY = 600 - this.yPadding;
        //    float aEY = aBY - (float)this.AxisLength();
        //    Gl.glBegin(Gl.GL_LINES);
        //    Gl.glVertex3f(x, aBY, 0.0f);
        //    Gl.glVertex3f(x, aEY, 0.0f);
        //    Gl.glEnd();
        //    Gl.glLineWidth(1.0f);
        //    Gl.glEnable(Gl.GL_LINE_SMOOTH);
        //}

        /// <summary>
        /// Draw the little triangles above the axes 
        /// </summary>
        private void DrawDrempels() {
            ColumnProperty[] cp = vis.VisualizationWindow.Space.ColumnProperties;
            float axisSpacing = AxisSpacing();
            int columnName = 0;
            float aX = LeftPos;
            float aY = TopPos + 15f/this.Height;
            float xd =  5f / this.Width;
            float yd = 10f / this.Height;

            Gl.glColor3fv(ColorManagement.DescriptionColor.RGB);

            foreach (Column c in vis.VisualizationWindow.Space) {
                Gl.glLoadName(columnName);
                Gl.glBegin(Gl.GL_TRIANGLES);
                if (!cp[columnName].IsAscendingOrder()) {
                    Gl.glVertex2f(aX, aY);
                    Gl.glVertex2f(aX + xd, aY + yd);
                    Gl.glVertex2f(aX - xd, aY + yd);
                } else {
                    Gl.glVertex2f(aX, aY + yd);
                    Gl.glVertex2f(aX - xd, aY);
                    Gl.glVertex2f(aX + xd, aY);
                }
                Gl.glEnd();
                columnName++;
                aX += axisSpacing;
            }
        }

        /// <summary>
        /// Draw the actual lines representing the data points
        /// </summary>
        private void DrawLines() {
            Gl.glClipPlane(Gl.GL_CLIP_PLANE0, new double[] { 0.0, -1.0, 0.0, TopPos });
            Gl.glClipPlane(Gl.GL_CLIP_PLANE1, new double[] { 0.0, 1.0, 0.0, -BottomPos });
            Gl.glEnable(Gl.GL_CLIP_PLANE0);
            Gl.glEnable(Gl.GL_CLIP_PLANE1);

            Gl.glLineWidth(vis.LineWidth);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();

            Gl.glLoadIdentity();
            Gl.glTranslatef(LeftPos, BottomPos, 0f);
            Gl.glScalef(RightPos - LeftPos, TopPos - BottomPos, 1f);
            Gl.glCallList(this.linesDisplayList);

            Gl.glPopMatrix();

            Gl.glDisable(Gl.GL_CLIP_PLANE0);
            Gl.glDisable(Gl.GL_CLIP_PLANE1);
        }

        /// <summary>
        /// Draw the scales on the axes
        /// </summary>
        private void DrawScales() {
            if (this.vis.AxesScale != 0) {
                Gl.glPushAttrib(Gl.GL_LINE_BIT);
                Gl.glLineWidth(0.5f);
                float axisSpacing = this.AxisSpacing();

                //Extrema of the Axes
                float axesBeginY = BottomPos;
                float axesEndY   = TopPos;

                //Coordiantes of the scalling lines
                float x = LeftPos;
                float y = axesBeginY;

                float distanceScalingLines = ( axesBeginY - axesEndY ) / this.vis.AxesScale;

                float halfTick = 3f / this.Width;
                float stringPos = 5f / this.Width;

                int i = 0;
                Space space = vis.VisualizationWindow.Space;
                for (int c = 0; c < space.Dimension; c++) {
                    String[] scalingValuesStrings = this.scaleText[i].ToArray();
                    y = axesBeginY;
                    for (int j = 0; j <= this.vis.AxesScale; j++) {
                        Gl.glColor3fv(ColorManagement.AxesColor.RGB);
                        Gl.glBegin(Gl.GL_LINES);
                            Gl.glVertex2f(x-halfTick, y);
                            Gl.glVertex2f(x+halfTick, y);
                        Gl.glEnd();
                        Gl.glColor3fv(ColorManagement.DescriptionColor.RGB);
                        Gl.glRasterPos2f(x+stringPos, y);
                        Glut.glutBitmapString(Glut.GLUT_BITMAP_HELVETICA_10, scalingValuesStrings[j]);
                        y -= distanceScalingLines;
                    }
                    i++;
                    x += axisSpacing;
                }
                Gl.glPopAttrib();
            }
        }

        /// <summary>
        /// Draw the delete button for a column directly to the frontbuffer
        /// </summary>
        /// <param name="column">x-Position of the mouse cursor in absolute Window coordinates</param>
        private void ShowDeleteButton(int x) {
            int column = ColumnAtPosition(x);
            if (-1 == column) return;

            MakeCurrentContext();
            float axisSpacing = AxisSpacing();
            float tpx = 4f / Width;
            float tpy = 4f / Height;
            PushMatrices();

            SetupProjectionFlat(true, false, false);
            SetupModelView(true);

            Gl.glDrawBuffer(Gl.GL_FRONT);
            Gl.glColor3f(1, 0, 0);
            Gl.glBegin(Gl.GL_LINES);
                Gl.glVertex2f(LeftPos + column * axisSpacing - tpx, TopPos + 30f / Height + tpy);
                Gl.glVertex2f(LeftPos + column * axisSpacing + tpx, TopPos + 30f / Height - tpy);
                Gl.glVertex2f(LeftPos + column * axisSpacing - tpx, TopPos + 30f / Height - tpy);
                Gl.glVertex2f(LeftPos + column * axisSpacing + tpx, TopPos + 30f / Height + tpy);
            Gl.glEnd();
            Gl.glFlush();

            PopMatrices();
        }

        /// <summary>
        /// Hides all delete buttons by drawign directly to the frontbuffer
        /// </summary>
        private void HideDeleteButtons() {
            MakeCurrentContext();
            float axisSpacing = AxisSpacing();
            PushMatrices();

            SetupProjectionFlat(true, false, false);
            SetupModelView(true);

            Gl.glDrawBuffer(Gl.GL_FRONT);
            Gl.glColor3fv(ColorManagement.BackgroundColor.RGB);
            Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex2f(0f, 1f);
                Gl.glVertex2f(1f, 1f);
                Gl.glVertex2f(1f, TopPos + 25f / Height);
                Gl.glVertex2f(0f, TopPos + 25f / Height);
            Gl.glEnd();
            Gl.glFlush();

            PopMatrices();
        }

        /// <summary>
        /// Copies the content of the FrontBuffer to the BackBuffer
        /// </summary>
        private void BackupFrontBuffer() {
            MakeCurrentContext();
            CopyPixels(Gl.GL_FRONT, Gl.GL_BACK, 0, 0, Width, Height);
        }

        #endregion

        #region Manual Event Notification
        /// <summary>
        /// Performs necessary actions to account for changed PointSets or Spaces in the Visualization
        /// </summary>
        public void ReInit() {
            CreateLineDisplayList();
            CreateScaleText();
            addColumnMenuStrip.ReGenerateEntries();
            Invalidate();
        }

        /// <summary>
        /// Performs necessary actions to account for changed Selections in the Visualization
        /// </summary>
        public void CurrentSelectionChanged() {
            CreateLineDisplayList();
            Invalidate();
        }

        #endregion

        #region PickRegion Handling

        /// <summary>Needed to check wether a region was entered or left with the mousecursor.</summary>
        private PickRegion currentRegion;

        /// <summary>Extension of MouseEventArgs that contains Information about the old and the new PickRegion</summary>
        class PickRegionChangedEventArgs : MouseEventArgs {
            PickRegion newRegion;
            PickRegion oldRegion;

            public PickRegionChangedEventArgs(MouseButtons button, int clicks, int x, int y, int delta,
                    PickRegion newRegion, PickRegion oldRegion) : base(button, clicks, x, y, delta){
                this.newRegion = newRegion;
                this.oldRegion = oldRegion;
            }

            public PickRegion NewRegion {
                get { return newRegion; }
            }

            public PickRegion OldRegion {
                get { return oldRegion; }
            }
        }

        /// <summary>Handler signature for the PickRegionChanged Event</summary>
        protected delegate void PickRegionChangedEventHandler(object sender, PickRegionChangedEventArgs e);
        /// <summary>Fired whenever the PickRegion under the mouse cursor changes</summary>
        protected event PickRegionChangedEventHandler PickRegionChanged;

        /// <summary>Checks wether the PickRegion has changed and fires the PickRegionChangedEvent if necessary</summary>
        private void FirePickRegionChangedEventIfNecessary(MouseEventArgs ev) {
            PickRegion newRegion = RegionAtPosition(ev.X, ev.Y);
            if (newRegion == currentRegion) {
                return;
            } else {
                if (null != PickRegionChanged) PickRegionChanged(this,
                    new PickRegionChangedEventArgs(ev.Button, ev.Clicks, ev.X, ev.Y, ev.Delta, newRegion, currentRegion));
                currentRegion = newRegion;
            }
        }

        /// <summary>Calculates the PickRegion at the given Position</summary>
        private PickRegion RegionAtPosition(int x, int y) {
            int absTop    = (int)(   TopPos * Height);
            int absBottom = (int)(BottomPos * Height);
            int absLeft   = (int)(  LeftPos * Width );
            int absRight  = (int)( RightPos * Width );
            y = this.Height - y;
            //if (x >= absLeft && x <= absRight) {
                if (y >= absTop && y < absTop + 30)
                    return PickRegion.Drempels;
                if (y >= absTop + 30)
                    return PickRegion.DeleteButtons;
                if (y <= absTop && y >= absBottom)
                    return PickRegion.Lines;
            //}
            return PickRegion.Other;
        }

        #endregion

        #region Event Handling

        private void PickRegionChangedHandler(object sender, PickRegionChangedEventArgs ev) {
            if (PickRegion.DeleteButtons == ev.OldRegion) {
                HideDeleteButtons();
            }
        }

        private void MouseDownEventHandler(object sender, MouseEventArgs ev) {
            this.mouseDragStartPoint = new Vector(ev.X, ev.Y);

            if (PickRegion.Lines == currentRegion) {
                PickingBegin(new Vector(ev.X, ev.Y));
            } else if (PickRegion.Drempels == currentRegion) {
                MoveColumnBegin(new Vector(ev.X, ev.Y));
            }
        }

        private void MouseMoveEventHandler(object sender, MouseEventArgs ev) {
            if (PickRegion.DeleteButtons == currentRegion) {
                HideDeleteButtons();
                ShowDeleteButton(ev.X);
            }

            PickingUpdate(new Vector(ev.X, ev.Y));                
            MoveColumnUpdate(new Vector(ev.X, ev.Y));
            
            FirePickRegionChangedEventIfNecessary(ev);
        }

        private void MouseUpEventHandler(object sender, MouseEventArgs ev) {
            if (mouseDragStartPoint.X == ev.X && mouseDragStartPoint.Y == ev.Y) { //clicked
                if (MouseButtons.Left == ev.Button) { //Leftclick
                    if (PickRegion.DeleteButtons == currentRegion) {
                        RemoveColumnAt(ev.X);
                    } else if (PickRegion.Drempels == currentRegion) {
                        FlipColumn(new Vector(ev.X, ev.Y));
                    }
                } else if (MouseButtons.Right == ev.Button) { //rightclick
                    if (PickRegion.Lines == currentRegion) {
                        addColumnMenuStrip.Show(this, ev.X, ev.Y);
                    } else if (PickRegion.Drempels == currentRegion) {
                        ShowColumnPropertyDialog(ev.X);
                    }
                    
                }

            }
            MoveColumnEnd(new Vector(ev.X, ev.Y));
            PickingEnd(new Vector(ev.X, ev.Y));
        }

        #endregion

        #region Picking
        //TODO das neue Picking liesse sich eigentlich nach OpenGLControl auslagern wenns funktional geschrieben wird
        /// <summary>(X,Y) tuple storing the startpoint of a selection rectangle</summary>
        private Vector pickingStart;

        /// <summary>
        /// Starts the picking process
        /// </summary>
        /// <param name="pickingStart">Vector containing X and Y (window based coordinates, 0 is top) for the new startpoint of the picking rectangle.</param>
        private void PickingBegin(Vector pickingStart) {
            this.pickingStart  = pickingStart;
            BackupFrontBuffer();
        }

        /// <summary>
        /// Redraws the picking rectangle
        /// </summary>
        /// <param name="pickingEnd">Vector containing X and Y (window based coordinates, 0 is top) for the new endpoint of the picking rectangle.</param>
        private void PickingUpdate(Vector pickingEnd) {
            if (null != pickingStart) {
                PickingRestoreArea(pickingStart, pickingEnd);
                PickingDrawRectangle(pickingStart, pickingEnd);
            }
        }

        /// <summary>
        /// Restores the area around the Pickingrectangle from the Backup in the Backbuffer
        /// </summary>
        /// <param name="pickingStart">Vector containing X and Y (window based coordinates, 0 is top) for the startpoint of the picking rectangle.</param>
        /// <param name="pickingEnd">Vector containing X and Y (window based coordinates, 0 is top) for the endpoint of the picking rectangle.</param>
        private void PickingRestoreArea(Vector pickingStart, Vector pickingEnd) {
            MakeCurrentContext();
            CopyPixels(Gl.GL_BACK, Gl.GL_FRONT, 0, 0, Width, Height);
        }

        /// <summary>
        /// Draws the picking rectangle to the frontbuffer,
        /// based on the values of pickingStart and PickingEnd
        /// </summary>
        /// <param name="pickingStart">Vector containing X and Y (window based coordinates, 0 is top) for the startpoint of the picking rectangle.</param>
        /// <param name="pickingEnd">Vector containing X and Y (window based coordinates, 0 is top) for the endpoint of the picking rectangle.</param>
        private void PickingDrawRectangle(Vector pickingStart, Vector pickingEnd) {
            if (null != pickingStart && null != pickingEnd) {
                MakeCurrentContext();
                PushMatrices();
                SetupProjectionFlat(true, true, true);
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glDrawBuffer(Gl.GL_FRONT);
                Gl.glLoadIdentity();
                //Inner gray area
                Gl.glColor4f(0.7f, 0.7f, 0.7f, 0.4f);
                Gl.glRecti(pickingStart.X, pickingStart.Y, pickingEnd.X, pickingEnd.Y);
                //Outer border
                Gl.glColor4f(0.9f, 0.1f, 0.1f, 0.7f);
                Gl.glLineWidth(1f);
                Gl.glPushAttrib(Gl.GL_LINE_BIT);
                Gl.glDisable(Gl.GL_LINE_SMOOTH);
                Gl.glBegin(Gl.GL_LINE_STRIP);
                    Gl.glVertex2i(pickingStart.X, pickingStart.Y);
                    Gl.glVertex2i(pickingStart.X, pickingEnd.Y);
                    Gl.glVertex2i(pickingEnd.X,   pickingEnd.Y);
                    Gl.glVertex2i(pickingEnd.X,   pickingStart.Y);
                    Gl.glVertex2i(pickingStart.X, pickingStart.Y);
                Gl.glEnd();
                Gl.glPopAttrib();
                //Restore previous modelview matrix
                PopMatrices();
                Gl.glFlush();
            }
        }

        /// <summary>
        /// End the picking process.
        /// Restores the screen from the backbuffer,
        /// resets the picking state,
        /// performs the actual OpenGL Picking and finally
        /// updates the CurrentSelection
        /// </summary>
        /// <param name="pickingEnd">Vector containing X and Y (window based coordinates, 0 is top) for the endpoint of the picking rectangle.</param>
        private void PickingEnd(Vector pickingEnd) {
            if (null != pickingStart) {
                PickingRestoreArea(pickingStart, pickingEnd);

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
                this.pickingStart = null;
                
                if (Control.ModifierKeys == Keys.Shift) {
                    ProjectController.CurrentSelection.AddRange(pickedPoints);
                } else if ( Control.ModifierKeys == Keys.Control ) {
                    ProjectController.CurrentSelection.RemovePoints(pickedPoints);
                } else {
                    ProjectController.CurrentSelection.ClearAndAddRange(pickedPoints);
                }
            }
        }

        /// <summary>
        /// Picks the Points in the given Rectangle
        /// </summary>
        /// <param name="x">x-Coordinate (Window based, left is 0) of the picking region's center</param>
        /// <param name="y">y-Coordinate (Window based, top is 0) of the picking region's center</param>
        /// <param name="w">Width of the Picking Rectangle</param>
        /// <param name="h">Height of the Picking Rectangle</param>
        /// <returns>An array containing the picked Points</returns>
        private Point[] PerformPicking(int x, int y, int w, int h) {
            PushMatrices();
            int[] selectBuffer = new int[vis.VisualizationWindow.DisplayedPointSet.Length * 4];
            int[] viewport = new int[4];
            int hits;

            //Extract viewport
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

            //Designate SelectBuffer and switch to Select mode
            Gl.glSelectBuffer(selectBuffer.Length, selectBuffer);
            Gl.glRenderMode(Gl.GL_SELECT);

            //Initialize Picking Matrix
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            // create picking region near cursor location
            Glu.gluPickMatrix(x, (viewport[3] - y), w, h, viewport);

            //Draw Lines
            SetupProjectionFlat(false, false, false);
            SetupModelView(true);

            DrawLines();

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

        #region Column Interaction

        private int? moveColumn;

        private void MoveColumnBegin(Vector startPoint) {
            int column = ColumnAtPosition(startPoint.X);
            if (0 <= column) {
                MakeCurrentContext();
                BackupFrontBuffer();
                this.moveColumn = column;
            }
        }

        private void MoveColumnUpdate(Vector newPos) {
            if (null != moveColumn) {
                CopyPixels(Gl.GL_BACK, Gl.GL_FRONT, 0, 0, Width, Height);
                this.MoveColumnDrawLine(newPos.X);
            }
        }

        private void MoveColumnEnd(Vector pos) {
            if (null != moveColumn) {
                int newPosition = NewPosition(moveColumn.Value, pos.X);
                if (moveColumn != newPosition) {
                    vis.VisualizationWindow.Space.MoveColumn(moveColumn.Value, newPosition);
                    ReInit();
                } else {
                    CopyPixels(Gl.GL_BACK, Gl.GL_FRONT, 0, 0, Width, Height);
                    Gl.glFlush();
                }
                moveColumn = null;
            }
        }

        private void MoveColumnDrawLine(int x) {
            if (null != moveColumn) {
                PushMatrices();
                SetupProjectionFlat(true, false, false);
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glDrawBuffer(Gl.GL_FRONT);
                Gl.glLoadIdentity();

                Gl.glPushAttrib(Gl.GL_LINE_BIT);
                Gl.glDisable(Gl.GL_LINE_SMOOTH);
                Gl.glLineWidth(5.0f);
                Gl.glColor3f(10.0f, 0.0f, 0.0f);
                Gl.glBegin(Gl.GL_LINES);
                    Gl.glVertex3f((float) x/Width, TopPos,    0.0f);
                    Gl.glVertex3f((float) x/Width, BottomPos, 0.0f);
                Gl.glEnd();
                Gl.glPopAttrib();

                Gl.glFlush();
                //Restore previous modelview matrix
                PopMatrices();
            }
        }

        /// <summary>
        /// Calculate the new Position of a moved Column in the space.
        /// </summary>
        /// <remarks>
        /// This is a bit complicated unfortunately, due to the way the Space.MoveColumn method is implemented.
        /// In that method, a new position for a Column isn't given in terms of "put this Column before that Column",
        /// but in terms of "I want to access this Column under this index from now on".
        /// As a result, the calculation of the new position is dependent on the direction the column is moved in.
        /// 
        /// The new position is calculated as follows:
        /// - Assume the column to be moved is column A
        /// - Begin from the left, checking each axis' position.
        /// - If the mouse pointer is left of that axis return its column as the new position
        ///   
        /// As a result, if the mouse pointer is left of column X, the new position will be X as well.
        /// If the mouse pointer is JUST left of column A, the new position will be A as well. Correct.
        /// Now, just continuing like this, the mouse pointer just to the right of column A would result in a new
        /// position of A+1, which is undesired. The new Position should still be A there.
        /// 
        /// To achieve this, a cut is made at the moved axis and things are counted differently from here.
        /// - Examine axis A+1, if the mouse is to its left, it must be just right of axis A.
        ///   (The previous cases have been handled already.)
        ///   Return (A+1)-1
        /// - Continue like in the first iteration until the end is reached.
        /// - If the mouse pointer isn't left of the last axis, it has to be to its right side, dimension-1 is returned.
        /// </remarks>
        /// <param name="oldPosition">The index of the Column to be moved</param>
        /// <param name="mouseX">The x-coordinate of the mouse pointer (absolute, window based pixels)</param>
        /// <returns>The new index for the column</returns>
        private int NewPosition(int oldPosition, int mouseX) {
            int dimension = vis.VisualizationWindow.Space.Dimension;
            //position of the leftmost axis (pixel based)
            int left        = (int)(LeftPos * Width);
            //spacing between the axes (pixel based)
            int axisSpacing = (int)(AxisSpacing() * Width);

            //left of old position
            for (int col = 0; col <= oldPosition; col++) {
                if (mouseX < left + col * axisSpacing) return col;
            }

            //right of old position
            for (int col = oldPosition + 1; col < dimension; col++) {
                if (mouseX < left + col * axisSpacing) return col - 1;
            }

            return dimension - 1;
        }

        /// <summary>
        /// Remove Column at the position given by the mouse pointer
        /// </summary>
        /// <param name="x">The mouse pointers x coordinate (absolute, window-based)</param>
        private void RemoveColumnAt(int x) {
            int column = ColumnAtPosition(x);
            if (0 <= column) vis.RemoveColumn(column);
        }

        /// <summary>
        /// Switch Min/Max of a Column
        /// </summary>
        /// <param name="pos">Mouse position in absolute window-based coordinates</param>
        private void FlipColumn(Vector pos) {
            int column = ColumnAtPosition(pos.X);
            if (column >= 0) {
                vis.VisualizationWindow.Space.ColumnProperties[column].SwitchOrientation();
                ReInit();
            }
        }

        /// <summary>
        /// Insert Column at the position given by the mouse pointer
        /// </summary>
        /// <param name="x">The mouse pointers x coordinate (absolute, window-based)</param>
        /// <param name="col">The column to insert</param>
        private void InsertColumn(int x, Column col) {
            vis.VisualizationWindow.Space.InsertColumn(col, NewPosition(Width, x));
            ReInit();//TODO das Redrawen muss strukturierter gehen (Events), hier nur als schneller Fix
        }

        /// <summary>
        /// Display the ColumnPropertyDialog for the column at the position given by the mouse pointer
        /// </summary>
        /// <param name="x">The mouse pointers x coordinate (absolute, window-based)</param>
        private void ShowColumnPropertyDialog(int x) {
            int column = ColumnAtPosition(x);
            if (0 <= column) {
                ColumnPropertyDialog cpd = new ColumnPropertyDialog(vis.VisualizationWindow.Space.ColumnProperties[column]);
                cpd.Location = Cursor.Position;
                if (cpd.ShowDialog() == DialogResult.OK) ReInit();
            }
        }

        #endregion

        /// <summary>
        /// The context menu for inserting Columns into the displayed Space
        /// </summary>
        private class AddColumnMenuStrip : ContextMenuStrip {
            /// <summary>reference to the containing ParallelPlotControl</summary>
            private ParallelPlotControl ppcontrol;
            /// <summary>
            /// The x-coordinate where the context menu was opened.
            /// Needed to calculate the insert-position in the Space for the new Column
            /// </summary>
            private int x;

            /// <param name="ppcontrol">The containing ParallelPlotControl</param>
            public AddColumnMenuStrip(ParallelPlotControl ppcontrol)
                : base() {
                this.ppcontrol = ppcontrol;
                ReGenerateEntries();
            }

            protected override void Dispose(bool disposing) {
                base.Dispose(disposing);
                this.ppcontrol = null;
                Items.Clear();
            }

            /// <summary>
            /// Overridden to store the x and y coordinates before calling base.
            /// </summary>
            /// <param name="control">The containing control</param>
            /// <param name="x">The x position within the containing control at which the menu should be opened. Should use the mouse position.</param>
            /// <param name="y">The y position within the containing control at which the menu should be opened. Should use the mouse position.</param>
            public new void Show(Control control, int x, int y) {
                this.x = x;
                base.Show(control, x, y);
            }

            /// <summary>
            /// Delegate function for the Menu Entries to insert the Column
            /// </summary>
            /// <remarks>
            /// Delegates to ParallelplotControl.InsertColumn.
            /// Necessary because the x-coordinate isn'nt easily available in the anonymous delegates that
            /// handle the click events in the MenuItems.
            /// </remarks>
            /// <param name="c">The column to insert</param>
            private void InsertColumn(Column c) {
                ppcontrol.InsertColumn(x, c);
            }

            /// <summary>
            /// (Re)generates the Menu Entries
            /// </summary>
            public void ReGenerateEntries() {
                Items.Clear();
                ToolStripLabel label = new ToolStripLabel("Add Column");
                label.Font = new System.Drawing.Font(label.Font, System.Drawing.FontStyle.Bold);
                this.Items.Add(label);
                this.Items.Add(new ToolStripSeparator());
                Column[] columns = ppcontrol.vis.VisualizationWindow.DisplayedPointSet.ColumnSet.Columns;
                foreach (Column col in columns) {
                    ToolStripMenuItem mi = new ToolStripMenuItem(col.Label);
                    mi.Tag = col;
                    mi.Click += delegate(object sender, EventArgs e) {
                        InsertColumn(mi.Tag as Column);
                    };
                    this.Items.Add(mi);
                }

            }
        }

        /// <summary>
        /// Describes the PickRegion
        /// </summary>
        enum PickRegion {
            /// <summary>
            /// The topmost area with the delete buttons
            /// </summary>
            DeleteButtons,
            /// <summary>
            /// The area containing the drempels
            /// </summary>
            Drempels,
            /// <summary>
            /// The main drawing area for the lines
            /// </summary>
            Lines,
            /// <summary>
            /// Another area (everything outside the named areas)
            /// </summary>
            Other,
            /// <summary>
            /// Can indicate that no region is currentlich active
            /// </summary>
            None
        }
    }
}
