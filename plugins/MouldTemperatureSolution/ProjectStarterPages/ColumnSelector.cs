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

using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;

namespace Pavel.Plugins.ProjectStarterPages {

    /// <summary>
    /// This ProjectStarterPage allows the user to choose the Columns for visualization.
    /// </summary>
    public partial class ColumnSelector : Pavel.GUI.ProjectStarterPage {

        #region Fields

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox avaColListBox;
        private System.Windows.Forms.ListBox selColListBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button remButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        #endregion

        #region Constructors and Initializing

        /// <summary>
        /// Calls the InitializeComponent method.
        /// </summary>
        public ColumnSelector() {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes all the elements of this ProjectStarterPage
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.avaColListBox = new System.Windows.Forms.ListBox();
            this.selColListBox = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.remButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.avaColListBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.selColListBox, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.addButton, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.remButton, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15.45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.55F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 37.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 37.5F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(315, 302);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(309, 46);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please choose the columns for the drilling visualization:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Available Columns";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(192, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Selected Columns";
            // 
            // avaColListBox
            // 
            this.avaColListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.avaColListBox.FormattingEnabled = true;
            this.avaColListBox.HorizontalScrollbar = true;
            this.avaColListBox.Location = new System.Drawing.Point(3, 77);
            this.avaColListBox.Name = "avaColListBox";
            this.tableLayoutPanel1.SetRowSpan(this.avaColListBox, 2);
            this.avaColListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.avaColListBox.Size = new System.Drawing.Size(120, 212);
            this.avaColListBox.TabIndex = 3;
            // 
            // selColListBox
            // 
            this.selColListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selColListBox.FormattingEnabled = true;
            this.selColListBox.HorizontalScrollbar = true;
            this.selColListBox.Location = new System.Drawing.Point(192, 77);
            this.selColListBox.Name = "selColListBox";
            this.tableLayoutPanel1.SetRowSpan(this.selColListBox, 2);
            this.selColListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.selColListBox.Size = new System.Drawing.Size(120, 212);
            this.selColListBox.TabIndex = 4;
            // 
            // addButton
            // 
            this.addButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.addButton.Location = new System.Drawing.Point(129, 161);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(57, 23);
            this.addButton.TabIndex = 5;
            this.addButton.Text = ">>";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // remButton
            // 
            this.remButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.remButton.Location = new System.Drawing.Point(129, 190);
            this.remButton.Name = "remButton";
            this.remButton.Size = new System.Drawing.Size(57, 23);
            this.remButton.TabIndex = 6;
            this.remButton.Text = "<<";
            this.remButton.UseVisualStyleBackColor = true;
            this.remButton.Click += new System.EventHandler(this.RemButton_Click);
            // 
            // ColumnSelector
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ColumnSelector";
            this.Size = new System.Drawing.Size(315, 302);
            this.ParentChanged += new System.EventHandler(this.ColumnSelector_ParentChanged);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        #region Methods

        #region ProjectStarterPage Members

        /// <summary>
        /// Fills the list of DrillingColumns in the MouldTemperatureUseCase.
        /// </summary>
        override public Boolean Execute() {
            List<int> visColumns = new List<int>();
            foreach (Object o in selColListBox.Items){
                visColumns.Add(((Column) o).Index);
            }
            (Pavel.Framework.ProjectController.Project.UseCase as MouldTemperatureUseCase).DrillingColumns = visColumns;
            return true;
        }

        /// <summary>
        /// Clears the list of DrillingColumns in the MouldTemperatureUseCase.
        /// </summary>
        override public void Undo() {
            (Pavel.Framework.ProjectController.Project.UseCase as MouldTemperatureUseCase).DrillingColumns = null;
        }


        override public void Reset() { }

        /// <summary>
        /// Return true.
        /// </summary>
        /// <returns>True</returns>
        override public Boolean HasCorrectInput() {
            return true;
        }

        #endregion

        #endregion

        #region Event Handling Stuff

        #region Buttons

        /// <summary>
        /// Handles the clicking of the remButton.
        /// </summary>
        /// <param name="sender">remButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void RemButton_Click(object sender, EventArgs e) {
            List<Column> selColumns = new List<Column>();
            foreach (Object o in selColListBox.SelectedItems) {
                selColumns.Add((Column)o);
            }
            foreach (Column c in selColumns) {
                selColListBox.Items.Remove(c);
                avaColListBox.Items.Add(c);
            }
        }

        /// <summary>
        /// Handles the clicking of the addButton.
        /// </summary>
        /// <param name="sender">addButton</param>
        /// <param name="e">Standard EventArgs</param>
        private void AddButton_Click(object sender, EventArgs e) {
            List<Column> selColumns = new List<Column>();
            foreach (Object o in avaColListBox.SelectedItems) {
                selColumns.Add((Column)o);
            }
            foreach (Column c in selColumns) {
                avaColListBox.Items.Remove(c);
                selColListBox.Items.Add(c);
            }
        }

        #endregion 

        /// <summary>
        /// Initializes the selColListBox(with the decision space Columns)
        /// and the avaColListBox (with the remaining Columns), when the parent or the ColumnSelector is changed.
        /// </summary>
        /// <param name="sender">ParentChanged event</param>
        /// <param name="e">Standard EventArgs</param>
        private void ColumnSelector_ParentChanged(object sender, EventArgs e) {
            // Fill available columns list
            avaColListBox.Items.Clear();
            avaColListBox.Items.AddRange(Pavel.Framework.ProjectController.Project.columns.ToArray());

            // Initially select decision space columns
            selColListBox.Items.Clear();
            foreach (Pavel.Framework.Space s in Pavel.Framework.ProjectController.Project.spaces)
                if ( s.Label == "Decision Space" ) {
                    for ( int i = 0; i < s.Dimension; i++ ) {
                        selColListBox.Items.Add(s[i]);
                        avaColListBox.Items.Remove(s[i]);
                    }
                }
        }

        #endregion
    }
}
