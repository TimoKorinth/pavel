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
using System.Windows.Forms.Design;
using Pavel.GUI;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Pavel.GUI.Visualizations;

namespace Pavel.GUI.SolutionVisualizations {
    /// <summary>
    /// The parent class to all solutions. Is a Form and
    /// implements the ICustomTypeDescriptor interface to display only properties
    /// with the ShowInProperties attribute in the PropertyControl.
    /// </summary>
    public abstract class Solution : TabableForm, ICustomTypeDescriptor {

        #region Fields

        protected Pavel.Framework.Point[] points;
        protected int referencePointIndex = -1;
        protected int currentPointIndex = -1;
        protected int index = 0;

        protected DataGridView dataGridView;
        protected TableLayoutPanel legendContainer;
        protected ZapControl zapControl;
        protected CompareManyControl compManyControl;
        protected ComparisonControl compRefControl;
        protected GlyphControl glyphControl;

        protected Mode selectedMode = Mode.Zapping;
        public enum Mode { Zapping, CompareToRef, CompareToMany };

        #endregion

        #region Properties

        /// <value>
        /// Gets SelectedMode or sets it.
        /// </value>
        [ShowInProperties]
        [Category("Modes")]
        [DisplayName("Displaymode")]
        [Description("You can choose between Zapping, CompareToRef and CompareToMany. When selecting Zapping, you can go through the single solution by means of buttons. When selecting CompareToRef, one solution can be compared to an other selected solution. The temperature values will be calculated like (reference point - current point). In CompareToMany-mode, you can compare many solutions with each other.")]
        public Mode SelectedMode {
            get { return selectedMode; }
            set {
                selectedMode = value;
                ChangeMode();
            }
        }

