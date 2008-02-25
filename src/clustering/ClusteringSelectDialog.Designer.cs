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

namespace Pavel.Clustering {
    partial class ClusteringSelectDialog {
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
            this.components = new System.ComponentModel.Container();
            this.helpToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.nameLabel = new System.Windows.Forms.Label();
            this.pointSetLabel = new System.Windows.Forms.Label();
            this.algorithmLabel = new System.Windows.Forms.Label();
            this.spaceLabel = new System.Windows.Forms.Label();
            this.actionButton = new System.Windows.Forms.Button();
            this.relevanceLabel = new System.Windows.Forms.Label();
            this.columnSetDropDown = new System.Windows.Forms.ComboBox();
            this.algorithmBox = new System.Windows.Forms.ComboBox();
            this.pointSetDropDown = new System.Windows.Forms.ComboBox();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.progressLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.percentLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.clusteringArgsPanel = new System.Windows.Forms.Panel();
            this.dataGroupBox = new System.Windows.Forms.GroupBox();
            this.relevanceList = new System.Windows.Forms.TreeView();
            this.clusteringParameterGroupBox = new System.Windows.Forms.GroupBox();
            this.progressPanel = new System.Windows.Forms.Panel();
            this.argsTable = new System.Windows.Forms.Panel();
            this.dataGroupBox.SuspendLayout();
            this.clusteringParameterGroupBox.SuspendLayout();
            this.progressPanel.SuspendLayout();
            this.argsTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(17, 9);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(101, 13);
            this.nameLabel.TabIndex = 4;
            this.nameLabel.Text = "Name of ClusterSet:";
            this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.helpToolTip.SetToolTip(this.nameLabel, "Specify a name for the resulting ClusterSet");
            // 
            // pointSetLabel
            // 
            this.pointSetLabel.AutoSize = true;
            this.pointSetLabel.Location = new System.Drawing.Point(32, 16);
            this.pointSetLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.pointSetLabel.Name = "pointSetLabel";
            this.pointSetLabel.Size = new System.Drawing.Size(96, 13);
            this.pointSetLabel.TabIndex = 0;
            this.pointSetLabel.Text = "PointSet to cluster:";
            this.pointSetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.helpToolTip.SetToolTip(this.pointSetLabel, "Select a PointSet that will be clustered");
            // 
            // algorithmLabel
            // 
            this.algorithmLabel.AutoSize = true;
            this.algorithmLabel.Location = new System.Drawing.Point(26, 16);
            this.algorithmLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.algorithmLabel.Name = "algorithmLabel";
            this.algorithmLabel.Size = new System.Drawing.Size(102, 13);
            this.algorithmLabel.TabIndex = 1;
            this.algorithmLabel.Text = "Clustering-Algorithm:";
            this.algorithmLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.helpToolTip.SetToolTip(this.algorithmLabel, "Select an Clustering-Algorithm");
            // 
            // spaceLabel
            // 
            this.spaceLabel.AutoSize = true;
            this.spaceLabel.Location = new System.Drawing.Point(87, 42);
            this.spaceLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.spaceLabel.Name = "spaceLabel";
            this.spaceLabel.Size = new System.Drawing.Size(41, 13);
            this.spaceLabel.TabIndex = 1;
            this.spaceLabel.Text = "Space:";
            this.spaceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.helpToolTip.SetToolTip(this.spaceLabel, "Select a Space for clustering that defines scaling  and offset of relevant column" +
                    "s.");
            // 
            // actionButton
            // 
            this.actionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.actionButton.Location = new System.Drawing.Point(12, 319);
            this.actionButton.Margin = new System.Windows.Forms.Padding(2);
            this.actionButton.MinimumSize = new System.Drawing.Size(90, 0);
            this.actionButton.Name = "actionButton";
            this.actionButton.Size = new System.Drawing.Size(120, 24);
            this.actionButton.TabIndex = 4;
            this.actionButton.Text = "Start Clustering";
            this.helpToolTip.SetToolTip(this.actionButton, "Run Clustering with the selected parameters");
            this.actionButton.UseVisualStyleBackColor = true;
            this.actionButton.Click += new System.EventHandler(this.ActionButton_Click);
            // 
            // relevanceLabel
            // 
            this.relevanceLabel.AutoSize = true;
            this.relevanceLabel.Location = new System.Drawing.Point(32, 71);
            this.relevanceLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.relevanceLabel.Name = "relevanceLabel";
            this.relevanceLabel.Size = new System.Drawing.Size(96, 13);
            this.relevanceLabel.TabIndex = 6;
            this.relevanceLabel.Text = "Relevant Columns:";
            this.relevanceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.helpToolTip.SetToolTip(this.relevanceLabel, "Select Clumns that should be regarded for clustering-distances");
            // 
            // columnSetDropDown
            // 
            this.columnSetDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.columnSetDropDown.FormattingEnabled = true;
            this.columnSetDropDown.Location = new System.Drawing.Point(134, 39);
            this.columnSetDropDown.Margin = new System.Windows.Forms.Padding(2);
            this.columnSetDropDown.Name = "columnSetDropDown";
            this.columnSetDropDown.Size = new System.Drawing.Size(216, 21);
            this.columnSetDropDown.TabIndex = 3;
            this.columnSetDropDown.SelectedValueChanged += new System.EventHandler(this.ColumnSetDropDown_SelectedValueChanged);
            // 
            // algorithmBox
            // 
            this.algorithmBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.algorithmBox.FormattingEnabled = true;
            this.algorithmBox.Location = new System.Drawing.Point(136, 13);
            this.algorithmBox.Margin = new System.Windows.Forms.Padding(2);
            this.algorithmBox.Name = "algorithmBox";
            this.algorithmBox.Size = new System.Drawing.Size(214, 21);
            this.algorithmBox.TabIndex = 2;
            this.algorithmBox.SelectedValueChanged += new System.EventHandler(this.AlgorithmBox_SelectedValueChanged);
            // 
            // pointSetDropDown
            // 
            this.pointSetDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pointSetDropDown.FormattingEnabled = true;
            this.pointSetDropDown.Location = new System.Drawing.Point(134, 13);
            this.pointSetDropDown.Margin = new System.Windows.Forms.Padding(2);
            this.pointSetDropDown.Name = "pointSetDropDown";
            this.pointSetDropDown.Size = new System.Drawing.Size(216, 21);
            this.pointSetDropDown.TabIndex = 2;
            this.pointSetDropDown.SelectedValueChanged += new System.EventHandler(this.PointSetBox_SelectedValueChanged);
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(124, 6);
            this.nameBox.Margin = new System.Windows.Forms.Padding(2);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(237, 20);
            this.nameBox.TabIndex = 5;
            this.nameBox.TextChanged += new System.EventHandler(this.NameBox_TextChanged);
            // 
            // progressLabel
            // 
            this.progressLabel.AutoEllipsis = true;
            this.progressLabel.Location = new System.Drawing.Point(0, 2);
            this.progressLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(362, 14);
            this.progressLabel.TabIndex = 7;
            this.progressLabel.Text = "Status";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(0, 18);
            this.progressBar.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar.Maximum = 1000;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(362, 26);
            this.progressBar.Step = 100;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 6;
            this.progressBar.UseWaitCursor = true;
            // 
            // percentLabel
            // 
            this.percentLabel.Location = new System.Drawing.Point(0, 46);
            this.percentLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.percentLabel.Name = "percentLabel";
            this.percentLabel.Size = new System.Drawing.Size(362, 14);
            this.percentLabel.TabIndex = 8;
            this.percentLabel.Text = "0 %";
            this.percentLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(259, 319);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButton.MinimumSize = new System.Drawing.Size(90, 0);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(120, 24);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Close";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // clusteringArgsPanel
            // 
            this.clusteringArgsPanel.AutoSize = true;
            this.clusteringArgsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.clusteringArgsPanel.Location = new System.Drawing.Point(11, 34);
            this.clusteringArgsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.clusteringArgsPanel.Name = "clusteringArgsPanel";
            this.clusteringArgsPanel.Size = new System.Drawing.Size(0, 0);
            this.clusteringArgsPanel.TabIndex = 11;
            // 
            // dataGroupBox
            // 
            this.dataGroupBox.Controls.Add(this.relevanceList);
            this.dataGroupBox.Controls.Add(this.columnSetDropDown);
            this.dataGroupBox.Controls.Add(this.pointSetDropDown);
            this.dataGroupBox.Controls.Add(this.spaceLabel);
            this.dataGroupBox.Controls.Add(this.pointSetLabel);
            this.dataGroupBox.Controls.Add(this.relevanceLabel);
            this.dataGroupBox.Location = new System.Drawing.Point(9, 31);
            this.dataGroupBox.Name = "dataGroupBox";
            this.dataGroupBox.Size = new System.Drawing.Size(364, 168);
            this.dataGroupBox.TabIndex = 11;
            this.dataGroupBox.TabStop = false;
            this.dataGroupBox.Text = "Data";
            // 
            // relevanceList
            // 
            this.relevanceList.CheckBoxes = true;
            this.relevanceList.Location = new System.Drawing.Point(136, 71);
            this.relevanceList.Name = "relevanceList";
            this.relevanceList.Size = new System.Drawing.Size(214, 91);
            this.relevanceList.TabIndex = 8;
            this.relevanceList.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.relevanceList_AfterCheck);
            // 
            // clusteringParameterGroupBox
            // 
            this.clusteringParameterGroupBox.Controls.Add(this.algorithmLabel);
            this.clusteringParameterGroupBox.Controls.Add(this.clusteringArgsPanel);
            this.clusteringParameterGroupBox.Controls.Add(this.algorithmBox);
            this.clusteringParameterGroupBox.Location = new System.Drawing.Point(9, 205);
            this.clusteringParameterGroupBox.Name = "clusteringParameterGroupBox";
            this.clusteringParameterGroupBox.Size = new System.Drawing.Size(361, 46);
            this.clusteringParameterGroupBox.TabIndex = 12;
            this.clusteringParameterGroupBox.TabStop = false;
            this.clusteringParameterGroupBox.Text = "Clustering Parameter";
            // 
            // progressPanel
            // 
            this.progressPanel.Controls.Add(this.progressLabel);
            this.progressPanel.Controls.Add(this.progressBar);
            this.progressPanel.Controls.Add(this.percentLabel);
            this.progressPanel.Location = new System.Drawing.Point(12, 258);
            this.progressPanel.Name = "progressPanel";
            this.progressPanel.Size = new System.Drawing.Size(362, 60);
            this.progressPanel.TabIndex = 13;
            // 
            // argsTable
            // 
            this.argsTable.Controls.Add(this.nameLabel);
            this.argsTable.Controls.Add(this.nameBox);
            this.argsTable.Controls.Add(this.clusteringParameterGroupBox);
            this.argsTable.Controls.Add(this.dataGroupBox);
            this.argsTable.Location = new System.Drawing.Point(1, 1);
            this.argsTable.Name = "argsTable";
            this.argsTable.Size = new System.Drawing.Size(373, 257);
            this.argsTable.TabIndex = 14;
            // 
            // ClusteringSelectDialog
            // 
            this.AcceptButton = this.actionButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(387, 355);
            this.Controls.Add(this.argsTable);
            this.Controls.Add(this.progressPanel);
            this.Controls.Add(this.actionButton);
            this.Controls.Add(this.cancelButton);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClusteringSelectDialog";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose algorithm and arguments for clustering";
            this.dataGroupBox.ResumeLayout(false);
            this.dataGroupBox.PerformLayout();
            this.clusteringParameterGroupBox.ResumeLayout(false);
            this.clusteringParameterGroupBox.PerformLayout();
            this.progressPanel.ResumeLayout(false);
            this.argsTable.ResumeLayout(false);
            this.argsTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip helpToolTip;
        private System.Windows.Forms.ComboBox columnSetDropDown;
        private System.Windows.Forms.Label spaceLabel;
        private System.Windows.Forms.ComboBox algorithmBox;
        private System.Windows.Forms.Label algorithmLabel;
        private System.Windows.Forms.ComboBox pointSetDropDown;
        private System.Windows.Forms.Label pointSetLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button actionButton;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label percentLabel;
        private System.Windows.Forms.Label relevanceLabel;
        private System.Windows.Forms.Panel clusteringArgsPanel;
        private System.Windows.Forms.GroupBox dataGroupBox;
        private System.Windows.Forms.GroupBox clusteringParameterGroupBox;
        private System.Windows.Forms.Panel progressPanel;
        private System.Windows.Forms.Panel argsTable;
        private System.Windows.Forms.TreeView relevanceList;

    }
}