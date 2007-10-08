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

namespace Pavel.Framework {
    /// <summary>
    /// A Selection of Pavel.Framework.Points
    /// </summary>
    public class Selection:IEnumerable<Point> {

        #region Fields
        private Dictionary<Point,object> points = new Dictionary<Point,object>();
        private String label = "";
        private bool active = false;

        /// <value>Event fired if Selection is modified </value>
        [field: NonSerializedAttribute()]
        public event EventHandler SelectionModified;

        #endregion

        #region Properties

        /// <value> Gets the current number of Points in the Selection </value>
        [CoverageExclude]
        public int Length {
            get { return points.Count; }
        }

        /// <value> Gets the label of the Selection or sets it </value>
        [CoverageExclude]
        public String Label {
            get { return label; }
            set { label = value; }
        }

        /// <value> Gets the active-state of the Selection or sets it </value>
        [CoverageExclude]
        public bool Active {
            get { return active; }
            set { active = value; }
        }

        /// <value> Gets an array containing all Points in the Selection </value>
        public Point[] GetPoints {
            get {
                Point[] points = new Point[this.points.Count];
                this.points.Keys.CopyTo(points, 0);
                return points;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a single Point to the Selection and fires a SelectionModified-event.
        /// </summary>
        /// <param name="point">Point to add</param>
        public void Add(Point point){
            if (!this.points.ContainsKey(point)){ 
                this.points.Add(point, null);
                if ( null != SelectionModified ) { SelectionModified(this, EventArgs.Empty); }
            }            
        }

        /// <summary>
        /// Adds the Points not already contained in the Selection and fires a SelectionModified-event.
        /// </summary>
        /// <param name="points">A number of Points to be added</param>
        public void AddRange(IEnumerable<Point> points) {
            int pointsAdded = 0;
            foreach ( Point p in points ) { 
                if (!this.points.ContainsKey(p)){ 
                    this.points.Add(p, null);
                    pointsAdded++;
                } 
            }
            if ( null != SelectionModified && pointsAdded != 0 ) { SelectionModified(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Removes a single Point from the Selection.
        /// Fires a SelectionModified-event if the given Point could be removed.
        /// </summary>
        /// <param name="point">Point to be removed</param>
        public void Remove(Point point) {
            if ( this.points.Remove(point) ) {
                if ( null != SelectionModified ) { SelectionModified(this, EventArgs.Empty); }
            }
        }

        /// <summary>
        /// Removes all given Points from the Selection, if they are in Selection.
        /// Fires a SelectionModified-event if at least one of the given points could be removed.
        /// </summary>
        /// <param name="points">Number of Points to be removed</param>
        public void RemovePoints(IEnumerable<Point> points) {
            int pointsAdded = 0;
            foreach ( Point p in points ) {
                if ( this.points.Remove(p) ) { pointsAdded++; }
            }
            if ( null != SelectionModified && pointsAdded != 0 ) { SelectionModified(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Checks if the given Point is contained in the Selection.
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if point is in the Selection.</returns>
        public bool Contains(Point point) {
            return points.ContainsKey(point);
        }

        /// <summary>
        /// Clears the Selection.
        /// </summary>
        public void Clear() {
            if ( points.Count == 0 ) { return; }
            this.points.Clear();
            if ( null != SelectionModified ) { SelectionModified(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Clears the selection and adds a single point
        /// </summary>
        /// <param name="p">Point to add in empty selection</param>
        public void ClearAndAdd(Point p) {
            this.points.Clear();
            this.points.Add(p, null);
            if ( null != SelectionModified ) { SelectionModified(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Clears the selection and add multiple points
        /// </summary>
        /// <param name="points">Points to add in empty selection</param>
        public void ClearAndAddRange(IEnumerable<Point> points) {
            this.points.Clear();
            foreach ( Point p in points ) {
                this.points.Add(p, null);
            }
            if ( null != SelectionModified ) { SelectionModified(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Overrides the ToString() method to return a modified label.
        /// </summary>
        /// <returns>Modified label</returns>
        public override String ToString() {
            return "\"" + label + "\" Länge:" + Length;
        }

        #region Enumerators

        /// <summary>
        /// Returns an Enumerator over the Points contained in the Selection.
        /// </summary>
        /// <returns>An Enumerator over the Points contained in the Selection</returns>
        [CoverageExclude]
        public IEnumerator<Point> GetEnumerator() {
            return points.Keys.GetEnumerator();
        }

        /// <summary>
        /// Returns an Enumerator over the Points contained in the Selection.
        /// </summary>
        /// <returns>An Enumerator over the Points contained in the Selection</returns>
        [CoverageExclude]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// EventHandler for Selection modifications.
        /// </summary>
        /// <param name="sender">This Selection</param>
        /// <param name="e">Standard EventArgs</param>
        public delegate void SelectionModifiedEventHandler(object sender, EventArgs e);
        
        #endregion
    }  
}