        /// <value>Gets the displayed Space for the Glyphs or sets it</value>
        [ShowInProperties]
        [Category("Glyphs")]
        [DisplayName("Glyph Space")]
        [Description("Select the Space for the Glyphs")]
        [Editor(typeof(SpaceEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Space GlyphSpace {
            get { return glyphControl.SelectedSpace; }
            set {
                if (value.IsViewOfColumnSet(glyphControl.CommonColumnSet)) {
                    if (value.Dimension <= 0) { throw new ApplicationException("Trying to display viewing ColumnSet without Columns."); }
                    if (glyphControl.SelectedSpace != null) {
                        glyphControl.SelectedSpace.Parent.Changed -= GlyphSpaceChanged;
                    }
                    glyphControl.SelectedSpace = value.CreateLocalCopy();
                    glyphControl.SelectedSpace.Parent.Changed += GlyphSpaceChanged;
                } else {
                    throw new ApplicationException("Space is not a view of the PointSets ColumnSet");
                }
            }
        }

        public Pavel.Framework.Point ReferencePoint {
            get { return referencePointIndex == -1 ? null : points[referencePointIndex]; }
        }

        public Pavel.Framework.Point CurrentPoint {
            get { return currentPointIndex == -1 ? null : points[currentPointIndex]; }
        }

        /// <value> Gets the glyphControl </value>
        public GlyphControl GlyphControl { get { return glyphControl; } }

        public abstract string Label { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the form
        /// </summary>
        /// <param name="p"></param>
        public virtual void Initialize(Pavel.Framework.Point[] p) {
            ProjectController.SpaceRemoved += GlyphSpaceChanged;
            this.FormClosing += delegate(object sender, FormClosingEventArgs e) {
                ProjectController.SpaceRemoved -= GlyphSpaceChanged;
                glyphControl.SelectedSpace.Parent.Changed -= GlyphSpaceChanged;
            };
            this.TabPag.Text = "Solution Window";
        }

        public abstract int ChangePoint(bool forwardDirection);

        protected abstract void ChangeMode();

        /// <summary>
        /// Initialize the legend container.
        /// </summary>
        protected void InitializeLegendContainer() {
            legendContainer.AutoSize = true;
            legendContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            legendContainer.Dock = DockStyle.Fill;
            legendContainer.RowCount = 3;
            legendContainer.ColumnCount = 1;
            legendContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            legendContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            legendContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            legendContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            legendContainer.Controls.Add(glyphControl, 0, 0);
            glyphControl.Dock = DockStyle.Fill;
            glyphControl.AutoSize = true;
            legendContainer.Controls.Add(dataGridView, 0, 1);
            zapControl = new ZapControl(points, this);
            compManyControl = new CompareManyControl(points);
            compRefControl = new ComparisonControl(points);
            legendContainer.Controls.Add(zapControl, 0, 2);
        }

        /// <summary>
        /// Initialize the dataGridView.
        /// </summary>
        protected void InitializeDataGrid() {
            dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.AllowUserToOrderColumns = false;
            dataGridView.AllowUserToResizeColumns = true;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.RowHeadersVisible = false;
            dataGridView.ColumnCount = 2;
            dataGridView.Columns[0].Name = "Column";
            dataGridView.Columns[1].Name = "Value";
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.ReadOnly = true;
        }

        /// <summary>
        /// Fills the dataGridView with the values of the displayed Pavel.Framework.Point.
        /// </summary>
        protected void FillDataGrid() {
            dataGridView.Rows.Clear();
            string[] tmp;
            switch (selectedMode) {
                case Mode.CompareToRef: {
                        if (null != ReferencePoint) {
                            tmp = new string[] { "Index", CurrentPoint.Tag.Index.ToString(), ReferencePoint.Tag.Index.ToString() };
                        } else
                            tmp = new string[] { "Index", CurrentPoint.Tag.Index.ToString() };
                        break;
                    }
                default: {
                        tmp = new string[] { "Index", CurrentPoint.Tag.Index.ToString() };
                        break;
                    }
            }
            this.dataGridView.Rows.Add(tmp);
            foreach (Column col in CurrentPoint.ColumnSet) {
                switch (selectedMode) {
                    case Mode.CompareToRef: {
                            if (null != ReferencePoint) {
                                tmp = new string[] { col.Label, CurrentPoint[col].ToString(), ReferencePoint[col].ToString() };
                            } else
                                tmp = new string[] { col.Label, CurrentPoint[col].ToString() };
                            break;
                        }
                    default: {
                            tmp = new string[] { col.Label, CurrentPoint[col].ToString() };
                            break;
                        }
                }
                this.dataGridView.Rows.Add(tmp);
            }
        }

        /// <summary>
        /// Determines the Space initially chosen for the Glyphs.
        /// The chosen Space should contain as many Columns as possible,
        /// while containing as few of the Columns given by <paramref name="avoidColumns"/> as possible.
        /// </summary>
        /// <param name="avoidColumns">Columns to be avoided</param>
        protected void InitializeGlyphSpace(List<int> avoidColumns) {
            List<Space> spaces = new List<Space>();
            foreach (Space s in Space.AllSpacesForColumnSet(glyphControl.CommonColumnSet)) {
                    spaces.Add(s);
            }
            //Create a default space if no actual space fits
            //this is just for rare exceptional cases
            if (spaces.Count == 0 && ProjectController.Project.UseCase.SolutionColumnSet!=null) {
                spaces.Add(new Space(ProjectController.Project.UseCase.SolutionColumnSet,"Default-Space"));
            } else PavelMain.LogBook.Error("No Space could be foundnd to show selected points");
            // If, like in the Default UseCase, there are no specific Columns to be avoided, the first Space is chosen.
            if (avoidColumns == null) {
                GlyphSpace = spaces[0];
            } else {
                int[] initialSpace = new int[3];
                for (int i = 0; i < spaces.Count; i++) {
                    // For every Space the number of avoidColumns contained is counted.
                    int contained = 0;
                    for (int j = 0; j < spaces[i].ColumnProperties.Length; j++) {
                        for (int k = 0; k < avoidColumns.Count; k++) {
                            if (spaces[i].ColumnProperties[j].Column.Index == avoidColumns[k]) { contained++; }
                        }
                    }
                    // A Space with less avoidColumns is better.
                    // Of Spaces with the same amount of avoidColumns, the one with the most Columns is best.
                    if ((contained <= initialSpace[0]) && ((spaces[i]).ColumnProperties.Length > initialSpace[1])) {
                        initialSpace[0] = contained;
                        initialSpace[1] = spaces[i].ColumnProperties.Length;
                        initialSpace[2] = i;
                    }
                }

                GlyphSpace = spaces[initialSpace[2]];
            }
        }

        #endregion

        #region Event Handling Stuff

        protected abstract void GlyphSpaceChanged(object sender, EventArgs e);

        /// <summary>
        /// Event, when key is released
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">KeyEventArgs</param>
        protected void OnKeyUpEvent(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Right:
                    ChangePoint(true);
                    break;
                case Keys.Left:
                    ChangePoint(false);
                    break;
            }
        }

        #endregion

        #region ICustomTypeDescriptor Member

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
            Attribute[] a = new Attribute[attributes.Length + 1];
            a[0] = new ShowInPropertiesAttribute();
            attributes.CopyTo(a, 1);
            return TypeDescriptor.GetProperties(this.GetType(), a);
        }

        #region No own Implementations

        public PropertyDescriptorCollection GetProperties() {
            return TypeDescriptor.GetProperties(typeof(Solution));
        }

        public object GetPropertyOwner(PropertyDescriptor pd) {
            return this;
        }

        public object GetEditor(Type editorBaseType) {
            return TypeDescriptor.GetEditor(typeof(Solution), editorBaseType);
        }

        public AttributeCollection GetAttributes() {
            return TypeDescriptor.GetAttributes(typeof(Solution));
        }

        public string GetClassName() {
            return TypeDescriptor.GetClassName(typeof(Solution));
        }

        public string GetComponentName() {
            return TypeDescriptor.GetComponentName(typeof(Solution));
        }

        public TypeConverter GetConverter() {
            return TypeDescriptor.GetConverter(typeof(Solution));
        }

        public EventDescriptor GetDefaultEvent() {
            return TypeDescriptor.GetDefaultEvent(typeof(Solution));
        }

        public PropertyDescriptor GetDefaultProperty() {
            return TypeDescriptor.GetDefaultProperty(typeof(Solution));
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) {
            return TypeDescriptor.GetEvents(typeof(Solution), attributes);
        }

        public EventDescriptorCollection GetEvents() {
            return TypeDescriptor.GetEvents(typeof(Solution));
        }

        #endregion

        #endregion

        #region SpaceEditor

        /// <summary>
        /// A DropDown ListBox displaying the Spaces in the PropertyControl.
        /// </summary>
        class SpaceEditor : UITypeEditor {
            /// <summary>
            /// Handles the selection of Spaces in the ListBox.
            /// </summary>
            /// <param name="context">The context</param>
            /// <param name="provider">The service provider</param>
            /// <param name="value">Value</param>
            /// <returns>The selected item</returns>
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
                ListBox listBox = new ListBox();
                foreach (Space s in Space.AllSpacesForColumnSet((context.Instance as Solution).glyphControl.CommonColumnSet)) {
                    listBox.Items.Add(s);
                    if (s.Label.Equals(((Solution)context.Instance).GlyphSpace.Label))
                        listBox.SelectedItem = s;
                }
                //Create a default space if no actual space fits
                //this is just for rare exceptional cases
                if ( listBox.Items.Count == 0 && ProjectController.Project.UseCase.SolutionColumnSet != null ) {
                    listBox.Items.Add(new Space(ProjectController.Project.UseCase.SolutionColumnSet, "Default-Space"));
                } else PavelMain.LogBook.Error("No Space could be found to show selected points");
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
