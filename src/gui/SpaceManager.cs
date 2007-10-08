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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Pavel.Framework;
using Pavel.GUI.Visualizations;


namespace Pavel.GUI {
    /// <summary>
    /// Manager to create new Spaces and manipulate them.
    /// </summary>
    public partial class SpaceManager : Form {

        #region Fields

        private Space selectedSpace;
        private ListBox dragDropOrigin;
        private bool activeChanged;
        private bool compatiblePointsetsVisible;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the SpaceManager and subscribes to events.
        /// </summary>
        public SpaceManager() {
            InitializeComponent();
            this.activeChanged = false;
            this.compatiblePointsetsVisible = true;
            this.spaceComboBox.DataSource = ProjectController.Project.spaces;
            int spaceIndex = PavelMain.MainWindow.ActiveSpaceIndex();
            if (spaceIndex >= 0) { this.spaceComboBox.SelectedIndex = spaceIndex; }
            this.columnListBox.DataSource = ProjectController.Project.columns;
            ChangeSpaceDataSource();
            DisplayCompatiblePointSets();
            addColumnButton.Enabled = true;
            //bind Drag&Drop Events
            this.spaceListBox.DragEnter += delegate(object sender, DragEventArgs e) {
                if (dragDropOrigin == spaceListBox) {
                    e.Effect = DragDropEffects.Move;
                } else if (dragDropOrigin == columnListBox) {
                    e.Effect = DragDropEffects.Copy;
                }
            };
            this.spaceListBox.DragDrop += SpaceListBox_DragDrop;
            this.columnListBox.MouseDown += this.ListBox_MouseDown;
            this.columnListBox.DragEnter += delegate(object sender, DragEventArgs e) {
                if (dragDropOrigin == spaceListBox) {
                    e.Effect = DragDropEffects.Move;
                }
            };
            this.columnListBox.DragDrop += ColumnListBox_DragDrop;
            this.spaceListBox.MouseDown += this.ListBox_MouseDown;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the compatible PointSets in the pointSetListBox.
        /// </summary>
        private void DisplayCompatiblePointSets() {
            this.pointSetListBox.Items.Clear();
            ColumnProperty[] columnProperties = new ColumnProperty[this.spaceListBox.Items.Count];
            for (int i = 0; i < this.spaceListBox.Items.Count; i++) {
                columnProperties[i] = (ColumnProperty)this.spaceListBox.Items[i];
            }
            Space editedSpace = new Space(columnProperties, "TempSpace");
            foreach (PointSet ps in ProjectController.Project.pointSets) {
                if (editedSpace.IsViewOfColumnSet(ps.ColumnSet)) {
                    pointSetListBox.Items.Add(ps);
                }
            }
        }

        /// <summary>
        /// Checks whether the selectedSpace is currently displayed in a VisualizationWindow.
        /// </summary>
        /// <returns>True, if the selectedSpace is currently displayed in a VisualizationWindow</returns>
        private bool SpaceInUse() {
            foreach (Form f in PavelMain.MainWindow.MdiChildren) {
                VisualizationWindow vw = (f as VisualizationWindow);
                if (vw != null) {
                    if (vw.Space.Parent == this.selectedSpace) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Updates the spaceList and the buttons.
        /// </summary>
        private void ChangeSpaceDataSource() {

            this.spaceListBox.Items.Clear();
            if ((this.selectedSpace = spaceComboBox.SelectedItem as Space) != null) {
                this.spaceListBox.Items.AddRange(this.selectedSpace.ColumnProperties);
            }
            moveUpButton.Enabled = false;
            moveDownButton.Enabled = false;
            deleteSpaceButton.Enabled = true;
            resetButton.Enabled = false;
        }

        /// <summary>
        /// Saves the selectedSpace. When one or more Column is still in the Space,
        /// it simply gets the new ColumnProperties. When the Space contains no Column,
        /// a new empty Space is created and the spaceComboBox is updated.
        /// </summary>
        private void SaveSelectedSpace() {
            if (this.spaceListBox.Items.Count >= 1) {
                //Create new columnProperty-Array for the Space
                ColumnProperty[] columnProperties = new ColumnProperty[this.spaceListBox.Items.Count];
                for (int i = 0; i < this.spaceListBox.Items.Count; i++) {
                    columnProperties[i] = (ColumnProperty)this.spaceListBox.Items[i];
                }
                this.selectedSpace.ColumnProperties = columnProperties;
            } else {
                for (int i = 0; i < ProjectController.Project.spaces.Count; i++) {
                    if (selectedSpace.Label == ProjectController.Project.spaces[i].Label) {
                        ProjectController.Project.spaces[i] = new Space(new ColumnProperty[] { }, selectedSpace.Label);

                        // The spaceComboBox must be updated to display the Columns correctly the next time the empty Space is chosen.
                        object tempItem = spaceComboBox.SelectedItem;
                        spaceComboBox.SelectedValueChanged -= SpaceComboBox_SelectedValueChanged;
                        this.spaceComboBox.DataSource = null;
                        this.spaceComboBox.DataSource = ProjectController.Project.spaces;
                        spaceComboBox.SelectedItem = tempItem;
                        spaceComboBox.SelectedValueChanged += SpaceComboBox_SelectedValueChanged;
                        break;
                    }
                }
            }
        }

        #endregion

        #region Event Handling Stuff

        #region Drag&Drop

        /// <summary>
        /// Initializes the beginning of a drag&drop-operation.
        /// </summary>
        /// <param name="sender">A ListBox</param>
        /// <param name="e">Standard MouseEventArgs</param>
        private void ListBox_MouseDown(object sender, MouseEventArgs e) {
            ListBox senderListBox = (ListBox)sender;
            int indexOfItem = senderListBox.IndexFromPoint(e.X, e.Y);
            // check we clicked down on a string 
            if (indexOfItem >= 0 && indexOfItem < senderListBox.Items.Count) {
                this.dragDropOrigin = senderListBox;
                if (senderListBox == spaceListBox) {
                    senderListBox.DoDragDrop(senderListBox.Items[indexOfItem], DragDropEffects.Move);
                    this.removeColumnButton.Enabled = true;
                } else if (senderListBox == columnListBox) {
                    senderListBox.DoDragDrop(senderListBox.Items[indexOfItem], DragDropEffects.Copy);
                    this.addColumnButton.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Handles the dropping of Columns to a Space.
        /// </summary>
        /// <param name="sender">The SpaceListBox</param>
        /// <param name="e">Standard DragEventArgs</param>
        private void SpaceListBox_DragDrop(object sender, DragEventArgs e) {
            // get the places where is the dropped column
            System.Drawing.Point pt = ((ListBox)sender).PointToClient(new System.Drawing.Point(e.X, e.Y));
            int indexOfItem = spaceListBox.IndexFromPoint(pt.X, pt.Y);
            if (indexOfItem > spaceListBox.Items.Count || indexOfItem == -1) {
                indexOfItem = spaceListBox.Items.Count;
            }
            bool changed = false;
            if (dragDropOrigin == columnListBox && e.Data.GetDataPresent("Pavel.Framework.Column")) {
                //Drag from columnListBox to spaceListBox ==> add a column
                foreach (Column newColumn in columnListBox.SelectedItems) {
                    ColumnProperty newCP = newColumn.DefaultColumnProperty.Clone();
                    this.spaceListBox.Items.Insert(indexOfItem, newCP);
                    indexOfItem++;
                }
                changed = true;
            }
            if (dragDropOrigin == spaceListBox && e.Data.GetDataPresent("Pavel.Framework.ColumnProperty")) {
                //Drag from spaceListBox to spaceListBox ==> move a column or select a column
                ColumnProperty movedCP = (ColumnProperty)e.Data.GetData("Pavel.Framework.ColumnProperty");
                int indexOfMovedItem = spaceListBox.Items.IndexOf(movedCP);
                if (indexOfItem != indexOfMovedItem) { //Move a column
                    if (indexOfMovedItem >= 0 && indexOfMovedItem < spaceListBox.Items.Count) {
                        this.spaceListBox.Items.Remove(movedCP);
                        //Decrement index if an item before the drop target has been removed
                        if (indexOfMovedItem < indexOfItem)
                            indexOfItem--;
                        this.spaceListBox.Items.Insert(indexOfItem, movedCP);
                        this.spaceListBox.SelectedItem = movedCP;
                        changed = true;
                    }
                } else { //Select a column
                    if (spaceListBox.SelectedItems.Count == 1) {
                        if (spaceListBox.SelectedIndex == 0) {
                            //first item selected
                            moveUpButton.Enabled = false;
                        } else {
                            moveUpButton.Enabled = true;
                        }
                        if (spaceListBox.SelectedIndex == spaceListBox.Items.Count - 1) {
                            //Last item selected
                            moveDownButton.Enabled = false;
                        } else {
                            moveDownButton.Enabled = true;
                        }
                    } else {
                        moveUpButton.Enabled = false;
                        moveDownButton.Enabled = false;
                    }
                }
            }
            if (changed) {
                this.activeChanged = true;
                resetButton.Enabled = true;
                DisplayCompatiblePointSets();
            }
        }

        /// <summary>
        /// Handles the dropping of Columns to the ColumnListBox.
        /// </summary>
        /// <param name="sender">The ColumnListBox</param>
        /// <param name="e">Standard DragEventArgs</param>
        private void ColumnListBox_DragDrop(object sender, DragEventArgs e) {
            if (this.dragDropOrigin == spaceListBox) {
                RemoveColumnButton_Click(sender, e);
            }
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Opens the NewSpaceDialog to create a new Space.
        /// </summary>
        /// <param name="sender">newSpace Button</param>
        /// <param name="e">Standard EventArgs</param>
        private void NewSpace_Click(object sender, EventArgs e) {
            NewSpaceDialog nsvd = new NewSpaceDialog();
            if (nsvd.ShowDialog() == DialogResult.OK) {
                ProjectController.Project.spaces.Add(nsvd.NewSpace);
                this.spaceComboBox.DataSource = null;
                this.spaceComboBox.DataSource = ProjectController.Project.spaces;
                this.spaceComboBox.SelectedItem = nsvd.NewSpace;
            }
        }

        /// <summary>
        /// Opens the SaveSpaceAsDialog to rename the selected Space.
        /// </summary>
        /// <param name="sender">renameSaveButton</param>
        /// <param name="e">EventArgs</param>
        private void RenameSpaceButton_Click(object sender, EventArgs e) {
            SaveSpaceAsDialog sad = new SaveSpaceAsDialog(this.selectedSpace, true);
            if (sad.ShowDialog() == DialogResult.OK) {
                this.spaceComboBox.SelectedValueChanged -= SpaceComboBox_SelectedValueChanged;
                this.spaceComboBox.DataSource = null;
                this.spaceComboBox.DataSource = ProjectController.Project.spaces;
                this.spaceComboBox.SelectedItem = sad.SavedSpace;
                this.spaceComboBox.SelectedValueChanged += SpaceComboBox_SelectedValueChanged;
            }
        }

        /// <summary>
        /// Deletes the selected Space.
        /// </summary>
        /// <param name="sender">deleteSpaceButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void DeleteSpaceButton_Click(object sender, EventArgs e) {
            if ((this.selectedSpace != null) &&
                (MessageBox.Show("Do you really want to delete this Space?\nAll visualizations of this Space will automatically be close!", "Delete " + this.selectedSpace.Label, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)) {
                ProjectController.RemoveSpace(this.selectedSpace);
                this.activeChanged = false;
                this.spaceComboBox.DataSource = null;
                this.spaceComboBox.DataSource = ProjectController.Project.spaces;
                ChangeSpaceDataSource();
                DisplayCompatiblePointSets();
                if (spaceComboBox.Items.Count > 0) {
                    spaceComboBox.SelectedIndex = 0;
                }
                ProjectController.SetProjectChanged(true);
            }
        }

        /// <summary>
        /// Moves the selected Column up one position in the spaceListBox.
        /// </summary>
        /// <param name="sender">moveUpButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void MoveUpButton_Click(object sender, EventArgs e) {
            if (this.spaceListBox.SelectedIndex > 0) {
                //Swap Columns in ListView
                int pos = this.spaceListBox.SelectedIndex;
                object tmp = this.spaceListBox.SelectedItem;
                this.spaceListBox.Items.RemoveAt(pos);
                this.spaceListBox.Items.Insert(pos - 1, tmp);
                this.spaceListBox.SelectedIndex = pos - 1;
                this.activeChanged = true;
                resetButton.Enabled = true;
            }
        }

        /// <summary>
        /// Moves the selected Column down one position in the spaceListBox.
        /// </summary>
        /// <param name="sender">moveDownButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void MoveDownButton_Click(object sender, EventArgs e) {
            if (this.spaceListBox.SelectedIndex < spaceListBox.Items.Count - 1) {
                //Swap Columns in ListView
                int pos = this.spaceListBox.SelectedIndex;
                object tmp = this.spaceListBox.SelectedItem;
                this.spaceListBox.Items.RemoveAt(pos);
                this.spaceListBox.Items.Insert(pos + 1, tmp);
                this.spaceListBox.SelectedIndex = pos + 1;
                this.activeChanged = true;
                resetButton.Enabled = true;
            }
        }

        /// <summary>
        /// Opens the indiColumnWindow.
        /// </summary>
        /// <param name="sender">newColumnButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void NewColumnButton_Click(object sender, EventArgs e) {
            //Check if the spacemanager is the single window
            if (PavelMain.MainWindow.MdiChildren.Length != 0) {
                if (MessageBox.Show("You cannot add a new column until all visualization windows are closed. Close open visualizations?", "Close open visualizations?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
                    PavelMain.MainWindow.CloseAllChildren();
                } else { return; }
            }
            IndividualColumnWindow indiColumnWindow = new IndividualColumnWindow();
            DialogResult dr = indiColumnWindow.ShowDialog();
            if (dr == DialogResult.OK) {
                this.columnListBox.DataSource = null;
                this.columnListBox.DataSource = ProjectController.Project.columns;
            }
        }

        /// <summary>
        /// Add the selected Column to the Space.
        /// </summary>
        /// <param name="sender">addColumnButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void AddColumnButton_Click(object sender, EventArgs e) {
            int indexOfItem = spaceListBox.SelectedIndex;
            if (indexOfItem == -1) { indexOfItem++; }
            foreach (Column newColumn in columnListBox.SelectedItems) {
                ColumnProperty newCP = newColumn.DefaultColumnProperty.Clone();
                this.spaceListBox.Items.Insert(indexOfItem, newCP);
                indexOfItem++;
            }
            this.activeChanged = true;
            resetButton.Enabled = true;
            DisplayCompatiblePointSets();
        }

        /// <summary>
        /// Removes the selected Column from the spaceListBox.
        /// </summary>
        /// <param name="sender">removeColumnButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void RemoveColumnButton_Click(object sender, EventArgs e) {
            int selection = this.spaceListBox.SelectedIndex;
            ColumnProperty[] cps = new ColumnProperty[spaceListBox.SelectedItems.Count];
            for (int i = 0; i < cps.Length; i++) {
                cps[i] = (ColumnProperty)spaceListBox.SelectedItems[i];
            }
            foreach (ColumnProperty cp in cps) {
                this.spaceListBox.Items.Remove(cp);
            }
            //select item above the deleted item
            if (this.spaceListBox.Items.Count > 0) {
                if (selection == 0) {
                    this.spaceListBox.SelectedIndex = 0;
                } else {
                    this.spaceListBox.SelectedIndex = selection - 1;
                }
            }
            if (spaceListBox.SelectedIndex >= 0) {
                removeColumnButton.Enabled = true;
            } else {
                removeColumnButton.Enabled = false;
            }
            this.activeChanged = true;
            resetButton.Enabled = true;
            DisplayCompatiblePointSets();
        }

        private void ShowHideCompatiblePointsetsButton_Click(object sender, EventArgs e) {
            this.compatiblePointsetsVisible = !this.compatiblePointsetsVisible;
            if (this.compatiblePointsetsVisible) {
                showHideCompatiblePointsetsButton.Image = global::Pavel.Properties.Resources.Down;
                pointSetListBox.Visible = true;
                compatiblePointSetsLabel.Location = new System.Drawing.Point(7, 400);
                compatiblePointSetsPanel.Location = new System.Drawing.Point(117, 397);
                spaceListBox.Size = new System.Drawing.Size(245, 303);
                spacePanel.Size = new System.Drawing.Size(245, 324);
            } else {
                showHideCompatiblePointsetsButton.Image = global::Pavel.Properties.Resources.Up;
                pointSetListBox.Visible = false;
                compatiblePointSetsLabel.Location = new System.Drawing.Point(7, 487);
                compatiblePointSetsPanel.Location = new System.Drawing.Point(117, 484);
                spaceListBox.Size = new System.Drawing.Size(245, 394);
                spacePanel.Size = new System.Drawing.Size(245, 415);
            }
        }

        /// <summary>
        /// Resets the Space.
        /// </summary>
        /// <param name="sender">resetButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void ResetButton_Click(object sender, EventArgs e) {
            ChangeSpaceDataSource();
            this.activeChanged = false;
        }

        /// <summary>
        /// The OK button saves the active Space and closes the dialog.
        /// </summary>
        /// <param name="sender">cancelButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void OkButton_Click(object sender, EventArgs e) {
            if (this.activeChanged) {
                SaveSelectedSpace();
                this.activeChanged = false;
                resetButton.Enabled = false;
                ProjectController.SetProjectChanged(true);
            }

            this.Close();
        }

        /// <summary>
        /// The cancel button simply closes the dialog and does not save the active Space.
        /// </summary>
        /// <param name="sender">cancelButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void CancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }

        #endregion

        /// <summary>
        /// Updates the spaceListBox, when a different Space is selected.
        /// </summary>
        /// <param name="sender">spaceComboBox</param>
        /// <param name="e">Standard EventArgs</param>
        private void SpaceComboBox_SelectedValueChanged(object sender, EventArgs e) {
            if (this.activeChanged && selectedSpace != this.spaceComboBox.SelectedItem) {
                DialogResult result = MessageBox.Show("Save changes?", "Continue?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result) {
                    case DialogResult.Yes:
                        SaveSelectedSpace();
                        ChangeSpaceDataSource();
                        DisplayCompatiblePointSets();
                        this.activeChanged = false;
                        break;
                    case DialogResult.No:
                        ChangeSpaceDataSource();
                        DisplayCompatiblePointSets();
                        this.activeChanged = false;
                        break;
                    case DialogResult.Cancel:
                        this.spaceComboBox.SelectedItem = selectedSpace;
                        return;
                }
            } else {
                if (!this.activeChanged) {
                    ChangeSpaceDataSource();
                    DisplayCompatiblePointSets();
                }
            }
            if (columnListBox.SelectedIndex >= 0) {
                addColumnButton.Enabled = true;
            } else {
                addColumnButton.Enabled = false;
            }
            if (spaceListBox.SelectedIndex >= 0) {
                removeColumnButton.Enabled = true;
            } else {
                removeColumnButton.Enabled = false;
            }
        }

        /// <summary>
        /// Erases all Spaces with less than two Columns when the SpaceManager is closed.
        /// </summary>
        /// <param name="sender">SpaceManager</param>
        /// <param name="e">Standard FormClosingEventArgs</param>
        private void SpaceManager_FormClosing(object sender, FormClosingEventArgs e) {
            // Checks whether there is any Space containing less than two Columns.
            List<Space> tooSmall = new List<Space>();
            string message = "The following Spaces contain less than two Columns:";
            foreach (Space s in ProjectController.Project.spaces) {
                if (s.ColumnProperties.Length < 2) {
                    tooSmall.Add(s);
                    message += "\n                      " + s.Label;
                }
            }
            message += "\n\n" + "If you continue these Spaces will be deleted.";

            // If there is a Space with less than two Columns the user is warned, that such Spaces will be erased.
            if (tooSmall.Count > 0) {
                DialogResult result = MessageBox.Show(message, "Continue?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                switch (result) {
                    case DialogResult.OK:
                        for (int i = 0; i < tooSmall.Count; i++) {
                            ProjectController.RemoveSpace(tooSmall[i]);
                        }
                        ProjectController.SetProjectChanged(true);
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
        }

        #endregion

    }
}