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

namespace Pavel.GUI {
    partial class SpaceManager {
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
            this.spaceListBox = new System.Windows.Forms.ListBox();
            this.columnListBox = new System.Windows.Forms.ListBox();
            this.pointSetListBox = new System.Windows.Forms.ListBox();
            this.spaceComboBox = new System.Windows.Forms.ComboBox();
            this.compatiblePointSetsLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.moveUpButton = new System.Windows.Forms.ToolStripButton();
            this.moveDownButton = new System.Windows.Forms.ToolStripButton();
            this.spacePanel = new System.Windows.Forms.Panel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.resetButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.compatiblePointSetsPanel = new System.Windows.Forms.Panel();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.showHideCompatiblePointsetsButton = new System.Windows.Forms.ToolStripButton();
            this.closeButton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.newSpace = new System.Windows.Forms.ToolStripButton();
            this.renameSpaceButton = new System.Windows.Forms.ToolStripButton();
            this.deleteSpaceButton = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.addColumnButton = new System.Windows.Forms.ToolStripButton();
            this.removeColumnButton = new System.Windows.Forms.ToolStripButton();
            this.newColumnButton = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.spacePanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.compatiblePointSetsPanel.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // spaceListBox
            // 
            this.spaceListBox.AllowDrop = true;
            this.spaceListBox.FormattingEnabled = true;
            this.spaceListBox.Location = new System.Drawing.Point(0, 21);
            this.spaceListBox.Name = "spaceListBox";
            this.spaceListBox.Size = new System.Drawing.Size(245, 303);
            this.spaceListBox.TabIndex = 0;
            // 
            // columnListBox
            // 
            this.columnListBox.AllowDrop = true;
            this.columnListBox.FormattingEnabled = true;
            this.columnListBox.Location = new System.Drawing.Point(280, 80);
            this.columnListBox.Name = "columnListBox";
            this.columnListBox.Size = new System.Drawing.Size(224, 420);
            this.columnListBox.TabIndex = 1;
            // 
            // pointSetListBox
            // 
            this.pointSetListBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.pointSetListBox.FormattingEnabled = true;
            this.pointSetListBox.Location = new System.Drawing.Point(9, 417);
            this.pointSetListBox.Name = "pointSetListBox";
            this.pointSetListBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.pointSetListBox.Size = new System.Drawing.Size(246, 82);
            this.pointSetListBox.TabIndex = 2;
            // 
            // spaceComboBox
            // 
            this.spaceComboBox.FormattingEnabled = true;
            this.spaceComboBox.Location = new System.Drawing.Point(122, 17);
            this.spaceComboBox.Name = "spaceComboBox";
            this.spaceComboBox.Size = new System.Drawing.Size(153, 21);
            this.spaceComboBox.TabIndex = 3;
            this.spaceComboBox.SelectedValueChanged += new System.EventHandler(this.SpaceComboBox_SelectedValueChanged);
            // 
            // compatiblePointSetsLabel
            // 
            this.compatiblePointSetsLabel.AutoSize = true;
            this.compatiblePointSetsLabel.Location = new System.Drawing.Point(7, 400);
            this.compatiblePointSetsLabel.Name = "compatiblePointSetsLabel";
            this.compatiblePointSetsLabel.Size = new System.Drawing.Size(110, 13);
            this.compatiblePointSetsLabel.TabIndex = 4;
            this.compatiblePointSetsLabel.Text = "Compatible Point Sets";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Current Space :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(282, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Available Columns:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.AllowMerge = false;
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveUpButton,
            this.moveDownButton});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(245, 23);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // moveUpButton
            // 
            this.moveUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveUpButton.Image = global::Pavel.Properties.Resources.MoveUp;
            this.moveUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(23, 20);
            this.moveUpButton.Text = "Move Column Up";
            this.moveUpButton.Click += new System.EventHandler(this.MoveUpButton_Click);
            // 
            // moveDownButton
            // 
            this.moveDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveDownButton.Image = global::Pavel.Properties.Resources.MoveDown;
            this.moveDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Size = new System.Drawing.Size(23, 20);
            this.moveDownButton.Text = "Move Column Down";
            this.moveDownButton.Click += new System.EventHandler(this.MoveDownButton_Click);
            // 
            // spacePanel
            // 
            this.spacePanel.Controls.Add(this.toolStrip1);
            this.spacePanel.Controls.Add(this.spaceListBox);
            this.spacePanel.Location = new System.Drawing.Point(9, 59);
            this.spacePanel.Name = "spacePanel";
            this.spacePanel.Size = new System.Drawing.Size(245, 324);
            this.spacePanel.TabIndex = 8;
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.resetButton);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.okButton);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.compatiblePointSetsPanel);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.closeButton);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel3);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel2);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.newColumnButton);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.pointSetListBox);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.compatiblePointSetsLabel);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.label2);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.label3);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.columnListBox);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.spaceComboBox);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(519, 545);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(519, 545);
            this.toolStripContainer1.TabIndex = 9;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 25);
            this.toolStripContainer1.TopToolStripPanelVisible = false;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(205, 514);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 30;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(318, 514);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 29;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // compatiblePointSetsPanel
            // 
            this.compatiblePointSetsPanel.Controls.Add(this.toolStrip4);
            this.compatiblePointSetsPanel.Location = new System.Drawing.Point(117, 397);
            this.compatiblePointSetsPanel.Name = "compatiblePointSetsPanel";
            this.compatiblePointSetsPanel.Size = new System.Drawing.Size(23, 18);
            this.compatiblePointSetsPanel.TabIndex = 28;
            // 
            // toolStrip4
            // 
            this.toolStrip4.AllowMerge = false;
            this.toolStrip4.CanOverflow = false;
            this.toolStrip4.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip4.ImageScalingSize = new System.Drawing.Size(13, 13);
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showHideCompatiblePointsetsButton});
            this.toolStrip4.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip4.Location = new System.Drawing.Point(0, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip4.Size = new System.Drawing.Size(23, 20);
            this.toolStrip4.TabIndex = 0;
            this.toolStrip4.Text = "Hide the Compatible Point Sets";
            // 
            // showHideCompatiblePointsetsButton
            // 
            this.showHideCompatiblePointsetsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showHideCompatiblePointsetsButton.Image = global::Pavel.Properties.Resources.Down;
            this.showHideCompatiblePointsetsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showHideCompatiblePointsetsButton.Name = "showHideCompatiblePointsetsButton";
            this.showHideCompatiblePointsetsButton.Size = new System.Drawing.Size(23, 17);
            this.showHideCompatiblePointsetsButton.Text = "toolStripButton1";
            this.showHideCompatiblePointsetsButton.Click += new System.EventHandler(this.ShowHideCompatiblePointsetsButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(429, 514);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 27;
            this.closeButton.Text = "Cancel";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.toolStrip3);
            this.panel3.Location = new System.Drawing.Point(281, 17);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(158, 21);
            this.panel3.TabIndex = 26;
            // 
            // toolStrip3
            // 
            this.toolStrip3.AllowMerge = false;
            this.toolStrip3.CanOverflow = false;
            this.toolStrip3.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSpace,
            this.renameSpaceButton,
            this.deleteSpaceButton});
            this.toolStrip3.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip3.Size = new System.Drawing.Size(158, 23);
            this.toolStrip3.TabIndex = 0;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // newSpace
            // 
            this.newSpace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newSpace.Image = global::Pavel.Properties.Resources.NewDocumentHS;
            this.newSpace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newSpace.Name = "newSpace";
            this.newSpace.Size = new System.Drawing.Size(23, 20);
            this.newSpace.Text = "New Space";
            this.newSpace.Click += new System.EventHandler(this.NewSpace_Click);
            // 
            // renameSpaceButton
            // 
            this.renameSpaceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.renameSpaceButton.Image = global::Pavel.Properties.Resources.Rename;
            this.renameSpaceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.renameSpaceButton.Name = "renameSpaceButton";
            this.renameSpaceButton.Size = new System.Drawing.Size(23, 20);
            this.renameSpaceButton.Text = "Rename Space";
            this.renameSpaceButton.Click += new System.EventHandler(this.RenameSpaceButton_Click);
            // 
            // deleteSpaceButton
            // 
            this.deleteSpaceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteSpaceButton.Image = global::Pavel.Properties.Resources.CloseProject;
            this.deleteSpaceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteSpaceButton.Name = "deleteSpaceButton";
            this.deleteSpaceButton.Size = new System.Drawing.Size(23, 20);
            this.deleteSpaceButton.Text = "Delete Space";
            this.deleteSpaceButton.Click += new System.EventHandler(this.DeleteSpaceButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.toolStrip2);
            this.panel2.Location = new System.Drawing.Point(254, 199);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(26, 47);
            this.panel2.TabIndex = 25;
            // 
            // toolStrip2
            // 
            this.toolStrip2.AllowMerge = false;
            this.toolStrip2.CanOverflow = false;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addColumnButton,
            this.removeColumnButton});
            this.toolStrip2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.Size = new System.Drawing.Size(26, 47);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // addColumnButton
            // 
            this.addColumnButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addColumnButton.Image = global::Pavel.Properties.Resources.ArrowToLeft;
            this.addColumnButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addColumnButton.Name = "addColumnButton";
            this.addColumnButton.Size = new System.Drawing.Size(23, 20);
            this.addColumnButton.Text = "addColumnButton";
            this.addColumnButton.ToolTipText = "Add the selected Column to the Space";
            this.addColumnButton.Click += new System.EventHandler(this.AddColumnButton_Click);
            // 
            // removeColumnButton
            // 
            this.removeColumnButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeColumnButton.Image = global::Pavel.Properties.Resources.ArrowToRight;
            this.removeColumnButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeColumnButton.Name = "removeColumnButton";
            this.removeColumnButton.Size = new System.Drawing.Size(23, 20);
            this.removeColumnButton.Text = "removeColumnButton";
            this.removeColumnButton.ToolTipText = "Delete the selected Column from the Space";
            this.removeColumnButton.Click += new System.EventHandler(this.RemoveColumnButton_Click);
            // 
            // newColumnButton
            // 
            this.newColumnButton.Location = new System.Drawing.Point(400, 53);
            this.newColumnButton.Name = "newColumnButton";
            this.newColumnButton.Size = new System.Drawing.Size(104, 23);
            this.newColumnButton.TabIndex = 24;
            this.newColumnButton.Text = "New Column";
            this.newColumnButton.UseVisualStyleBackColor = true;
            this.newColumnButton.Click += new System.EventHandler(this.NewColumnButton_Click);
            // 
            // SpaceManager
            // 
            this.AcceptButton = this.okButton;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(519, 545);
            this.Controls.Add(this.spacePanel);
            this.Controls.Add(this.toolStripContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpaceManager";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Space Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpaceManager_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.spacePanel.ResumeLayout(false);
            this.spacePanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.compatiblePointSetsPanel.ResumeLayout(false);
            this.compatiblePointSetsPanel.PerformLayout();
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox spaceListBox;
        private System.Windows.Forms.ListBox columnListBox;
        private System.Windows.Forms.ListBox pointSetListBox;
        private System.Windows.Forms.ComboBox spaceComboBox;
        private System.Windows.Forms.Label compatiblePointSetsLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripButton moveUpButton;
        private System.Windows.Forms.ToolStripButton moveDownButton;
        private System.Windows.Forms.Panel spacePanel;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Button newColumnButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton addColumnButton;
        private System.Windows.Forms.ToolStripButton removeColumnButton;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton newSpace;
        private System.Windows.Forms.ToolStripButton deleteSpaceButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel compatiblePointSetsPanel;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton showHideCompatiblePointsetsButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ToolStripButton renameSpaceButton;
    }
}