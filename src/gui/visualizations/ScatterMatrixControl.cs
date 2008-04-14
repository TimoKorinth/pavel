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
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;
using System.Windows.Forms;
using Pavel.Framework;
using Tao.FreeGlut;

namespace Pavel.GUI.Visualizations {
    /// <summary>
    /// Actual visualization of the ScatterMatrix.
    /// </summary>
    public class ScatterMatrixControl : OpenGLControl {

        #region Fields

        // Legend as a ToolTip
        private Legend xLegend;
        private Legend yLegend;

        private ScatterMatrix vis;
        private Vector mouseDownStartPoint;
        private Vector mouseDownEndPoint;
        private short[][] vertexArray;
        private float xAxesSize, yAxesSize;
        // Distance between the scatterplots
        private int dist = 10;
        // Amount of columns and rows in the scattermatrix
        private int xDimension, yDimension;
        // The positions of the displayed and the selected scatterplots are stored in lists.
        // (0,0) is the bottom left corner. The x value is increased horizontally, the y value vertically.
        private List<System.Drawing.Point> displayed = new List<System.Drawing.Point>();
        private List<System.Drawing.Point> selected = new List<System.Drawing.Point>();
        // Stores the previous displayed-lists, when the user zooms in, to restore upon zooming out.
        private List<List<System.Drawing.Point>> zoomLog = new List<List<System.Drawing.Point>>();

        // Bitmap to replace the actual visualization during resize or move.
        private Bitmap resizeBitmap = null;

        #endregion

        #region Properties

        /// <value>Gets the visualization this control belongs to</value>
        public Visualization Visualization { get { return vis; } }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes the ScatterMatrixControl.
        /// The ScatterMatrix is initially square, so xDimension equals yDimension.
        /// All possible Scatterplots are displayed.
        /// </summary>
        /// <param name="vis">The ScatterMatrix this control belongs to</param>
        public ScatterMatrixControl(ScatterMatrix vis) : base() {
            this.vis = vis;
            this.xLegend = new Legend(this);
            this.yLegend = new Legend(this);
            this.Dock = DockStyle.Fill;
            this.xDimension = this.yDimension = vis.VisualizationWindow.Space.Dimension;
            // As the scattermtrix is originally square, all axes have the same size.
            // The size is calculated, so that xDimension plots plus the distances between them
            // fit into the window with the size of 800
            this.xAxesSize = this.yAxesSize = (800 - (this.xDimension - 1) * dist) / this.xDimension;
            // Initializes the displayed-list with all possible scatterplots
            for (int column = 0; column < this.yDimension; column++) {
                for (int row = 0; row < this.xDimension; row++) {
                    this.displayed.Add(new System.Drawing.Point(row, column));
                }
            }
            this.CreateVertexArray();
            this.SetView();

            this.MouseDown += this.MouseDownEvent;
            this.MouseMove += this.MouseMoveEvent;
            this.MouseUp += this.MouseUpEvent;
            this.vis.VisualizationWindow.ResizeBegin += this.BeginResize;
            this.vis.VisualizationWindow.ResizeEnd += this.EndResize;
        }

        /// <summary>
        /// Sets the list of scatterplots displayed in the scattermatrix and calculates the
        /// number of rows and columns in the scattermatrix and the corresponding sizes of the axes.
        /// Then the vertexArray is recreated.
        /// </summary>
        /// <param name="displayed">List of scatterplots to be displayed</param>
        /// <param name="selected">List of selected scatterplots</param>
        public void Reconstruct(List<System.Drawing.Point> displayed, List<System.Drawing.Point> selected) {
            this.displayed = displayed;
            AdaptPointSize();
            if (displayed.Count >= 225) { vis.PointSize = 3.0f; } else if (displayed.Count <= 16) { vis.PointSize = 10.0f; } else { vis.PointSize = 3.0f - 7.0f * ((float)displayed.Count - 225.0f) / 209.0f; }
            this.selected = selected;
            // The dimensions are calculated, so that the resulting scattermatrix is roughly square,
            // with more columns than rows and as little free elements as possible.
            // The xDimension is the next lower integer of the division of the number of
            // displayed scatterplots by its squareroot rounded to the next lower integer.
            this.xDimension = (int)(displayed.Count / (int)Math.Sqrt(displayed.Count));
            // The yDimension is the squareroot of the number of displayed scatterplots rounded
            // to the next lower integer.
            this.yDimension = (int)Math.Sqrt(displayed.Count);
            // This adds the rows and columns lost by the rounding needed
            // to fit all displayed scatterplots into the scattermatrix.
            if (xDimension * yDimension < displayed.Count) {
                if (yDimension < xDimension) { this.yDimension++; } else { this.xDimension++; }
            }
            // The axesSizes are calculated, so that amount of plots equalling the corresponding
            // dimension plus the distances between them fits into the window with the size of 800
            this.xAxesSize = (int)((800 - (this.xDimension - 1) * dist) / this.xDimension);
            this.yAxesSize = (int)((800 - (this.yDimension - 1) * dist) / this.yDimension);
            this.CreateVertexArray();
        }

