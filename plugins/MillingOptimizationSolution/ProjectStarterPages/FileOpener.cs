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

namespace Pavel.Plugins.ProjectStarterPages {

    /// <summary>
    /// This is one of the ProjectStarterPages. It gives the opportunity to choose the required files.
    /// It tries to guess/fill the other files.
    /// </summary>
    public partial class FileOpener : Pavel.GUI.ProjectStarterPageFileOpener {

        #region Fields

        private List<string> stlFileNames = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this ProjectStarterPage.
        /// </summary>
        public FileOpener()
            : base() {
            InitializeComponent();
            stlFileComboBox.Items.Add("");
            this.AddTextBox(objectiveSpaceTextBox, "Objective ColumnSet File|*-obj.txt", browseObj, resetObjBtn);
            this.AddTextBox(decisionSpaceTextBox, "Decision ColumnSet File|*-dec.txt", browseDec, resetDecBtn);
            this.AddTextBox(positionFileTextBox, "Position File|*-pos.txt", browsePos, resetPosBtn);
            this.AddTextBox(toolFileTextBox, "Tool File|*-too.txt", browseToolFile, resetToolBtn);
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

            StreamReader objReader = null;
            StreamReader decReader = null;

            try {
                this.Parent.Parent.Cursor = Cursors.WaitCursor;
                if (objectiveSpaceTextBox.Tag != null) {
                    objReader = new StreamReader((string)objectiveSpaceTextBox.Tag);
                }
                if (decisionSpaceTextBox.Tag != null) {
                    decReader = new StreamReader((string)decisionSpaceTextBox.Tag);
                }
                Pavel.Framework.ParserResult pr = new MillingOptimizationParser().Parse(objReader, decReader);
                Pavel.Framework.ProjectController.NewProject(pr);

            } catch (Exception e) {
                this.Parent.Parent.Cursor = Cursors.Default;
                Pavel.Framework.PavelMain.LogBook.Error(e.Message, true);
                flag = false;
            } finally {
                this.Parent.Parent.Cursor = Cursors.Default;
                if (null != objReader) { objReader.Close(); }
                if (null != decReader) { decReader.Close(); }
            }
            if (stlFileComboBox.SelectedIndex > 0) {
                (Pavel.Framework.ProjectController.Project.UseCase as MillingOptimizationUseCase).STLFile = stlFileNames[stlFileComboBox.SelectedIndex - 1];
            }
            else {
                (Pavel.Framework.ProjectController.Project.UseCase as MillingOptimizationUseCase).STLFile = "";
            }
            if (positionFileTextBox.Tag != null) {
                (Pavel.Framework.ProjectController.Project.UseCase as MillingOptimizationUseCase).PosFile = (string)positionFileTextBox.Tag;
            }
            else {
                (Pavel.Framework.ProjectController.Project.UseCase as MillingOptimizationUseCase).PosFile = "";
            }
            if (toolFileTextBox.Tag != null) {
                (Pavel.Framework.ProjectController.Project.UseCase as MillingOptimizationUseCase).ToolFile = (string)toolFileTextBox.Tag;
            }
            else {
                (Pavel.Framework.ProjectController.Project.UseCase as MillingOptimizationUseCase).ToolFile = "";
            }
            Pavel.Framework.ProjectController.Project.UseCase.SolutionColumnSet = new Pavel.Framework.ColumnSet(Pavel.Framework.ProjectController.Project.columns);
            return flag;
        }

        /// <summary>
        /// Resets the Project.
        /// </summary>
        override public void Undo() {
            Pavel.Framework.ProjectController.ResetProject();
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
        }

        /// <summary>
        /// Checks whether at least the obj and dec space are selected and whether all files exist.
        /// </summary>
        override public Boolean HasCorrectInput() {
            if ((objectiveSpaceTextBox.Tag != null) && (decisionSpaceTextBox.Tag != null)
                    && (stlFileComboBox.SelectedIndex > 0)
                    && (positionFileTextBox.Tag != null) && (toolFileTextBox.Tag!=null)){
                return true;
            } else {
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
                stlFileComboBox.Items.Add(fileNameParts[fileNameParts.Length - 1]);
            }
        }

        #endregion

        #region Event Handling Stuff

        # region Browse Buttons
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
                string[] fileNameParts = fD.FileName.Split('\\');
                stlFileComboBox.SelectedItem = fileNameParts[fileNameParts.Length - 1];
                FillTextBoxes(fD.FileName);
            }
        }

        #endregion

        private void TextBoxWasFilled(object sender, EventArgs e) {
            FillSTL((((sender as Button).Tag as TextBox).Tag as String));
        }

        private void ResetBtn_Click(object sender, EventArgs e) {
            Reset();
        }

        #endregion
    }
}
