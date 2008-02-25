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
using System.IO;
using System.Windows.Forms;
using Pavel.GUI;
using System.ComponentModel;
using Pavel.GUI.Visualizations;
using Pavel.GUI.SolutionVisualizations;
using Tao.FreeGlut;
using Tao.OpenGl;

namespace Pavel.Plugins {
    class MillingSimulationSolutionControl : OpenGLControl {

        private int displayListBase;
        private MillingSimulationSolution solutionWindow;

        public MillingSimulationSolutionControl(MillingSimulationSolution solutionWindow) : base() {
            this.solutionWindow = solutionWindow;
            InitializeOpenGL();
            InitializeDisplayListIds();
            GenerateDisplayLists();
            RegisterEvents();
        }

        ~MillingSimulationSolutionControl() {
            MakeCurrentContext();
            Gl.glDeleteLists(displayListBase, solutionWindow.sortedProfiles.Length);
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            UnregisterEvents();
        }

        private void InitializeOpenGL() {
            this.keepAspect = false;
        }

        private void RegisterEvents() {
            foreach (MillingSimulationSolution.Profile profile in solutionWindow.sortedProfiles) {
                profile.Changed += profileChanged;
            }
        }

        private void UnregisterEvents() {
            foreach (MillingSimulationSolution.Profile profile in solutionWindow.sortedProfiles) {
                profile.Changed -= profileChanged;
            }
        }

        /// <summary>
        /// Handle Changes in a displayed profile's properties.
        /// </summary>
        void profileChanged(object sender, EventArgs e) {
            Invalidate();
        }

        internal void ProfileColumnChanged() {
            GenerateDisplayLists();
            Invalidate();
        }

        /// <summary>
        /// Reserve as many displaylist ID's as necessary for displaying all profiles
        /// and asssign those displaylist ID's to the Profiles.
        /// </summary>
        private void InitializeDisplayListIds() {
            MakeCurrentContext();
            displayListBase = Gl.glGenLists(solutionWindow.sortedProfiles.Length);
            int list = displayListBase;
            foreach (MillingSimulationSolution.Profile p in solutionWindow.sortedProfiles) {
                p.displaylist = list++;
            }
        }

        private void GenerateDisplayLists() {
            foreach (MillingSimulationSolution.Profile p in solutionWindow.sortedProfiles) {
                GenerateDisplayList(p);
            }
        }

        /// <summary>
        /// Generate the displaylist for a profile.
        /// </summary>
        /// <param name="profile"></param>
        private void GenerateDisplayList(MillingSimulationSolution.Profile profile) {
            MakeCurrentContext();
            Gl.glNewList(profile.displaylist, Gl.GL_COMPILE);
            profile.Render(solutionWindow.ProfileColumn);
            Gl.glEndList();
        }

        protected override void SetupModelViewMatrixOperations() {
            Gl.glLoadIdentity();
        }

        /// <summary>
        /// Render the displaylists containing the profiles and the scales.
        /// </summary>
        protected override void RenderContent() {
            PushMatrices();
            SetupProjectionFlat(true, false, false);
            int numProfiles = solutionWindow.sortedProfiles.Length;
            double profileHeight = 1d / numProfiles;
            int i = 0;
            Gl.glEnable(Gl.GL_CLIP_PLANE0);
            Gl.glEnable(Gl.GL_CLIP_PLANE1);
            foreach (MillingSimulationSolution.Profile prof in solutionWindow.sortedProfiles) {
                Gl.glPushMatrix();
                //Shift to correct Position
                Gl.glTranslated(0d, i * profileHeight, 0d);
                Gl.glScaled(1d, profileHeight, 1d);

                //Center around 0 in y-direction
                Gl.glTranslated(0d, 0.5f, 0d);

                //Place Clipping Planes to stop bleeding the profiles into each other
                Gl.glClipPlane(Gl.GL_CLIP_PLANE0, new double[] { 0,  1, 0, 0.5f });
                Gl.glClipPlane(Gl.GL_CLIP_PLANE1, new double[] { 0, -1, 0, 0.5f });

                //Profile Specific ScaleTrans
                Gl.glPushMatrix();
                Gl.glScaled(prof.xScale, (float)solutionWindow.YScale, 1d);
                Gl.glTranslated(prof.xTranslation, prof.yTranslation[solutionWindow.ProfileColumn], 0d);
                prof.RenderScales(solutionWindow.ProfileColumn, solutionWindow.scaleStepDistance);
                Gl.glCallList(prof.displaylist);
                Gl.glPopMatrix();

                //Borders between Profiles
                if (i != 0) {
                    Gl.glColor3fv(ColorManagement.CurrentSelectionColor.RGB);
                    Gl.glBegin(Gl.GL_LINES);
                        Gl.glVertex2d(0, -0.5);
                        Gl.glVertex2d(1, -0.5);
                    Gl.glEnd();
                }

                Gl.glPopMatrix();
                i++;
            }
            Gl.glDisable(Gl.GL_CLIP_PLANE0);
            Gl.glDisable(Gl.GL_CLIP_PLANE1);
            PopMatrices();
        }
    }
}
