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
using System.Diagnostics;
using System.IO;
using Pavel.Framework;
using Pavel.GUI.Visualizations;

namespace Pavel.GUI {
    /// <summary>
    /// The menu of PAVELs MainWindow.
    /// </summary>
    public class MainMenuStrip : MenuStrip {

        #region Fields

        private ToolStripMenuItem fileMenuItem;
        private ToolStripMenuItem startProjectItem;
        private ToolStripMenuItem saveProject;
        private ToolStripMenuItem saveProjectAs;
        private ToolStripMenuItem exportData;
        private ToolStripMenuItem closeProject;
        private ToolStripMenuItem log;
        private ToolStripMenuItem exit;

        private ToolStripMenuItem visualisationMenuItem;

        private ToolStripMenuItem solutionMenuItem;

        private ToolStripMenuItem viewMenuItem;
        private ToolStripMenuItem propertiesItem;
        private ToolStripMenuItem colorProfileItem;

        private ToolStripMenuItem windowsMenuItem;
        private ToolStripMenuItem tileHorizontalWindows;
        private ToolStripMenuItem tileVerticalWindows;
        private ToolStripMenuItem cascadeWindows;

        private ToolStripMenuItem helpMenuItem;
        private ToolStripMenuItem about;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the menu and subscribes to events.
        /// </summary>
        public MainMenuStrip()
            : base() {
            InitFileMenuItem();
            InitVisualisationsMenuItem();
            InitSolutionsMenuItem();
            InitViewMenuItem();
            InitWindowsMenuItem();
            InitHelpMenuItem();

            Items.Add(fileMenuItem);
            Items.Add(visualisationMenuItem);
            Items.Add(solutionMenuItem);
            Items.Add(viewMenuItem);
            Items.Add(windowsMenuItem);
            Items.Add(helpMenuItem);

            SubscribeToEvents();
        }

