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
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using Pavel.Framework;
using Pavel.GUI;

namespace Pavel.GUI.Visualizations {
    /// <summary>
    /// Parent class of all VisualiationWindows.
    /// </summary>
    public partial class VisualizationWindow : TabableForm, ICustomTypeDescriptor {

        #region Fields

        private PointSet pointSet;
        private PointSet displayedPointSet;
        private Space space;
        private Visualization visualization;

        private bool filterMode = false;
        private bool clusterMode = false;
        private List<Selection> clusterSelections;

        #endregion

        #region Properties

        /// <value>Gets the PointSet displayed in this VisualizationWindow or sets it and updates the display</value>
        [ShowInProperties]
        [Category("Data")]
        [Editor(typeof(PointSetEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public PointSet PointSet {
            get { return pointSet; }
            set {
                PointSet oldpointSet;
                if (value.ColumnSet.Dimension != 0 && value.Length > 0) {
                    oldpointSet = pointSet;
                    pointSet = value;
                    CreateDisplayedPointSet();

                    List<Space> spaces = new List<Space>(Space.AllSpacesForColumnSet(pointSet.ColumnSet));
                    if (spaces.Count == 0) {
                        throw new ApplicationException("No Space fits for this Point Set");
                    }
                    //Old Space is null or doesn't fit anymore
                    if (space == null || !spaces.Contains(space.Parent)) {
                        Space = spaces[0];
                    }
                } else {
                    throw new ApplicationException("Trying to open Point Set without Points or ColumnSet without Columns.");
                }
                UpdateTitle();
                if (visualization != null) {
                    visualization.UnbindEvents(oldpointSet);
                    visualization.PointSetModified(this,null);
                    visualization.ToolStrip.UpdateToolStrip();
                    visualization.BindEvents();
                }
            }
        }

        /// <value>Gets the displayed Space or sets it</value>
        [ShowInProperties]
        [Category("Data")]
        [Editor(typeof(SpaceEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Space Space {
            get { return space; }
            set {
                if (value.IsViewOfColumnSet(pointSet.ColumnSet)) {
                    if (value.Dimension <= 0) { throw new ApplicationException("Trying to display viewing ColumnSet without Columns."); }
                    space = value.CreateLocalCopy();
                    if (visualization != null) {
                        visualization.UpdateSpace();
                    }
                    UpdateTitle();
                    //Refresh PropertyControl
                    PavelMain.MainWindow.PropertyControl.VisualizationWindow = visualization.VisualizationWindow;
                } else {
                    throw new ApplicationException("Space is not a view of the pointset ColumnSet");
                }
            }
        }

        /// <value>Gets the displayed VisualizationType or sets it</value>
        [ShowInProperties]
        [Category("Display")]
        [TypeConverter(typeof(VisTypeConverter))]
        public String VisualizationType {
            get { return visualization.GetType().Name; }
            set {
                if (Type.GetType("Pavel.GUI.Visualizations." + value, true, true).IsSubclassOf(typeof(Visualization))) {
                    if (null != this.visualization) {
                        this.RevertMergeToolStrips();
                        this.visualization.UnbindEvents();
                        this.Controls.Remove(this.visualization.Control);
                    }
                    try {
                        this.visualization = (Visualization)Activator.CreateInstance(Type.GetType("Pavel.GUI.Visualizations." + value), this);
                    } catch (System.Reflection.TargetInvocationException e) {
                        throw e.InnerException;
                    }
                    this.Controls.Add(visualization.Control);
                    this.MergeToolStrips();
                    UpdateTitle();
                    //Refresh PropertyControl
                    PavelMain.MainWindow.PropertyControl.VisualizationWindow = visualization.VisualizationWindow;
                } else {
                    throw new ApplicationException(value + "is no Valid Visualization");
                }
            }
        }
        
        /// <value>Gets the filterMode or sets it</value>
        public bool FilterMode {
            get { return filterMode; }
            set {
                filterMode = value;
                CreateDisplayedPointSet();
                visualization.PointSetModified(this, null);
            }
        }

        /// <value>Gets the clusterMode or sets it</value>
        public bool ClusterMode {
            get { return clusterMode; }
            set {
                clusterMode = value;
                CreateDisplayedPointSet();
                visualization.PointSetModified(this, null);
            }
        }

        /// <value>Gets the displayed PointSet</value>
        public PointSet DisplayedPointSet {
            get { return displayedPointSet; }
        }

        /// <value>Gets the Visualization</value>
        public Visualization Visualization {
            get { return visualization; }
        }

        #endregion

        #region Constructors

        /// <summary> Empty Constructor for VS8 Designer </summary>
        public VisualizationWindow()
            : base() {
        }

        /// <summary>
        /// Sets the PointSet, the Space and the VisualizationType.
        /// </summary>
        /// <param name="pointSet">PointSet to be displayed</param>
        /// <param name="space">Space to be displayed</param>
        /// <param name="visType">VisualizationType to be displayed</param>
        public VisualizationWindow(PointSet pointSet, Space space, String visType) {
            try {
                InitializeComponent();
                this.pointSet = pointSet;
                CreateDisplayedPointSet();
                this.space = space.CreateLocalCopy();
                this.VisualizationType = visType;
                UpdateTitle();

                ProjectController.SpaceRemoved += SpaceRemoved;
                this.space.Parent.Changed += SpaceChanged;
                // Listen to Focus
                this.Deactivate += VisualizationWindow_Deactivated;
                this.Activated += VisualizationWindow_Activate;
                this.FormClosed += VisualizationWindow_Closed;
            } catch (OutOfMemoryException) { }

        }

        #endregion

        #region Methods

        #region ICustomTypeDescriptor Member

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
            Attribute[] a = new Attribute[attributes.Length + 1];
            a[0] = new ShowInPropertiesAttribute();
            attributes.CopyTo(a, 1);
            return TypeDescriptor.GetProperties(this.GetType(), a);
        }

        #region No own Implementations

        public PropertyDescriptorCollection GetProperties() {
            return TypeDescriptor.GetProperties(typeof(VisualizationWindow));
        }

        public object GetPropertyOwner(PropertyDescriptor pd) {
            return this;
        }

        public object GetEditor(Type editorBaseType) {
            return TypeDescriptor.GetEditor(typeof(VisualizationWindow), editorBaseType);
        }

        public AttributeCollection GetAttributes() {
            return TypeDescriptor.GetAttributes(typeof(VisualizationWindow));
        }

        public string GetClassName() {
            return TypeDescriptor.GetClassName(typeof(VisualizationWindow));
        }

        public string GetComponentName() {
            return TypeDescriptor.GetComponentName(typeof(VisualizationWindow));
        }

        public TypeConverter GetConverter() {
            return TypeDescriptor.GetConverter(typeof(VisualizationWindow));
        }

        public EventDescriptor GetDefaultEvent() {
            return TypeDescriptor.GetDefaultEvent(typeof(VisualizationWindow));
        }

        public PropertyDescriptor GetDefaultProperty() {
            return TypeDescriptor.GetDefaultProperty(typeof(VisualizationWindow));
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) {
            return TypeDescriptor.GetEvents(typeof(VisualizationWindow), attributes);
        }

        public EventDescriptorCollection GetEvents() {
            return TypeDescriptor.GetEvents(typeof(VisualizationWindow));
        }


        #endregion


        #endregion

        /// <summary>
        /// Creates the displayed PointSet depending on the mode (clusterMode, filterMode).
        /// </summary>
        public void CreateDisplayedPointSet() {
            // Delete old Selections
            if (clusterSelections != null) {
                ProjectController.RemoveSelections(clusterSelections);
            }

            PointSet ps = pointSet;
            if (clusterMode && ps is Clustering.ClusterSet) {
                ps = CreateClusterPointSet(ps as Clustering.ClusterSet);
            }
            if (filterMode) {
                ps = ps.CreateFilteredPointSet(space);
            }
            displayedPointSet = ps;
        }

        /// <summary>
        /// Creates a PointSet from the given ClusterSet <paramref name="clusterSet"/>.
        /// </summary>
        /// <param name="clusterSet">A ClusterSet</param>
        /// <returns>A PointSet with all cluster centres and the Points they represent</returns>
        public PointSet CreateClusterPointSet(Clustering.ClusterSet clusterSet) {
            clusterSelections = new List<Selection>();

            PointSet clusterPointSet = new PointSet(clusterSet.Label, pointSet.ColumnSet);
            if (clusterSet.PointLists.Count != 1)
                throw new ApplicationException("Clustering is not valid. More than one PointList");
            clusterPointSet.Add(clusterSet.PointLists[0]);
            for (int i = 0; i < clusterSet.PointLists[0].Count; i++) {
                PointSet ps = ((clusterSet.PointLists[0][i]) as Clustering.Cluster).PointSet;
                Selection s = new Selection();
                s.Add(clusterSet.PointLists[0][i]);
                s.Label = "Cluster " + i;
                s.Active = true;
                foreach (PointList pl in ps.PointLists) {
                    clusterPointSet.Add(pl);
                    s.AddRange(pl);
                }
                clusterSelections.Add(s); 
            }
            ProjectController.AddSelections(clusterSelections);
            return clusterPointSet;
        }

        /// <summary>
        /// Updates the title of the VisualizationWindow depending on the displayed Space and PointSet.
        /// </summary>
        private void UpdateTitle() {
            if (space == null || pointSet == null) {
                this.Text = "";
            } else if (null == visualization) {
                this.Text = space.Label + ": " + pointSet.Label;
            } else {
                this.Text = visualization.GetType().Name + " " + space.Label + ": " + pointSet.Label;
            }
            this.TabPag.Text = this.Text;
        }

        /// <summary>
        /// Merges the FreeToolStrip of the Visualization with the placeholder FreeToolStrip in PavelMain.MainWindow.
        /// </summary>
        private void MergeToolStrips() {
            if (null != this.visualization.ToolStrip) {
                ToolStripManager.Merge(this.visualization.ToolStrip, PavelMain.MainWindow.VisualizationToolStrip);
                PavelMain.MainWindow.VisualizationToolStrip.Visible = true;
            }
        }

        /// <summary>
        /// Reverts the merge of the FreeToolStrip of the Visualization and the placeholder FreeToolStrip in PavelMain.MainWindow.
        /// </summary>
        private void RevertMergeToolStrips() {
            if (null != this.visualization.ToolStrip) {
                ToolStripManager.RevertMerge(PavelMain.MainWindow.VisualizationToolStrip);
                PavelMain.MainWindow.VisualizationToolStrip.Visible = false;
            }
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Handles the deactivation of the VisualizationWindow.
        /// </summary>
        /// <param name="o">The VisualiztaionWindow</param>
        /// <param name="e">Standard EventArgs</param>
        public void VisualizationWindow_Deactivated(object o, EventArgs e) {
            this.RevertMergeToolStrips();
        }

        /// <summary>
        /// Handles the activation of the VisualizationWindow.
        /// </summary>
        /// <param name="o">The VisualizationWindow</param>
        /// <param name="e">Standard EventArgs</param>
        public void VisualizationWindow_Activate(object o, EventArgs e) {
            this.MergeToolStrips();
        }

        /// <summary>
        /// Handles the closing of the VisualizationWindow.
        /// </summary>
        /// <param name="o">The VisualizationWindow</param>
        /// <param name="e">Standard EventArgs</param>
        public void VisualizationWindow_Closed(object o, EventArgs e) {
            if (PavelMain.MainWindow.MdiChildren.Length == 1) { this.RevertMergeToolStrips(); }
            this.visualization.UnbindEvents();

            ProjectController.SpaceRemoved -= SpaceRemoved;
            this.space.Parent.Changed -= SpaceChanged;
            this.Deactivate -= this.VisualizationWindow_Deactivated;
            this.Activated -= VisualizationWindow_Activate;
            this.FormClosed -= VisualizationWindow_Closed;
        }

        /// <summary>
        /// Closes the VisualizationWindow when the displayed Space is removed.
        /// </summary>
        /// <param name="o">The list of Spaces of this Project</param>
        /// <param name="e">Standard EventArgs</param>
        public void SpaceRemoved(object o, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// Updates the VisualizationWindow when the Space is changed.
        /// </summary>
        /// <param name="o">The displayed Space</param>
        /// <param name="e">Standard EventArgs</param>
        public void SpaceChanged(object o, EventArgs e) {
            this.Space = this.space.Parent;
        }

        #endregion

        #region PropertyWindowEditors

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
                foreach (Space s in Space.AllSpacesForColumnSet((context.Instance as VisualizationWindow).PointSet.ColumnSet)) {
                    listBox.Items.Add(s);
                    if (s.Label.Equals(((VisualizationWindow)context.Instance).Space.Label))
                        listBox.SelectedItem = s;
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

        #region PointSetEditor

        /// <summary>
        /// A DropDown ListBox displaying the PointSets in the PropertyControl.
        /// </summary>
        class PointSetEditor : UITypeEditor {
            /// <summary>
            /// Handles the selection of PointSets in the ListBox.
            /// </summary>
            /// <param name="context">The context</param>
            /// <param name="provider">The service provider</param>
            /// <param name="value">Value</param>
            /// <returns>The selected item</returns>
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
                ListBox listBox = new ListBox();
                foreach (Pavel.Framework.PointSet p in Pavel.Framework.ProjectController.Project.pointSets) {
                    listBox.Items.Add(p);
                }
                listBox.SelectedItem = ((VisualizationWindow)context.Instance).PointSet;
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

        #region VisTypeConverter

        /// <summary>
        /// A DropDown ListBox displaying the Visualizations in the PropertyControl.
        /// </summary>
        class VisTypeConverter : TypeConverter {

            /// <summary>
            /// Handles the selection of Visualizations in the ListBox.
            /// </summary>
            /// <param name="context">The context</param>
            /// <returns>The Visualization</returns>
            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
                List<String> visualizations = new List<string>();

                foreach (Type type in System.Reflection.Assembly.GetAssembly(typeof(Visualization)).GetTypes()) {
                    if (type.IsSubclassOf(typeof(Visualization)))
                        visualizations.Add(type.Name);
                }

                return new TypeConverter.StandardValuesCollection(visualizations);
            }

            /// <summary>
            /// Returns true.
            /// </summary>
            /// <param name="context">The context</param>
            /// <returns>True</returns>
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
                return true;
            }
        }

        #endregion

        #endregion
    }

}