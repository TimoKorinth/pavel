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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Pavel.Framework;

namespace Pavel.GUI.SolutionVisualizations {
    public class CompareManyControl : UserControl {
        #region Fields
        private TableLayoutPanel tableLayoutPanel1;
        private ListView compareManylistView;
        private ColumnHeader columnHeader1;
        private Point[] points;
        private int highlightedPoint;
        private Label pointsLabel;
        private Label showAsLabel;
        private ComboBox compareDescriptionComboBox1;
        private ColumnSet commonColumnSet;
        #endregion

        #region Constructor
        public CompareManyControl(Point[] p) {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Dock = DockStyle.Fill;
            this.points = p;
            InitializeComponent();
            if ( points == null || points.Length == 0 )
                throw new ArgumentException("At least one point must be given");
            commonColumnSet = p[0].ColumnSet;
            foreach ( Pavel.Framework.Point point in p ) {
                commonColumnSet = ColumnSet.Union(point.ColumnSet, commonColumnSet);
            }
            FillColumnsBox();
            FillListBox();
            this.compareDescriptionComboBox1.SelectedIndex = 0;
            this.compareManylistView.SelectedIndices.Add(0);
        }

        private void InitializeComponent() {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.compareManylistView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.pointsLabel = new System.Windows.Forms.Label();
            this.showAsLabel = new System.Windows.Forms.Label();
            this.compareDescriptionComboBox1 = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.pointsLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.showAsLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.compareDescriptionComboBox1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.compareManylistView, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(208, 170);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // compareManylistView
            // 
            this.compareManylistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.tableLayoutPanel1.SetColumnSpan(this.compareManylistView, 2);
            this.compareManylistView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compareManylistView.FullRowSelect = true;
            this.compareManylistView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.compareManylistView.HideSelection = false;
            this.compareManylistView.Location = new System.Drawing.Point(3, 50);
            this.compareManylistView.Name = "compareManylistView";
            this.compareManylistView.Size = new System.Drawing.Size(202, 117);
            this.compareManylistView.TabIndex = 1;
            this.compareManylistView.UseCompatibleStateImageBehavior = false;
            this.compareManylistView.View = System.Windows.Forms.View.SmallIcon;
            this.compareManylistView.SelectedIndexChanged += new System.EventHandler(this.compareManylistView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Points";
            this.columnHeader1.Width = 140;
            // 
            // pointsLabel
            // 
            this.pointsLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.pointsLabel, 2);
            this.pointsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pointsLabel.Location = new System.Drawing.Point(3, 0);
            this.pointsLabel.Name = "pointsLabel";
            this.pointsLabel.Size = new System.Drawing.Size(42, 13);
            this.pointsLabel.TabIndex = 2;
            this.pointsLabel.Text = "Points";
            // 
            // showAsLabel
            // 
            this.showAsLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.showAsLabel.AutoSize = true;
            this.showAsLabel.Location = new System.Drawing.Point(3, 27);
            this.showAsLabel.Name = "showAsLabel";
            this.showAsLabel.Size = new System.Drawing.Size(48, 13);
            this.showAsLabel.TabIndex = 3;
            this.showAsLabel.Text = "Show as";
            // 
            // compareDescriptionComboBox1
            // 
            this.compareDescriptionComboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.compareDescriptionComboBox1.FormattingEnabled = true;
            this.compareDescriptionComboBox1.Location = new System.Drawing.Point(58, 24);
            this.compareDescriptionComboBox1.Name = "compareDescriptionComboBox1";
            this.compareDescriptionComboBox1.Size = new System.Drawing.Size(147, 21);
            this.compareDescriptionComboBox1.TabIndex = 0;
            this.compareDescriptionComboBox1.SelectedIndexChanged += new System.EventHandler(this.compareDescriptionComboBox1_SelectedIndexChanged);
            // 
            // CompareManyControl
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CompareManyControl";
            this.Size = new System.Drawing.Size(211, 173);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Properties

        public int HighlightedPoint {
            get { return highlightedPoint; }
            set {
                highlightedPoint = value;
                compareManylistView.Items[highlightedPoint].Selected = true; ;
            }
            
        }

        public List<int> SelectedPoints {
            get {
                List<int> indices = new List<int>();
                foreach (int i in compareManylistView.SelectedIndices) { 
                    indices.Add(i);
                }
                return indices; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void FillListBox() {
            compareManylistView.Items.Clear();
            if (compareDescriptionComboBox1.SelectedIndex > 0) {
                foreach (Point p in points) {
                    compareManylistView.Items.Add(p[(Column)(compareDescriptionComboBox1.SelectedItem)].ToString());
                }
            }
            else {
                foreach (Point p in points) {
                    compareManylistView.Items.Add(p.Tag.ToString());
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void FillColumnsBox() {
            compareDescriptionComboBox1.Items.Clear();
            compareDescriptionComboBox1.Items.Add("Sequential Number");
            foreach (Column c in commonColumnSet) {
                compareDescriptionComboBox1.Items.Add(c);
            }
        }

        #endregion

        #region Events
        
        public event EventHandler<ComparisonEventArgs> ComparisonPointsChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void compareDescriptionComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            FillListBox();
        }

        private void compareManylistView_SelectedIndexChanged(object sender, EventArgs e) {
            int nrSelected = compareManylistView.SelectedIndices.Count;
            if (nrSelected > 0) {
                highlightedPoint = compareManylistView.SelectedIndices[compareManylistView.SelectedIndices.Count - 1];
            }
            else {
                highlightedPoint = 0;
            }
            if (ComparisonPointsChanged != null) {
                ComparisonPointsChanged(sender, new ComparisonEventArgs(-1, -1, -1));
            }
        }
        #endregion

    }

}