        /// <summary>
        /// Dispose Method
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            UnSubscribeEvents();
            base.Dispose(disposing);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the file menu.
        /// </summary>
        private void InitFileMenuItem() {
            fileMenuItem = new ToolStripMenuItem("&File");
            startProjectItem = new ToolStripMenuItem("S&tart/Open Project...", Pavel.Properties.Resources.NewDocumentHS,
                new EventHandler(delegate(object sender, EventArgs e) { PavelMain.MainWindow.StartProject(); }),
                Keys.Control | Keys.N);
            saveProject = new ToolStripMenuItem("&Save Project", Pavel.Properties.Resources.saveButton,
                new EventHandler(delegate(object sender, EventArgs e) { PavelMain.MainWindow.SaveProject(); }),
                Keys.Control | Keys.S);
            saveProject.Enabled = false;
            saveProjectAs = new ToolStripMenuItem("Save Project As", Pavel.Properties.Resources.saveAsButton,
                new EventHandler(delegate(object sender, EventArgs e) { PavelMain.MainWindow.SaveProjectAs(); }),
                Keys.Control |Keys.Shift | Keys.S);
            saveProjectAs.Enabled = false;
            //TODO Icon einfügen
            exportData = new ToolStripMenuItem("&Export to File ...", null,
                new EventHandler(delegate(object sender, EventArgs e) { Export.ExportData(); }),
                Keys.Control | Keys.E);
            exportData.Enabled = false;
            closeProject = new ToolStripMenuItem("&Close Project", Pavel.Properties.Resources.CloseProject,
                new EventHandler(delegate(object sender, EventArgs e) { PavelMain.MainWindow.CloseProject(); }),
                Keys.Control | Keys.W);
            closeProject.Enabled = false;
            //TODO Icon einfügen
            log = new ToolStripMenuItem("&Log", null,
                new EventHandler(delegate(object sender, EventArgs e) { PavelMain.MainWindow.ShowLog(); }),
                Keys.Control | Keys.L);
            exit = new ToolStripMenuItem("&Exit", null,
                new EventHandler(delegate(object sender, EventArgs e) { PavelMain.MainWindow.Close(); }),
                Keys.Control | Keys.Q);

            fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                startProjectItem,
                saveProject,
                saveProjectAs,
                exportData,
                closeProject,
                new ToolStripSeparator(),
                log,
                new ToolStripSeparator(),
                exit
            });
        }

        /// <summary>
        /// Initializes the view menu.
        /// </summary>
        private void InitViewMenuItem() {
            viewMenuItem = new ToolStripMenuItem("&View");
            propertiesItem = new ToolStripMenuItem("&Properties");
            propertiesItem.CheckOnClick = true;
            propertiesItem.CheckState = CheckState.Checked;
            viewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                propertiesItem
            });

            propertiesItem.Click += delegate(object sender, EventArgs e) { PavelMain.MainWindow.ShowPropertyWindow(propertiesItem.Checked); };

            colorProfileItem = new ToolStripMenuItem("&ColorProfile...");
            colorProfileItem.Click += delegate(object sender, EventArgs e) {
                ColorProfileDialog dlg = new ColorProfileDialog();
                dlg.ShowDialog();
            };
            viewMenuItem.DropDownItems.Add(colorProfileItem);
        }

        /// <summary>
        /// Initializes the windows menu.
        /// </summary>
        private void InitWindowsMenuItem() {
            windowsMenuItem = new ToolStripMenuItem("&Window");
            tileHorizontalWindows = new ToolStripMenuItem("Tile &Horizontal");
            tileHorizontalWindows.Image = Properties.Resources.TileWindowsHorizontallyHS;
            tileVerticalWindows = new ToolStripMenuItem("Tile &Vertical");
            tileVerticalWindows.Image = Properties.Resources.TileWindowsVerticallyHS;
            cascadeWindows = new ToolStripMenuItem("&Cascase Windows");
            cascadeWindows.Image = Properties.Resources.CascadeWindowsHS;
            windowsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                tileHorizontalWindows,
                tileVerticalWindows,
                cascadeWindows,
                new ToolStripSeparator()
            });
            MdiWindowListItem = windowsMenuItem;

            tileHorizontalWindows.Click += delegate(object sender, EventArgs e) { PavelMain.MainWindow.LayoutMdi(MdiLayout.TileHorizontal); };
            tileVerticalWindows.Click += delegate(object sender, EventArgs e) { PavelMain.MainWindow.LayoutMdi(MdiLayout.TileVertical); };
            cascadeWindows.Click += delegate(object sender, EventArgs e) { PavelMain.MainWindow.LayoutMdi(MdiLayout.Cascade); };
        }

        /// <summary>
        /// Initializes the help menu.
        /// </summary>
        private void InitHelpMenuItem() {
            helpMenuItem = new ToolStripMenuItem("&Help");
            about = new ToolStripMenuItem("&About");

            helpMenuItem.DropDownItems.Add(about);

            about.Click += delegate(object sender, EventArgs e) {
                AboutPavel aboutPavel = new AboutPavel();
                aboutPavel.ShowDialog();
            };
        }

        /// <summary>
        /// Initalizes the visualisations menu.
        /// </summary>
        private void InitVisualisationsMenuItem() {
            visualisationMenuItem = new ToolStripMenuItem("Vis&ualisations");
            visualisationMenuItem.Enabled = false;

            Type[] t = System.Reflection.Assembly.GetAssembly(typeof(Visualization)).GetTypes();

            Array.Sort(t, CompareType);

            foreach (Type type in t) {
                if (type.IsSubclassOf(typeof(Visualization))) {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(type.Name);
                    System.Reflection.PropertyInfo pinfo = type.GetProperty("Icon");
                    if (null != pinfo) {
                        menuItem.Image = (System.Drawing.Bitmap)pinfo.GetValue(null, null);
                    }
                    else {
                        menuItem.Image = null;
                    }
                    menuItem.Tag = type;
                    menuItem.Click += delegate(object sender, EventArgs e) {
                        PavelMain.MainWindow.OpenVisualizationWindow((sender as ToolStripMenuItem).Tag as Type);
                    };
                    visualisationMenuItem.DropDownItems.Add(menuItem);
                }
            }

        }

        /// <summary>
        /// Initialises the solution menu item.
        /// </summary>
        private void InitSolutionsMenuItem() {
            solutionMenuItem = new ToolStripMenuItem("&Solutions");
            solutionMenuItem.Enabled = false;

            solutionMenuItem.EnabledChanged += delegate(object sender, EventArgs e) {
                this.SetSingleSolutionButton(sender, e);
            };
        }


        /// <summary>
        /// Subscribes to events.
        /// </summary>
        private void SubscribeToEvents() {
            ProjectController.ProjectOpened += ProjectController_ProjectOpened;
            ProjectController.ProjectReset += ProjectController_ProjectReset;
            ProjectController.ProjectChanged += ProjectController_ProjectChanged;
            ProjectController.ProjectExport += ProjectController_ProjectSaved;
            ProjectController.CurrentSelection.SelectionModified += CurrentSelection_SelectionModified;
        }

        /// <summary>
        /// Unsubscribes to events.
        /// </summary>
        private void UnSubscribeEvents() {
            ProjectController.ProjectOpened -= ProjectController_ProjectOpened;
            ProjectController.ProjectReset -= ProjectController_ProjectReset;
            ProjectController.CurrentSelection.SelectionModified -= CurrentSelection_SelectionModified;
        }

        #region Helper methods

        /// <summary>
        /// Search predicate for Default Solution
        /// </summary>
        /// <param name="iuc">UseCase searched</param>
        /// <returns>True, if default UseCase, false otherwise</returns>
        private Boolean IsDefaultSolution(IUseCase iuc) {
            if (iuc.Label == "Default")
                return true;
            else
                return false;
        }

        /// <summary>
        /// Compares types
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns>-1 if x&lt;y, 0 if x=y, +1 if x&gt;y</returns>
        private int CompareType(Type x, Type y) {
            return x.Name.CompareTo(y.Name);
        }

        #endregion

        #endregion

        #region Event Handling Stuff

        private void ProjectController_ProjectReset(object sender, EventArgs e) {
            saveProject.Enabled = false;
            saveProjectAs.Enabled = false;
            exportData.Enabled = false;
            closeProject.Enabled = false;
            visualisationMenuItem.Enabled = false;
        }

        private void ProjectController_ProjectOpened(object sender, EventArgs e) {
            saveProject.Enabled = false;
            saveProjectAs.Enabled = true;
            exportData.Enabled = true;
            closeProject.Enabled = true;
            visualisationMenuItem.Enabled = true;
        }

        private void ProjectController_ProjectChanged(object sender, EventArgs e) {
            saveProject.Enabled = true;
        }

        private void ProjectController_ProjectSaved(object sender, EventArgs e) {
            saveProject.Enabled = false;
        }

        private void SetSingleSolutionButton(object sender, EventArgs e) {
            if (this.solutionMenuItem.Enabled) {
                solutionMenuItem.DropDown.Items.Clear();
                ToolStripMenuItem menuItem = new ToolStripMenuItem(Pavel.Framework.ProjectController.Project.UseCase.SolutionInstance.Label);
                menuItem.Click += delegate(object sender2, EventArgs e2) {
                    PavelMain.MainWindow.OpenSolutionWindow(Pavel.Framework.ProjectController.Project.UseCase.SolutionInstance);
                };
                solutionMenuItem.DropDownItems.Add(menuItem);

                if ("Default" != Pavel.Framework.ProjectController.Project.UseCase.Label) {
                    IUseCase iuc = PavelMain.PluginManager.AvailableUseCases.Find(IsDefaultSolution);
                    if (iuc != null) {
                        menuItem = new ToolStripMenuItem(iuc.SolutionInstance.Label);
                        menuItem.Click += delegate(object sender2, EventArgs e2) {
                            PavelMain.MainWindow.OpenSolutionWindow(iuc.SolutionInstance);
                        };
                        solutionMenuItem.DropDownItems.Add(menuItem);
                    }
                }
            }
        }

        private void CurrentSelection_SelectionModified(object sender, EventArgs e) {
            if (ProjectController.CurrentSelection.Length >= 1)
                solutionMenuItem.Enabled = true;
            else
                solutionMenuItem.Enabled = false;
        }

        #endregion
    }
}
