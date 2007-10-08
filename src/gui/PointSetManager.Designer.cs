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
    partial class PointSetManager {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && ( components != null )) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PointSetManager));
            this.closeButton = new System.Windows.Forms.Button();
            this.pointSetNameBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.changeToolStrip = new System.Windows.Forms.ToolStrip();
            this.createButton = new System.Windows.Forms.ToolStripButton();
            this.createComplementaryButton = new System.Windows.Forms.ToolStripButton();
            this.pointSetFromClusterButton = new System.Windows.Forms.ToolStripButton();
            this.renameButton = new System.Windows.Forms.ToolStripButton();
            this.eraseButton = new System.Windows.Forms.ToolStripButton();
            this.pointSetList = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.changeToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(135, 254);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.ExitButtonClick);
            // 
            // pointSetNameBox
            // 
            this.pointSetNameBox.Location = new System.Drawing.Point(13, 38);
            this.pointSetNameBox.Name = "pointSetNameBox";
            this.pointSetNameBox.Size = new System.Drawing.Size(198, 20);
            this.pointSetNameBox.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.changeToolStrip);
            this.panel1.Location = new System.Drawing.Point(12, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(198, 21);
            this.panel1.TabIndex = 1;
            // 
            // changeToolStrip
            // 
            this.changeToolStrip.AllowMerge = false;
            this.changeToolStrip.CanOverflow = false;
            this.changeToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.changeToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createButton,
            this.createComplementaryButton,
            this.pointSetFromClusterButton,
            this.renameButton,
            this.eraseButton});
            this.changeToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.changeToolStrip.Location = new System.Drawing.Point(0, 0);
            this.changeToolStrip.Name = "changeToolStrip";
            this.changeToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.changeToolStrip.Size = new System.Drawing.Size(198, 23);
            this.changeToolStrip.TabIndex = 4;
            this.changeToolStrip.Text = "toolStrip1";
            // 
            // createButton
            // 
            this.createButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.createButton.Image = global::Pavel.Properties.Resources.NewFromSelectedHS;
            this.createButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(23, 20);
            this.createButton.Text = "createButton";
            this.createButton.ToolTipText = "Create a new PointSet containing the currently selected points.";
            this.createButton.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // createComplementaryButton
            // 
            this.createComplementaryButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.createComplementaryButton.Image = global::Pavel.Properties.Resources.NewFromUnselectedHS;
            this.createComplementaryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.createComplementaryButton.Name = "createComplementaryButton";
            this.createComplementaryButton.Size = new System.Drawing.Size(23, 20);
            this.createComplementaryButton.Text = "createComplementaryButton";
            this.createComplementaryButton.ToolTipText = "Create a new PointSet containing all points, but the currently selected.";
            this.createComplementaryButton.Click += new System.EventHandler(this.CreateComplementaryButton_Click);
            // 
            // pointSetFromClusterButton
            // 
            this.pointSetFromClusterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pointSetFromClusterButton.Image = ((System.Drawing.Image)(resources.GetObject("pointSetFromClusterButton.Image")));
            this.pointSetFromClusterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pointSetFromClusterButton.Name = "pointSetFromClusterButton";
            this.pointSetFromClusterButton.Size = new System.Drawing.Size(23, 20);
            this.pointSetFromClusterButton.Text = "pointSetFromClusterButton";
            this.pointSetFromClusterButton.ToolTipText = "Create a Point Set from a ClusterSet";
            this.pointSetFromClusterButton.Click += new System.EventHandler(this.PointSetFromClusterButton_Click);
            // 
            // renameButton
            // 
            this.renameButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.renameButton.Image = global::Pavel.Properties.Resources.Rename;
            this.renameButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.renameButton.Name = "renameButton";
            this.renameButton.Size = new System.Drawing.Size(23, 20);
            this.renameButton.Text = "toolStripButton3";
            this.renameButton.ToolTipText = "Rename the selected PointSet.";
            this.renameButton.Click += new System.EventHandler(this.RenameButton_Click);
            // 
            // eraseButton
            // 
            this.eraseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.eraseButton.Image = global::Pavel.Properties.Resources.Delete;
            this.eraseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.eraseButton.Name = "eraseButton";
            this.eraseButton.Size = new System.Drawing.Size(23, 20);
            this.eraseButton.Text = "toolStripButton2";
            this.eraseButton.ToolTipText = "Erase the selected PointSet.";
            this.eraseButton.Click += new System.EventHandler(this.EraseButton_Click);
            // 
            // pointSetList
            // 
            this.pointSetList.FormattingEnabled = true;
            this.pointSetList.Location = new System.Drawing.Point(12, 70);
            this.pointSetList.Name = "pointSetList";
            this.pointSetList.Size = new System.Drawing.Size(198, 173);
            this.pointSetList.TabIndex = 5;
            this.pointSetList.SelectedIndexChanged += new System.EventHandler(this.SelectedPointSetChanged);
            // 
            // PointSetManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(223, 289);
            this.Controls.Add(this.pointSetList);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pointSetNameBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PointSetManager";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "PointSet Manager";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.changeToolStrip.ResumeLayout(false);
            this.changeToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pointSetNameBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip changeToolStrip;
        private System.Windows.Forms.ToolStripButton createButton;
        private System.Windows.Forms.ToolStripButton createComplementaryButton;
        private System.Windows.Forms.ToolStripButton eraseButton;
        private System.Windows.Forms.ToolStripButton renameButton;
        private System.Windows.Forms.ToolStripButton pointSetFromClusterButton;
        private System.Windows.Forms.ListBox pointSetList;
    }
}