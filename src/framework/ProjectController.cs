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
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;

namespace Pavel.Framework {
    /// <summary>
    /// Controller for Pavels Projects.
    /// </summary>
    public static class ProjectController {

        #region Fields

        private static Project project = new Project();
        private static Selection currentSelection = new Selection();
        private static List<Selection> selections = new List<Selection>();

        #region Event Handling
   		/// <value> Event fired if the list of Spaces has been modified. </value>
        public static event EventHandler SpaceRemoved;
        /// <value> EventHandler for resetting the Project </value>
        public static event EventHandler ProjectReset;
        /// <value> EventHandler for opening the Project </value>
        public static event EventHandler ProjectOpened;
        /// <value> EventHandler for new Project </value>
        public static event EventHandler ProjectNew;
        /// <value> EventHandler for exporting the Project </value>
        public static event EventHandler ProjectExport;
        /// <value> EventHandler for changing the Project </value>
        public static event EventHandler ProjectChanged;
        /// <value> EventHandler for changes to the list of Selections </value>
        public static event EventHandler SelectionsChanged;
        /// <value> EventHandler for changes to the state of the list of Selections </value>
        public static event EventHandler SelectionsStateChanged;

        #endregion

        #endregion

        #region Properties

        /// <value> Gets the current Project </value>
        public static Project Project { get { return project; } }

        /// <value> Gets a globally accessible selection that contains the currently selected Points or set it </value>
        public static Selection CurrentSelection {
            [CoverageExclude]
            get { return currentSelection; }
            set {
                if (null == value) { throw new System.NullReferenceException(); } else { currentSelection = value; }
            }
        }

