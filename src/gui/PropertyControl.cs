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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Pavel.Framework;
using Pavel.GUI.Visualizations;
using Pavel.GUI.SolutionVisualizations;

namespace Pavel.GUI {
    /// <summary>
    /// Contains the PropertyGrids for the Visualizations and the VisualizationWindows.
    /// </summary>
    public partial class PropertyControl : UserControl {

        #region Properties

        /// <value>
        /// Assigns a VisualizationWindow to the PropertyGrids.
        /// Sets the event subscriptions to the new VisualizationWindow
        /// </value>
        public Form VisualizationWindow {
            set {
                if (value is VisualizationWindow) {
                    visualizationWindowPropertyGrid.SelectedObject = value;
                    label2.Text = "Visualization";
                    visualizationPropertyGrid.SelectedObject = (null == value) ? null : (value as VisualizationWindow).Visualization;
                    splitContainer1.Panel1Collapsed = false;
                } else if (value is Solution) {
                    visualizationPropertyGrid.SelectedObject = value;
                    label2.Text = "Solution";
                    visualizationWindowPropertyGrid.SelectedObject = null;
                    splitContainer1.Panel1Collapsed = true;
                } else {
                    visualizationWindowPropertyGrid.SelectedObject = null;
                    label2.Text = "Visualization";
                    visualizationPropertyGrid.SelectedObject = null;
                }
                
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the PropertyControl.
        /// </summary>
        public PropertyControl() {
            InitializeComponent();
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// The MDIChild of the MainWindow changes.
        /// Change current VisualizationWindow if the activated window is a VisualizationWindow.
        /// Does nothing if the activated window is this.
        /// Sets current VisualizationWindow to null in all other cases.
        /// </summary>
        /// <param name="sender">MainWindow</param>
        /// <param name="e">Standard EventArgs</param>
        public void MainWindow_MdiChildActivate(object sender, EventArgs e) {
            Form activatedWindow = (sender as MainWindow).ActiveMdiChild;
            VisualizationWindow = activatedWindow;
        }   

        #endregion
    }

    /// <summary>
    /// Use this Attribute to denote that a Property should be displayed in the PropertyWindow.
    /// </summary>
    public class ShowInPropertiesAttribute : Attribute {
    }
}
