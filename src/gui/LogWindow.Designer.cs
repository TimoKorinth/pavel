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
    partial class LogWindow {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogWindow));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.errorButton = new System.Windows.Forms.ToolStripButton();
            this.warningButton = new System.Windows.Forms.ToolStripButton();
            this.messageButton = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.logGrid = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.errorButton,
            this.warningButton,
            this.messageButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(584, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // errorButton
            // 
            this.errorButton.Checked = true;
            this.errorButton.CheckOnClick = true;
            this.errorButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.errorButton.Image = ((System.Drawing.Image)(resources.GetObject("errorButton.Image")));
            this.errorButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.errorButton.Name = "errorButton";
            this.errorButton.Size = new System.Drawing.Size(56, 22);
            this.errorButton.Text = "Errors";
            this.errorButton.CheckedChanged += new System.EventHandler(this.LogChanged);
            // 
            // warningButton
            // 
            this.warningButton.Checked = true;
            this.warningButton.CheckOnClick = true;
            this.warningButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.warningButton.Image = ((System.Drawing.Image)(resources.GetObject("warningButton.Image")));
            this.warningButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.warningButton.Name = "warningButton";
            this.warningButton.Size = new System.Drawing.Size(72, 22);
            this.warningButton.Text = "Warnings";
            this.warningButton.CheckedChanged += new System.EventHandler(this.LogChanged);
            // 
            // messageButton
            // 
            this.messageButton.Checked = true;
            this.messageButton.CheckOnClick = true;
            this.messageButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.messageButton.Image = ((System.Drawing.Image)(resources.GetObject("messageButton.Image")));
            this.messageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.messageButton.Name = "messageButton";
            this.messageButton.Size = new System.Drawing.Size(74, 22);
            this.messageButton.Text = "Messages";
            this.messageButton.CheckedChanged += new System.EventHandler(this.LogChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.logGrid, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(584, 269);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // logGrid
            // 
            this.logGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.logGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.logGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.logGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logGrid.Location = new System.Drawing.Point(3, 28);
            this.logGrid.Name = "logGrid";
            this.logGrid.ReadOnly = true;
            this.logGrid.Size = new System.Drawing.Size(578, 238);
            this.logGrid.TabIndex = 4;
            // 
            // LogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 269);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Logbook";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Logbook";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton errorButton;
        private System.Windows.Forms.ToolStripButton warningButton;
        private System.Windows.Forms.ToolStripButton messageButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView logGrid;
    }
}