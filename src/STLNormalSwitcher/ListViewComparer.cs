// Part of STLNormalSwitcher: A program to switch normal vectors in STL-files
//
// Copyright (C) 2007  PG500, ISF, University of Dortmund
//      PG500 are: Christoph Begau, Christoph Heuel, Raffael Joliet, Jan Kolanski,
//                 Mandy Kröller, Christian Moritz, Daniel Niggemann, Mathias Stöber,
//                 Timo Stönner, Jan Varwig, Dafan Zhai
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
// The licence can also be found at: http://www.gnu.org/licenses/old-licenses/gpl-2.0.txt
//
// For more information and contact details look at STLNormalSwitchers website:
//      http://normalswitcher.sourceforge.net/
//
// Check out PAVEl (http://pavel.sourceforge.net/) another great program brought to you by PG500.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace STLNormalSwitcher {
    /// <summary>
    /// Allows sorting of the columns of a ListView.
    /// </summary>
    public class ListViewComparer : IComparer {

        #region Fields

        private int col;
        private SortOrder order;

        #endregion

        #region Constructors

        /// <summary>
        /// Sets the column to <paramref name="col"/> and the order to <paramref name="order"/>.
        /// </summary>
        /// <param name="col">Column to be sorted</param>
        /// <param name="order">Order to sort it in</param>
        public ListViewComparer(int col, SortOrder order) {
            this.col = col;
            this.order = order;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compares two objects.
        /// </summary>
        /// <param name="x">Object 1</param>
        /// <param name="y">Object 2</param>
        /// <returns>Value indicating the correct order of the two objects</returns>
        public int Compare(object x, object y) {
            ListViewItem item1, item2;
            item1 = (ListViewItem)x;
            item2 = (ListViewItem)y;

            if (this.order == SortOrder.Ascending) {
                return item1.SubItems[col].Text.CompareTo(item2.SubItems[col].Text);
            } else {
                return item2.SubItems[col].Text.CompareTo(item1.SubItems[col].Text);
            }
        }

        #endregion
    }
}
