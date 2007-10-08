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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Pavel.GUI {

    /// <summary>
    /// An enhanced ToolTip that displays text from a given ListBox left of the given position.
    /// </summary>
    public class Legend : ToolTip {

        #region Fields

        private ListBox toolTipText = new ListBox();
        private IWin32Window owner;

        #endregion

        #region Properties

        public ListBox ToolTipText {
            get { return toolTipText; }
            set { toolTipText = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the ToolTip with the given <paramref name="owner"/>.
        /// </summary>
        /// <param name="owner">Owner of the ToolTip</param>
        public Legend(IWin32Window owner) : base() {
            this.owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Use this method instead of Show(). Shows the ToolTip at the given location.
        /// </summary>
        /// <param name="location">Location of the top-right corner of the ToolTipForm</param>
        /// <param name="duration">Time for which the Legend should be displayed</param>
        public void ShowMe(Point location, int duration) {
            this.Hide(owner);
            this.Show(GetText(), this.owner, this.TopLeft(location), duration);
        }

        /// <summary>
        /// Transforms the text stored in a ListBox (used to determine the width) into a string;
        /// </summary>
        /// <returns>A string displaying the Items of the toolTipText in separate rows.</returns>
        private string GetText() {
            string text = "";
            for (int i = 0; i < toolTipText.Items.Count - 1; i++) {
                text += toolTipText.Items[i] + "\n";
            }
            text += toolTipText.Items[toolTipText.Items.Count - 1];

            return text;
        }

        /// <summary>
        /// Determines the top-left corner of the ToolTip.
        /// </summary>
        /// <param name="topRight">The top-right corner of the ToolTipForm</param>
        /// <returns>The top-left corner of the ToolTipForm</returns>
        private Point TopLeft(Point topRight) {
            Point topLeft = new Point();
            topLeft.Y = topRight.Y;
            topLeft.X = topRight.X - this.GetWidth();
            return topLeft;
        }

        /// <summary>
        /// Determines the width of the ToolTip to.
        /// </summary>
        /// <returns>Width of the ToolTip</returns>
        private int GetWidth() {
            float width = 0;
            Graphics graphic = this.toolTipText.CreateGraphics();
            Font font = this.toolTipText.Font;

            for (int i = 0; i < this.toolTipText.Items.Count; i++) {
                String item = (string)this.toolTipText.Items[i];
                // This gets the actual graphical width of the string.
                float itemWidth = graphic.MeasureString(item, font).Width;
                if (itemWidth > width) {
                    width = itemWidth;
                }
            }

            return (int)width;
        }

        #endregion
    }
}
