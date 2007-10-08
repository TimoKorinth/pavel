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
using System.IO;

namespace Pavel.Plugins.ProjectStarterPages {

    /// <summary>
    /// This is one of the ProjectStarterPages. It gives the opportunity to choose the required files.
    /// It tries to guess/fill the other files if empty.
    /// </summary>
    public partial class FileOpener : Pavel.GUI.ProjectStarterPageFileOpener {

        #region Fields

        private List<string> stlFileNames = new List<string>();
        private List<string> additionalFiles = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this ProjectStarterPage.
        /// </summary>
        public FileOpener()
            : base() {
            InitializeComponent();
            stlFileComboBox.Items.Add("");
            this.AddTextBox(objectiveSpaceTextBox, "Objective ColumnSet Files|*-obj.txt", browseObj, clearObjButton);
            this.AddTextBox(decisionSpaceTextBox, "Decision ColumnSet Files|*-dec.txt", browseDec, clearDecButton);
            this.AddTextBox(simTextBox, "Simulation Files|*-sim.txt", browseSIM, clearSimButton);
            this.TextBoxFilledOut += TextBoxWasFilled;
        }

        #endregion

        #region Methods

        #region ProjectStarterPage Members

        /// <summary>
        /// Parses the given files.
        /// </summary>
        override public Boolean Execute() {
            Boolean flag = true;

            StreamReader[] readers = new StreamReader[3 + addFilesLB.Items.Count];

            try {
                this.Parent.Parent.Cursor = Cursors.WaitCursor;
                if (objectiveSpaceTextBox.Tag != null) {
                    readers[0] = new StreamReader(objectiveSpaceTextBox.Tag.ToString());
                }
                if (decisionSpaceTextBox.Tag != null) {
                    readers[1] = new StreamReader(decisionSpaceTextBox.Tag.ToString());
                }
                if (simTextBox.Tag != null) {
                    readers[2] = new StreamReader(simTextBox.Tag.ToString());
                }
                else { readers[2] = null; }
                for (int i = 0; i < addFilesLB.Items.Count; i++) {
                    readers[i + 3] = new StreamReader(additionalFiles[i]);
                }
                Pavel.Framework.ParserResult pr = new MouldTemperatureParser().Parse(readers);
                Pavel.Framework.ProjectController.NewProject(pr);
            }
            catch (Exception e) {
                this.Parent.Parent.Cursor = Cursors.Default;
                Pavel.Framework.PavelMain.LogBook.Error(e.Message, true);
                flag = false;
            }
            finally {
                this.Parent.Parent.Cursor = Cursors.Default;
                foreach (StreamReader sr in readers) {
                    if (null != sr) { sr.Close(); }
                }
            }

            if (stlFileComboBox.SelectedIndex > 0) {
                (Pavel.Framework.ProjectController.Project.UseCase as MouldTemperatureUseCase).STLFile = stlFileNames[stlFileComboBox.SelectedIndex - 1];
            }
            else {
                (Pavel.Framework.ProjectController.Project.UseCase as MouldTemperatureUseCase).STLFile = "";
            }
            Pavel.Framework.ProjectController.Project.UseCase.SolutionColumnSet = new Pavel.Framework.ColumnSet(Pavel.Framework.ProjectController.Project.columns);
            return flag;
        }

        /// <summary>
        /// Empties all the file TextBoxes.
        /// </summary>
        override public void Reset() {
            this.ClearAllTextBoxes();
            stlFileComboBox.SelectedIndex = -1;
            stlFileNames.Clear();
            stlFileComboBox.Items.Clear();
            stlFileComboBox.Items.Add("");
            additionalFiles.Clear();
            addFilesLB.Items.Clear();
        }

        /// <summary>
        /// Checks whether at least the obj and dec space are selected and whether all files exist.
        /// </summary>
        override public Boolean HasCorrectInput() {
            if ((objectiveSpaceTextBox.Tag != null) && (decisionSpaceTextBox.Tag != null)
                && (stlFileComboBox.SelectedIndex > 0) && (simTextBox.Tag != null)) {
                return true;
            }
            else {
                Pavel.Framework.PavelMain.LogBook.Warning("Please choose all files!", true);
                return false;
            };
        }

        #endregion

        /// <summary>
        /// Fills the STL file ComboBox.
        /// </summary>
        /// <param name="fileName">Path to the STL file</param>
        private void FillSTL(String fileName) {
            if (fileName.Contains("\\")) {
                string[] stlFiles = Directory.GetFiles(fileName.Substring(0, fileName.LastIndexOf('\\')), "*.stl");
                foreach (String s in stlFiles) {
                    AddSTL(s);
                }
            }
        }

        /// <summary>
        /// Adds an STL file to the ComboBox if it is not already in it.
        /// </summary>
        /// <param name="fileName">Path to the STL file</param>
        private void AddSTL(String fileName) {
            if (!stlFileNames.Contains(fileName)) {
                stlFileNames.Add(fileName);
                string[] fileNameParts = fileName.Split('\\');
                String stlFileName = fileNameParts[0] +
                    "\\...\\" +
                    fileNameParts[fileNameParts.Length - 1];
                stlFileComboBox.Items.Add(stlFileName);
                stlFileComboBox.SelectedItem = stlFileName;
                this.toolTip.SetToolTip(stlFileComboBox, fileName);
            }
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Opens an OpenFileDialog to select the STL file.
        /// Tries to automatically fill the other TextBoxes if they are empty.
        /// </summary>
        /// <param name="sender">browseSTL</param>
        /// <param name="e">Standard EventArgs</param>
        private void BrowseSTL_Click(object sender, EventArgs e) {
            OpenFileDialog fD = new OpenFileDialog();
            fD.Filter = "STL File|*.stl|All Files|*.*";
            fD.CheckFileExists = true;
            fD.Title = "Choose STL File";
            if (fD.ShowDialog() == DialogResult.OK) {
                AddSTL(fD.FileName);
                FillTextBoxes(fD.FileName);
            }
        }

        private void ResetAddFilesBtn_Click(object sender, EventArgs e) {
            additionalFiles.Clear();
            addFilesLB.Items.Clear();
        }

        private void AddFileBtn_Click(object sender, EventArgs e) {
            OpenFileDialog fD = new OpenFileDialog();
            fD.Filter = "All Files|*.*;|" +
                        "Objective ColumnSet|*-obj.txt;|" +
                        "Decision ColumnSet|*-dec.txt;";
            fD.CheckFileExists = true;
            fD.Multiselect = true;
            fD.Title = "Choose ColumnSet Files";
            if (fD.ShowDialog() == DialogResult.OK) {
                foreach (string s in fD.FileNames) {
                    if (!additionalFiles.Contains(s)) {
                        this.additionalFiles.Add(s);
                        string[] fileNameParts = s.Split('\\');
                        addFilesLB.Items.Add(fileNameParts[fileNameParts.Length - 1]);
                    }
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e) {
            Reset();
        }

        private void TextBoxWasFilled(object sender, EventArgs e) {
            FillSTL((((sender as Button).Tag as TextBox).Tag as String));
        }

        private void DelBtn_Click(object sender, EventArgs e) {
            while (addFilesLB.SelectedItems.Count > 0) {
                additionalFiles.RemoveAt(addFilesLB.SelectedIndices[0]);
                addFilesLB.Items.Remove(addFilesLB.SelectedItems[0]);
            }
        }

        #endregion
    }
}
