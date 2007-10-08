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
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Globalization;

namespace STLNormalSwitcher {
    /// <summary>
    /// The main class of the STLNormalSwitcher.
    /// </summary>
    public static class STLNormalSwitcher {
        /// <summary>
        /// Main method sets the CurrentCulture and starts the Application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.CurrentCulture = new CultureInfo("en-US");
            if (args.Length == 0) {
                Application.Run(new NormalSwitcherForm());
            } else {
                if (args[1] == "Open") {
                    Application.Run(new NormalSwitcherForm(args[0]));
                }
            }
        }
    }
}