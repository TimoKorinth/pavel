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
    partial class HistogramOpener : Pavel.GUI.ProjectStarterPage {
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
            this.label4 = new System.Windows.Forms.Label();
            this.testFileBox = new System.Windows.Forms.TextBox();
            this.testFileButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.simFileBox = new System.Windows.Forms.TextBox();
            this.simFileButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.decimalSeperatorComboBox = new System.Windows.Forms.ComboBox();
            this.seperatorLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.78852F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.21148F));
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.testFileBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.testFileButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.simFileBox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.simFileButton, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(331, 285);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Histogram File";
            // 
            // testFileBox
            // 
            this.testFileBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.testFileBox.Location = new System.Drawing.Point(3, 17);
            this.testFileBox.Name = "testFileBox";
            this.testFileBox.ReadOnly = true;
            this.testFileBox.Size = new System.Drawing.Size(225, 20);
            this.testFileBox.TabIndex = 10;
            // 
            // testFileButton
            // 
            this.testFileButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.testFileButton.Location = new System.Drawing.Point(234, 16);
            this.testFileButton.Name = "testFileButton";
            this.testFileButton.Size = new System.Drawing.Size(75, 23);
            this.testFileButton.TabIndex = 3;
            this.testFileButton.Text = "Browse";
            this.testFileButton.UseVisualStyleBackColor = true;
            this.testFileButton.Click += new System.EventHandler(this.testFileButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Simulation Histogram";
            // 
            // simFileBox
            // 
            this.simFileBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.simFileBox.Location = new System.Drawing.Point(3, 59);
            this.simFileBox.Name = "simFileBox";
            this.simFileBox.ReadOnly = true;
            this.simFileBox.Size = new System.Drawing.Size(225, 20);
            this.simFileBox.TabIndex = 13;
            // 
            // simFileButton
            // 
            this.simFileButton.Location = new System.Drawing.Point(234, 58);
            this.simFileButton.Name = "simFileButton";
            this.simFileButton.Size = new System.Drawing.Size(75, 23);
            this.simFileButton.TabIndex = 14;
            this.simFileButton.Text = "Browse";
            this.simFileButton.UseVisualStyleBackColor = true;
            this.simFileButton.Click += new System.EventHandler(this.simFileButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.decimalSeperatorComboBox);
            this.panel1.Controls.Add(this.seperatorLabel);
            this.panel1.Location = new System.Drawing.Point(3, 87);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(225, 29);
            this.panel1.TabIndex = 12;
            // 
            // decimalSeperatorComboBox
            // 
            this.decimalSeperatorComboBox.FormattingEnabled = true;
            this.decimalSeperatorComboBox.Items.AddRange(new object[] {
            ".",
            ","});
            this.decimalSeperatorComboBox.Location = new System.Drawing.Point(103, 3);
            this.decimalSeperatorComboBox.Name = "decimalSeperatorComboBox";
            this.decimalSeperatorComboBox.Size = new System.Drawing.Size(64, 21);
            this.decimalSeperatorComboBox.TabIndex = 1;
            // 
            // seperatorLabel
            // 
            this.seperatorLabel.AutoSize = true;
            this.seperatorLabel.Location = new System.Drawing.Point(3, 6);
            this.seperatorLabel.Name = "seperatorLabel";
            this.seperatorLabel.Size = new System.Drawing.Size(94, 13);
            this.seperatorLabel.TabIndex = 0;
            this.seperatorLabel.Text = "Decimal Seperator";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Location = new System.Drawing.Point(3, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 117);
            this.label1.TabIndex = 11;
            this.label1.Text = "Format: CSV file\r\nField seperator: \";\" (semicolon) or TAB\r\n\r\nSpalte 1: Frequenz\r\n" +
                "Spalte 2: Höhe 1, Laser 1\r\nSpalte 3: Höhe 1, Laser 2\r\nSpalte 4: Höhe 2, Laser 1\r" +
                "\nSpalte 5: Höhe 2, Laser 2\r\n...";
            // 
            // HistogramOpener
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HistogramOpener";
            this.Size = new System.Drawing.Size(331, 285);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button testFileButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox testFileBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox decimalSeperatorComboBox;
        private System.Windows.Forms.Label seperatorLabel;
        private System.Windows.Forms.TextBox simFileBox;
        private System.Windows.Forms.Button simFileButton;
        private System.Windows.Forms.Label label2;
    }
}
