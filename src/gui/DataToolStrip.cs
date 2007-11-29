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
using Pavel.GUI.Visualizations;

namespace Pavel.GUI {
    /// <summary>
    /// ToolStrip containing Controls related to viewing and editing data
    /// </summary>
    class DataToolStrip : ToolStrip {

        #region Fields

        private ToolStripSplitButton visualizationButton;
        private ToolStripSplitButton singleSolutionButton;
        private ToolStripButton clusteringButton;
        private ToolStripButton spaceManagerButton;
        private ToolStripButton pointSetManagerButton;
        private ToolStripButton saveSelectionButton;
        private CheckComboBox selectionsComboBox;
        private ToolStripControlHost selectionsComboBoxHost;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor that initializes all the Buttons and subscribes to events
        /// </summary>
        public DataToolStrip()
            : base() {
            InitializeVisualizationButton();
            InitializeSingleSolutionButton();
            InitializeClusteringButton();
            InitializeSpaceManagerButton();
            InitializePointSetManagerButton();
            InitializeSaveSelectionButton();
            InitializeSelectionsComboBox();

            Items.Add(visualizationButton);
            Items.Add(singleSolutionButton);
            Items.Add(clusteringButton);
            Items.Add(spaceManagerButton);
            Items.Add(pointSetManagerButton);
            Items.Add(selectionsComboBoxHost);
            Items.Add(saveSelectionButton);

            SubscribeToEvents();
        }

        /// <summary>
        /// Dispose Method
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            UnsubscribeFromEvents();
            base.Dispose(disposing);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the visualizationButton.
        /// </summary>
        private void InitializeVisualizationButton() {
            visualizationButton = new ToolStripSplitButton();

            visualizationButton.Image = Pavel.Properties.Resources.Visualizations;
            visualizationButton.Text = "Open new Visualization Window";
            visualizationButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            visualizationButton.Enabled = false;

            Type[] t = System.Reflection.Assembly.GetAssembly(typeof(Visualization)).GetTypes();

            Array.Sort(t, CompareType);

            foreach (Type type in t) {
                if (type.IsSubclassOf(typeof(Visualization))) {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(type.Name);
                    System.Reflection.PropertyInfo pinfo = type.GetProperty("Icon");
                    if (null != pinfo) {
                        menuItem.Image = (System.Drawing.Bitmap)pinfo.GetValue(null, null);
                    } else {
                        menuItem.Image = null;
                    }
                    menuItem.Tag = type;
                    menuItem.Click += delegate(object sender, EventArgs e) {
                        PavelMain.MainWindow.OpenVisualizationWindow((sender as ToolStripMenuItem).Tag as Type);
                        SetDefaultVisualization(sender as ToolStripMenuItem);
                    };
                    visualizationButton.DropDownItems.Add(menuItem);
                }
            }
            if (visualizationButton.DropDownItems.Count > 0) {
                SetDefaultVisualization(visualizationButton.DropDownItems[0] as ToolStripMenuItem);
            }
        }

        /// <summary>
        /// Change the behaviour and Image of the Visualization Button to the last clicked one
        /// </summary>
        /// <param name="item">ToolStripMenuItem that was clicked</param>
        private void SetDefaultVisualization(ToolStripMenuItem item) {
            visualizationButton.DefaultItem = item;
            System.Reflection.PropertyInfo pinfo = (item.Tag as Type).GetProperty("Icon");
            if (null != pinfo) {
                visualizationButton.Image = (System.Drawing.Bitmap)pinfo.GetValue(null, null);
            } else {
                visualizationButton.Image = Pavel.Properties.Resources.Visualizations;
            }
        }

        /// <summary>
        /// Initializes the singleSolutionButton.
        /// </summary>
        private void InitializeSingleSolutionButton() {
            singleSolutionButton = new ToolStripSplitButton();
            singleSolutionButton.Text = "Display Single Solution";
            singleSolutionButton.Image = Pavel.Properties.Resources.ColumnGroupCreate;
            singleSolutionButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            singleSolutionButton.ImageTransparentColor = System.Drawing.Color.Black;
            singleSolutionButton.Enabled = false;
        }

