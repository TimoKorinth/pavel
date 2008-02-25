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

using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using Pavel.Framework;
using System;

namespace Pavel.GUI.Visualizations {

    /// <summary>
    /// Parent class of all Visualizations.
    /// </summary>
    public abstract class Visualization : ICustomTypeDescriptor {

        #region Fields

        private VisualizationWindow visualizationWindow;

        #endregion

        #region Properties

        /// <value>Gets the ToolStrip of the Visualization</value>
        public abstract VisualizationStandardToolStrip ToolStrip { get; }

        /// <value>Gets the Control of the Visualization</value>
        public abstract Control Control { get; }

        /// <value>Gets the VisualizationWindow of the Visualization</value>
        public VisualizationWindow VisualizationWindow { get { return visualizationWindow; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Standard constructor that sets the VisualizationWindow and subscribes to events.
        /// </summary>
        /// <param name="vw">The VisualizationWindow</param>
        public Visualization(VisualizationWindow vw) {
            visualizationWindow = vw;
            BindEvents();
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
            return TypeDescriptor.GetProperties(typeof(Visualization));
        }

        public object GetPropertyOwner(PropertyDescriptor pd) {
            return this;
        }

        public object GetEditor(Type editorBaseType) {
            return TypeDescriptor.GetEditor(typeof(Visualization), editorBaseType);
        }

        public AttributeCollection GetAttributes() {
            return TypeDescriptor.GetAttributes(typeof(Visualization));
        }

        public string GetClassName() {
            return TypeDescriptor.GetClassName(typeof(Visualization));
        }

        public string GetComponentName() {
            return TypeDescriptor.GetComponentName(typeof(Visualization));
        }

        public TypeConverter GetConverter() {
            return TypeDescriptor.GetConverter(typeof(Visualization));
        }

        public EventDescriptor GetDefaultEvent() {
            return TypeDescriptor.GetDefaultEvent(typeof(Visualization));
        }

        public PropertyDescriptor GetDefaultProperty() {
            return TypeDescriptor.GetDefaultProperty(typeof(Visualization));
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) {
            return TypeDescriptor.GetEvents(typeof(Visualization), attributes);
        }

        public EventDescriptorCollection GetEvents() {
            return TypeDescriptor.GetEvents(typeof(Visualization));
        }


        #endregion

        #endregion

        /// <summary>
        /// Space changed.
        /// </summary>
        public abstract void UpdateSpace();

        /// <summary>
        /// Make a screenshot
        /// </summary>
        /// <returns>Image of the contents of the VisualizationWindow</returns>
        public abstract System.Drawing.Bitmap Screenshot();

        /// <summary>
        /// Unbinds the events for a given PointSet.
        /// </summary>
        /// <param name="ps">PointSet to unbind events for</param>
        public void UnbindEvents(PointSet ps) {
            ProjectController.CurrentSelection.SelectionModified -= this.SelectionModified;
            ProjectController.SelectionsStateChanged -= this.SelectionsStateChanged;
            ps.pointSetModified -= this.PointSetModified;       
        }

        /// <summary>
        /// Unbinds events.
        /// </summary>
        public void UnbindEvents() {
            ProjectController.CurrentSelection.SelectionModified -= this.SelectionModified;
            ProjectController.SelectionsStateChanged -= SelectionsStateChanged;
            visualizationWindow.PointSet.pointSetModified -= this.PointSetModified;
        }

        /// <summary>
        /// Binds events.
        /// </summary>
        public void BindEvents() {
            ProjectController.CurrentSelection.SelectionModified += this.SelectionModified;
            ProjectController.SelectionsStateChanged += SelectionsStateChanged;
            visualizationWindow.PointSet.pointSetModified += this.PointSetModified;            
        }

        /// <summary>
        /// Deletes all selected Points from the PointSet and clears the CurrentSelection.
        /// </summary>
        public void DeleteSelectedPoints() {
            this.visualizationWindow.PointSet.DeleteSelectedPoints();
        }

        /// <summary>
        /// Inverts the selection of the PointSet, first it clears the current selection, 
        /// then adds all previously unselected points
        /// </summary>
        public void InvertSelection( ) {
            List<Point> unselectedPoints = new List<Point>();
            foreach ( Point p in visualizationWindow.PointSet ) {
                if ( !ProjectController.CurrentSelection.Contains(p) ) {
                    unselectedPoints.Add(p);
                }
            }
            ProjectController.CurrentSelection.ClearAndAddRange(unselectedPoints);
        }

        /// <summary>
        /// Checks if the CurrentSelection contains points that are actually displayed in this Visualization.
        /// </summary>
        /// <returns>True if one or more Points in the displayed PointSet is contained in the current selection</returns>
        public bool VisiblePointsAreSelected() {
            PointSet displayedPointSet = VisualizationWindow.DisplayedPointSet;
            Selection currentSelection = ProjectController.CurrentSelection;
            for (int pi = 0; pi < displayedPointSet.Length; pi++) {
                if (currentSelection.Contains(displayedPointSet[pi])) return true;
            }
            return false;
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Event fired when the current PointSet has been modified, e.g. points have been deleted
        /// </summary>
        /// <param name="sender">PointSet</param>
        /// <param name="e">Standard EventArgs</param>
        public virtual void PointSetModified(object sender, EventArgs e) {
            visualizationWindow.CreateDisplayedPointSet();
        }


        /// <summary>
        /// VizualizationWindow signals: Selection has been modified
        /// Override to react to this Event.
        /// </summary>
        /// <param name="sender">Selection</param>
        /// <param name="e">Standard EventArgs</param>
        public abstract void SelectionModified(object sender, EventArgs e);

        /// <summary>
        /// Selection has been loaded
        /// Standard-Implementation just calls SelectionModified
        /// Override to add customize functionality
        /// </summary>
        /// <param name="sender">Selection</param>
        /// <param name="e">Standard EventArgs</param>
        public virtual void SelectionsStateChanged(object sender, EventArgs e) {
            SelectionModified(sender, e);
        }

        #endregion
    }
}