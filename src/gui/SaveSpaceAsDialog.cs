using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Pavel.Framework;

namespace Pavel.GUI {
    /// <summary> Saves a given Space under a new name. </summary>
    public partial class SaveSpaceAsDialog : Form {

        #region Fields

        private Space workingSpace;
        private Space savedSpace;
        private bool rename;

        #endregion

        #region Properties

        /// <value>Gets the saved Space </value>
        public Space SavedSpace { get { return savedSpace; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the Form and takes the Space to be saved under a new name.
        /// If <paramref name="rename"/> is true, the Space is simply renamed.
        /// </summary>
        /// <param name="workingSpace">Space to be saved under a new name.</param>
        /// <param name="rename">True, if the dialog is just used to rename a Space</param>
        public SaveSpaceAsDialog(Space workingSpace, bool rename) {
            InitializeComponent();

            this.workingSpace = workingSpace;
            this.rename = rename;

            if (rename == true) {
                this.Text = "Rename Space";
                this.nameLabel.Text = "New name for " + workingSpace.Label + "?";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks whether the given name is unique and valid (not empty)
        /// and adds the Space with the given name to the list of Spaces in the Project.
        /// If rename is true, the Space is simply renamed.
        /// </summary>
        private void ChangeSpace() {
            bool uniqueName = true;
            foreach (Space space in ProjectController.Project.spaces) {
                if (nameTextBox.Text.Trim() == space.Label)
                    uniqueName = false;
            }
            if (rename) {
                if ((uniqueName || (workingSpace.Label == nameTextBox.Text.Trim())) && nameTextBox.Text.Trim() != "") {
                    workingSpace.Label = nameTextBox.Text.Trim();
                    this.savedSpace = workingSpace;
                    this.DialogResult = DialogResult.OK;
                } else { MessageBox.Show("Please enter a valid & unique name!", "Error"); }
            } else {
                if (uniqueName && nameTextBox.Text.Trim() != "") {
                    savedSpace = new Space(workingSpace.ColumnProperties, nameTextBox.Text.Trim());
                    ProjectController.Project.spaces.Add(savedSpace);
                    this.DialogResult = DialogResult.OK;
                } else { MessageBox.Show("Please enter a valid & unique name!", "Error"); }
            }
        }

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Calls the ChangeSpace method to save the Space.
        /// </summary>
        /// <param name="sender">saveButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void SaveButton_Click(object sender, EventArgs e) {
            this.ChangeSpace();
        }

        /// <summary>
        /// If the Enter key is pressed in the nameTextBox, the ChangeSpace method is called.
        /// </summary>
        /// <param name="sender">nameTextBox</param>
        /// <param name="e">Standard KeyEventArgs</param>
        private void NameTextBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                this.ChangeSpace();
            }
        }

        #endregion

    }
}