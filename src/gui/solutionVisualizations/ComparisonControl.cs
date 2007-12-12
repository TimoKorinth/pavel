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
using System.Windows.Forms;
using Pavel.Framework;

namespace Pavel.GUI.SolutionVisualizations {
    public class ComparisonControl : UserControl {

        #region Fields

        private TableLayoutPanel MainTableLayout;
        private TrackBar FadingBar;
        private Point[] points;
        private int referencePoint;
        private Button SetRefPoint;
        private ComboBox descriptionComboBox;
        private ListView listView;
        private ColumnHeader columnHeader1;
        private Label refLabel;
        private Label curLabel;
        private int comparisonPoint;
        private Label showAssLabel;
        private Label pointsLabel;
        private ColumnSet commonColumnSet;

        #endregion

        #region Properties

        public int ReferencePoint {
            get { return referencePoint; }
            set { referencePoint = value; }
        }

        public int ComparisonPoint {
            get { return comparisonPoint; }
            set { comparisonPoint = value; }
        }

        #endregion

        #region Constructor

        public ComparisonControl(Point[] p) {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Dock = DockStyle.Fill;
            this.points = p;
            referencePoint = -1;
            InitializeComponent();
            if (points == null || points.Length == 0)
                throw new ArgumentException("At least one point must be given");
            commonColumnSet = p[0].ColumnSet;
            foreach (Pavel.Framework.Point point in p) {
                commonColumnSet = ColumnSet.Union(point.ColumnSet, commonColumnSet);
            }
            FillColumnsBox();
            FillListBox();
            this.descriptionComboBox.SelectedIndex = 0;
            this.listView.SelectedIndices.Add(0);
        }

        private void InitializeComponent() {
            this.MainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.descriptionComboBox = new System.Windows.Forms.ComboBox();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.refLabel = new System.Windows.Forms.Label();
            this.FadingBar = new System.Windows.Forms.TrackBar();
            this.curLabel = new System.Windows.Forms.Label();
            this.SetRefPoint = new System.Windows.Forms.Button();
            this.showAssLabel = new System.Windows.Forms.Label();
            this.pointsLabel = new System.Windows.Forms.Label();
            this.MainTableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FadingBar)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTableLayout
            // 
            this.MainTableLayout.AutoSize = true;
            this.MainTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainTableLayout.ColumnCount = 3;
            this.MainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.MainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.MainTableLayout.Controls.Add(this.descriptionComboBox, 1, 1);
            this.MainTableLayout.Controls.Add(this.listView, 0, 2);
            this.MainTableLayout.Controls.Add(this.refLabel, 2, 2);
            this.MainTableLayout.Controls.Add(this.FadingBar, 2, 3);
            this.MainTableLayout.Controls.Add(this.curLabel, 2, 4);
            this.MainTableLayout.Controls.Add(this.SetRefPoint, 0, 5);
            this.MainTableLayout.Controls.Add(this.showAssLabel, 0, 1);
            this.MainTableLayout.Controls.Add(this.pointsLabel, 0, 0);
            this.MainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTableLayout.Location = new System.Drawing.Point(0, 0);
            this.MainTableLayout.Name = "MainTableLayout";
            this.MainTableLayout.RowCount = 6;
            this.MainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.MainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.MainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTableLayout.Size = new System.Drawing.Size(216, 192);
            this.MainTableLayout.TabIndex = 0;
            // 
            // descriptionComboBox
            // 
            this.descriptionComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.descriptionComboBox.FormattingEnabled = true;
            this.descriptionComboBox.Location = new System.Drawing.Point(58, 19);
            this.descriptionComboBox.Name = "descriptionComboBox";
            this.descriptionComboBox.Size = new System.Drawing.Size(115, 21);
            this.descriptionComboBox.TabIndex = 3;
            this.descriptionComboBox.SelectedIndexChanged += new System.EventHandler(this.DescriptionComboBox_SelectedIndexChanged);
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.MainTableLayout.SetColumnSpan(this.listView, 2);
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.Location = new System.Drawing.Point(3, 44);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.MainTableLayout.SetRowSpan(this.listView, 3);
            this.listView.Size = new System.Drawing.Size(170, 121);
            this.listView.TabIndex = 4;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.SmallIcon;
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            this.listView.Click += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Point";
            this.columnHeader1.Width = 144;
            // 
            // refLabel
            // 
            this.refLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.refLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refLabel.Location = new System.Drawing.Point(179, 41);
            this.refLabel.Name = "refLabel";
            this.refLabel.Size = new System.Drawing.Size(34, 20);
            this.refLabel.TabIndex = 5;
            this.refLabel.Text = "Ref";
            this.refLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FadingBar
            // 
            this.FadingBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FadingBar.Location = new System.Drawing.Point(179, 64);
            this.FadingBar.Maximum = 100;
            this.FadingBar.Name = "FadingBar";
            this.FadingBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.FadingBar.Size = new System.Drawing.Size(34, 81);
            this.FadingBar.TabIndex = 1;
            this.FadingBar.TickFrequency = 5;
            this.FadingBar.Value = 50;
            this.FadingBar.ValueChanged += new System.EventHandler(this.FadingBar_ValueChanged);
            // 
            // curLabel
            // 
            this.curLabel.AutoSize = true;
            this.curLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.curLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.curLabel.Location = new System.Drawing.Point(179, 148);
            this.curLabel.Name = "curLabel";
            this.curLabel.Size = new System.Drawing.Size(34, 20);
            this.curLabel.TabIndex = 6;
            this.curLabel.Text = "Cur";
            this.curLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SetRefPoint
            // 
            this.MainTableLayout.SetColumnSpan(this.SetRefPoint, 2);
            this.SetRefPoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SetRefPoint.Location = new System.Drawing.Point(3, 171);
            this.SetRefPoint.Name = "SetRefPoint";
            this.SetRefPoint.Size = new System.Drawing.Size(170, 19);
            this.SetRefPoint.TabIndex = 2;
            this.SetRefPoint.Text = "Set as reference";
            this.SetRefPoint.UseVisualStyleBackColor = true;
            this.SetRefPoint.Click += new System.EventHandler(this.SetRefPoint_Click);
            // 
            // showAssLabel
            // 
            this.showAssLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.showAssLabel.AutoSize = true;
            this.showAssLabel.Location = new System.Drawing.Point(3, 22);
            this.showAssLabel.Name = "showAssLabel";
            this.showAssLabel.Size = new System.Drawing.Size(48, 13);
            this.showAssLabel.TabIndex = 7;
            this.showAssLabel.Text = "Show as";
            // 
            // pointsLabel
            // 
            this.pointsLabel.AutoSize = true;
            this.MainTableLayout.SetColumnSpan(this.pointsLabel, 2);
            this.pointsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pointsLabel.Location = new System.Drawing.Point(3, 0);
            this.pointsLabel.Name = "pointsLabel";
            this.pointsLabel.Size = new System.Drawing.Size(42, 13);
            this.pointsLabel.TabIndex = 8;
            this.pointsLabel.Text = "Points";
            // 
            // ComparisonControl
            // 
            this.Controls.Add(this.MainTableLayout);
            this.Name = "ComparisonControl";
            this.Size = new System.Drawing.Size(216, 192);
            this.MainTableLayout.ResumeLayout(false);
            this.MainTableLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FadingBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Methods