        /// <summary>
        /// Initializes the clusteringButton.
        /// </summary>
        private void InitializeClusteringButton() {
            clusteringButton = new ToolStripButton();
            clusteringButton.Text = "Clustering";
            clusteringButton.Image = Pavel.Properties.Resources.ColumnGroupRemove;
            clusteringButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            clusteringButton.ImageTransparentColor = System.Drawing.Color.Black;
            clusteringButton.Enabled = false;

            clusteringButton.Click += delegate(object sender, EventArgs e) {
                if (ProjectController.Project.pointSets.Count == 0)
                    return;

                Clustering.ClusteringSelectDialog cSD = new Pavel.Clustering.ClusteringSelectDialog();
                cSD.ShowDialog();
            };
        }

        /// <summary>
        /// Initializes the spaceManagerButton.
        /// </summary>
        private void InitializeSpaceManagerButton() {
            spaceManagerButton = new ToolStripButton();
            spaceManagerButton.Text = "Space Manager";
            spaceManagerButton.Image = Pavel.Properties.Resources.Edit;
            spaceManagerButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            spaceManagerButton.ImageTransparentColor = System.Drawing.Color.Black;
            spaceManagerButton.Enabled = false;

            spaceManagerButton.Click += delegate(object sender, EventArgs e) {
                PavelMain.MainWindow.OpenSpaceManager();
            };
        }

        /// <summary>
        /// Initializes the pointSetManagerButton.
        /// </summary>
        private void InitializePointSetManagerButton() {
            pointSetManagerButton = new ToolStripButton();
            pointSetManagerButton.Text = "PointSet Manager";
            pointSetManagerButton.Image = Pavel.Properties.Resources.NewDocumentHS;
            pointSetManagerButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            pointSetManagerButton.ImageTransparentColor = System.Drawing.Color.White;
            pointSetManagerButton.Enabled = false;

            pointSetManagerButton.Click += delegate(object sender, EventArgs e) {
                PavelMain.MainWindow.OpenPointSetManager();
            };
        }

        /// <summary>
        /// Initializes the saveSelectionButton.
        /// </summary>
        private void InitializeSaveSelectionButton() {
            saveSelectionButton = new ToolStripButton();
            saveSelectionButton.Text = "Save Selection";
            saveSelectionButton.Image = Pavel.Properties.Resources.SaveSelections;
            saveSelectionButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveSelectionButton.ImageTransparentColor = System.Drawing.Color.White;

            saveSelectionButton.Click += delegate(object sender, EventArgs e) {
                if (selectionsComboBox.Text.Length == 0) {
                    ProjectController.AddCurrentSelectionToSelections();
                } else {
                    ProjectController.AddCurrentSelectionToSelections(selectionsComboBox.Text);
                }
                selectionsComboBox.Text = "";
            };
        }

        /// <summary>
        /// Initializes the selectionsComboBox.
        /// </summary>
        private void InitializeSelectionsComboBox() {
            selectionsComboBox = new CheckComboBox();
            selectionsComboBoxHost = new ToolStripControlHost(selectionsComboBox);
            selectionsComboBox.CheckStateChanged += delegate(object sender, EventArgs e) {
                for (int i = 0; i < selectionsComboBox.Items.Count; i++) {
                    if ((selectionsComboBox.Items[i] is CheckComboBoxItem)) {
                        ((CheckComboBoxItem)selectionsComboBox.Items[i]).Selection.Active = ((CheckComboBoxItem)selectionsComboBox.Items[i]).CheckState;
                    }
                }
                ProjectController.SelectionStateChanged();
            };
        }

        /// <summary>
        /// Initializes the single solution button, to open a single solution when the singleSolutionButton is clicked.
        /// </summary>
        private void ActivateSingleSolutionButton() {
            singleSolutionButton.DropDown.Items.Clear();
            ToolStripMenuItem menuItem = new ToolStripMenuItem(
                Pavel.Framework.ProjectController.Project.UseCase.SolutionInstance.Label);
            menuItem.Click += delegate(object sender2, EventArgs e2) {
                PavelMain.MainWindow.OpenSolutionWindow(Pavel.Framework.ProjectController.Project.UseCase.SolutionInstance);
            };
            singleSolutionButton.DropDownItems.Add(menuItem);
            singleSolutionButton.DefaultItem = singleSolutionButton.DropDownItems[0];

            if ( "Default" != Pavel.Framework.ProjectController.Project.UseCase.Label ) {
                IUseCase iuc = PavelMain.PluginManager.AvailableUseCases.Find(IsDefaultSolution);
                if ( iuc != null ) {
                    menuItem = new ToolStripMenuItem(iuc.SolutionInstance.Label);
                    menuItem.Click += delegate(object sender2, EventArgs e2) {
                        PavelMain.MainWindow.OpenSolutionWindow(iuc.SolutionInstance);
                    };
                    singleSolutionButton.DropDownItems.Add(menuItem);
                }
            }
        }

