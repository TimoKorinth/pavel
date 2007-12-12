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
using Pavel.GUI.Visualizations;

namespace Pavel.GUI.SolutionVisualizations {
    public class ZapControl : UserControl {

        #region Fields
        private Solution solution;
        private Button next;
        private Button back;
        private Label selectedLabel;
        private Point[] points;
        #endregion

        #region Constructor

        public ZapControl(Point[] p, Solution solution) {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Dock = DockStyle.Fill;
            this.points = p;
            this.solution = solution;
            InitializeButtons();
            InitializeTableLayout();
        }

        public void InitializeTableLayout() {
            TableLayoutPanel buttonLayout = new TableLayoutPanel();
            buttonLayout.AutoSize = true;
            buttonLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonLayout.Dock = DockStyle.Fill;
            this.Controls.Add(buttonLayout);

            buttonLayout.ColumnCount = 2;
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            buttonLayout.RowCount = 2;
            buttonLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            buttonLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            buttonLayout.Controls.Add(this.selectedLabel, 0, 0);
            buttonLayout.SetColumnSpan(this.selectedLabel, 2);

            buttonLayout.Controls.Add(this.back, 0, 1);
            buttonLayout.Controls.Add(this.next, 1, 1);
        }

        /// <summary>
        /// Initialize the buttons.
        /// </summary>
        private void InitializeButtons() {
            selectedLabel = new Label();
            selectedLabel.Text = selectedLabel.Text = "Solution: 1/" + points.Length.ToString();
            selectedLabel.Dock = DockStyle.Fill;
            next = new Button();
            next.Text = "Next >";
            next.Anchor = AnchorStyles.Right;
            next.Click += OnChangePointClickEvent;

            back = new Button();
            back.Text = "< Back";
            back.Anchor = AnchorStyles.Left;
            back.Click += OnChangePointClickEvent;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Changes the current point
        /// </summary>
        /// <param name="forwardDirection">If true, the next will be displayed, otherwise the last one</param>
        private void ChangePoint(Boolean forwardDirection) {
            int index = solution.ChangePoint(forwardDirection);
            selectedLabel.Text = "Solution: " + (index + 1) + "/" + points.Length.ToString();
        }

        #endregion

        #region Eventhandling

        /// <summary>
        /// The Eventhandling from the navigation buttons
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Eventargs</param>
        private void OnChangePointClickEvent(object sender, EventArgs e) {
            if (sender.Equals(next)) {
                ChangePoint(true);
            } else {
                ChangePoint(false);
            }
        }
        #endregion

    }
}
