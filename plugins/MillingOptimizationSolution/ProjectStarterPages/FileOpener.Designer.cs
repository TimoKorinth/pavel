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
            this.label3 = new System.Windows.Forms.Label();
            this.browseSTL = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.browseObj = new System.Windows.Forms.Button();
            this.objectiveSpaceTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.decisionSpaceTextBox = new System.Windows.Forms.TextBox();
            this.browseDec = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.stlFileComboBox = new System.Windows.Forms.ComboBox();
            this.positionFileTextBox = new System.Windows.Forms.TextBox();
            this.browsePos = new System.Windows.Forms.Button();
            this.PositionFile = new System.Windows.Forms.Label();
            this.browseToolFile = new System.Windows.Forms.Button();
            this.ToolFile = new System.Windows.Forms.Label();
            this.toolFileTextBox = new System.Windows.Forms.TextBox();
            this.resetObjBtn = new System.Windows.Forms.Button();
            this.resetDecBtn = new System.Windows.Forms.Button();
            this.resetPosBtn = new System.Windows.Forms.Button();
            this.resetToolBtn = new System.Windows.Forms.Button();
            this.resetBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "STL File";
            // 
            // browseSTL
            // 
            this.browseSTL.Dock = System.Windows.Forms.DockStyle.Top;
            this.browseSTL.Location = new System.Drawing.Point(257, 156);
            this.browseSTL.Margin = new System.Windows.Forms.Padding(6, 0, 3, 3);
            this.browseSTL.Name = "browseSTL";
            this.browseSTL.Size = new System.Drawing.Size(71, 23);
            this.browseSTL.TabIndex = 8;
            this.browseSTL.Text = "Browse";
            this.browseSTL.UseVisualStyleBackColor = true;
            this.browseSTL.Click += new System.EventHandler(this.BrowseSTL_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Objective ColumnSet File";
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
            // browseObj
            // 
            this.browseObj.Dock = System.Windows.Forms.DockStyle.Top;
            this.browseObj.Location = new System.Drawing.Point(257, 54);
            this.browseObj.Margin = new System.Windows.Forms.Padding(6, 0, 3, 3);
            this.browseObj.Name = "browseObj";
            this.browseObj.Size = new System.Drawing.Size(71, 23);
            this.browseObj.TabIndex = 3;
            this.browseObj.Text = "Browse";
            this.browseObj.UseVisualStyleBackColor = true;
            // 
            // objectiveSpaceTextBox
            // 
            this.objectiveSpaceTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectiveSpaceTextBox.Location = new System.Drawing.Point(3, 57);
            this.objectiveSpaceTextBox.Name = "objectiveSpaceTextBox";
            this.objectiveSpaceTextBox.ReadOnly = true;
            this.objectiveSpaceTextBox.Size = new System.Drawing.Size(225, 20);
            this.objectiveSpaceTextBox.TabIndex = 2;
            this.objectiveSpaceTextBox.WordWrap = false;
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
            // decisionSpaceTextBox
            // 
            this.decisionSpaceTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.decisionSpaceTextBox.Location = new System.Drawing.Point(3, 108);
            this.decisionSpaceTextBox.Name = "decisionSpaceTextBox";
            this.decisionSpaceTextBox.ReadOnly = true;
            this.decisionSpaceTextBox.Size = new System.Drawing.Size(225, 20);
            this.decisionSpaceTextBox.TabIndex = 4;
            // 
            // browseDec
            // 
            this.browseDec.Dock = System.Windows.Forms.DockStyle.Top;
            this.browseDec.Location = new System.Drawing.Point(257, 105);
            this.browseDec.Margin = new System.Windows.Forms.Padding(6, 0, 3, 3);
            this.browseDec.Name = "browseDec";
            this.browseDec.Size = new System.Drawing.Size(71, 23);
            this.browseDec.TabIndex = 5;
            this.browseDec.Text = "Browse";
            this.browseDec.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 231F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.browseDec, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.decisionSpaceTextBox, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.objectiveSpaceTextBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.browseObj, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.browseSTL, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.stlFileComboBox, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.positionFileTextBox, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.browsePos, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this.PositionFile, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.browseToolFile, 2, 10);
            this.tableLayoutPanel1.Controls.Add(this.ToolFile, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.toolFileTextBox, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.resetObjBtn, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.resetDecBtn, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.resetPosBtn, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.resetToolBtn, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.resetBtn, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 11;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.22513F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.26496F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.52991F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.26496F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.52991F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.26496F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.52991F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.390258F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 17F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(331, 285);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // stlFileComboBox
            // 
            this.stlFileComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.stlFileComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.stlFileComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stlFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stlFileComboBox.FormattingEnabled = true;
            this.stlFileComboBox.Location = new System.Drawing.Point(3, 159);
            this.stlFileComboBox.Name = "stlFileComboBox";
            this.stlFileComboBox.Size = new System.Drawing.Size(225, 21);
            this.stlFileComboBox.TabIndex = 10;
            // 
            // positionFileTextBox
            // 
            this.positionFileTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positionFileTextBox.Location = new System.Drawing.Point(3, 208);
            this.positionFileTextBox.Name = "positionFileTextBox";
            this.positionFileTextBox.ReadOnly = true;
            this.positionFileTextBox.Size = new System.Drawing.Size(225, 20);
            this.positionFileTextBox.TabIndex = 11;
            // 
            // browsePos
            // 
            this.browsePos.Dock = System.Windows.Forms.DockStyle.Top;
            this.browsePos.Location = new System.Drawing.Point(257, 205);
            this.browsePos.Margin = new System.Windows.Forms.Padding(6, 0, 3, 3);
            this.browsePos.Name = "browsePos";
            this.browsePos.Size = new System.Drawing.Size(71, 23);
            this.browsePos.TabIndex = 12;
            this.browsePos.Text = "Browse";
            this.browsePos.UseVisualStyleBackColor = true;
            // 
            // PositionFile
            // 
            this.PositionFile.AutoSize = true;
            this.PositionFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PositionFile.Location = new System.Drawing.Point(3, 190);
            this.PositionFile.Name = "PositionFile";
            this.PositionFile.Size = new System.Drawing.Size(76, 13);
            this.PositionFile.TabIndex = 13;
            this.PositionFile.Text = "Position File";
            // 
            // browseToolFile
            // 
            this.browseToolFile.Location = new System.Drawing.Point(257, 252);
            this.browseToolFile.Margin = new System.Windows.Forms.Padding(6, 0, 3, 3);
            this.browseToolFile.Name = "browseToolFile";
            this.browseToolFile.Size = new System.Drawing.Size(71, 23);
            this.browseToolFile.TabIndex = 14;
            this.browseToolFile.Text = "Browse";
            this.browseToolFile.UseVisualStyleBackColor = true;
            // 
            // ToolFile
            // 
            this.ToolFile.AutoSize = true;
            this.ToolFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToolFile.Location = new System.Drawing.Point(3, 235);
            this.ToolFile.Name = "ToolFile";
            this.ToolFile.Size = new System.Drawing.Size(56, 13);
            this.ToolFile.TabIndex = 15;
            this.ToolFile.Text = "Tool File";
            // 
            // toolFileTextBox
            // 
            this.toolFileTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolFileTextBox.Location = new System.Drawing.Point(3, 255);
            this.toolFileTextBox.Name = "toolFileTextBox";
            this.toolFileTextBox.ReadOnly = true;
            this.toolFileTextBox.Size = new System.Drawing.Size(225, 20);
            this.toolFileTextBox.TabIndex = 16;
            // 
            // resetObjBtn
            // 
            this.resetObjBtn.Location = new System.Drawing.Point(234, 57);
            this.resetObjBtn.Name = "resetObjBtn";
            this.resetObjBtn.Size = new System.Drawing.Size(14, 23);
            this.resetObjBtn.TabIndex = 17;
            this.resetObjBtn.Tag = "";
            this.resetObjBtn.Text = "X";
            this.resetObjBtn.UseVisualStyleBackColor = true;
            // 
            // resetDecBtn
            // 
            this.resetDecBtn.Location = new System.Drawing.Point(234, 108);
            this.resetDecBtn.Name = "resetDecBtn";
            this.resetDecBtn.Size = new System.Drawing.Size(14, 23);
            this.resetDecBtn.TabIndex = 18;
            this.resetDecBtn.Tag = "";
            this.resetDecBtn.Text = "X";
            this.resetDecBtn.UseVisualStyleBackColor = true;
            // 
            // resetPosBtn
            // 
            this.resetPosBtn.Location = new System.Drawing.Point(234, 208);
            this.resetPosBtn.Name = "resetPosBtn";
            this.resetPosBtn.Size = new System.Drawing.Size(14, 23);
            this.resetPosBtn.TabIndex = 20;
            this.resetPosBtn.Tag = "";
            this.resetPosBtn.Text = "X";
            this.resetPosBtn.UseVisualStyleBackColor = true;
            // 
            // resetToolBtn
            // 
            this.resetToolBtn.Location = new System.Drawing.Point(234, 255);
            this.resetToolBtn.Name = "resetToolBtn";
            this.resetToolBtn.Size = new System.Drawing.Size(14, 23);
            this.resetToolBtn.TabIndex = 21;
            this.resetToolBtn.Tag = "";
            this.resetToolBtn.Text = "X";
            this.resetToolBtn.UseVisualStyleBackColor = true;
            // 
            // resetBtn
            // 
            this.resetBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.resetBtn.Location = new System.Drawing.Point(254, 11);
            this.resetBtn.Name = "resetBtn";
            this.resetBtn.Size = new System.Drawing.Size(74, 23);
            this.resetBtn.TabIndex = 22;
            this.resetBtn.Tag = "";
            this.resetBtn.Text = "Reset";
            this.resetBtn.UseVisualStyleBackColor = true;
            this.resetBtn.Click += new System.EventHandler(this.ResetBtn_Click);
            // 
            // FileOpener
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FileOpener";
            this.Size = new System.Drawing.Size(331, 285);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button browseSTL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button browseObj;
        private System.Windows.Forms.TextBox objectiveSpaceTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox decisionSpaceTextBox;
        private System.Windows.Forms.Button browseDec;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox stlFileComboBox;
        private System.Windows.Forms.TextBox positionFileTextBox;
        private System.Windows.Forms.Button browsePos;
        private System.Windows.Forms.Label PositionFile;
        private System.Windows.Forms.Button browseToolFile;
        private System.Windows.Forms.Label ToolFile;
        private System.Windows.Forms.TextBox toolFileTextBox;
        private System.Windows.Forms.Button resetObjBtn;
        private System.Windows.Forms.Button resetDecBtn;
        private System.Windows.Forms.Button resetPosBtn;
        private System.Windows.Forms.Button resetToolBtn;
        private System.Windows.Forms.Button resetBtn;

    }
}