        #region Helper methods
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

        /// <summary>
        /// Subscribes to events.
        /// </summary>
        private void SubscribeToEvents() {
            ProjectController.ProjectOpened += ProjectController_ProjectOpened;
            ProjectController.ProjectReset += ProjectController_ProjectReset;
            ProjectController.SelectionsChanged += ProjectController_SelectionsChanged;
            ProjectController.CurrentSelection.SelectionModified += UpdateSingleSolutionButton;
        }

        /// <summary>
        /// Unsubscribes from events.
        /// </summary>
        private void UnsubscribeFromEvents() {
            ProjectController.ProjectOpened -= ProjectController_ProjectOpened;
            ProjectController.ProjectReset -= ProjectController_ProjectReset;
            ProjectController.SelectionsChanged -= ProjectController_SelectionsChanged;
            ProjectController.CurrentSelection.SelectionModified -= UpdateSingleSolutionButton;
        }

        void ProjectController_ProjectOpened(object sender, EventArgs e) {
            visualizationButton.Enabled = true;
            clusteringButton.Enabled = true;
            spaceManagerButton.Enabled = true;
            pointSetManagerButton.Enabled = true;
            ActivateSingleSolutionButton();
        }

        void ProjectController_ProjectReset(object sender, EventArgs e) {
            visualizationButton.Enabled = false;
            clusteringButton.Enabled = false;
            spaceManagerButton.Enabled = false;
            pointSetManagerButton.Enabled = false;
        }

        void ProjectController_SelectionsChanged(object sender, EventArgs e) {
            selectionsComboBox.Items.Clear();
            if (ProjectController.Selections.Count > 0) {
                selectionsComboBox.Items.Add(new CheckComboBoxString("Select all...", CheckComboBoxString.Operations.SelectAll));
                selectionsComboBox.Items.Add(new CheckComboBoxString("Deselect all...", CheckComboBoxString.Operations.DeselectAll));
                selectionsComboBox.Items.Add(new CheckComboBoxString("Remove selected...", CheckComboBoxString.Operations.RemoveSelected));
                selectionsComboBox.Items.Add(new CheckComboBoxString("Set to current...", CheckComboBoxString.Operations.SetToCurrent));
            }
            int colIndex = 2;
            foreach (Selection s in ProjectController.Selections) {
                if (s.Label.Length == 0) {
                    CheckComboBoxItem item = new CheckComboBoxItem(s.Length.ToString(), s.Active, s);
                    item.BackColor = ColorManagement.GetColor(colIndex).ToColor();
                    selectionsComboBox.Items.Add(item);
                } else {
                    CheckComboBoxItem item = new CheckComboBoxItem(s.Label, s.Active, s);
                    item.BackColor = ColorManagement.GetColor(colIndex).ToColor();
                    selectionsComboBox.Items.Add(item);
                }
                colIndex++;
            }
        }

        /// <summary>
        /// Search predicate for Default Solution
        /// </summary>
        /// <param name="iuc">UseCase searched</param>
        /// <returns>True, if default UseCase, false otherwise</returns>
        private Boolean IsDefaultSolution(IUseCase iuc) {
            if (iuc.Label == "Default") return true;
            else return false;
        }

        /// <summary>
        /// Changes the state ob the singleSolutionButton.
        /// </summary>
        /// <param name="sender">ProjectController.CurrentSelection</param>
        /// <param name="e">Standard EventArgs</param>
        private void UpdateSingleSolutionButton(object sender, EventArgs e) {
            singleSolutionButton.Enabled = (ProjectController.CurrentSelection.Length >= 1);
        }

        #endregion
    }
}
