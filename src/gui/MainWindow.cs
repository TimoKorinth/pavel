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
using System.Windows.Forms;
using Pavel.Framework;
using System.Resources;
using System.Collections.Generic;
using Pavel.GUI.Visualizations;
using Pavel.GUI.SolutionVisualizations;

namespace Pavel.GUI {
    /// <summary>
    /// MainWindow contains the complete handling for the views.
    /// </summary>
    public partial class MainWindow : Form {

        #region Fields

        private MainStatusStrip statusBar;
        private TabControl tabControl;

        #endregion

        #region Properties

        /// <value> Gets the propertyControl of this MainWindow </value>
        public PropertyControl PropertyControl { get { return propertyControl; } }

        /// <value> Gets the statusBar of this MainWindow </value>
        public MainStatusStrip StatusBar { get { return statusBar; } }

        /// <value> Gets the tabControl of this MainWindow </value>
        public TabControl TabControl { get { return tabControl; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the MainWindow and subscribes to events.
        /// </summary>
        public MainWindow() {
            // Designer code
            this.InitializeComponent();
            this.Icon = Properties.Resources.PavelIcon;
            //Connect PropertyWindow to VisualizationWindows
            MdiChildActivate += propertyControl.MainWindow_MdiChildActivate;
            InitializeToolStrips();
            InitializeTabControl();

            SubscribeToEvents();
        }

        #endregion

        #region Methods

        #region Initialization

        /// <summary>
        /// Initializes the ToolStrips.
        /// </summary>
        private void InitializeToolStrips() {
            this.topToolStripPanel.Controls.Add(new DataToolStrip());
            this.topToolStripPanel.Controls.Add(new ProjectToolStrip());
            MainMenuStrip mainMenu = new MainMenuStrip();
            this.Controls.Add(mainMenu);
            mainMenu.Dock = DockStyle.Top;
            this.MainMenuStrip = mainMenu;
            this.statusBar = new MainStatusStrip();
            this.Controls.Add(this.statusBar);
            this.statusBar.Dock = DockStyle.Bottom;
        }

        /// <summary>
        /// Initialize the tabControl
        /// </summary>
        private void InitializeTabControl() {
            tabControl = new TabControl();
            tabControl.Height = 24;
            tabControl.Dock = DockStyle.Bottom;
            tabControl.Visible = false;
            this.Controls.Add(tabControl);
        }
        #endregion

        #region Project related Methods

        /// <summary>
        /// Subscribes to the Shown event of the MainWindow.
        /// Used to open the ProjectStarter dialog, when no file or a wrong file is given as argument.
        /// </summary>
        public void SubscribeToShown() {
            this.Shown += this.MainWindow_Shown;
        }

        /// <summary>
        /// Starts a Project using the ProjectStarter class.
        /// </summary>
        public void StartProject() {
            if (ProjectController.Project.UseCase == null || CloseProject() != DialogResult.Cancel) {
                ProjectStarter uCW = new ProjectStarter(PavelMain.PluginManager);
                uCW.ShowDialog();
            };
        }

        /// <summary>
        /// Saves the project.
        /// </summary>
        public void SaveProject() {
            String filename;
            if (ProjectController.Project.Name.Equals("Untitled")) {
                SaveFileDialog savePavelProj = new SaveFileDialog();
                savePavelProj.Title = "Save Project";
                savePavelProj.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                savePavelProj.Filter = "Pavel Project File (*.pav)|*.pav";
                savePavelProj.AddExtension = true;
                savePavelProj.DefaultExt = "pav";
                savePavelProj.OverwritePrompt = true;

                if (savePavelProj.ShowDialog() != DialogResult.OK) {
                    return;
                }
                filename = savePavelProj.FileName;
            } else {
                filename = ProjectController.Project.Name;
            }

            try {
                Cursor.Current = Cursors.WaitCursor;
                ProjectController.ExportProject(filename);
                Cursor.Current = Cursors.Default;
            } catch (ApplicationException ex) {
                Cursor.Current = Cursors.Default;
                PavelMain.LogBook.Error("The following error occured, while trying to open the selected file:" + "\n" + ex.Message, true);
            }
        }

        /// <summary>
        /// Saves the project with another file name.
        /// </summary>
        public void SaveProjectAs() {
            SaveFileDialog savePavelProjAs = new SaveFileDialog();
            savePavelProjAs.Title = "Save Project As";
            savePavelProjAs.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            savePavelProjAs.Filter = "Pavel Project File (*.pav)|*.pav";
            savePavelProjAs.AddExtension = true;
            savePavelProjAs.DefaultExt = "pav";
            savePavelProjAs.OverwritePrompt = true;
            if (savePavelProjAs.ShowDialog() == DialogResult.OK) {
                try {
                    Cursor.Current = Cursors.WaitCursor;
                    ProjectController.ExportProject(savePavelProjAs.FileName);
                    Cursor.Current = Cursors.Default;
                } catch (ApplicationException ex) {
                    Cursor.Current = Cursors.Default;
                    PavelMain.LogBook.Error("The following error occured, while trying to open the selected file:" + "\n" + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Opens an "Open Project" dialog to open a project from a .pav file.
        /// </summary>
        public Boolean OpenProject() {
            Boolean retValue = false;
            OpenFileDialog openPavelFile = new OpenFileDialog();
            openPavelFile.Title = "Open Project";
            openPavelFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openPavelFile.Filter = "Pavel Project File (*.pav)|*.pav";

            if (openPavelFile.ShowDialog() == DialogResult.OK) {
                //Close open windows
                foreach (Form f in this.MdiChildren) {
                    f.Close();
                }
                try {
                    Cursor.Current = Cursors.WaitCursor;
                    ProjectController.OpenSavedProject(openPavelFile.FileName);
                    retValue = true;
                } catch (ApplicationException ex) {
                    PavelMain.LogBook.Error("The following error occured, while trying to open the selected file:" + "\n" + ex.Message, true);
                    retValue = false;
                } finally { Cursor.Current = Cursors.Default; }
            }
            return retValue;
        }

        /// <summary>
        /// Opens a dialog asking the user if the Project should be saved.
        /// </summary>
        /// <returns>Returns the DialogResult</returns>
        public DialogResult CloseProject() {
            DialogResult closeResult;

            if (ProjectController.Project.Changed) {
                closeResult = MessageBox.Show("Do you want to save your project before closing it?",
                    Application.ProductName,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

                // If it should be saved, it is done here.
                if (closeResult == DialogResult.Yes) {
                    this.SaveProject();
                }
            } else {
                closeResult = DialogResult.No;
            }

            // If the result is canceled, the project is not closed.
            if (closeResult != DialogResult.Cancel) {

                // iterate over mdiChildren
                foreach (Form f in this.MdiChildren) {
                    // don't close PropertyWindow
                    //if (f != this.propertyWindow) {
                    f.Close();
                    //}
                }

                // delete old columnSets and reference to masterdata
                ProjectController.ResetProject();

            }

            return closeResult;
        }

        /// <summary>
        /// Shows a LogWindow.
        /// </summary>
        internal void ShowLog() {
            LogWindow lw = new LogWindow();
            lw.MdiParent = this;
            lw.EnableTab(tabControl);
            lw.Show();
        }

        #endregion

        #region Data related Methods

        /// <summary>
        /// Opens a new SolutionWindow that contains the currently selected Point.
        /// </summary>
        /// <param name="solution">The Solution to be displayed</param>
        public void OpenSolutionWindow(Solution solution) {
            List<Pavel.Framework.Point> points = new List<Pavel.Framework.Point>();

            if (ProjectController.Project.UseCase.SolutionColumnSet != null) {
                //Get all points of the current selection and
                //Remove all points that don't fit in the solutionSpace, because they have not all needed columns
                foreach (Pavel.Framework.Point point in ProjectController.CurrentSelection.GetPoints) {
                    if (ProjectController.Project.UseCase.SolutionColumnSet.IsSubSetOf(point.ColumnSet)) {
                        points.Add(point);
                    }
                }
                if (points.Count == 0) {
                    PavelMain.LogBook.Warning("No selected point has a sufficent columnSet to be display.", true);
                    return;
                }
            } else { points.AddRange(ProjectController.CurrentSelection.GetPoints); }

            // Create a Solution Window for the given solution plugin
#if !DEBUG
            try {
#endif
                solution.Initialize(points.ToArray());
                solution.MdiParent = this;
                solution.EnableTab(tabControl);
                solution.Show();
#if !DEBUG
            } catch (Exception ex) {
                PavelMain.LogBook.Error("The following error occured, while trying to display the selected solution(s):" + "\n" + ex.Message, true);
            }
#endif

        }

        /// <summary>
        /// Opens a new VisualizationWindow.
        /// </summary>
        /// <param name="visualizationType">
        /// The type of the Visualization to be displayed in the VisualizationWindow.
        /// Must be a subclass of Visualization.
        /// </param>
        public void OpenVisualizationWindow(Type visualizationType) {
            try {
                Space    selectedSpace;
                PointSet selectedPointSet;
                if (ProjectController.Project.pointSets.Count != 1 || ProjectController.Project.spaces.Count != 1) {
                    SpaceSelectDialog sSD = new SpaceSelectDialog(ProjectController.Project.pointSets);
                    if (sSD.ShowDialog() == DialogResult.OK && sSD.SelectedSpace != null) {
                        selectedSpace    = sSD.SelectedSpace;
                        selectedPointSet = sSD.SelectedPointSet;
                    } else {
                        return;
                    }
                } else {
                    selectedSpace    = ProjectController.Project.spaces[0];
                    selectedPointSet = ProjectController.Project.pointSets[0];
                }
                VisualizationWindow visWindow = new VisualizationWindow(selectedPointSet, selectedSpace, visualizationType.Name);
                visWindow.EnableTab(this.tabControl);
                visWindow.MdiParent = this;
                visWindow.Show();
            } catch (Exception e) {
#if DEBUG
                throw e;
#else
                MessageBox.Show(e.Message, "Error");
#endif
            }
        }

        /// <summary>
        /// Opens a SpaceManager.
        /// </summary>
        public void OpenSpaceManager() {
            SpaceManager spaceManager = new SpaceManager();
            spaceManager.ShowDialog();
            spaceManager.Dispose();
        }

        /// <summary>
        /// Opens a PointSetManager.
        /// </summary>
        public void OpenPointSetManager() {
            PointSetManager psManager;
            if (this.ActiveMdiChild is VisualizationWindow) {
                psManager = new PointSetManager(((VisualizationWindow)this.ActiveMdiChild).DisplayedPointSet);
            } else {
                psManager = new PointSetManager();
            }
            psManager.ShowDialog();
        }

        #endregion

        #region MainWindow-display related Methods

        /// <summary>
        /// Adds a ToolStrip to the topToolStripPanel after all other visible ToolStrips.
        /// </summary>
        /// <param name="ts">The ToolStrip to be added</param>
        public void AddToolStrip(ToolStrip ts) {
            if (topToolStripPanel.Controls.Contains(ts)) return;
            int pos = 0;
            foreach (Control c in topToolStripPanel.Controls) {
                if (c.Visible) pos = Math.Max(pos, c.Bounds.Right);
            }
            topToolStripPanel.Join(ts,pos,0);
        }

        /// <summary>
        /// Remove a ToolStrip from the topToolStripPanel
        /// </summary>
        /// <param name="ts">The ToolStrip to be removed</param>
        public void RemoveToolStrip(ToolStrip ts) {
            topToolStripPanel.Controls.Remove(ts);
        }

        /// <summary>
        /// Closes all MDI-children.
        /// </summary>
        public void CloseAllChildren() {
            foreach (Form child in this.MdiChildren) { child.Close(); }
        }

        /// <summary>
        /// Toggle the visibility of the Property Control
        /// </summary>
        /// <param name="show"></param>
        public void ShowPropertyWindow(bool show) {
            this.propertyControl.Visible = show;
            this.splitter1.Visible = show;
        }

        /// <summary>
        /// Returns the index of the Space of the active MDI Child within the global Space List
        /// </summary>
        /// <returns>zero-based index, or -1 if error</returns>
        public int ActiveSpaceIndex() {
            if (null != this.ActiveMdiChild && (this.ActiveMdiChild is VisualizationWindow)) {
                Space activeSpace = ((VisualizationWindow)(this.ActiveMdiChild)).Space;
                return ProjectController.Project.spaces.IndexOf(activeSpace.Parent);
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the Space of the active MDI Child within the List of Spaces
        /// given by the PointSet
        /// </summary>
        /// <param name="pointSet">PointSet to search the Space</param>
        /// <returns>zero-based index, or -1 if error</returns>
        public int ActiveSpaceIndex(PointSet pointSet) {
            if (null != this.ActiveMdiChild && (this.ActiveMdiChild is VisualizationWindow)) {
                Space activeSpace = ((VisualizationWindow)(this.ActiveMdiChild)).Space;
                return Space.AllSpacesForColumnSet(pointSet.ColumnSet).IndexOf(activeSpace.Parent);
            }
            return -1;
        }

        /// <summary>
        /// Returns the global index of the PointSet of the active MDI Child within the global PointSet List
        /// </summary>
        /// <returns>zero-based index, or -1 if error</returns>
        public int ActivePointSetIndex() {
            if (null != this.ActiveMdiChild && (this.ActiveMdiChild is VisualizationWindow)) {
                PointSet activePointSet = ((VisualizationWindow)(this.ActiveMdiChild)).PointSet;
                return ProjectController.Project.pointSets.IndexOf(activePointSet);
            }
            return -1;
        }

        /// <summary>
        /// Subscribes to the events
        /// </summary>
        private void SubscribeToEvents() {
            ProjectController.ProjectReset   += ChangeTitle;
            ProjectController.ProjectOpened  += ChangeTitle;
            ProjectController.ProjectNew     += ChangeTitle;
            ProjectController.ProjectExport  += ChangeTitle;
            ProjectController.ProjectChanged += ChangeTitle;
            tabControl.SelectedIndexChanged  += tabControl_SelectedIndexChanged;
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        private void UnsubscribeFromEvents() {
            ProjectController.ProjectReset   -= ChangeTitle;
            ProjectController.ProjectOpened  -= ChangeTitle;
            ProjectController.ProjectNew     -= ChangeTitle;
            ProjectController.ProjectExport  -= ChangeTitle;
            ProjectController.ProjectChanged -= ChangeTitle;
            tabControl.SelectedIndexChanged  -= tabControl_SelectedIndexChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeTitle(object sender, EventArgs e) {
            if (ProjectController.Project.Name == null) {
                this.Text = "PAVEL";
            } else {
                String titleName = ProjectController.Project.Name.Substring(ProjectController.Project.Name.LastIndexOf('\\') + 1, ProjectController.Project.Name.Length - ProjectController.Project.Name.LastIndexOf('\\') - 1);
                if (ProjectController.Project.Changed) {
                    this.Text = "PAVEL - " + titleName + " *";
                } else {
                    this.Text = "PAVEL - " + titleName;
                }
            }
        }

        #endregion

        #endregion

        #region Event Handling Stuff

        /// <summary>
        /// Displays the ProjectStarter, when the MainWindow is first shown.
        /// </summary>
        /// <param name="sender">The Show method of the MainWindow</param>
        /// <param name="e">Standard EventArgs</param>
        private void MainWindow_Shown(object sender, EventArgs e) {
            this.StartProject();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e) {
        #if !DEBUG
            if (ProjectController.Project.Changed) {
                DialogResult closeResult = MessageBox.Show("Project changed, save it?",
                    Application.ProductName,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

                if (closeResult == DialogResult.Yes) {
                    this.SaveProject();
                }
            }
        #endif
        }

        /// <summary>
        /// Activates the mdi-child if a tab is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_SelectedIndexChanged(object sender, System.EventArgs e) {
            foreach ( Form childForm in this.MdiChildren ) {
                TabableForm f = childForm as TabableForm;
                //Check for its corresponding MDI child form
                if ( f != null && f.TabPag.Equals(tabControl.SelectedTab) ) {
                    //Activate the MDI child form
                    childForm.Select();
                }
            }
        }
        #endregion
    }
}