        /// <summary>
        /// Adapts the size of the points in the ScatterMatrix according to the amount of Scatterplots displayed.
        /// If at least 225 Scatterplots are displayed, size 3 is chosen.
        /// For 16 Scatterplots or less, size 10 is adequate.
        /// Between these amounts the size is calculated by the formula vis.PointSize = 3 - 7 * (displayed.Count - 225) / 209.
        /// </summary>
        public void AdaptPointSize() {
            if (displayed.Count >= 225) { vis.PointSize = 3; }
            else if (displayed.Count <= 16) { vis.PointSize = 10; }
            else { vis.PointSize = 3 - 7 * (displayed.Count - 225) / 209; }
            PavelMain.MainWindow.PropertyControl.VisualizationWindow = vis.VisualizationWindow;
        }

        /// <summary>
        /// Updates the legend with the axes of the ScatterPlot at the mouse-location.
        /// The minimum and maximum displayed values of the axes are also displayed.
        /// </summary>
        /// <param name="selectedPlot">The position of the selected ScatterPlot in the displayed-list</param>
        private void SetLegendText(int selectedPlot) {
            this.xLegend.ToolTipText.Items.Clear();
            this.xLegend.ToolTipText.Items.Add(vis.VisualizationWindow.Space.ColumnProperties[displayed[selectedPlot].X].ToString()
                + "  (" + vis.VisualizationWindow.Space.ColumnProperties[displayed[selectedPlot].X].Min + " , "
                + vis.VisualizationWindow.Space.ColumnProperties[displayed[selectedPlot].X].Max + ")");
            this.yLegend.ToolTipText.Items.Clear();
            this.yLegend.ToolTipText.Items.Add(vis.VisualizationWindow.Space.ColumnProperties[displayed[selectedPlot].Y].ToString()
                + "  (" + vis.VisualizationWindow.Space.ColumnProperties[displayed[selectedPlot].Y].Min + " , "
                + vis.VisualizationWindow.Space.ColumnProperties[displayed[selectedPlot].Y].Max + ")");
        }

        #endregion

        #region Methods

        #region VertexArray

        /// <summary>
        /// Creates the twodimensional vertexArray. The array contains an array for every Point in the PointSet,
        /// containing the position of that Point in the displayed ScatterPlots.
        /// </summary>
        private void CreateVertexArray() {
            PointSet ps = vis.VisualizationWindow.DisplayedPointSet;
            ColumnProperty[] cp = vis.VisualizationWindow.Space.ColumnProperties;
            double scaledX, scaledY;
            this.vertexArray = new short[ps.Length][];
            for (int i = 0; i < this.vertexArray.Length; i++) {
                this.vertexArray[i] = new short[2 * displayed.Count];
            }
            PavelMain.MainWindow.StatusBar.StartProgressBar(0, ps.Length, "Create Vertex Array...");
            int[] map = vis.VisualizationWindow.Space.CalculateMap(ps.ColumnSet);
            for (int pointIndex = 0; pointIndex < ps.Length; pointIndex++) {

                if (pointIndex % 100 == 0) PavelMain.MainWindow.StatusBar.IncrementProgressBar(100);

                for (int dispIndex = 0; dispIndex < displayed.Count; dispIndex++) {
                    scaledX = ps[pointIndex].ScaledValue(map[displayed[dispIndex].X], cp[displayed[dispIndex].X]);
                    scaledY = ps[pointIndex].ScaledValue(map[displayed[dispIndex].Y], cp[displayed[dispIndex].Y]);
                
                    // pointIndex determines the position in the pointset.
                    // ((dispIndex % xDimension) * (xAxesSize + dist)) calculates the location of
                    // the origin (x axis) of the current scatterplot.
                    // ((dispIndex / xDimension) * (yAxesSize + dist)) calculates the location of
                    // the origin (y axis) of the current scatterplot.
                    // The point position is normalized and scaled to the AxesSize - 4.
                    // In the end 2 is added. This, in combination with the scaling creates a slight
                    // offset, shifting the extreme points into the boxes, instead of placing them on
                    // the box-lines.
                    if ((scaledX >= 0) && (scaledX <= 1) && (scaledY >= 0) && (scaledY <= 1)) {
                        this.vertexArray[pointIndex][2 * dispIndex] = (short)(((dispIndex % xDimension) * (xAxesSize + dist)) + (scaledX * (xAxesSize - 4)) + 2);
                        this.vertexArray[pointIndex][2 * dispIndex + 1] = (short)(((dispIndex / xDimension) * (yAxesSize + dist)) + (scaledY * (yAxesSize - 4)) + 2);
                    } else {
                        this.vertexArray[pointIndex][2 * dispIndex] = -100;
                        this.vertexArray[pointIndex][2 * dispIndex + 1] = -100;
                    }
                }
            }
            PavelMain.MainWindow.StatusBar.EndProgressBar();
        }

        #endregion

        #region OpenGL

