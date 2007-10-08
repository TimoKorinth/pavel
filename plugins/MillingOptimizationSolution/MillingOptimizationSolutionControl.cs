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

namespace Pavel.Plugins {

    /// <summary>
    /// SingleSolutionControl: This shows a 3D-view of the single solution
    /// of a MillingOptimizationSolution. The workpiece has to be in STL, 
    /// the other geometry, here the milling tool, are calculated from 
    /// the decision space. The positions of the milling cutter has 
    /// to be used to visualize the milling at the right points and the tool information
    /// are used to create the right cutter.
    /// </summary>
    public class MillingOptimizationSolutionControl : OpenGLControl {

        #region Fields

        private MillingOptimizationSolution solution;
        private Pavel.Framework.Point[] points;
        private List<int> selectedPoints = new List<int>();
        private float[] posArray;
        private float[] toolArray;
        private float[] vertexArray;
        private float[] normalArray;
        private float xRot, yRot, xDiff, yDiff;
        private float zoom = 1f;
        private float factorLength = 0.5f;
        private float factorRadius = 20.0f;

        #endregion

        #region Constructors

        /// <summary>
        /// The control visualizing the MillingOptimizationSolution.
        /// </summary>
        /// <param name="solution"> The matching solution</param>
        /// <param name="vertexArray"> Array of the vertices of the workpiece</param>
        /// <param name="normalArray"> Array of the normal vectors of the workpiece</param>
        /// <param name="p"> Current point to draw</param>
        /// <param name="posArray"> Array of the position at the workpiece where the tool has to be </param>
        /// <param name="toolArray"> Array of information of the tool</param>
        public MillingOptimizationSolutionControl(MillingOptimizationSolution solution, float[] vertexArray, float[] normalArray, Pavel.Framework.Point[] p, float[] posArray, float[] toolArray) {
            this.solution = solution;
            this.vertexArray = vertexArray;
            this.normalArray = normalArray;
            this.points = p;
            this.posArray = posArray;
            this.toolArray = toolArray;
            this.MouseWheel += this.MouseWheelEvent;
            this.MouseDown += this.MouseDownEvent;
            this.MouseMove += this.MouseMoveEvent;
            this.Dock = DockStyle.Fill;
            solution.GlyphControl.UpdateGlyphs(GetGlyphPoints());
            RefreshArrays();
            SetLight();
        }

        #endregion

        #region Methods

        #region Drawing

        // OpenGLControl Member
        protected override void SetupModelViewMatrixOperations( ) {
            Gl.glScalef(zoom / 150f, zoom / 150f, zoom / 150f);
            Gl.glRotatef(-this.xRot, 1.0f, 0.0f, 0.0f);
            Gl.glRotatef(this.yRot, 0.0f, 0.0f, 1.0f);
            Gl.glTranslatef(0, 0, -(2f * halfHeight));
        }

