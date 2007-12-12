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
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Pavel.GUI {
    /// <summary>
    /// Combines members for all FileOpener ProjectStarterPages.
    /// </summary>
    public abstract class ProjectStarterPageFileOpener : Pavel.GUI.ProjectStarterPage {

        #region Fields

        private List<TextBox> fileNameBoxes;
        private Dictionary<String, String> filterDict;
        protected ToolTip toolTip;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the ProjectStarterPageFileOpener
        /// </summary>
        public ProjectStarterPageFileOpener() {
            toolTip = new ToolTip();
            fileNameBoxes = new List<TextBox>();
            filterDict = new Dictionary<string, string>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the Project.
        /// </summary>
        override public void Undo() {
            Pavel.Framework.ProjectController.ResetProject();
        }

        /// <summary>
        /// Adds a TextBox to the FileOpener ProjectStarterPage.
        /// </summary>
        /// <param name="textBox">TextBox to be controlled</param>
        /// <param name="filter">Filter that appears in the OpenDialog. Has to be like
        /// "Filterdescription|*-dec.txt". The first before the "|" will be used as 
        /// the open dialog descrition, the second will be used as suffix without first character (normally *) 
        /// to add other values</param>
        /// <param name="browseBtn">Button, that is used to show the open dialog. All things will be handled
        /// in this class, no need to define anything more outside</param>
        /// <param name="resetBtn">Button, that clears the text box. All things will be handled here, no need to
        /// define anythinge more</param>
        public void AddTextBox(TextBox textBox, String filter, Button browseBtn, Button resetBtn) {
            fileNameBoxes.Add(textBox);
            filterDict[textBox.Name] = filter;
            browseBtn.Tag = textBox;
            browseBtn.Click += this.OpenBrowseBtnClick;
            resetBtn.Tag = textBox;
            resetBtn.Click += this.clearTextBox;
        }

        /// <summary>
        /// Shows an OpenFileDialog for the specified TextBox.
        /// </summary>
        /// <param name="title">Title for opening the file</param>
        /// <param name="targetBox">TargetTextBox. Has to be added with AddTextBox before</param>
        /// <returns>True, if clicked Ok in the dialog, false otherwise.</returns>
        protected Boolean OpenBrowseDialog(String title, TextBox targetBox) {
            Boolean retValue = false;
            OpenFileDialog fD = new OpenFileDialog();
            fD.CheckFileExists = true;
            fD.Title = title;
            fD.Filter = filterDict[targetBox.Name] + "|All text files|*.txt|All files|*.*";
            if (fD.ShowDialog() == DialogResult.OK) {
                targetBox.Text = getShortFilename(fD.FileName);
                targetBox.Tag = fD.FileName;
                toolTip.SetToolTip(targetBox, fD.FileName);
                FillTextBoxes(fD.FileName);
                retValue = true;
            }
            return retValue;
        }

        /// <summary>
        /// Browse button was clicked. The corresponding text box to be filled will be determined
        /// by the sender.tag.
        /// </summary>
        /// <param name="sender">Browse button, that is clicked. Sender.tag contains TextBox to be handled</param>
        /// <param name="e">Normal eventargs</param>
        protected void OpenBrowseBtnClick(object sender, EventArgs e) {
            String[] splitFilter = filterDict[((sender as Button).Tag as TextBox).Name].Split('|');
            if (OpenBrowseDialog(splitFilter[0] + "...", ((sender as Button).Tag as TextBox))) {
                if (this.TextBoxFilledOut != null) {
                    this.TextBoxFilledOut(sender, e);
                }
            }
        }

        /// <summary>
        /// Tries to fill all TextBboxes when one file is chosen.
        /// </summary>
        /// <param name="fileName">File chosen</param>
        protected void FillTextBoxes(String fileName) {
            if (fileName.Contains("-")) {
                String generalName = fileName.Substring(0, fileName.LastIndexOf('-'));
                foreach (TextBox tb in fileNameBoxes) {
                    String[] splitFilter = filterDict[tb.Name].Split('|');
                    String curFile = generalName + splitFilter[1].Substring(1);
                    if ((tb.Text == "") && (File.Exists(curFile))) {
                        tb.Tag = curFile;
                        tb.Text = getShortFilename(curFile);
                        toolTip.SetToolTip(tb, curFile);
                    }
                }
            }
        }

        /// <summary>
        /// Clears all registered TextBoxes.
        /// </summary>
        protected void ClearAllTextBoxes() {
            foreach (TextBox tb in fileNameBoxes) {
                ClearTextBox(tb);
            }
        }

        /// <summary>
        /// Clears the given TextBox.
        /// </summary>
        /// <param name="tb">TextBox to be cleared</param>
        protected void ClearTextBox(TextBox tb) {
            tb.Tag = null;
            tb.Text = "";
        }

        protected void clearTextBox(object sender, EventArgs e) {
            this.ClearTextBox(((sender as Button).Tag as TextBox));
        }

        /// <summary>
        /// This Event will be fired, if the OpenBrowseDialog is handled successful.
        /// </summary>
        public event EventHandler TextBoxFilledOut;

        /// <summary>
        /// Shortens a given filename.
        /// </summary>
        /// <param name="longFilename">Filename to be shortened</param>
        /// <returns>Shortened filename, e.g. "C:\...\Filename.txt"</returns>
        private String getShortFilename(String longFilename) {
            String[] fileNameParts = longFilename.Split('\\');
            return fileNameParts[0] + @"\...\" + fileNameParts[fileNameParts.Length - 1];
        }

        #endregion
    }
}
