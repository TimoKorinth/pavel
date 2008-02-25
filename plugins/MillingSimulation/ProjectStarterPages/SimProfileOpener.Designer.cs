namespace Pavel.Plugins.ProjectStarterPages {
    partial class SimProfileOpener {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.displacementProfilesLabel = new System.Windows.Forms.Label();
            this.selectProfilesButton = new System.Windows.Forms.Button();
            this.profileList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.displacementProfilesLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.selectProfilesButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.profileList, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(299, 347);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // displacementProfilesLabel
            // 
            this.displacementProfilesLabel.AutoSize = true;
            this.displacementProfilesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.displacementProfilesLabel.Location = new System.Drawing.Point(3, 0);
            this.displacementProfilesLabel.Name = "displacementProfilesLabel";
            this.displacementProfilesLabel.Size = new System.Drawing.Size(111, 13);
            this.displacementProfilesLabel.TabIndex = 9;
            this.displacementProfilesLabel.Text = "Simulation Profiles";
            // 
            // selectProfilesButton
            // 
            this.selectProfilesButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.selectProfilesButton.Location = new System.Drawing.Point(3, 16);
            this.selectProfilesButton.Name = "selectProfilesButton";
            this.selectProfilesButton.Size = new System.Drawing.Size(75, 23);
            this.selectProfilesButton.TabIndex = 3;
            this.selectProfilesButton.Text = "Browse";
            this.selectProfilesButton.UseVisualStyleBackColor = true;
            this.selectProfilesButton.Click += new System.EventHandler(this.selectProfilesButton_Click);
            // 
            // profileList
            // 
            this.profileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.profileList.FormattingEnabled = true;
            this.profileList.HorizontalScrollbar = true;
            this.profileList.Location = new System.Drawing.Point(3, 45);
            this.profileList.Name = "profileList";
            this.profileList.Size = new System.Drawing.Size(293, 199);
            this.profileList.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 247);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(268, 26);
            this.label1.TabIndex = 11;
            this.label1.Text = "The filenames are ordered lexically and assigned to the Histograms in that order." +
                "";
            // 
            // SimProfileOpener
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SimProfileOpener";
            this.Size = new System.Drawing.Size(299, 347);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label displacementProfilesLabel;
        private System.Windows.Forms.Button selectProfilesButton;
        private System.Windows.Forms.ListBox profileList;
        private System.Windows.Forms.Label label1;
    }
}