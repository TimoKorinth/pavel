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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Pavel.GUI;
using System.Drawing;
using System.Globalization;

namespace Pavel.Framework {
    /// <summary>
    /// PavelMain is the overall controller giving access to all relevant data.
    /// </summary>
    [CoverageExclude]
    public static class PavelMain {

        #region Fields

        private static Pavel.GUI.MainWindow mainWindow;
        private static Pavel.Framework.LogBook logBook;
        private static PluginManager pluginManager;

        #endregion

        #region Properties

        /// <value>Gets Pavels MainWindow</value>
        public static Pavel.GUI.MainWindow MainWindow { get { return mainWindow; } }

        /// <value>Gets Pavels LogBook</value>
        public static Pavel.Framework.LogBook LogBook { get { return PavelMain.logBook; } }

        /// <value>Gets Pavels PluginManager</value>
        public static PluginManager PluginManager { get { return pluginManager; } }

        #endregion

        /// <summary>
        /// The Main thread running the application.
        /// </summary>
        /// <param name="args">A path to a .pav file to be opened directly can be given as an argument.</param>
        [STAThread]
        public static void Main(string[] args) {
        #if !DEBUG
            //Shows SplashScreen
            SplashScreen screen = new SplashScreen(TimeSpan.FromSeconds(2), new Bitmap(Properties.Resources.Pavel));
            screen.TopMost = true;
        #endif

            pluginManager = new PluginManager(Application.StartupPath + @"\Plugins");
            // Creates Instance for the MainWindow
            logBook = new Pavel.Framework.LogBook();
            Application.EnableVisualStyles();
            Application.CurrentCulture = new CultureInfo("en-US");

            mainWindow = new Pavel.GUI.MainWindow();
            if (args.Length == 0) {
                mainWindow.SubscribeToShown();
            } else {
                try { ProjectController.OpenSavedProject(args[0]); } catch { mainWindow.SubscribeToShown(); }
            }
            Application.Run(mainWindow);
        }
    }

    #region Global objects

    /// <summary>
    /// A global delegate
    /// </summary>
    public delegate void VoidDelegate();

    #endregion
}
