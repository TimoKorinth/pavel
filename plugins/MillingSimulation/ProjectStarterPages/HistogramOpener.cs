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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Pavel.Framework;

/// <summary>
/// Namespace for the ProjectStarterPages.
/// </summary>
namespace Pavel.Plugins.ProjectStarterPages {

    /// <summary>
    /// This is one of the ProjectStarterPages, it allows to choose the input files.
    /// </summary>
    public partial class HistogramOpener : Pavel.GUI.ProjectStarterPage {

        #region Constructors

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public HistogramOpener() {
            InitializeComponent();
            decimalSeperatorComboBox.SelectedIndex = 0;
        }

        #endregion

        #region Methods

        #region ProjectStarterPage Members

        /// <summary>
        /// Parses the list of files.
        /// </summary>
        override public Boolean Execute() {
            Boolean flag = true;
            try {
                this.Parent.Parent.Cursor = Cursors.WaitCursor;
                using(StreamReader reader = new StreamReader(testFileBox.Text, System.Text.Encoding.GetEncoding(1252))) {                
                    ExtendedParserResult pr = new MillingSimulationParser((string)decimalSeperatorComboBox.Text).ParseHistogram(reader);
                    pr.MasterPointSet.Label = "Test Histogram";
                    Pavel.Framework.ProjectController.NewProject(pr);
                    MillingSimulationUseCase.openingHistogramPointSets = pr.additionalPointSets;
                }
                //Falls vorhanden, SimulationsFile auch einlesen
                if (File.Exists(this.simFileBox.Text)) {
                    using (StreamReader reader = new StreamReader(simFileBox.Text, System.Text.Encoding.GetEncoding(1252))) {
                        ExtendedParserResult pr = new MillingSimulationParser((string)decimalSeperatorComboBox.Text).ParseHistogram(reader);
                        pr.MasterPointSet.Label = "Simulation Histogram";
                        Pavel.Framework.ProjectController.Project.pointSets.Add(pr.MasterPointSet);
                        MillingSimulationUseCase.simulationPointSet = pr.MasterPointSet;

                        //Kombiniertes PointSet
                        PointSet combinedPointSet = new PointSet("Combined PointSet", pr.MasterPointSet.ColumnSet);
                        combinedPointSet.AddRange(ProjectController.Project.pointSets[0]);
                        combinedPointSet.AddRange(ProjectController.Project.pointSets[1]);
                        ProjectController.Project.pointSets.Add(combinedPointSet);
                        Selection s_test = new Selection();
                        s_test.Label = "Test Histogram";
                        s_test.AddRange(ProjectController.Project.pointSets[0]);
                        Selection s_sim = new Selection();
                        s_sim.AddRange(ProjectController.Project.pointSets[1]);
                        s_sim.Label = "Simulation Histogram";
                        ProjectController.AddSelection(s_test);
                        ProjectController.AddSelection(s_sim);
                    }
                }
            } catch (Exception e) {
                flag = false;
#if !DEBUG
                Pavel.Framework.PavelMain.LogBook.Error(e.Message, true);
#else
                throw e;
#endif
            } finally {
                this.Parent.Parent.Cursor = Cursors.Default;
            }
            return flag;
        }

        /// <summary>
        /// Resets the Project.
        /// </summary>
        override public void Undo() {
            Pavel.Framework.ProjectController.ResetProject();
        }

        /// <summary>
        /// Clears the list of selected files.
        /// </summary>
        override public void Reset() {
            this.testFileBox.Text = "";
        }

        /// <summary>
        /// Checks whether at least one file is selected.
        /// </summary>
        override public Boolean HasCorrectInput() {
            if (File.Exists(this.testFileBox.Text)) {
                return true;
            } else {
                Pavel.Framework.PavelMain.LogBook.Warning("Please select an existing test histogram!", true);
                return false;
            };
        }

        #endregion

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Opens an OpenFileDialog to select an arbitrary number of files to be opened.
        /// </summary>
        /// <param name="sender">The browseObj button</param>
        /// <param name="e">Standard EventArgs</param>
        private void testFileButton_Click(object sender, EventArgs e) {
            OpenFileDialog fD = new OpenFileDialog();
            fD.Filter = "All Files|*.*;";
            fD.CheckFileExists = true;
            fD.Multiselect = false;
            fD.Title = "Choose Histogram File";
            if (fD.ShowDialog() == DialogResult.OK) {
                this.testFileBox.Text = fD.FileName;
            }
        }

        #endregion

        private void simFileButton_Click(object sender, EventArgs e) {
            OpenFileDialog fD = new OpenFileDialog();
            fD.Filter = "All Files|*.*;";
            fD.CheckFileExists = true;
            fD.Multiselect = false;
            fD.Title = "Choose Histogram File";
            if (fD.ShowDialog() == DialogResult.OK) {
                this.simFileBox.Text = fD.FileName;
            }
        }

    }
}
