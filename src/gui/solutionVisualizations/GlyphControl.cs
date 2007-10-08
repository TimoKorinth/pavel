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
using System.Windows.Forms;
using Pavel.Framework;
using Pavel.GUI.Visualizations;

using System.ComponentModel;
using System.Drawing;
using System.Data;
using Tao.OpenGl;
using Tao.FreeGlut;

namespace Pavel.GUI.SolutionVisualizations {

    #region Class GlyphControl

    /// <summary>
    /// A UserControl containing a label and a CheckedListBox displaying a given
    /// array of Columns.
    /// </summary>
    public class GlyphControl : UserControl, IDisposable {

        #region Fields

        private TableLayoutPanel tableLayout = new TableLayoutPanel();
        private Label columnLabel = new Label();
        private ContextMenu menu = new ContextMenu();
        private CheckedListBox columnCheckBox = new CheckedListBox();
        private GlyphOpenGLControl glyphOpenGLControl;

        /// <value> Event fired if the list of checked columns has been modified. </value>
        [field: NonSerializedAttribute()]
        public event GlyphColumnsChangedEventHandler glyphColumnsChanged;

        /// <value>
        /// The selectedSpace selected from the spaceComboBox
        /// </value>
        private Space selectedSpace;

        /// <value>
        /// The columnProperties contained in the selected Space minus the unselected 
        /// Columns(properties) from the columnCheckBox
        /// </value>
        private ColumnProperty[] displayedColumnProperties;

        /// <value> The set of columns common over all points to be displayed/// </value>
        private ColumnSet commonColumnSet;

        #endregion

        #region Properties

        /// <value> Returns the selected ColumnProperties which the glyph should display </value>
        internal ColumnProperty[] DisplayedColumnProperties {
            get { return displayedColumnProperties; }
        }

        /// <value> Gets the ColumnSet common to all the displayed Points </value>
        internal ColumnSet CommonColumnSet { get { return commonColumnSet; } }

