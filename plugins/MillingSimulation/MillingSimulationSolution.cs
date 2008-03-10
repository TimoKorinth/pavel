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
using Tao.OpenGl;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Tao.FreeGlut;

namespace Pavel.Plugins {
    public class MillingSimulationSolution : Solution {

        private MillingSimulationSolutionControl solutionControl;
        internal Profile[] sortedProfiles;

        private double yScale;
        private Column profileColumn;
        internal int scaleSteps = 10;
        internal double scaleStepDistance;

        [ShowInProperties]
        [Category("Misc")]
        [DisplayName("y Scale")]
        [Description("Scale the Profiles in y-Direction")]
        public double YScale {
            get { return yScale; }
            set { yScale = value; solutionControl.Invalidate(); }
        }

        [ShowInProperties]
        [Category("Misc")]
        [DisplayName("Scalesteps")]
        [Description("Amount of horizontal scales in the plot")]
        public int ScaleSteps {
            get { return scaleSteps; }
            set { scaleSteps = value; InitializeScaleStepDistance(); solutionControl.Invalidate(); }
        }

        [ShowInProperties]
        [Category("Misc")]
        [DisplayName("Profile Column")]
        [Description("Which Column of the profile to display")]
        [Editor(typeof(ColumnEditor), typeof(UITypeEditor))]
        public Column ProfileColumn {
            get { return profileColumn; }
            set { profileColumn = value; InitializeScale(); solutionControl.ProfileColumnChanged(); }
        }

        /// <summary>
        /// A list of selectable Columns for the ProfileColumn Property Editor
        /// </summary>
        internal Column[] SelectableColumns {
            get { return MillingSimulationParser.ProfileColumnSet.Columns; }
        }

        public override void Initialize(Point[] p) {
            this.TabPag.Text = "Solution Window";
            this.Text = "Displacement Profiles";
            this.Size = new System.Drawing.Size(700, 300);

            InitializeSortedProfiles(p);
            InitializeProfileColumn();
            InitializeScale();
            InitializeLayout();
        }

        /// <summary>
        /// Builds the sorted array of Profiles for the rest of the Solutionwindow
        /// to work with. The Profiles are extracted from the currently selected Points.
        /// </summary>
        private void InitializeSortedProfiles(Point[] points) {
            Dictionary<PointSet, int> pointSets = new Dictionary<PointSet, int>();
            foreach (Point p in points) {
                HeightPoint hp = p as HeightPoint;
                if (!pointSets.ContainsKey(hp.profile)) pointSets.Add(hp.profile, hp.height);
            }
            List<Profile> profiles = new List<Profile>();
            foreach (KeyValuePair<PointSet, int> pair in pointSets) {
                profiles.Add(new Profile(pair.Key, pair.Value));
            }
            profiles.Sort(delegate(Profile x, Profile y) { return x.Height - y.Height; });
            sortedProfiles = profiles.ToArray();
        }

        /// <summary>
        /// Sets the initially displayed profile column.
        /// </summary>
        private void InitializeProfileColumn() {
            profileColumn = MillingSimulationParser.ProfileColumnSet.Columns[0];
        }

        /// <summary>
        /// Initializes the yScale depending on the MIN and MAX values of the currently displayed profiles
        /// with respect to the displayed profile column.
        /// </summary>
        private void InitializeScale() {
            double maxDistance = 0;
            foreach (Profile prof in sortedProfiles) {
                maxDistance = Math.Max(maxDistance, prof.yMinMaxMean[Result.MAX][profileColumn] - prof.yMinMaxMean[Result.MIN][profileColumn]);
            }
            yScale = 1d / maxDistance;
            if (Double.IsInfinity(yScale))
                yScale = 1;
            InitializeScaleStepDistance();
        }

        /// <summary>
        /// Initialize the scaleStepDistance with regarding to the MIN and MAX in y-direction and the
        /// selected number of scaleSteps.
        /// </summary>
        private void InitializeScaleStepDistance() {
            double maxDistance = 0;
            foreach (Profile prof in sortedProfiles) {
                maxDistance = Math.Max(maxDistance, prof.yMinMaxMean[Result.MAX][profileColumn] - prof.yMinMaxMean[Result.MIN][profileColumn]);
            }
            scaleStepDistance = Math.Round(maxDistance / scaleSteps, 4);
        }

