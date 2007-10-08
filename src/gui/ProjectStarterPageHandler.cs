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
    /// This class is a collection of all the ProjectStarterPages to the given useCase.
    /// It is required for the useCase execution and handles the communication with the ProjectStarter.
    /// </summary>
    public class ProjectStarterPageHandler {

        #region Fields

        private List<ProjectStarterPage> controls;
        private int indexer;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the ProjectPageHandler with an empty list of ProjectPages.
        /// </summary>
        public ProjectStarterPageHandler() {
            controls = new List<ProjectStarterPage>();
            indexer = 0;
        }

        /// <summary>
        /// Constructs the ProjectPageHandler with the given list <paramref name="controls"/> of ProjectPages.
        /// </summary>
        /// <param name="controls">A list of ProjectPages</param>
        public ProjectStarterPageHandler(List<ProjectStarterPage> controls)
            : base() {
            controls.AddRange(controls);
        }

        #endregion

        #region Methods

        # region ProjectStarter page movement

        /// <summary>
        /// Clicking the fwdButton in the ProjectStarter invokes this method.
        /// The current ProjectStarterPage is executed and the next one returned.
        /// </summary>
        /// <returns>The next ProjectStarterPage</returns>
        public ProjectStarterPage nextPageControl() {
            // execution of the current page
            if (this.HasStarted()){
                if (!controls[indexer - 1].Execute()) {
                    throw new Exception("Execution failed");
                }
            }

            // returning the next page
            if ((indexer < controls.Count)) {
                return controls[indexer++];
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// Clicking the backButton in the ProjectStarter invokes this method.
        /// The results of the current ProjectStarterPage are undone and the previous page returned.
        /// </summary>
        /// <returns>The previous ProjectStarterPage</returns>
        public ProjectStarterPage previousPageControl() {
            indexer--;
            if (indexer > 0) {
                controls[indexer-1].Undo();
                return controls[indexer-1];
            }
            else {
                return null;
            }
        }

        # endregion

        #region Helpers

        /// <summary>
        /// Adds the given control to the collection of ProjectStarterPages.
        /// </summary>
        /// <param name="control">A ProjectStarterPage to add to controls</param>
        public void Add(ProjectStarterPage control) {
            this.controls.Add(control);
        }

        /// <summary>
        /// Resets the indexer that hold the index of the current page.
        /// </summary>
        public void ResetIndexer() {
            indexer = 0;
        }

        /// <summary>
        /// Resets the indexer and all ProjectStarter pages in the given collection.
        /// </summary>
        public void Reset() {
            ResetIndexer();
            foreach (ProjectStarterPage wp in controls) {
                wp.Reset();
            }
        }

        /// <summary>
        /// Returns true if the last ProjectStarter page is reached.
        /// </summary>
        /// <returns>True if the last ProjectStarter page is reached</returns>
        public Boolean IsLastPage() {
            return (indexer >= controls.Count);
        }

        /// <summary>
        /// Returns true if the ProjectStarter has started.
        /// </summary>
        /// <returns>True if the ProjectStarter has started</returns>
        public Boolean HasStarted() {
            return (indexer > 0);
        }

        /// <summary>
        /// Checks whether the current ProjectStarterPage has correct input to be executed.
        /// </summary>
        /// <returns>True if the current ProjectStarterPage has correct input to be executed</returns>
        public Boolean HasCorrectInput() {
            if (this.HasStarted()) {
                return controls[indexer - 1].HasCorrectInput();
            }
            return true;
        }

        #endregion

        #endregion
    }
}