        /// <summary>
        /// Initialize the properties of OpenGL
        /// </summary>
        protected override void InitOpenGL() {
            Gl.glClearColor(ColorManagement.BackgroundColor.R, ColorManagement.BackgroundColor.G, ColorManagement.BackgroundColor.B, 1.0f);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_NORMAL_ARRAY);
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            //Light settings
            float[] ambientLight = { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] diffuseLight = { 0.8f, 0.8f, 0.8f, 1.0f };
            float[] matSpecular = { 0.6f, 0.6f, 0.6f, 1f };
            float[] matShininess = { 40.0f };
            float[] lightPosition = { 70.0f, 0f, 120.0f, 1.0f };
            float[] lmodelAmbient = { 0.2f, 0.2f, 0.2f, 1.0f };
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, matSpecular);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, matShininess);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambientLight);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, diffuseLight);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPosition);
            Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, lmodelAmbient);
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
            DrawCoordinateSystem();
            DrawMilling();
            DrawWorkpieceSTL();
        }

        /// <summary>
        /// Main method to draw milling cutter.
        /// </summary>
        private void DrawMilling() {
            VectorF tmp = new VectorF();

            List<int> visColumns =
                (Pavel.Framework.ProjectController.Project.UseCase as MillingOptimizationUseCase).MillingColumns;
            int toolCount = (visColumns.Count / 2) - 1;

            int arrayposition;
            int betaPos;
            int gammaPos;

            ColorOGL color;

            for (int i = 0; i <= toolCount; i++) {
                arrayposition = i * 3;
                betaPos = visColumns[i * 2];
                gammaPos = visColumns[i * 2 + 1];
                switch (solution.SelectedMode) {
                    case MillingOptimizationSolution.Mode.CompareToMany: {
                            for (int j = 0; j < selectedPoints.Count; j++) {
                                tmp = CalculateToolDirection(points[selectedPoints[j]].Values[betaPos], points[selectedPoints[j]].Values[gammaPos]);
                                color = Pavel.GUI.Visualizations.ColorManagement.GetColor(j);
                                DrawSimpleTool(arrayposition, tmp, points[selectedPoints[j]].Values[gammaPos],
                                    points[selectedPoints[j]].Values[betaPos], color, false);
                            }
                            break;
                        }

                    case MillingOptimizationSolution.Mode.CompareToRef: {
                            //draw ReferencePoint
                            tmp = CalculateToolDirection(solution.ReferencePoint.Values[betaPos], solution.ReferencePoint.Values[gammaPos]);
                            color = Pavel.GUI.Visualizations.ColorManagement.CurrentSelectionColor;
                            DrawSimpleTool(arrayposition, tmp, solution.ReferencePoint.Values[gammaPos],
                                solution.ReferencePoint.Values[betaPos], color, true);

                            //draw CurrentPoint
                            if (solution.ReferencePoint != solution.CurrentPoint) {
                                tmp = CalculateToolDirection(solution.CurrentPoint.Values[betaPos], solution.CurrentPoint.Values[gammaPos]);
                                color = Pavel.GUI.Visualizations.ColorManagement.UnselectedColor;
                                DrawSimpleTool(arrayposition, tmp, solution.CurrentPoint.Values[gammaPos],
                                    solution.CurrentPoint.Values[betaPos], color, false);
                            }
                            break;
                        }

                    case MillingOptimizationSolution.Mode.Zapping: {
                            tmp = CalculateToolDirection(solution.CurrentPoint.Values[betaPos],
                                solution.CurrentPoint.Values[gammaPos]);
                            DrawTool(arrayposition, tmp, solution.CurrentPoint.Values[gammaPos],
                                solution.CurrentPoint.Values[betaPos]);
                            break;
                        }
                }
            }
            if (solution.SelectedMode == MillingOptimizationSolution.Mode.CompareToRef) {
                DrawMillingToolPlane(solution.ReferencePoint, visColumns);
            }

        }

        /// <summary>
        /// Draws the milling tool. It consists of a sphere with a shank attached to it and a frustum at the end of the shank.
        /// </summary>
        /// <param name="arrayposition">Position in the array </param>
        /// <param name="tmp"> The direction vector of the tool </param>
        /// <param name="gamma"> Angle from z-axis in direction to the y-axis </param>
        /// <param name="beta"> Angle around the z-axis </param>
        private void DrawTool(int arrayposition, VectorF tmp, double gamma, double beta) {
            int slices = 10;
            int stacks = 10;
            double length = this.toolArray[1]; //lenght of the cylinder
            double radius = this.toolArray[0]; //radius of the cylinder
            float cylinderlength = (float)(length / 4); //the length of the cylinder of the milling
            float conelength = (float)(length / 2); //the length of the bigger end of the milling cylinder
            float beginRadius = (float)(radius * 2); //the radius of the beginn of the bigger milling cylinder
            float endRadius = (float)(radius * 4); //the radius of the bigger end of the milling cylinder
            double sphereRadius = radius; //the radius of the sphere at the beginning of the milling

            //draws the sphere at the end of the milling
            Gl.glColor4f(1.0f, 1.0f, 0.0f, 1.0f);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glTranslatef(posArray[arrayposition] + tmp.X * (float)sphereRadius, posArray[arrayposition + 1] + tmp.Y * (float)sphereRadius, posArray[arrayposition + 2] + tmp.Z * (float)sphereRadius);
            Glut.glutSolidSphere(sphereRadius, slices, stacks);

            //draws the milling cylinder
            Gl.glRotated((gamma * 180.0 / Math.PI), 0, 0, 1);
            Gl.glRotated((beta * 180.0 / Math.PI), 0, 1, 0);

            Gl.glColor4f(1.0f, 1.0f, 1.0f, 1.0f);
            Glu.GLUquadric quadobj = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(quadobj, Glu.GLU_FILL);
            Glu.gluCylinder(quadobj, radius, radius, cylinderlength, slices, stacks);

            //draws the bigger end of the milling cylinder
            Gl.glTranslatef(0, 0, cylinderlength);
            Gl.glColor4f(1.0f, 0.0f, 0.0f, 1.0f);
            Glu.gluDisk(quadobj, 0, beginRadius, slices, stacks);
            Glu.gluCylinder(quadobj, beginRadius, endRadius, conelength, slices, stacks);

            //draws the disk at the end of the milling
            Gl.glTranslatef(0.0f, 0.0f, conelength);
            Glu.gluDisk(quadobj, 0, endRadius, slices, stacks);

            Glu.gluDeleteQuadric(quadobj);

            Gl.glPopMatrix();
        }

        /// <summary>
        /// Draws a simple milling tool. It consists only of a sphere with a small shank.
        /// </summary>
        /// <param name="arrayposition">Position in the array </param>
        /// <param name="tmp"> The direction vector of the tool </param>
        /// <param name="gamma"> Angle from z-axis in direction to the y-axis </param>
        /// <param name="beta"> Angle around the z-axis </param>
        /// <param name="color">Color of the simple milling tool</param>
        /// <param name="reference">True, if it is the reference point</param>
        private void DrawSimpleTool(int arrayposition, VectorF tmp, double gamma, double beta, ColorOGL color, bool reference) {
            int slices = 10;
            int stacks = 10;
            float tmpFactorRadius = factorRadius;
            if (reference) {
                tmpFactorRadius *= 5;
            }
            float cylinderlength = this.toolArray[1] * factorLength; //length of the cylinder
            float radius = this.toolArray[0] / tmpFactorRadius; //radius of the cylinder

            //draws the sphere at the end of the milling
            Gl.glCullFace(Gl.GL_BACK);
            Gl.glEnable(Gl.GL_CULL_FACE);
            Gl.glPushMatrix();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glColor4f(color.R, color.G, color.B, 0.5f);
            Gl.glTranslatef(posArray[arrayposition] + tmp.X * radius, posArray[arrayposition + 1] + tmp.Y * radius, posArray[arrayposition + 2] + tmp.Z * radius);
            Glut.glutSolidSphere(radius, slices, stacks);

            //draws the milling cylinder
            Gl.glRotated((gamma * 180.0 / Math.PI), 0, 0, 1);
            Gl.glRotated((beta * 180.0 / Math.PI), 0, 1, 0);

            Glu.GLUquadric quadobj = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(quadobj, Glu.GLU_FILL);
            Glu.gluCylinder(quadobj, radius, radius, cylinderlength, slices, stacks);

            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_CULL_FACE);
        }

        /// <summary>
        /// Draws a plane of the reference point on mode CompareToRef.
        /// </summary>
        /// <param name="p">Point to be displayed</param>
        /// <param name="visColumns">List of the visualization columns</param>
        private void DrawMillingToolPlane(Pavel.Framework.Point p, List<int> visColumns) {
            int toolcounts = (visColumns.Count - 1) / 2;
            ColorOGL color = Pavel.GUI.Visualizations.ColorManagement.CurrentSelectionColor;

            float planeLength = this.toolArray[1] * factorLength;
            VectorF first;
            int arrayposition;

            Gl.glBegin(Gl.GL_QUAD_STRIP);
            Gl.glColor4f(color.R, color.G, color.B, 0.5f);
            for (int i = 0; i <= toolcounts; i++) {
                arrayposition = i * 3;
                Gl.glVertex3f(posArray[arrayposition], posArray[arrayposition + 1], posArray[arrayposition + 2]);

                first = CalculateToolDirection(p.Values[visColumns[i * 2]], p.Values[visColumns[i * 2 + 1]]);
                first = first * planeLength;
                first = first + new VectorF(posArray[arrayposition], posArray[arrayposition + 1], posArray[arrayposition + 2]);
                Gl.glVertex3f(first.X, first.Y, first.Z);
            }
            Gl.glEnd();
        }

        /// <summary>
        /// Draws the workpiece.
        /// </summary>
        private void DrawWorkpieceSTL() {
            Gl.glPushMatrix();
            Gl.glPolygonMode(Gl.GL_BACK, Gl.GL_FILL);
            Gl.glTranslatef(-50f, -50f, -50f);
            Gl.glLineWidth(1.0f);
            Gl.glColor4f(0.1f, 0.5f, 0.5f, 0.5f);

            Gl.glVertexPointer(3, Gl.GL_FLOAT, 0, this.vertexArray);
            Gl.glNormalPointer(Gl.GL_FLOAT, 0, this.normalArray);
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, this.vertexArray.Length / 3);
            Gl.glPopMatrix();
        }

        /// <summary>
        /// Draws the coordinate system, the x-axis in red, the y-axis in green, the z-axis in blue
        /// </summary>
        private void DrawCoordinateSystem() {
            Gl.glLineWidth(1.0f);
            //x-axis in red
            Gl.glColor3f(1.0f, 0.0f, 0.0f);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3f(-200, 0, 0);
            Gl.glVertex3f(200, 0, 0);
            Gl.glEnd();

            //y-axis in green
            Gl.glColor3f(0.0f, 1.0f, 0.0f);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3f(0, -200, 0);
            Gl.glVertex3f(0, 200, 0);
            Gl.glEnd();

            //z-axis in blue
            Gl.glColor3f(0.0f, 0.0f, 1.0f);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3f(0, 0, -200);
            Gl.glVertex3f(0, 0, 200);
            Gl.glEnd();
        }

        #endregion

        /// <summary>
        /// Calculates the normal vectors for the vertices from those for the faces,
        /// depending on the value of solution.NicerRendering.
        /// </summary>
        internal void RefreshArrays() {
            normalArray = DirtyLittleOpenGlHelper.ExpandNormalArray(solution.SmoothRendering, solution.OrginalStlNormalArray, vertexArray);
        }

        /// <summary>
        /// Replaces the current Pavel.Framework.Point with the list of <paramref name="selected"/>.
        /// </summary>
        /// <param name="selected">List of new Pavel.Framework.Points</param>
        public void ChangePoint(List<int> selected) {
            selectedPoints.Clear();
            selectedPoints.AddRange(selected);
            solution.GlyphControl.UpdateGlyphs(GetGlyphPoints());
            this.Invalidate();
        }

        /// <summary>
        /// Calculates the tool direction by means of beta and gamma.
        /// </summary>
        /// <param name="beta"> Angle around the z-axis </param>
        /// <param name="gamma"> Angle from z-axis in direction to the y-axis </param>
        /// <returns> A vector of the direction, [0]: x-value, [1]: y-value, [2]: z-value </returns>
        private VectorF CalculateToolDirection(double beta, double gamma) {
            VectorF q = new VectorF();
            q.X = (float)(Math.Cos(gamma) * Math.Sin(beta));
            q.Y = (float)(Math.Sin(gamma) * Math.Sin(beta));
            q.Z = (float)(Math.Cos(beta));
            return q;
        }

        public List<Pavel.Framework.Point> GetGlyphPoints() {
            List<Pavel.Framework.Point> glyphPoints = new List<Pavel.Framework.Point>();

            switch (solution.SelectedMode) {
                case MillingOptimizationSolution.Mode.Zapping: {
                        glyphPoints.Add(solution.CurrentPoint);
                        break;
                    }
                case MillingOptimizationSolution.Mode.CompareToRef: {
                        glyphPoints.Add(solution.CurrentPoint);
                        if (solution.ReferencePoint != null) {
                            glyphPoints.Add(solution.ReferencePoint);
                        }
                        break;
                    }
                case MillingOptimizationSolution.Mode.CompareToMany: {
                        for (int i = 0; i < selectedPoints.Count; i++) {
                            glyphPoints.Add(points[selectedPoints[i]]);
                        }
                        break;
                    }

            }

            return glyphPoints;
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// When a button of the mouse is pressed, the values for the rotation are initialized.
        /// </summary>
        /// <param name="sender"> The mouse </param>
        /// <param name="ev"> Standard MouseEventArgs </param>
        private void MouseDownEvent(object sender, MouseEventArgs ev) {
            MakeCurrentContext();
            this.xDiff = ev.X - this.yRot;
            this.yDiff = ev.Y + this.xRot;
        }

        /// <summary>
        /// When the mouse is moved with the left or the right mouse button pressed, a rotation will be conducted.
        /// </summary>
        /// <param name="sender"> The mouse </param>
        /// <param name="ev"> Standard MouseEventArgs </param>
        private void MouseMoveEvent(object sender, MouseEventArgs ev) {
            if (ev.Button == MouseButtons.Left | ev.Button == MouseButtons.Right) {
                this.yRot = ev.X - this.xDiff;
                this.xRot = -ev.Y + this.yDiff;
                this.Invalidate();
            }
        }

        /// <summary>
        /// When the mouse wheel is moved, the view is zoomed in or out. 
        /// </summary>
        /// <param name="sender"> The mouse wheel </param>
        /// <param name="ev"> Standard MouseEventArgs </param>
        private void MouseWheelEvent(object sender, MouseEventArgs ev) {
            if ( zoom - ev.Delta / 1000f > 0.5f && zoom - ev.Delta / 1000f < 2f ) {
                this.zoom -= ev.Delta / 1000f;
            }
            this.Invalidate();
        }

        #endregion
    }
}