        /// <summary>
        /// Add the OpenGLControl and the ProfilePanels to the Window
        /// </summary>
        private void InitializeLayout() {
            TableLayoutPanel tbl = new TableLayoutPanel();
            tbl.ColumnCount = 2;
            tbl.RowCount = sortedProfiles.Length;
            for (int i = 0; i < sortedProfiles.Length; i++) {
                ProfilePanel pp = new ProfilePanel(sortedProfiles[i]);
                pp.Dock = DockStyle.Fill;
                tbl.Controls.Add(pp, 0, sortedProfiles.Length - 1 - i); // From Bottom to Top
                tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / sortedProfiles.Length));
            }
            this.solutionControl = new MillingSimulationSolutionControl(this);
            this.solutionControl.Dock = DockStyle.Fill;
            this.solutionControl.Visible = true;
            tbl.Controls.Add(this.solutionControl);
            tbl.SetRowSpan(this.solutionControl, sortedProfiles.Length);
            tbl.Dock = DockStyle.Fill;
            this.Controls.Add(tbl);
        }
        
        public override string Label {
            get { return "Displacement Profile"; }
        }

        /// <summary>
        /// Not used for this use case but needed because the Solution-
        /// crap was designed so horribly.
        /// </summary>
        public new Space GlyphSpace {
            get { return ProjectController.Project.spaces[0]; }
            set { }
        }

        /// <summary>
        /// Not used for this use case but needed because the Solution-
        /// crap was designed so horribly.
        /// </summary>
        public override int ChangePoint(bool forwardDirection) {
            return index;
        }

        /// <summary>
        /// Not used for this use case but needed because the Solution-
        /// crap was designed so horribly.
        /// </summary>
        protected override void ChangeMode() {
            
        }

        /// <summary>
        /// Not used for this use case but needed because the Solution-
        /// crap was designed so horribly.
        /// </summary>
        protected override void GlyphSpaceChanged(object sender, EventArgs e) {

        }

        /// <summary>
        /// Represents a displayed profile with all of its relevant properties.
        /// </summary>
        internal class Profile {
            /// <summary>
            /// The index of the displaylist used for rendering this Profile.
            /// Used in MillingSimulationSolutionControl
            /// </summary>
            internal int displaylist;

            /// <summary>
            /// The pointSet containing the Profile
            /// </summary>
            internal PointSet pointSet;

            /// <summary>
            /// The xTranslation (for OpenGL linear transformation).
            /// Recalculated whenever inPoint or outPoint change
            /// </summary>
            internal double xTranslation;

            /// <summary>
            /// The yTranslation (for OpenGL linear transformation).
            /// Contains the correct translation for every possible
            /// profileColumn.
            /// </summary>
            internal Point yTranslation;

            /// <summary>
            /// The xScale (for OpenGL linear transformation).
            /// Recalculated whenever inPoint or outPoint change
            /// </summary>
            internal double xScale;

            /// <summary>
            /// Needed for various calculations.
            /// Determined once in the Constructor
            /// </summary>
            internal Point[] yMinMaxMean;

            /// <summary>
            /// InPoint for the x linear transformation.
            /// All points before the inPoint aren't drawn
            /// </summary>
            private int inPoint;

            /// <summary>
            /// OutPoint for the x linear transformation.
            /// All points after the outPoint aren't drawn
            /// </summary>
            private int outPoint;

            /// <summary>
            /// The height in which the current Profile was recorded
            /// Used to sort the profiles
            /// </summary>
            private int height;

            /// <summary>
            /// Whenever the properties of the profile change, this is fired
            /// </summary>
            internal event EventHandler Changed;

            internal int Height {
                get { return height; }
            }

            internal int InPoint {
                get { return inPoint; }
                set {
                    inPoint = value;
                    DetermineXScaleAndTranslation();
                    ThrowChangedEvent();
                }
            }

            internal int OutPoint {
                get { return outPoint; }
                set {
                    outPoint = value;
                    DetermineXScaleAndTranslation();
                    ThrowChangedEvent();
                }
            }

            internal Profile(PointSet profilePointSet, int height) {
                this.pointSet    = profilePointSet;
                this.yMinMaxMean = profilePointSet.MinMaxMean();
                this.height      = height;
                this.inPoint     = 0;
                this.outPoint    = profilePointSet.Length;
                DetermineYTranslation();
                DetermineXScaleAndTranslation();
            }

            private void DetermineXScaleAndTranslation() {
                xScale = 1d / (outPoint-inPoint);
                xTranslation = -inPoint;
            }

            private void DetermineYTranslation() {
                double[] transValues = yMinMaxMean[Result.MEAN].Values.Clone() as double[];
                for (int i = 0; i < transValues.Length; i++) {
                    transValues[i] = -transValues[i];
                }
                yTranslation = new Point(yMinMaxMean[Result.MEAN].ColumnSet, transValues);
            }

            private void ThrowChangedEvent() {
                if (Changed != null) Changed(this, new EventArgs());
            }

            /// <summary>
            /// Renders the Pointset.
            /// It is expected that this is called from a valid OpenGL Rendering context.
            /// No kind of scaling is performed, no display list management either.
            /// All that lies in the responsibility of the caller.
            /// </summary>
            /// <param name="column">Which profileColumn are we rendering?</param>
            internal void Render(Column column) {
                int colIndex = pointSet.ColumnSet.IndexOf(column);
                Gl.glColor3fv(ColorManagement.UnselectedColor.RGB);
                Gl.glBegin(Gl.GL_LINE_STRIP);
                Gl.glVertex2d(0, 0);
                int pointIndex = 0;
                foreach (Point p in pointSet) {
                    Gl.glVertex2d(pointIndex++, p[colIndex]);
                }
                Gl.glColor3fv(ColorManagement.UnselectedColor.RGBwithA(0f));
                Gl.glVertex2d(pointSet.Length - 1, 0);
                Gl.glEnd();
            }

            /// <summary>
            /// Renders the Scales
            /// Seperate from rendering the PointSet because that is usually rendered into a
            /// displaylist. We don't want to regenerate the entire displaylist whenever the scales
            /// change, thus they're rendered here seperately.
            /// </summary>
            /// <param name="column"></param>
            /// <param name="scaleStep"></param>
            internal void RenderScales(Column column, double scaleStep) {
                if (0d == scaleStep) return; //Otherwise the for-loop never exits
                double yMax = Math.Max(Math.Abs(yMinMaxMean[Result.MAX][column]), Math.Abs(yMinMaxMean[Result.MIN][column]));
                double end = pointSet.Length;
                Gl.glColor3fv(ColorManagement.AxesColor.RGB);
                Gl.glBegin(Gl.GL_LINES);
                for (double y = 0d; y <= yMax; y += scaleStep) {
                    Gl.glVertex2d(0, y);
                    Gl.glVertex2d(end, y);
                    Gl.glVertex2d(0, -y);
                    Gl.glVertex2d(end, -y);
                }
                Gl.glEnd();

                int[] rasterpos;
                int[] gueltig = new int[1];

                for (double y = 0d; y <= yMax; y += scaleStep) {
                    rasterpos = new int[2];
                    Gl.glRasterPos3d(-xTranslation, y, 0);
                    Gl.glGetIntegerv(Gl.GL_CURRENT_RASTER_POSITION, rasterpos);
                    Gl.glGetBooleanv(Gl.GL_CURRENT_RASTER_POSITION_VALID, gueltig);
                    if (gueltig[0] == 1) {
                        Gl.glWindowPos2d(rasterpos[0], rasterpos[1] + 2);
                        Glut.glutBitmapString(Glut.GLUT_BITMAP_HELVETICA_10, "  " + y.ToString());
                    }
                }                
            }
        }

        #region ColumnEditor

        /// <summary>
        /// A DropDown ListBox displaying the Columns in the PropertyControl.
        /// </summary>
        class ColumnEditor : UITypeEditor {
            /// <summary>
            /// Handles the selection of Columns in the ListBox.
            /// </summary>
            /// <param name="context">The context</param>
            /// <param name="provider">The service provider</param>
            /// <param name="value">Value</param>
            /// <returns>The selected item</returns>
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
                ListBox listBox = new ListBox();
                foreach (Column c in (context.Instance as MillingSimulationSolution).SelectableColumns) {
                    listBox.Items.Add(c);
                    if (c.Index == (context.Instance as MillingSimulationSolution).profileColumn.Index)
                        listBox.SelectedItem = c;
                }
                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                listBox.SelectedValueChanged += delegate(object sender, EventArgs e) { editorService.CloseDropDown(); };
                editorService.DropDownControl(listBox);
                return listBox.SelectedItem;
            }

            /// <summary>
            /// Returns the EditStyle.
            /// </summary>
            /// <param name="context">The context</param>
            /// <returns>UITypeEditorEditStyle.DropDown</returns>
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
                return UITypeEditorEditStyle.DropDown;
            }
        }

        #endregion

    }
}