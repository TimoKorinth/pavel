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

/// <summary>
/// Namespace for the ProjectStarterPages.
/// </summary>
namespace Pavel.Plugins.ProjectStarterPages {

    /// <summary>
    /// This is one of the ProjectStarterPages, it allows to choose the input files.
    /// </summary>
    public partial class FileOpener : Pavel.GUI.ProjectStarterPage {

        #region Fields

        private List<string> fileNames = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public FileOpener() {
            InitializeComponent();
        }

        #endregion

        #region Methods

        #region ProjectStarterPage Members

        /// <summary>
        /// Parses the list of files.
        /// </summary>
        override public Boolean Execute() {
            Boolean flag = true;
            StreamReader[] reader = new StreamReader[fileNames.Count];
            for ( int i = 0; i < fileNames.Count; i++ ) {
                reader[i] = new StreamReader(fileNames[i]);
            }
            try {
                this.Parent.Parent.Cursor = Cursors.WaitCursor;
                Pavel.Framework.ParserResult pr = new CSVParser().Parse(reader);
                Pavel.Framework.ProjectController.NewProject(pr);
            } catch (Exception e) {
                flag = false;
#if !DEBUG
                Pavel.Framework.PavelMain.LogBook.Error(e.Message, true);
#else
                throw e;
#endif
            } finally {
                this.Parent.Parent.Cursor = Cursors.Default;
                for ( int i = 0; i < fileNames.Count; i++ ) {
                    if (null != reader[i]) { reader[i].Close(); }
                }
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
            filesListBox.Items.Clear();
            fileNames.Clear();
        }

        /// <summary>
        /// Checks whether at least one file is selected.
        /// </summary>
        override public Boolean HasCorrectInput() {
            if (filesListBox.Items.Count != 0) {
                return true;
            }
            else {
                Pavel.Framework.PavelMain.LogBook.Warning("Please select at least one space file!", true);
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
        private void BrowseObj_Click(object sender, EventArgs e) {
            OpenFileDialog fD = new OpenFileDialog();
            fD.Filter = "All Files|*.*;|" +
                        "Objective ColumnSet|*-obj.txt;|" +
                        "Decision ColumnSet|*-dec.txt;";
            fD.CheckFileExists = true;
            fD.Multiselect = true;
            fD.Title = "Choose ColumnSet Files";
            if (fD.ShowDialog() == DialogResult.OK) {
                foreach (string s in fD.FileNames) {
                    if (!fileNames.Contains(s)) {
                        this.fileNames.Add(s);
                        string[] fileNameParts = s.Split('\\');
                        filesListBox.Items.Add(fileNameParts[fileNameParts.Length - 1]);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the selected files from the list.
        /// </summary>
        /// <param name="sender">The remBtn button</param>
        /// <param name="e">Standard EventArgs</param>
        private void RemBtn_Click(object sender, EventArgs e) {
            while (filesListBox.SelectedItems.Count > 0) {
                fileNames.RemoveAt(filesListBox.SelectedIndices[0]);
                filesListBox.Items.Remove(filesListBox.SelectedItems[0]);
            }
        }

        #endregion
    }
}
