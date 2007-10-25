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

namespace Pavel.GUI {
    /// <summary>
    /// This class is an abstract class to create ProjectStarterPages for the different useCases.
    /// It is a simple user control with additional methods especially for executing tasks (e.g. parsing) during the project start.
    /// </summary>
    public class ProjectStarterPage : UserControl {

        #region Execution of the page's task

        /// <summary>
        /// Executes the task of this page (e.g. parsing).
        /// </summary>
        /// <returns>True if the operation was successful</returns>
        virtual public Boolean Execute() {
            throw new NotImplementedException("ProjectStarterPage.Execute() hasn't been overridden!");
        }

        /// <summary>
        /// Undoes the results of this page's task (e.g. clears the project).
        /// </summary>
        virtual public void Undo() {
            throw new NotImplementedException("ProjectStarterPage.Undo() hasn't been overridden!");
        }

        /// <summary>
        /// Resets this page (e.g. when going back).
        /// </summary>
        virtual public void Reset() {
            throw new NotImplementedException("ProjectStarterPage.Reset() hasn't been overridden!");
        }

        /// <summary>
        /// Checks whether the page has a correct input for execution.
        /// </summary>
        virtual public Boolean HasCorrectInput() {
            throw new NotImplementedException("ProjectStarterPage.HasCorrectInput() hasn't been overridden!");
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the ProjectStarterPage.
        /// </summary>
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // ProjectStarterPage
            // 
            this.Name = "ProjectStarterPage";
            this.ResumeLayout(false);
        }

        #endregion
    }
}
