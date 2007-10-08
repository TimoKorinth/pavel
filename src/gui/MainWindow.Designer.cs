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
    partial class MainWindow {
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
                UnsubscribeFromEvents();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent() {
            this.topToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.leftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.rightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.bottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.propertyControl = new Pavel.GUI.PropertyControl();
            this.SuspendLayout();
            // 
            // topToolStripPanel
            // 
            this.topToolStripPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.topToolStripPanel.Name = "topToolStripPanel";
            this.topToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.topToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.topToolStripPanel.Size = new System.Drawing.Size(803, 0);
            // 
            // leftToolStripPanel
            // 
            this.leftToolStripPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.leftToolStripPanel.Name = "leftToolStripPanel";
            this.leftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.leftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.leftToolStripPanel.Size = new System.Drawing.Size(0, 602);
            // 
            // rightToolStripPanel
            // 
            this.rightToolStripPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightToolStripPanel.Location = new System.Drawing.Point(803, 0);
            this.rightToolStripPanel.Name = "rightToolStripPanel";
            this.rightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.rightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.rightToolStripPanel.Size = new System.Drawing.Size(0, 602);
            // 
            // bottomToolStripPanel
            // 
            this.bottomToolStripPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomToolStripPanel.Location = new System.Drawing.Point(0, 602);
            this.bottomToolStripPanel.Name = "bottomToolStripPanel";
            this.bottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.bottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.bottomToolStripPanel.Size = new System.Drawing.Size(803, 0);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(205, 0);
            this.splitter1.Margin = new System.Windows.Forms.Padding(2);
            this.splitter1.MinExtra = 10;
            this.splitter1.MinSize = 10;
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(5, 602);
            this.splitter1.TabIndex = 11;
            this.splitter1.TabStop = false;
            // 
            // propertyControl
            // 
            this.propertyControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.propertyControl.Location = new System.Drawing.Point(0, 0);
            this.propertyControl.Margin = new System.Windows.Forms.Padding(2);
            this.propertyControl.Name = "propertyControl";
            this.propertyControl.Padding = new System.Windows.Forms.Padding(4);
            this.propertyControl.Size = new System.Drawing.Size(205, 602);
            this.propertyControl.TabIndex = 12;
            this.propertyControl.VisualizationWindow = null;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 602);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.propertyControl);
            this.Controls.Add(this.bottomToolStripPanel);
            this.Controls.Add(this.rightToolStripPanel);
            this.Controls.Add(this.leftToolStripPanel);
            this.Controls.Add(this.topToolStripPanel);
            this.IsMdiContainer = true;
            this.Name = "MainWindow";
            this.Text = "PAVEL";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripPanel topToolStripPanel;
        private System.Windows.Forms.ToolStripPanel leftToolStripPanel;
        private System.Windows.Forms.ToolStripPanel rightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel bottomToolStripPanel;
        private System.Windows.Forms.Splitter splitter1;
        private PropertyControl propertyControl;
    }
}