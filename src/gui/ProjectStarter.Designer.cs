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

namespace Pavel.GUI {
    partial class ProjectStarter {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectStarter));
            this.formTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.standardLayout = new System.Windows.Forms.TableLayoutPanel();
            this.openProjectRB = new Pavel.GUI.DoubleClickableRadioButton();
            this.recentProjectsList = new System.Windows.Forms.ListBox();
            this.createProjectRB = new System.Windows.Forms.RadioButton();
            this.useCaseList = new System.Windows.Forms.ListBox();
            this.openRecentProjectRB = new System.Windows.Forms.RadioButton();
            this.recentProjectsLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.backButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.fwdButton = new System.Windows.Forms.Button();
            this.formTableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.mainPanel.SuspendLayout();
            this.standardLayout.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // formTableLayout
            // 
            this.formTableLayout.ColumnCount = 2;
            this.formTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.88664F));
            this.formTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.11336F));
            this.formTableLayout.Controls.Add(this.logoPictureBox, 0, 0);
            this.formTableLayout.Controls.Add(this.mainPanel, 1, 0);
            this.formTableLayout.Controls.Add(this.tableLayoutPanel1, 1, 2);
            this.formTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formTableLayout.Location = new System.Drawing.Point(0, 0);
            this.formTableLayout.Name = "formTableLayout";
            this.formTableLayout.RowCount = 3;
            this.formTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.21428F));
            this.formTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 76.78571F));
            this.formTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.formTableLayout.Size = new System.Drawing.Size(494, 375);
            this.formTableLayout.TabIndex = 0;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(3, 3);
            this.logoPictureBox.Name = "logoPictureBox";
            this.formTableLayout.SetRowSpan(this.logoPictureBox, 3);
            this.logoPictureBox.Size = new System.Drawing.Size(112, 369);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoPictureBox.TabIndex = 14;
            this.logoPictureBox.TabStop = false;
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.standardLayout);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(121, 3);
            this.mainPanel.Name = "mainPanel";
            this.formTableLayout.SetRowSpan(this.mainPanel, 2);
            this.mainPanel.Size = new System.Drawing.Size(370, 330);
            this.mainPanel.TabIndex = 2;
            this.mainPanel.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.MainPanel_ControlAdded);
            // 
            // standardLayout
            // 
            this.standardLayout.ColumnCount = 1;
            this.standardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.standardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.standardLayout.Controls.Add(this.openProjectRB, 0, 5);
            this.standardLayout.Controls.Add(this.recentProjectsList, 0, 4);
            this.standardLayout.Controls.Add(this.createProjectRB, 0, 0);
            this.standardLayout.Controls.Add(this.useCaseList, 0, 1);
            this.standardLayout.Controls.Add(this.openRecentProjectRB, 0, 2);
            this.standardLayout.Controls.Add(this.recentProjectsLabel, 0, 3);
            this.standardLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.standardLayout.Location = new System.Drawing.Point(0, 0);
            this.standardLayout.Name = "standardLayout";
            this.standardLayout.RowCount = 6;
            this.standardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.standardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 83.70786F));
            this.standardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.standardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.29214F));
            this.standardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 103F));
            this.standardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.standardLayout.Size = new System.Drawing.Size(370, 330);
            this.standardLayout.TabIndex = 0;
            // 
            // openProjectRB
            // 
            this.openProjectRB.AutoSize = true;
            this.openProjectRB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openProjectRB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openProjectRB.Location = new System.Drawing.Point(3, 299);
            this.openProjectRB.Name = "openProjectRB";
            this.openProjectRB.Size = new System.Drawing.Size(364, 28);
            this.openProjectRB.TabIndex = 22;
            this.openProjectRB.Text = "Open Project...";
            this.openProjectRB.UseVisualStyleBackColor = true;
            this.openProjectRB.CheckedChanged += new System.EventHandler(this.OpenProjectRB_CheckedChanged);
            // 
            // recentProjectsList
            // 
            this.recentProjectsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentProjectsList.FormattingEnabled = true;
            this.recentProjectsList.HorizontalScrollbar = true;
            this.recentProjectsList.Location = new System.Drawing.Point(3, 196);
            this.recentProjectsList.Name = "recentProjectsList";
            this.recentProjectsList.Size = new System.Drawing.Size(364, 95);
            this.recentProjectsList.TabIndex = 18;
            this.recentProjectsList.DoubleClick += new System.EventHandler(this.RecentProjectsList_DoubleClick);
            this.recentProjectsList.SelectedIndexChanged += new System.EventHandler(this.RecentProjectsList_SelectedIndexChanged);
            this.recentProjectsList.Click += new System.EventHandler(this.RecentProjectsList_Click);
            // 
            // createProjectRB
            // 
            this.createProjectRB.AutoSize = true;
            this.createProjectRB.Checked = true;
            this.createProjectRB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createProjectRB.Location = new System.Drawing.Point(3, 3);
            this.createProjectRB.Name = "createProjectRB";
            this.createProjectRB.Size = new System.Drawing.Size(135, 17);
            this.createProjectRB.TabIndex = 19;
            this.createProjectRB.TabStop = true;
            this.createProjectRB.Text = "Create New Project";
            this.createProjectRB.UseVisualStyleBackColor = true;
            this.createProjectRB.CheckedChanged += new System.EventHandler(this.CreateProjectRB_CheckedChanged);
            // 
            // useCaseList
            // 
            this.useCaseList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.useCaseList.FormattingEnabled = true;
            this.useCaseList.Location = new System.Drawing.Point(3, 27);
            this.useCaseList.Name = "useCaseList";
            this.useCaseList.Size = new System.Drawing.Size(364, 108);
            this.useCaseList.TabIndex = 20;
            this.useCaseList.DoubleClick += new System.EventHandler(this.UseCaseList_DoubleClick);
            this.useCaseList.SelectedIndexChanged += new System.EventHandler(this.UseCaseList_SelectedIndexChanged);
            this.useCaseList.Click += new System.EventHandler(this.UseCaseList_Click);
            // 
            // openRecentProjectRB
            // 
            this.openRecentProjectRB.AutoSize = true;
            this.openRecentProjectRB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openRecentProjectRB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openRecentProjectRB.Location = new System.Drawing.Point(3, 142);
            this.openRecentProjectRB.Name = "openRecentProjectRB";
            this.openRecentProjectRB.Size = new System.Drawing.Size(364, 26);
            this.openRecentProjectRB.TabIndex = 17;
            this.openRecentProjectRB.Text = "Open Recent Project...";
            this.openRecentProjectRB.UseVisualStyleBackColor = true;
            this.openRecentProjectRB.CheckedChanged += new System.EventHandler(this.OpenRecentProjectRB_CheckedChanged);
            // 
            // recentProjectsLabel
            // 
            this.recentProjectsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentProjectsLabel.Location = new System.Drawing.Point(3, 171);
            this.recentProjectsLabel.Name = "recentProjectsLabel";
            this.recentProjectsLabel.Size = new System.Drawing.Size(364, 22);
            this.recentProjectsLabel.TabIndex = 21;
            this.recentProjectsLabel.Text = "Recent Projects:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.backButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.fwdButton, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(121, 339);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(370, 33);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // backButton
            // 
            this.backButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backButton.Enabled = false;
            this.backButton.Location = new System.Drawing.Point(3, 3);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(86, 27);
            this.backButton.TabIndex = 2;
            this.backButton.Text = "< &Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cancelButton.Location = new System.Drawing.Point(279, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(88, 27);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // fwdButton
            // 
            this.fwdButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fwdButton.Enabled = false;
            this.fwdButton.Location = new System.Drawing.Point(95, 3);
            this.fwdButton.Name = "fwdButton";
            this.fwdButton.Size = new System.Drawing.Size(86, 27);
            this.fwdButton.TabIndex = 0;
            this.fwdButton.Text = "&Next >";
            this.fwdButton.UseVisualStyleBackColor = true;
            this.fwdButton.Click += new System.EventHandler(this.FwdButton_Click);
            // 
            // ProjectStarter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(494, 375);
            this.Controls.Add(this.formTableLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProjectStarter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project Use Case";
            this.formTableLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.mainPanel.ResumeLayout(false);
            this.standardLayout.ResumeLayout(false);
            this.standardLayout.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel formTableLayout;
        private System.Windows.Forms.Button fwdButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.TableLayoutPanel standardLayout;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.RadioButton openRecentProjectRB;
        private System.Windows.Forms.ListBox recentProjectsList;
        private System.Windows.Forms.RadioButton createProjectRB;
        private System.Windows.Forms.ListBox useCaseList;
        private System.Windows.Forms.Label recentProjectsLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DoubleClickableRadioButton openProjectRB;
    }
}