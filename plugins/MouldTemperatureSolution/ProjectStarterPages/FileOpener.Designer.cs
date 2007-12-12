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

namespace Pavel.Plugins.ProjectStarterPages {
    partial class FileOpener : Pavel.GUI.ProjectStarterPageFileOpener {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.browseDec = new System.Windows.Forms.Button();
            this.decisionSpaceTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.objectiveSpaceTextBox = new System.Windows.Forms.TextBox();
            this.browseObj = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.browseSTL = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.simTextBox = new System.Windows.Forms.TextBox();
            this.browseSIM = new System.Windows.Forms.Button();
            this.stlFileComboBox = new System.Windows.Forms.ComboBox();
            this.resetButton = new System.Windows.Forms.Button();
            this.clearObjButton = new System.Windows.Forms.Button();
            this.clearDecButton = new System.Windows.Forms.Button();
            this.clearSimButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.addFilesLB = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.addFileBtn = new System.Windows.Forms.Button();
            this.delBtn = new System.Windows.Forms.Button();
            this.resetAddFilesBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 68F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.browseDec, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.decisionSpaceTextBox, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.objectiveSpaceTextBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.browseObj, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.browseSTL, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.simTextBox, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.browseSIM, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.stlFileComboBox, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.resetButton, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.clearObjButton, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.clearDecButton, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.clearSimButton, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.addFilesLB, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 2, 10);
            this.tableLayoutPanel1.Controls.Add(this.resetAddFilesBtn, 1, 10);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 11;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.81043F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.515866F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.03174F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.515866F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.03174F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.515866F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.03174F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.515866F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.03174F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.516574F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.48259F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(367, 296);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "STL File";
            // 
            // browseDec
            // 
            this.browseDec.Dock = System.Windows.Forms.DockStyle.Top;
            this.browseDec.Location = new System.Drawing.Point(280, 107);
            this.browseDec.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.browseDec.Name = "browseDec";
            this.browseDec.Size = new System.Drawing.Size(84, 20);
            this.browseDec.TabIndex = 5;
            this.browseDec.Text = "Browse";
            this.toolTip.SetToolTip(this.browseDec, "Browse file");
            this.browseDec.UseVisualStyleBackColor = true;
            // 
            // decisionSpaceTextBox
            // 
            this.decisionSpaceTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.decisionSpaceTextBox.Location = new System.Drawing.Point(3, 107);
            this.decisionSpaceTextBox.Name = "decisionSpaceTextBox";
            this.decisionSpaceTextBox.ReadOnly = true;
            this.decisionSpaceTextBox.Size = new System.Drawing.Size(243, 20);
            this.decisionSpaceTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Decision ColumnSet File";
            // 
            // objectiveSpaceTextBox
            // 
            this.objectiveSpaceTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectiveSpaceTextBox.Location = new System.Drawing.Point(3, 59);
            this.objectiveSpaceTextBox.Name = "objectiveSpaceTextBox";
            this.objectiveSpaceTextBox.ReadOnly = true;
            this.objectiveSpaceTextBox.Size = new System.Drawing.Size(243, 20);
            this.objectiveSpaceTextBox.TabIndex = 2;
            this.objectiveSpaceTextBox.Tag = "";
            this.objectiveSpaceTextBox.WordWrap = false;
            // 
            // browseObj
            // 
            this.browseObj.Dock = System.Windows.Forms.DockStyle.Top;
            this.browseObj.Location = new System.Drawing.Point(280, 59);
            this.browseObj.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.browseObj.Name = "browseObj";
            this.browseObj.Size = new System.Drawing.Size(84, 20);
            this.browseObj.TabIndex = 3;
            this.browseObj.Text = "Browse";
            this.toolTip.SetToolTip(this.browseObj, "Browse file");
            this.browseObj.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(193, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Please choose the required files:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Objective ColumnSet File";
            // 
            // browseSTL
            // 
            this.browseSTL.Dock = System.Windows.Forms.DockStyle.Top;
            this.browseSTL.Location = new System.Drawing.Point(280, 203);
            this.browseSTL.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.browseSTL.Name = "browseSTL";
            this.browseSTL.Size = new System.Drawing.Size(84, 21);
            this.browseSTL.TabIndex = 8;
            this.browseSTL.Text = "Browse";
            this.toolTip.SetToolTip(this.browseSTL, "Browse file");
            this.browseSTL.UseVisualStyleBackColor = true;
            this.browseSTL.Click += new System.EventHandler(this.BrowseSTL_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Simulation Values File";
            // 
            // simTextBox
            // 
            this.simTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simTextBox.Location = new System.Drawing.Point(3, 155);
            this.simTextBox.Name = "simTextBox";
            this.simTextBox.ReadOnly = true;
            this.simTextBox.Size = new System.Drawing.Size(243, 20);
            this.simTextBox.TabIndex = 11;
            // 
            // browseSIM
            // 
            this.browseSIM.Dock = System.Windows.Forms.DockStyle.Top;
            this.browseSIM.Location = new System.Drawing.Point(280, 155);
            this.browseSIM.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.browseSIM.Name = "browseSIM";
            this.browseSIM.Size = new System.Drawing.Size(84, 20);
            this.browseSIM.TabIndex = 12;
            this.browseSIM.Text = "Browse";
            this.toolTip.SetToolTip(this.browseSIM, "Browse file");
            this.browseSIM.UseVisualStyleBackColor = true;
            // 
            // stlFileComboBox
            // 
            this.stlFileComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.stlFileComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.stlFileComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stlFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stlFileComboBox.FormattingEnabled = true;
            this.stlFileComboBox.Location = new System.Drawing.Point(3, 203);
            this.stlFileComboBox.Name = "stlFileComboBox";
            this.stlFileComboBox.Size = new System.Drawing.Size(243, 21);
            this.stlFileComboBox.TabIndex = 13;
            // 
            // resetButton
            // 
            this.resetButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.resetButton.Location = new System.Drawing.Point(280, 16);
            this.resetButton.Margin = new System.Windows.Forms.Padding(6, 0, 3, 3);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(84, 21);
            this.resetButton.TabIndex = 14;
            this.resetButton.Text = "Reset";
            this.toolTip.SetToolTip(this.resetButton, "Clear all fields");
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // clearObjButton
            // 
            this.clearObjButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.clearObjButton.Location = new System.Drawing.Point(249, 59);
            this.clearObjButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.clearObjButton.Name = "clearObjButton";
            this.clearObjButton.Size = new System.Drawing.Size(22, 20);
            this.clearObjButton.TabIndex = 15;
            this.clearObjButton.Text = "X";
            this.toolTip.SetToolTip(this.clearObjButton, "Clear field");
            this.clearObjButton.UseVisualStyleBackColor = true;
            // 
            // clearDecButton
            // 
            this.clearDecButton.Location = new System.Drawing.Point(249, 107);
            this.clearDecButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.clearDecButton.Name = "clearDecButton";
            this.clearDecButton.Size = new System.Drawing.Size(22, 20);
            this.clearDecButton.TabIndex = 16;
            this.clearDecButton.Text = "X";
            this.toolTip.SetToolTip(this.clearDecButton, "Clear field");
            this.clearDecButton.UseVisualStyleBackColor = true;
            // 
            // clearSimButton
            // 
            this.clearSimButton.Location = new System.Drawing.Point(249, 155);
            this.clearSimButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.clearSimButton.Name = "clearSimButton";
            this.clearSimButton.Size = new System.Drawing.Size(22, 20);
            this.clearSimButton.TabIndex = 17;
            this.clearSimButton.Text = "X";
            this.toolTip.SetToolTip(this.clearSimButton, "Clear field");
            this.clearSimButton.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 232);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Additional Files";
            // 
            // addFilesLB
            // 
            this.addFilesLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addFilesLB.FormattingEnabled = true;
            this.addFilesLB.Location = new System.Drawing.Point(3, 251);
            this.addFilesLB.Name = "addFilesLB";
            this.addFilesLB.Size = new System.Drawing.Size(243, 30);
            this.addFilesLB.TabIndex = 19;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.addFileBtn);
            this.flowLayoutPanel1.Controls.Add(this.delBtn);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(277, 251);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(87, 42);
            this.flowLayoutPanel1.TabIndex = 20;
            // 
            // addFileBtn
            // 
            this.addFileBtn.Location = new System.Drawing.Point(3, 3);
            this.addFileBtn.Name = "addFileBtn";
            this.addFileBtn.Size = new System.Drawing.Size(29, 25);
            this.addFileBtn.TabIndex = 0;
            this.addFileBtn.Text = "+";
            this.toolTip.SetToolTip(this.addFileBtn, "Add a File");
            this.addFileBtn.UseVisualStyleBackColor = true;
            this.addFileBtn.Click += new System.EventHandler(this.AddFileBtn_Click);
            // 
            // delBtn
            // 
            this.delBtn.Location = new System.Drawing.Point(38, 3);
            this.delBtn.Name = "delBtn";
            this.delBtn.Size = new System.Drawing.Size(29, 25);
            this.delBtn.TabIndex = 1;
            this.delBtn.Text = "-";
            this.toolTip.SetToolTip(this.delBtn, "Remove selected file");
            this.delBtn.UseVisualStyleBackColor = true;
            this.delBtn.Click += new System.EventHandler(this.DelBtn_Click);
            // 
            // resetAddFilesBtn
            // 
            this.resetAddFilesBtn.Location = new System.Drawing.Point(249, 251);
            this.resetAddFilesBtn.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.resetAddFilesBtn.Name = "resetAddFilesBtn";
            this.resetAddFilesBtn.Size = new System.Drawing.Size(22, 20);
            this.resetAddFilesBtn.TabIndex = 21;
            this.resetAddFilesBtn.Text = "X";
            this.toolTip.SetToolTip(this.resetAddFilesBtn, "Clear field");
            this.resetAddFilesBtn.UseVisualStyleBackColor = true;
            this.resetAddFilesBtn.Click += new System.EventHandler(this.ResetAddFilesBtn_Click);
            // 
            // FileOpener
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FileOpener";
            this.Size = new System.Drawing.Size(367, 296);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox objectiveSpaceTextBox;
        private System.Windows.Forms.Button browseObj;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button browseDec;
        private System.Windows.Forms.TextBox decisionSpaceTextBox;
        private System.Windows.Forms.Button browseSTL;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox simTextBox;
        private System.Windows.Forms.Button browseSIM;
        private System.Windows.Forms.ComboBox stlFileComboBox;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Button clearObjButton;
        private System.Windows.Forms.Button clearDecButton;
        private System.Windows.Forms.Button clearSimButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox addFilesLB;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button addFileBtn;
        private System.Windows.Forms.Button resetAddFilesBtn;
        private System.Windows.Forms.Button delBtn;
    }
}