        /// <value> Gets the Space the Glyphs are displayed in or sets it </value>
        public Space SelectedSpace {
            get { return selectedSpace; }
            set {
                selectedSpace = value;
                this.displayedColumnProperties = selectedSpace.ColumnProperties;
                InitializeColumnCheckBox();
                glyphOpenGLControl.UpdateGlyphs();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Intializes the control
        /// </summary>
        /// <param name="points">The Pavel.Framework.Points to be displayed in the Glyph</param>
        public GlyphControl(Pavel.Framework.Point[] points) {

            //Calculate the common ColumnSet over all points to be displayed
            if (points == null || points.Length == 0)
                throw new ArgumentException("At least one point must be given");
            commonColumnSet = points[0].ColumnSet;
            foreach (Pavel.Framework.Point p in points) {
                commonColumnSet = ColumnSet.Union(p.ColumnSet, commonColumnSet);
            }

            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Dock = DockStyle.Fill;

            glyphOpenGLControl = new GlyphOpenGLControl(this);
            columnCheckBox.SelectedValueChanged += ColumnCheckBox_SelectedValueChanged;

            InitializeTableLayout();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initalizes the tableLayout
        /// </summary>
        private void InitializeTableLayout() {
            tableLayout.AutoSize = true;
            tableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayout.Dock = DockStyle.Fill;
            this.Controls.Add(tableLayout);

            tableLayout.ColumnCount = 1;
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            tableLayout.RowCount = 3;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));

            tableLayout.Controls.Add(this.columnLabel, 0, 0);
            tableLayout.Controls.Add(this.columnCheckBox, 0, 1);
            tableLayout.Controls.Add(this.glyphOpenGLControl, 0, 2);

            this.columnLabel.Text = "Select Glyph-Columns:";
            this.columnLabel.Dock = DockStyle.Fill;
            this.glyphOpenGLControl.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Sets the CheckOnClick property of the columnCheckBox to true and
        /// fills the checkBox with the given columns.
        /// All items are initially checked.
        /// </summary>
        private void InitializeColumnCheckBox() {
            columnCheckBox.Dock = DockStyle.Fill;
            columnCheckBox.CheckOnClick = true;

            columnCheckBox.Items.Clear();
            for (int i = 0; i < this.selectedSpace.Dimension; i++) {
                columnCheckBox.Items.Add(selectedSpace.ColumnProperties[i], true);
            }
        }

        /// <summary>
        /// Updates the glyphs with the given <paramref name="points"/>.
        /// </summary>
        /// <param name="points">Points to be displayed as glyphs</param>
        public void UpdateGlyphs(List<Pavel.Framework.Point> points) {
            glyphOpenGLControl.UpdateGlyphs(points);
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// EventHandler for modifications of the glyphColumns.
        /// </summary>
        /// <param name="sender">This glyphList</param>
        /// <param name="e">Standard EventArgs</param>
        public delegate void GlyphColumnsChangedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// When the selected columns are changed, the glyphColumnsChanged event is fired.
        /// </summary>
        /// <param name="sender">columnCheckBox</param>
        /// <param name="e">Empty EventArgs</param>
        private void ColumnCheckBox_SelectedValueChanged(object sender, EventArgs e) {
            if (columnCheckBox.CheckedItems.Count == 0) return;
            displayedColumnProperties = new ColumnProperty[columnCheckBox.CheckedItems.Count];
            for ( int i = 0; i < columnCheckBox.CheckedIndices.Count; i++ ) {
                displayedColumnProperties[i] = (ColumnProperty)columnCheckBox.CheckedItems[i];
            }
            if (null != glyphColumnsChanged) { glyphColumnsChanged(this, EventArgs.Empty); }
        }

        #endregion
    }

    #endregion

    #region Class GlyphOpenGLControl

    /// <summary>
    /// An OpenGLControl containing the glyph.
    /// </summary>
    internal class GlyphOpenGLControl : OpenGLControl {

        #region Fields

        private GlyphControl glyphControl;
        private double[][] web;
        private List<double[]> glyphs = new List<double[]>();
        private List<float[]> glyphColors = new List<float[]>();
        private double webRadius = 90;
        private List<Pavel.Framework.Point> glyphPoints;

        #endregion

        #region Constructor

        /// <summary>
        /// Subscribes to the glyphColumnsChanged event of the <paramref name="glyphControl"/>.
        /// </summary>
        /// <param name="glyphControl">The GlyphControl this OpenGLControl belongs to.</param>
        public GlyphOpenGLControl(GlyphControl glyphControl) {
            this.glyphControl = glyphControl;
            this.glyphControl.glyphColumnsChanged += GlyphControl_glyphColumnsChanged;
        }

        #endregion

        #region Methods

        #region Init/RenderScene/SetView

        // OpenGLControl Member
        protected override void SetupModelViewMatrixOperations() { }

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
        }


        /// <summary>
        /// Sets view to correct size.
        /// </summary>
        private void SetView() {
            Gl.glViewport(0, 0, this.Width, this.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(-150 * WindowAspect * AspectCap, 150 * WindowAspect * AspectCap, -150, 150, -1, 1);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glFlush();
        }

        /// <summary>
        /// Everything, that has to be rendered, will be put here.
        /// </summary>
        protected override void RenderScene() {
            this.SetView();
            Gl.glDrawBuffer(Gl.GL_BACK);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glPushMatrix();

            //draw
            if (glyphControl.DisplayedColumnProperties.Length >= 2) { DrawGlyphs(); } else { DrawWarning(); }

            Gl.glPopMatrix();
        }
        #endregion

        #region Drawing

        /// <summary>
        /// Draws the web and the glyphs of the previously chosen Points.
        /// </summary>
        private void DrawGlyphs() {
            Gl.glDisable(Gl.GL_DEPTH_TEST);

            for (int i = 0; i < glyphControl.DisplayedColumnProperties.Length; i++) {
                //The cobweb
                Gl.glColor3fv(ColorManagement.AxesColor.RGB);
                Gl.glBegin(Gl.GL_LINES);
                    Gl.glVertex2d(web[0][2 * i], web[0][2 * i + 1]);
                    Gl.glVertex2d(web[1][2 * i], web[1][2 * i + 1]);
                Gl.glEnd();

                //The labels
                Gl.glColor3fv(ColorManagement.DescriptionColor.RGB);
                double degree = i * 360 / glyphControl.DisplayedColumnProperties.Length;
                this.PrintText((float) web[1][2 * i], (float) web[1][2 * i + 1], (float) degree, 16.0f, glyphControl.DisplayedColumnProperties[i].ToString());
            }

            Gl.glColor3fv(ColorManagement.AxesColor.RGB);
            //Inner ring
            Gl.glBegin(Gl.GL_LINE_LOOP);
            for (int i = 0; i < web[0].Length / 2; i++) {
                Gl.glVertex2d(web[0][2 * i], web[0][2 * i + 1]);
            }
            Gl.glEnd();

            //Outer ring
            Gl.glBegin(Gl.GL_LINE_LOOP);
            for (int i = 0; i < web[1].Length / 2; i++) {
                Gl.glVertex2d(web[1][2 * i], web[1][2 * i + 1]);
            }
            Gl.glEnd();

            //The glyphs
            Gl.glLineWidth(2.0f);
            for (int g = 0; g < glyphs.Count; g++) {
                Gl.glColor3fv(glyphColors[g]);
                Gl.glBegin(Gl.GL_LINE_LOOP);
                for (int i = 0; i < glyphControl.DisplayedColumnProperties.Length; i++) {
                    Gl.glVertex2d(glyphs[g][2 * i], glyphs[g][2 * i + 1]);
                }
                Gl.glEnd();
            }
        }

        /// <summary>
        /// Draws a message to tell the user that at least two columns have to be selected
        /// to display the web and the glyphs.
        /// </summary>
        private void DrawWarning() {
            Gl.glColor3f(1.0f, 0.0f, 0.0f);
            this.PrintText(-30.0f, 50.0f, 0.0f, 25.0f, "Select");
            this.PrintText(-35.0f, 0.0f, 0.0f, 25.0f, "at least");
            this.PrintText(-60.0f, -50.0f, 0.0f, 25.0f, "two columns!");
        }

        #endregion

        /// <summary>
        /// If at least one column is selected, the end points of the web, centered around the origin,
        /// and the glyphs for the glyphPoints are calculated and this Control is invalidated.
        /// </summary>
        internal void UpdateGlyphs() {
            double degree;
            glyphs.Clear();

            if (glyphControl.DisplayedColumnProperties != null) {
                web = new double[2][];
                for (int i = 0; i < web.Length; i++) {
                    web[i] = new double[2 * glyphControl.DisplayedColumnProperties.Length];
                }

                for ( int i = 0; i < glyphControl.DisplayedColumnProperties.Length; i++ ) {
                    degree = i * 360 / (web[0].Length / 2);
                    web[0][2 * i] = webRadius / 10 * Math.Cos(degree * Math.PI / 180);
                    web[0][2 * i + 1] = webRadius / 10 * Math.Sin(degree * Math.PI / 180);
                    web[1][2 * i] = (webRadius + webRadius / 10) * Math.Cos(degree * Math.PI / 180);
                    web[1][2 * i + 1] = (webRadius + webRadius / 10) * Math.Sin(degree * Math.PI / 180);
                }

                foreach (Pavel.Framework.Point point in glyphPoints) {
                    if (point != null) {
                        this.AddGlyph(point);
                    }
                }
            }

            this.Invalidate();
        }

        /// <summary>
        /// Sets the Pavel.Framework.Points to draw the glyphs for and updates the
        /// array of colors and the glyphs.
        /// </summary>
        /// <param name="points">The Pavel.Framework.Points to draw the glyphs for</param>
        internal void UpdateGlyphs(List<Pavel.Framework.Point> points) {
            this.glyphPoints = points;
            this.UpdateColors(points.Count);

            UpdateGlyphs();
        }

        /// <summary>
        /// Calculates the coordinates of the points of the glyph on the web of the
        /// chosen GlyphColumns for the given Pavel.Framework.Point and adds them to
        /// the list of glyphs. The values will be scaled using the the ColumnProperties of the
        /// chosen selectedSpace
        /// </summary>
        /// <param name="point">The Pavel.Framework.Point to make the glyph of</param>
        private void AddGlyph(Pavel.Framework.Point point) {
            double degree, c;

            if (point != null) {
                double[] glyph = new double[2 * glyphControl.DisplayedColumnProperties.Length];
                for ( int i = 0; i < glyphControl.DisplayedColumnProperties.Length; i++ ) {
                    degree = i * 360 / (web[0].Length / 2);
                    ColumnProperty prop = glyphControl.DisplayedColumnProperties[i];
                    double scaledValue = point.ScaledValue(glyphControl.DisplayedColumnProperties[i].Column, prop);
                    scaledValue = (scaledValue < 0) ? -0.1 : scaledValue;
                    scaledValue = (scaledValue > 1) ? 1.1 : scaledValue;
                    c = webRadius * scaledValue + webRadius / 10;

                    glyph[2 * i] = c * Math.Cos(degree * Math.PI / 180);
                    glyph[2 * i + 1] = c * Math.Sin(degree * Math.PI / 180);
                }
                glyphs.Add(glyph);
            }
        }

        /// <summary>
        /// Fills the list of colors with the amount of colors given by <paramref name="number"/>.
        /// </summary>
        /// <param name="number">Amount of colors to add</param>
        private void UpdateColors(int number) {
            glyphColors.Clear();

            for (int i = 0; i < number; i++) {
                glyphColors.Add(ColorManagement.GetColor(i).RGB);
            }
        }
        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Calls the method to update the glyphs, when the columns are changed.
        /// </summary>
        /// <param name="sender">columnCheckBox in GlyphControl</param>
        /// <param name="e">Empty EventArgs</param>
        private void GlyphControl_glyphColumnsChanged(object sender, EventArgs e) {
            UpdateGlyphs();
        }

        #endregion
    }
    #endregion
}
