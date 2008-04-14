using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Pavel.GUI.Visualizations {
    public partial class ColorProfileDialog : Form {
        public ColorProfileDialog() {
            InitializeComponent();
            this.comboBoxProfiles.DataSource = ColorManagement.ColorProfiles;
            this.comboBoxProfiles.SelectedIndexChanged += new EventHandler(this.SelectedIndexChanged);
            this.comboBoxProfiles.SelectedItem = ColorManagement.ActiveProfile;
            this.newProfile.Click += delegate {
                ColorManagement.ColorProfile newProfile = new ColorManagement.ColorProfile(this.comboBoxProfiles.Text);
                ColorManagement.ColorProfiles.Add(newProfile);
                this.comboBoxProfiles.DataSource = null;
                this.comboBoxProfiles.DataSource = ColorManagement.ColorProfiles;
                this.comboBoxProfiles.SelectedItem = newProfile;
            };
            SelectedIndexChanged(null, null);
        }

        private void SelectedIndexChanged(object sender, EventArgs e) {
            this.propertyGrid.SelectedObject = this.comboBoxProfiles.SelectedItem;
            ColorManagement.ColorProfile currentProfile = (ColorManagement.ColorProfile)this.comboBoxProfiles.SelectedItem;
            ColorManagement.SetActiveProfile(currentProfile);
            groupBoxProfile.Text = "Color Profile: " + currentProfile;
        }

        private void ok_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}