        /// <summary>
        /// Set view to correct size.
        /// </summary>
        private void SetView() {
            Gl.glViewport(0, 0, this.Width, this.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            // Nice border around the actual picture.
            Gl.glOrtho(-dist, xDimension * (xAxesSize + dist), -2 * dist, yDimension * (yAxesSize + dist), 1, -1);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glFlush();
        }

        protected override void InitOpenGL() {
            this.SetBackColor(ColorManagement.BackgroundColor);
            Gl.glShadeModel(Gl.GL_FLAT);
            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        protected override void RenderScene() {
            this.Cursor = Cursors.WaitCursor;
            this.SetView();
            Gl.glDrawBuffer(Gl.GL_BACK);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glPushMatrix();

            this.DrawAxes();
            this.DrawPoints(Gl.GL_RENDER);
            this.DrawLabels();
            this.MarkSelected();

            Gl.glPopMatrix();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Set the ClearColor for OpenGl to the given ColorOGL.
        /// </summary>
        /// <param name="back"></param>
        public void SetBackColor(ColorOGL back) {
            Gl.glClearColor(back.R, back.G, back.B, back.Color.A);
        }

        #endregion

        #region Drawing

        protected override void SetupModelViewMatrixOperations() {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Draws the points for each scatterplot from the verteyArray,
        /// so that the points of the CurrentCollection are marked.
        /// </summary>
        /// <param name="mode">Can be either GL_SELECT od GL_RENDER</param>
        private void DrawPoints(int mode) {
            Gl.glPointSize(vis.PointSize);
            Gl.glColor4f(ColorManagement.UnselectedColor.R, ColorManagement.UnselectedColor.G, ColorManagement.UnselectedColor.B, vis.AlphaPoints);
            for (int i = 0; i < this.vertexArray.Length; i++) {
                if (mode == Gl.GL_SELECT) { Gl.glLoadName(i); }
                if (!(ProjectController.CurrentSelection.Contains(vis.VisualizationWindow.DisplayedPointSet[i]))) {
                    Gl.glVertexPointer(2, Gl.GL_SHORT, 0, this.vertexArray[i]);
                    Gl.glDrawArrays(Gl.GL_POINTS, 0, this.displayed.Count);
                }
            }
            // The selected points are drawn last, so that they are on top of the other points.
            Gl.glColor4f(ColorManagement.CurrentSelectionColor.R, ColorManagement.CurrentSelectionColor.G, ColorManagement.CurrentSelectionColor.B, vis.AlphaPoints);
            for (int i = 0; i < this.vertexArray.Length; i++) {
                if (ProjectController.CurrentSelection.Contains(vis.VisualizationWindow.DisplayedPointSet[i])) {
                    Gl.glVertexPointer(2, Gl.GL_SHORT, 0, this.vertexArray[i]);
                    Gl.glDrawArrays(Gl.GL_POINTS, 0, this.displayed.Count);
                }
            }
        }

        /// <summary>
        /// Draws the box for each scatterplot.
        /// </summary>
        private void DrawAxes() {
            Gl.glColor3fv(ColorManagement.AxesColor.RGB);
            for (int x = 0; x < this.xDimension; x++) {
                for (int y = 0; y < this.yDimension; y++) {
                    int displayPos = y * xDimension + x;
                    if (displayPos < displayed.Count) {
                        Gl.glBegin(Gl.GL_LINES);
                        Gl.glVertex2f(x * (this.xAxesSize + dist), y * (this.yAxesSize + dist));
                        Gl.glVertex2f(x * (this.xAxesSize + dist) + this.xAxesSize, y * (this.yAxesSize + dist));

                        Gl.glVertex2f(x * (this.xAxesSize + dist) + this.xAxesSize, y * (this.yAxesSize + dist));
                        Gl.glVertex2f(x * (this.xAxesSize + dist) + this.xAxesSize, y * (this.yAxesSize + dist) + this.yAxesSize);

                        Gl.glVertex2f(x * (this.xAxesSize + dist) + this.xAxesSize, y * (this.yAxesSize + dist) + this.yAxesSize);
                        Gl.glVertex2f(x * (this.xAxesSize + dist), y * (this.yAxesSize + dist) + this.yAxesSize);

                        Gl.glVertex2f(x * (this.xAxesSize + dist), y * (this.yAxesSize + dist) + this.yAxesSize);
                        Gl.glVertex2f(x * (this.xAxesSize + dist), y * (this.yAxesSize + dist));
                        Gl.glEnd();
                    }
                }
            }
        }

        /// <summary>
        /// Draws the labels indicating the axes displayed in each scatterplot.
        /// </summary>
        private void DrawLabels() {
            Gl.glColor3fv(ColorManagement.DescriptionColor.RGB);
            for (int x = 0; x < this.xDimension; x++) {
                for (int y = 0; y < this.yDimension; y++) {
                    int displayPos = y * xDimension + x;
                    if (displayPos < displayed.Count) {
                        Gl.glRasterPos2f(x * (this.xAxesSize + dist) + (this.xAxesSize / 2), y * (this.yAxesSize + dist) - 7);
                        Glut.glutBitmapString(Glut.GLUT_BITMAP_HELVETICA_10, displayed[displayPos].X.ToString());
                        Gl.glRasterPos2f(x * (this.xAxesSize + dist) - 3, y * (this.yAxesSize + dist) + (this.yAxesSize / 2) - 7);
                        Glut.glutBitmapString(Glut.GLUT_BITMAP_HELVETICA_10, displayed[displayPos].Y.ToString());
                    }
                }
            }
        }

        #endregion

        #region Selection

        /// <summary>
        /// Checks, whether the rectangle defined by start and end includes any scatterplots.
        /// </summary>
        /// <param name="start">OpenGL location of start-point</param>
        /// <param name="end">OpenGL location of end-point</param>
        /// <returns>true, if the location is within one of the ScatterPlots</returns>
        private bool Hit(Vector start, Vector end) {
            bool hit = true;
            if ((this.GetScatterplot(this.GetScatterplot(start)) >= displayed.Count) && (this.GetScatterplot(this.GetScatterplot(end)) >= displayed.Count)) {
                return false;
            }
            if (((start.X < 0) && (end.X < 0)) ||
                ((start.X > this.xDimension * (this.xAxesSize + dist) - dist) && (end.X > this.xDimension * (this.xAxesSize + dist) - dist)) ||
                ((start.Y < 0) && (end.Y < 0)) ||
                ((start.Y > this.yDimension * (this.yAxesSize + dist) - dist) && (end.Y > this.yDimension * (this.yAxesSize + dist) - dist))) {
                return false;

            }
            for (int x = 0; x < this.xDimension - 1; x++) {
                for (int y = 0; y < this.yDimension - 1; y++) {
                    if (((start.X >= x * (this.xAxesSize + dist) + this.xAxesSize) && (start.X <= (x + 1) * (this.xAxesSize + dist)) &&
                        (end.X >= x * (this.xAxesSize + dist) + this.xAxesSize) && (end.X <= (x + 1) * (this.xAxesSize + dist))) ||
                        ((start.Y >= y * (this.yAxesSize + dist) + this.yAxesSize) && (start.Y <= (y + 1) * (this.yAxesSize + dist)) &&
                        (end.Y >= y * (this.yAxesSize + dist) + this.yAxesSize) && (end.Y <= (y + 1) * (this.yAxesSize + dist)))) {
                        return false;
                    }
                }
            }
            return hit;
        }

        /// <summary>
        /// Determins the scatterplot at a position given as coordinates of the form.
        /// </summary>
        /// <param name="pos">Position in VisualizationWindow</param>
        /// <returns>Scatterplot at that position</returns>
        private System.Drawing.Point GetScatterplot(Vector pos) {
            System.Drawing.Point scatterplot = new System.Drawing.Point();

            if (pos.X / (xAxesSize + dist) > this.xDimension - 1) {
                scatterplot.X = this.xDimension - 1;
            } else if (pos.X / (xAxesSize + dist) < 0) {
                scatterplot.X = 0;
            } else {
                scatterplot.X = (int)(pos.X / (xAxesSize + dist));
            }
            if (pos.Y / (yAxesSize + dist) > this.yDimension - 1) {
                scatterplot.Y = this.yDimension - 1;
            } else if (pos.Y / (yAxesSize + dist) < 0) {
                scatterplot.Y = 0;
            } else {
                scatterplot.Y = (int)(pos.Y / (yAxesSize + dist));
            }
            for (int x = 0; x < this.xDimension; x++) {
                for (int y = 0; y < this.yDimension; y++) {
                    if ((pos.X >= x * (this.xAxesSize + dist) - dist / 2) && (pos.X <= x * (this.xAxesSize + dist) + this.xAxesSize + dist / 2)
                        && (pos.Y >= y * (this.yAxesSize + dist) - dist / 2) && (pos.Y <= y * (this.yAxesSize + dist) + this.yAxesSize + dist / 2)) {
                        return new System.Drawing.Point(x, y);
                    }
                }
            }
            return scatterplot;
        }

        /// <summary>
        /// Calculates the position of the scatterplot in the scattermatrix.
        /// </summary>
        /// <param name="sel">Scatterplotposition as integer</param>
        /// <returns>Matrixposition of that scatterplot</returns>
        private System.Drawing.Point GetScatterplot(int sel) {
            int x = sel % this.xDimension;
            int y = (int)(sel / this.xDimension);
            System.Drawing.Point pos = new System.Drawing.Point(x, y);
            return pos;
        }

        /// <summary>
        /// Calculates the potential position in the displayed-list.
        /// </summary>
        /// <remarks>If not all positions in the ScatterMatrix are filled, the returned
        /// value may be larger than displayed.Count. Make sure you check that before accessing
        /// the displayed-list with this value.
        /// </remarks>
        /// <param name="pos">Matrixposition of the scatterplot</param>
        /// <returns>Scatterplotposition as integer</returns>
        private int GetScatterplot(System.Drawing.Point pos) {
            return pos.Y * xDimension + pos.X;
        }

        /// <summary>
        /// Adds the selected scatterplots to the selected list,
        /// or removes them, if they were already selected.
        /// </summary>
        /// <param name="start">startpoint in OpenGL coordinates</param>
        /// <param name="end">endpoint in OpenGL coordinates</param>
        private void PlotsPicking(Vector start, Vector end) {

            // Calculates the bottom-left and the top-right scatterplot of the selection-rectangle.
            double w = Math.Abs(start.X - end.X);
            double h = Math.Abs(start.Y - end.Y);
            Vector bl = new Vector((int)(start.X + end.X - w) / 2, (int)(start.Y + end.Y - h) / 2, 0);
            Vector tr = new Vector((int)(start.X + end.X + w) / 2, (int)(start.Y + end.Y + h) / 2, 0);
            System.Drawing.Point bottomLeft = GetScatterplot(bl);
            System.Drawing.Point topRight = GetScatterplot(tr);

            // If there are empty elements in the scattermatrix (e.g. 5 scatterplots will be displayed in a
            // 3x2 matrix leaving the last element empty) and the user selects one of the empty elements,
            // the selected-list is cleared.
            if ((bottomLeft == topRight) && (bottomLeft.Y * xDimension + bottomLeft.X >= displayed.Count)) {
                this.selected.Clear();
            }
            // Those displayed scatterplots contained in the selected rectangle are added to the selected-list,
            // or removed, if they are already in it.
            for (int i = bottomLeft.X; i <= topRight.X; i++) {
                for (int j = bottomLeft.Y; j <= topRight.Y; j++) {
                    int displayPos = j * xDimension + i;
                    if (displayPos < displayed.Count) {
                        System.Drawing.Point element = displayed[displayPos];
                        if (selected.Contains(element)) {
                            selected.Remove(element);
                        } else { selected.Add(element); }
                    }
                }
            }
        }

        /// <summary>
        /// Sorts the items in the list by their position in the scattermatrix.
        /// The first item is the one in the bottom left corner.
        /// The list is sorted first by the position in the row and second by
        /// the position in the column.
        /// </summary>
        /// <param name="sel">A list of selected scatterplots as Points</param>
        /// <returns>The sorted list</returns>
        private List<System.Drawing.Point> SortSelection(List<System.Drawing.Point> sel) {
            List<System.Drawing.Point> sorted = new List<System.Drawing.Point>();
            for (int y = 0; y < vis.VisualizationWindow.Space.Dimension; y++) {
                for (int x = 0; x < vis.VisualizationWindow.Space.Dimension; x++) {
                    System.Drawing.Point temp = new System.Drawing.Point(x, y);
                    if (selected.Contains(temp)) {
                        sorted.Add(temp);
                    }
                }
            }
            return sorted;
        }

        /// <summary>
        /// Determins the points contained in the selection-rectangle.
        /// </summary>
        /// <param name="x">x-position of the center of the selection-rectangle</param>
        /// <param name="y">y-position of the center of the selection-rectangle</param>
        /// <param name="w">width of the selection-rectangle</param>
        /// <param name="h">heigth of the selection-rectangle</param>
        private void PointsPicking(int x, int y, double w, double h) {
            int length = vis.VisualizationWindow.DisplayedPointSet.Length * 4;
            int[] selectBuffer = new int[length];
            int hits;
            int[] viewport = new int[4];

            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

            Gl.glSelectBuffer(length, selectBuffer);
            Gl.glRenderMode(Gl.GL_SELECT);

            Gl.glInitNames();
            Gl.glPushName(0);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            // Creates a picking region with the dimensions of the selection-rectangle.
            Glu.gluPickMatrix((double)x, (double)(viewport[3] - y), w, h, viewport);
            Gl.glOrtho(-dist, xDimension * (xAxesSize + dist), -2 * dist, yDimension * (yAxesSize + dist), 1, -1);

            this.DrawPoints(Gl.GL_SELECT);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();

            hits = Gl.glRenderMode(Gl.GL_RENDER);
            ProcessPointHits(hits, selectBuffer);
        }

        /// <summary>
        /// Adds the selected points to the CurrentSelection.
        /// </summary>
        /// <param name="hits">Number of selected points</param>
        /// <param name="selectBuffer">Buffer of the selected points</param>
        private void ProcessPointHits(int hits, int[] selectBuffer) {
            int iID = selectBuffer[3];
            if ((Control.ModifierKeys != Keys.Control || Control.ModifierKeys != Keys.Shift) && hits == 0) {
                ProjectController.CurrentSelection.Clear();
            }
            if (hits > 0) {
                Pavel.Framework.Point[] selectedPointsBuffer = new Pavel.Framework.Point[hits];
                for (int i = 0; i < hits; i++) {
                    iID = selectBuffer[i * 4 + 3];
                    selectedPointsBuffer[i] = vis.VisualizationWindow.DisplayedPointSet[iID];
                }
                if (Control.ModifierKeys == Keys.Shift) {
                    ProjectController.CurrentSelection.AddRange(selectedPointsBuffer);
                } else if (Control.ModifierKeys == Keys.Control) {
                    ProjectController.CurrentSelection.RemovePoints(selectedPointsBuffer);
                } else {
                    ProjectController.CurrentSelection.ClearAndAddRange(selectedPointsBuffer);
                }
            }
        }

        #endregion

        #region Marking

        /// <summary>
        /// Marks the selected scatterplots with a rectangle drawn in XOR mode.
        /// If the scatterplots are already selected, they are deselected.
        /// </summary>
        public void MarkSelected() {
            if (selected.Count > 0) {
                for (int i = 0; i < selected.Count; i++) {
                    float[] corners;
                    corners = this.CalculateRect(GetScatterplot(displayed.IndexOf(selected[i])));
                    Gl.glEnable(Gl.GL_COLOR_LOGIC_OP);
                    Gl.glLogicOp(Gl.GL_XOR);
                    Gl.glColor4f(vis.SelectionColor.R, vis.SelectionColor.G, vis.SelectionColor.B, vis.AlphaPoints);
                    Gl.glDrawBuffer(Gl.GL_FRONT_AND_BACK);
                    Gl.glRectf(corners[0], corners[1], corners[2], corners[3]);
                    Gl.glDisable(Gl.GL_COLOR_LOGIC_OP);
                    Gl.glFlush();
                }
            }
        }

        /// <summary>
        /// Returns the bottom-left and top-right corner of a given scatterplot in an int array.
        /// </summary>
        /// <param name="sel">Scatterplotposition as Integer</param>
        /// <returns>bottom-left and top-right corner of that scatterplot in an int array</returns>
        private float[] CalculateRect(System.Drawing.Point sel) {
            float[] value = new float[4];
            value[0] = sel.X * (xAxesSize + dist);
            value[1] = sel.Y * (yAxesSize + dist);
            value[2] = sel.X * (xAxesSize + dist) + xAxesSize;
            value[3] = sel.Y * (yAxesSize + dist) + yAxesSize;
            return value;
        }

        #endregion

        #region Updating

        /// <summary>
        /// Resets the view to display all scatterplots again and clears the selected-list.
        /// </summary>
        public void ResetView() {
            vis.ResetProperties();
            if (zoomLog.Count != 0) {
                this.Reconstruct(zoomLog[zoomLog.Count - 1], new List<System.Drawing.Point>());
                this.zoomLog.Clear();
            } else {
                this.selected.Clear();
            }
            this.vis.VisualizationWindow.Refresh();
        }

        /// <summary>
        /// Makes the necessary changes, when the Space is changed.
        /// </summary>
        public void UpdateSpace() {
            this.MakeCurrentContext();
            //this.SetToolTipText();
            this.displayed.Clear();
            this.selected.Clear();
            this.zoomLog.Clear();
            this.InitOpenGL();
            this.xDimension = this.yDimension = vis.VisualizationWindow.Space.Dimension;
            this.xAxesSize = this.yAxesSize = (int)((800 - (this.xDimension - 1) * dist) / this.xDimension);
            for (int column = 0; column < this.yDimension; column++) {
                for (int row = 0; row < this.xDimension; row++) {
                    this.displayed.Add(new System.Drawing.Point(row, column));
                }
            }
            this.CreateVertexArray();
            this.ResetView();
            Refresh();
        }

        /// <summary>
        /// Makes the necessary changes, when the PointSet is changed.
        /// </summary>
        public void UpdatePointSet() {
            this.MakeCurrentContext();
            this.CreateVertexArray();
            Refresh();
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Calculates the given screen position into position on OpenGL-Viewport.
        /// </summary>
        /// <param name="x">X-Value</param>
        /// <param name="y">Y-Value</param>
        /// <returns>Calculated Vector</returns>
        protected Vector GetOGLPos(int x, int y) {
            Vector OGLPoint;
            int[] viewport = new int[4];
            double[] modelview = new double[16];
            double[] projection = new double[16];
            float winX, winY, winZ=0;
            double posX, posY, posZ;

            Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelview);
            Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projection);
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

            winX = (float)x;
            winY = (float)viewport[3] - (float)y;
            Gl.glReadPixels(x, (int)winY, 1, 1, Gl.GL_DEPTH_COMPONENT, Gl.GL_FLOAT, winZ);

            Glu.gluUnProject(winX, winY, winZ, modelview, projection, viewport, out posX, out posY, out posZ);

            OGLPoint = new Vector((int)posX, (int)posY, (int)posZ);

            return OGLPoint;
        }

        /// <summary>
        /// Calculates the screen location of the given OpenGL point.
        /// </summary>
        /// <param name="oglPoint">The Sytem.Drawing.Point in OpenGL coordinates</param>
        /// <returns>The Location of the oglPoint in screen coordinates (with respect to the OpenGLControl)</returns>
        protected System.Drawing.Point GetScreenPos(Vector oglPoint) {
            int[] viewport = new int[4];
            double[] modelview = new double[16];
            double[] projection = new double[16];
            double winX, winY, winZ;

            Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelview);
            Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projection);
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

            Glu.gluProject((double)oglPoint.X, (double)oglPoint.Y, (double)oglPoint.Z, modelview, projection, viewport, out winX, out winY, out winZ);

            return new System.Drawing.Point((int)winX, viewport[3] - (int)winY);
        }
        #endregion

        #endregion

        #region Event Handling Stuff

        #region Mouse

        /// <summary>
        /// Sets a new mouseDownStartPoint.
        /// Displays the legend of the selected plot, if the right MouseButton is clicked.
        /// </summary>
        /// <param name="sender">MouseButton</param>
        /// <param name="ev">MouseEventArgs</param>
        private void MouseDownEvent(object sender, MouseEventArgs ev) {
            MakeCurrentContext();
            if (ev.Button == MouseButtons.Left) {
                this.mouseDownStartPoint = new Vector(ev.X, ev.Y, 0);
            }
            if (ev.Button == MouseButtons.Right) {
                if (this.Hit(this.GetOGLPos(ev.X, ev.Y), this.GetOGLPos(ev.X, ev.Y))) {
                    System.Drawing.Point selectedPlot = this.GetScatterplot(this.GetOGLPos(ev.X, ev.Y));
                    // The real ScatterPlot-position (i.e. the position in the complete matrix).
                    int realPlot = selectedPlot.Y * xDimension + selectedPlot.X;
                    if (realPlot < displayed.Count) {
                        this.SetLegendText(realPlot);

                        System.Drawing.Point xLoc =
                            this.GetScreenPos(new Vector(selectedPlot.X * (int)(this.xAxesSize + dist) + (int)this.xAxesSize,
                            selectedPlot.Y * (int)(this.yAxesSize + dist),
                            0));
                        System.Drawing.Point yLoc =
                            this.GetScreenPos(new Vector(selectedPlot.X * (int)(this.xAxesSize + dist),
                            selectedPlot.Y * (int)(this.yAxesSize + dist) + (int)this.yAxesSize,
                            0));

                        this.xLegend.ShowMe(xLoc, 1000);
                        this.yLegend.ShowMe(yLoc, 1000);
                    }
                }
            }
        }


        /// <summary>
        /// Paints a rectangle to mark the selected ScatterPlots in the Scattermatrix.
        /// Erases a previous rectangle, if it exists.
        /// </summary>
        /// <param name="sender">MouseButton</param>
        /// <param name="ev">MouseEventArgs</param>
        private void MouseUpEvent(object sender, MouseEventArgs ev) {
            if (mouseDownStartPoint == null) // See end of the function for the reason of the test.
                return;
            if (ev.Button == MouseButtons.Left) {
                // Clears the selected-list, if the control key is not pressed.
                if (Control.ModifierKeys != Keys.Shift) {
                    System.Drawing.Point downP = GetScatterplot(this.GetOGLPos(ev.X, ev.Y));
                    int sel = downP.Y * xDimension + downP.X;
                    if (sel < displayed.Count) {
                        if (!((selected.Count == 1) && (selected[0] == displayed[sel]))) {
                            this.selected.Clear();
                        }
                    }
                }
                if (mouseDownEndPoint == null) {
                    mouseDownEndPoint = mouseDownStartPoint;
                }
                Gl.glEnable(Gl.GL_COLOR_LOGIC_OP);
                Gl.glLogicOp(Gl.GL_XOR);
                Gl.glColor3f(1.0f, 0.0f, 0.0f);
                Gl.glDrawBuffer(Gl.GL_FRONT);
                // Reset
                Vector end = this.GetOGLPos(mouseDownEndPoint.X, this.mouseDownEndPoint.Y);
                Vector down = this.GetOGLPos(mouseDownStartPoint.X, this.mouseDownStartPoint.Y);
                Gl.glRectf(down.X, down.Y, end.X, end.Y);
                Gl.glDisable(Gl.GL_COLOR_LOGIC_OP);
                if ((vis.PickMode == ScatterMatrix.ScattermatrixPickModes.Plots) && (this.Hit(down, end))) {
                    this.PlotsPicking(down, end);
                }
                mouseDownEndPoint = null;
                Gl.glFlush();
                if ( vis.PickMode == ScatterMatrix.ScattermatrixPickModes.Points ) {
                    this.Cursor = Cursors.WaitCursor;
                    if ((ev.X == this.mouseDownStartPoint.X) && (ev.Y == this.mouseDownStartPoint.Y)) {
                        this.PointsPicking(ev.X, ev.Y, 5.0, 5.0);
                        this.Invalidate();
                    } else { //Mouse Drag
                        double w = Math.Abs(ev.X - this.mouseDownStartPoint.X);
                        double h = Math.Abs(ev.Y - this.mouseDownStartPoint.Y);
                        //Check for minimum selection area (5x5)
                        if (w < 5) { w = 5; }
                        if (h < 5) { h = 5; }
                        this.PointsPicking((this.mouseDownStartPoint.X + ev.X) / 2, (this.mouseDownStartPoint.Y + ev.Y) / 2, w, h);
                        this.Invalidate();
                    }
                    this.Cursor = Cursors.Default;
                }

                this.Invalidate();
            }

            // If the user double clicked the title bar, C# will send a MouseUp event, but no MouseDown event.
            // So mouseDownStartPoint is set to null here. If the event is from the client area, mouseDownStartPoint
            // will not be null(it is set by MouseDownEvent). If the event is from the title bar, mouseDownStartPoint
            // will be null.
            mouseDownStartPoint = null;
        }

        /// <summary>
        /// Draws the selection-rectangle when the mouse is moved with the left button held down.
        /// </summary>
        /// <param name="sender">MouseButton</param>
        /// <param name="ev">MouseEventArgs</param>
        private void MouseMoveEvent(object sender, MouseEventArgs ev) {
            if (mouseDownStartPoint != null) {
                if (ev.Button == MouseButtons.Left && (ev.X != this.mouseDownStartPoint.X) && (ev.Y != this.mouseDownStartPoint.Y)) {

                    Vector downP, endP, curP;
                    downP = this.GetOGLPos(this.mouseDownStartPoint.X, this.mouseDownStartPoint.Y);
                    curP = this.GetOGLPos(ev.X, ev.Y);

                    Gl.glEnable(Gl.GL_COLOR_LOGIC_OP);
                    Gl.glLogicOp(Gl.GL_XOR);
                    Gl.glColor3f(1.0f, 0.0f, 0.0f);
                    Gl.glDrawBuffer(Gl.GL_FRONT);
                    // Reset old Rubberband
                    if (mouseDownEndPoint != null) {
                        endP = this.GetOGLPos(mouseDownEndPoint.X, this.mouseDownEndPoint.Y);
                        Gl.glRectf(downP.X, downP.Y, endP.X, endP.Y);
                    }
                    Gl.glRectf(downP.X, downP.Y, curP.X, curP.Y);
                    Gl.glDisable(Gl.GL_COLOR_LOGIC_OP);
                    Gl.glFlush();
                }
                mouseDownEndPoint = new Vector(ev.X, ev.Y, 0);
            }
        }

        #endregion

        #region Zoom

        /// <summary>
        /// Opens a ScatterPlot2D displaying the selected scatterplot if only one scatterplot is selected.
        /// Opens a ScatterMatrix displaying the selected scatterplots if more scatterplots are selected.
        /// If all scatterplots are selected, nothing happens.
        /// </summary>
        public void ShowSelected() {
            if (selected.Count == 1) {
                VisualizationWindow visWindow = new VisualizationWindow(
                        this.vis.VisualizationWindow.DisplayedPointSet, this.vis.VisualizationWindow.Space, "ScatterPlot");
                visWindow.MdiParent = PavelMain.MainWindow;
                System.Drawing.Point sel = selected[0];
                ScatterPlot sp = visWindow.Visualization as ScatterPlot;
                sp.AxisX = sp.VisualizationWindow.Space.ColumnProperties[sel.X];
                sp.AxisY = sp.VisualizationWindow.Space.ColumnProperties[sel.Y];
                sp.AxisZ = null;
                visWindow.EnableTab(PavelMain.MainWindow.TabControl);
                visWindow.Show();
            } else if ((selected.Count > 1) && (selected.Count <= displayed.Count)) {
                VisualizationWindow visWindow = new VisualizationWindow(
                        this.vis.VisualizationWindow.DisplayedPointSet, this.vis.VisualizationWindow.Space, "ScatterMatrix");
                visWindow.MdiParent = PavelMain.MainWindow;
                (visWindow.Visualization.Control as ScatterMatrixControl).Reconstruct(SortSelection(selected), SortSelection(selected));
                visWindow.EnableTab(PavelMain.MainWindow.TabControl);
                visWindow.Show();
            }
        }

        /// <summary>
        /// If at least one scatterplot and less than all scatterplots are selected,
        /// the selection is displayed in the same form and the previous display-list
        /// is added to the zoomLog.
        /// </summary>
        public void ZoomIn() {
            if ((selected.Count >= 1) && (selected.Count < displayed.Count)) {
                this.zoomLog.Insert(0, this.displayed);
                this.Reconstruct(SortSelection(selected), SortSelection(selected));
                this.vis.VisualizationWindow.Refresh();
            }
        }

        /// <summary>
        /// The previous zoom-level is displayed again and the first element of the
        /// zoomLog is removed.
        /// </summary>
        public void ZoomOut() {
            if (this.zoomLog.Count != 0) {
                this.Reconstruct(zoomLog[0], SortSelection(selected));
                this.zoomLog.RemoveAt(0);
                this.vis.VisualizationWindow.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Creates a screenshot and replaces the ParallelPlot3D by
        /// that screenshot when the form is resized or moved.
        /// </summary>
        /// <param name="sender">The VisualizationWindow</param>
        /// <param name="ev">ResizeBegin EventArgs</param>
        private void BeginResize(object sender, EventArgs ev) {
            resizeBitmap = this.Screenshot();
            this.Dock = DockStyle.None;
            vis.PictureBox.Image = resizeBitmap;
            vis.VisualizationWindow.Controls.Add(vis.PictureBox);
        }

        /// <summary>
        /// Replaces the screenshot by the ParallelPlot3D when the resize or move ends.
        /// </summary>
        /// <param name="sender">The VisualizationWindow</param>
        /// <param name="ev">ResizeEnd EventArgs</param>
        private void EndResize(object sender, EventArgs ev) {
            this.Dock = DockStyle.Fill;
            vis.VisualizationWindow.Controls.Remove(vis.PictureBox);
            resizeBitmap = null;
        }

        #endregion
    }
}