        /// <value> Gets a list of globally accessible selections that contains the saved selections or sets it </value>
        public static List<Selection> Selections {
            [CoverageExclude]
            get { return selections; }
            set {
                if (null == value) { throw new System.NullReferenceException(); } else { selections = value; }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Use this method to remove a Space from the list of Spaces.
        /// </summary>
        /// <param name="remove">Space to be removed</param>
        public static void RemoveSpace(Space remove) {
            if ( project.spaces.Contains(remove) ) { project.spaces.Remove(remove); }
            if ( null != SpaceRemoved ) { SpaceRemoved(null, EventArgs.Empty); }
        }

        /// <summary>
        /// Saves the current Project.
        /// </summary>
        /// <param name="filename">The full path of the file where the Project is saved</param>
        /// <exception cref="ApplicationException">Is thrown if an error occured while creating file</exception>
        public static void ExportProject(String filename) {
            if (project == null) { throw new ApplicationException("No Project opened"); }

            FileStream fileStream = null;
            try {
                fileStream = new FileStream(filename, FileMode.Create);
                MemoryStream memoryStream = new MemoryStream();
                GZipStream compressedStream = new GZipStream(fileStream, CompressionMode.Compress);
                BinaryFormatter binFormatter = new BinaryFormatter();
                binFormatter.Binder = new Binder();
                binFormatter.Serialize(memoryStream, project);
                memoryStream.WriteTo(compressedStream);
                compressedStream.Flush();
                compressedStream.Close();
                AddToRecentProjects(filename);
                SetProjectChanged(false);
                project.Name = filename;
                if (ProjectExport != null) ProjectExport(PavelMain.MainWindow, new EventArgs());
            } catch (Exception r) {
                throw new ApplicationException("File could not be created " + r);
            } finally {
                if (null != fileStream) fileStream.Close();
            }
        }

        /// <summary>
        /// Restore a previously saved Project.
        /// </summary>
        /// <param name="filename">The full path of the file to be opened</param>
        /// <exception cref="ApplicationException">Is thrown if an error occured while accessing the file</exception>
        public static void OpenSavedProject(String filename) {
            BinaryFormatter binFormatter = new BinaryFormatter();
            binFormatter.Binder = new Binder();
            FileStream fileStream = null;
            try {
                ResetProject();
                fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                GZipStream compressedStream = new GZipStream(fileStream, CompressionMode.Decompress);
                project = (Project)binFormatter.Deserialize(compressedStream);
                compressedStream.Close();
                SetProjectChanged(false);
                project.Name = filename;

                if (ProjectOpened != null) ProjectOpened(PavelMain.MainWindow, new EventArgs());
                AddToRecentProjects(filename);
                if ( Project.UseCase != null ) PavelMain.LogBook.Message("Opened Project:\n" + filename + "\nUseCase: " + Project.UseCase.Label, false);
            } catch (Exception e) {
                throw new ApplicationException("File could not be opened" + e, e);
            } finally { if (null != fileStream) fileStream.Close(); }
        }

        /// <summary>
        /// Creates a new Project.
        /// </summary>
        /// <param name="pr">The data for the Project returned by the Parsers</param>
        public static void NewProject(ParserResult pr) {
            //Handling ParserResults
            project.pointSets.Add(pr.MasterPointSet);
            project.spaces.AddRange(pr.Spaces);
            project.Name = "Untitled";
            if (Project.UseCase != null) {
                PavelMain.LogBook.Message("New Project created\nUseCase: " + Project.UseCase.Label, false);
                if (ProjectNew != null) ProjectNew(PavelMain.MainWindow, new EventArgs());
            }
            if (ProjectOpened != null) ProjectOpened(PavelMain.MainWindow, new EventArgs());
            SetProjectChanged(true);
        }


        /// <summary>
        /// Clears all references in PavelMain.
        /// </summary>
        public static void ResetProject() {
            project = new Project();
            if (ProjectOpened != null) ProjectReset(PavelMain.MainWindow, new EventArgs());
            currentSelection.Clear();
            ClearSelections();
            //Running the GC now, after never everything is cleared, cannot harm
            System.GC.Collect();
        }

        /// <summary>
        /// Creates a default ColumnProperty for each Column in the ColumnSet without a ColumnProperty.
        /// The default values for the ColumnProperties are the min&max of the Column in the PointSet.
        /// </summary>
        /// <param name="ps">The PointSet for which the ColumnProperties have to be created</param>
        public static void CreateMinMaxColumnProperties(PointSet ps) {
            Point[] minMaxMean = ps.MinMaxMean();
            for (int i = 0; i < ps.ColumnSet.Dimension; i++) {
                ps.ColumnSet.Columns[i].DefaultColumnProperty.SetMinMax(minMaxMean[Result.MIN][i], minMaxMean[Result.MAX][i]);
            }
        }

        /// <summary>
        /// Adds p as the first file to the recent projects in the file recentprojects.prp in PAVEls
        /// subfolder (determined by the Assemblyinformation) in the current users application directory.
        /// The maximum number of recent projects is ten. Projectfiles that no longer exist are removed
        /// from recentprojects.prp.
        /// </summary>
        /// <param name="p">The full path of the .pav file to be added.</param>
        public static void AddToRecentProjects(string p) {
            List<String> rplist = new List<string>();
            rplist.Add(p);

            DirectoryInfo dir = new DirectoryInfo(SpecialDirectories.CurrentUserApplicationData);
            if (!dir.Exists) {
                dir.Create();
            }

            if (File.Exists(dir + @"\recentprojects.prp")) {
                StreamReader recentProjects = File.OpenText(dir + @"\recentprojects.prp");

                string line = recentProjects.ReadLine();
                while (line != null) {
                    if ((File.Exists(line)) && (line != p)) { rplist.Add(line); }
                    line = recentProjects.ReadLine();
                }
                recentProjects.Dispose();
            }

            // Only one item is added at a time. This keeps the list from containing more than 10 items.
            if (rplist.Count > 10) { rplist.RemoveAt(10); }
            File.WriteAllLines(dir + @"\recentprojects.prp", rplist.ToArray());
        }

        public static void SetProjectChanged(bool changed) {
            if (changed && !project.Changed || !changed && project.Changed) {
                project.Changed = changed;
                if (ProjectChanged != null) ProjectChanged(PavelMain.MainWindow, new EventArgs());
            }
        }

        /// <summary>
        /// Creates a PointSet that contains only the selected Points or all Points if non of the Points in
        /// the PointSet is selected
        /// </summary>
        /// <param name="ps">A PointSet</param>
        /// <param name="selection">A Selection</param>
        /// <returns>The filteredPointSet or the soriginal PointSet if nothing was selected in the
        /// PointSet</returns>
        public static PointSet FilterPointSetBySelection(PointSet ps, Selection selection) {
            PointSet newPointSet = new PointSet(ps.Label, ps.ColumnSet);
            foreach (PointList pl in ps.PointLists) {
                PointList newPointList = new PointList(pl.ColumnSet);
                for (int i = 0; i < pl.Count; i++) {
                    if (selection.Contains(pl[i])) {
                        newPointList.Add(pl[i]);
                    }
                }
                if ( newPointList.Count != 0 ) {
                    newPointSet.PointLists.Add(newPointList);
                }
            }
            if (newPointSet.Length != 0) {
                // Return filtered PointSet by Selection
                return newPointSet;
            } else {
                // Nothing has been selected in this PointSet: return original PS
                return ps;
            }
        }

        #region Selections

        /// <summary>
        /// Throws the SelectionsStateChanged event.
        /// </summary>
        public static void SelectionStateChanged() {
            if (null != SelectionsStateChanged) { SelectionsStateChanged(PavelMain.MainWindow, new EventArgs()); }
        }

        /// <summary>
        /// Adds the current Selection to the list of Selections.
        /// </summary>
        public static void AddCurrentSelectionToSelections() {
            if (currentSelection.Length > 0) {
                Selection s = new Selection();
                s.AddRange(currentSelection);
                selections.Add(s);
                if (null != SelectionsChanged) { SelectionsChanged(PavelMain.MainWindow, new EventArgs()); }
            }
        }

        /// <summary>
        /// Adds the current Selection to the list of Selections and labels it with the given label.
        /// </summary>
        /// <param name="label">Label for the current Selection</param>
        public static void AddCurrentSelectionToSelections(String label) {
            if (currentSelection.Length > 0) {
                Selection s = new Selection();
                s.Label = label;
                s.AddRange(currentSelection);
                selections.Add(s);
                if (null != SelectionsChanged) { SelectionsChanged(PavelMain.MainWindow, new EventArgs()); }
            }
        }

        /// <summary>
        /// Replaces the current Selection with the union of all active Selections
        /// </summary>
        public static void SetSelectionAsCurrentSelection() {
            currentSelection.Clear();
            foreach (Selection s in selections) {
                if ( s.Active ) { currentSelection.AddRange(s); }
            }
        }

        /// <summary>
        /// Adds the given Selection to the list of Selections.
        /// </summary>
        /// <param name="selection">Selection to be added</param>
        public static void AddSelection(Selection selection) {
            selections.Add(selection);
            if (null != SelectionsChanged) { SelectionsChanged(PavelMain.MainWindow, new EventArgs()); }
        }

        /// <summary>
        /// Adds the given Selections to the list of Selections.
        /// </summary>
        /// <param name="selectionList">List of Selections to be added</param>
        public static void AddSelections(List<Selection> selectionList) {
            selections.AddRange(selectionList);
            if (null != SelectionsChanged) { SelectionsChanged(PavelMain.MainWindow, new EventArgs()); }
        }

        /// <summary>
        /// Removes the given Selection from the list of Selections.
        /// </summary>
        /// <param name="selection">Selection to be removed</param>
        public static void RemoveSelection(Selection selection) {
            selections.Remove(selection);
            if (null != SelectionsChanged) { SelectionsChanged(PavelMain.MainWindow, new EventArgs()); }
        }

        /// <summary>
        /// Removes the given Selections from the list of Selections.
        /// </summary>
        /// <param name="selectionList">List of Selections to be removed</param>
        public static void RemoveSelections(List<Selection> selectionList) {
            for (int i = 0; i<selectionList.Count;i++ ) {
                selections.Remove(selectionList[i]);
            }
            if (null != SelectionsChanged) { SelectionsChanged(PavelMain.MainWindow, new EventArgs()); }
        }

        /// <summary>
        /// Clears the selections
        /// </summary>
        public static void ClearSelections() {
            selections.Clear();
            if (null != SelectionsChanged) { SelectionsChanged(PavelMain.MainWindow, new EventArgs()); }
        }

        /// <summary>
        /// Removes the Selection given by its index from the list of Selections.
        /// </summary>
        /// <param name="index">Index of the Selection to be removed</param>
        public static void RemoveSelection(int index) {
            selections.RemoveAt(index);
            if (null != SelectionsChanged) { SelectionsChanged(PavelMain.MainWindow, new EventArgs()); }
        }

        /// <summary>
        /// Returns the fitting color to a point according to its selection status
        /// </summary>
        /// <param name="p">Point for which color is needed</param>
        /// <returns>the color of the point as ColorOGL</returns>
        public static GUI.Visualizations.ColorOGL GetSelectionColor(Point p) {
            if ( currentSelection.Contains(p) ) return GUI.Visualizations.ColorManagement.CurrentSelectionColor;
            for (int i=0; i < selections.Count;i++){
                Selection s = selections[i];
                if (s.Active && s.Contains(p)) {
                    return GUI.Visualizations.ColorManagement.GetColor(i + 2);
                }
            }
            return GUI.Visualizations.ColorManagement.UnselectedColor;
        }

        #endregion

        #endregion

        #region SerializationBinder

        /// <summary>
        /// This Binder helps to serialize/deserialize useCases
        /// </summary>
        public class Binder : System.Runtime.Serialization.SerializationBinder {
            public override Type BindToType(string assemblyName, string typeName) {
                Type type = null;
                string shortName = assemblyName.Split(',')[0];
                System.Reflection.Assembly[] lasms = AppDomain.CurrentDomain.GetAssemblies();

                foreach (System.Reflection.Assembly lasm in lasms) {
                    if (shortName == lasm.FullName.Split(',')[0]) {
                        type = lasm.GetType(typeName);
                        break;
                    }
                }
                return type;
            }
        }

        #endregion
    }
}