        private void FillColumnsBox() {
            descriptionComboBox.Items.Clear();
            descriptionComboBox.Items.Add("Sequential Number");
            foreach (Column c in commonColumnSet) {
                descriptionComboBox.Items.Add(c);
            }
        }

        private void FillListBox() {
            listView.Items.Clear();
            if (descriptionComboBox.SelectedIndex > 0) {
                foreach (Point p in points) {
                    listView.Items.Add(p[(Column)(descriptionComboBox.SelectedItem)].ToString());
                }
            } else {
                foreach (Point p in points) {
                    listView.Items.Add(p.Tag.ToString());
                }
            }
            if (referencePoint >= 0 && referencePoint < listView.Items.Count) {
                listView.Items[referencePoint].BackColor = System.Drawing.Color.Blue;
            }
            listView.Items[comparisonPoint].BackColor = System.Drawing.Color.Green;
        }
        #endregion

        #region Event Handling Stuff
        private void FadingBar_ValueChanged(object sender, EventArgs e) {
            if (this.listView.SelectedIndices.Count > 0) {
                SendChangedEvent(sender);
            }
        }

        public event EventHandler<ComparisonEventArgs> ComparisonPointsChanged;

        private void listView_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.listView.SelectedIndices.Count > 0) {
                listView.Items[comparisonPoint].BackColor = System.Drawing.Color.White;
                this.comparisonPoint = (int)listView.SelectedIndices[0];
                listView.Items[comparisonPoint].BackColor = Pavel.GUI.Visualizations.ColorManagement.UnselectedColor.Color;
                SendChangedEvent(sender);
                if (referencePoint >= 0 && referencePoint < listView.Items.Count) {
                    listView.Items[referencePoint].BackColor = Pavel.GUI.Visualizations.ColorManagement.CurrentSelectionColor.Color;
                }
            }
        }

        private void SetRefPoint_Click(object sender, EventArgs e) {
            if (listView.SelectedIndices.Count > 0) {
                if (referencePoint >= 0 && referencePoint < listView.Items.Count) {
                    listView.Items[referencePoint].BackColor = System.Drawing.Color.White;
                }
                listView.SelectedItems[0].BackColor = System.Drawing.Color.Blue;
                this.referencePoint = (int)listView.SelectedIndices[0];
                SendChangedEvent(sender);
            }
        }

        private void SendChangedEvent(object sender) {
            if (ComparisonPointsChanged != null) {
                ComparisonPointsChanged(sender, new ComparisonEventArgs(this.referencePoint, this.comparisonPoint, 1.0f - ((float)(FadingBar.Value) / FadingBar.Maximum)));
            }
        }

        private void DescriptionComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            FillListBox();
        }

        private void listView_DoubleClick(object sender, EventArgs e) {
            SetRefPoint_Click(sender, e);
        }
        #endregion
    }

    #region Class ComparisonEventArgs
    /// <summary>
    /// ComparisonEventArgs class
    /// </summary>
    public class ComparisonEventArgs : EventArgs {

        private int referencePoint;
        public int ReferencePoint {
            get { return referencePoint; }
        }

        private int selectedPoint;
        public int SelectedPoint {
            get { return selectedPoint; }
        }

        private float alphaValue;
        public float AlphaValue {
            get { return alphaValue; }
        }

        public ComparisonEventArgs(int ReferencePoint, int SelectedPoint, float alphaValue) {
            this.referencePoint = ReferencePoint;
            this.selectedPoint = SelectedPoint;
            this.alphaValue = (alphaValue);
        }
    }
    #endregion
}
