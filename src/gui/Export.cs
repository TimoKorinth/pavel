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
using Pavel.Framework;
using Pavel.Clustering;
using System.IO;
using System.Windows.Forms;

namespace Pavel.GUI {
    /// <summary>
    /// Class to export data.
    /// </summary>
    public static class Export {

        #region Methods

        /// <summary>
        /// Writes the data of the selected ClusterSet to a file.
        /// </summary>
        public static void ExportData() {
            SpaceSelectDialog sSD = new SpaceSelectDialog(ProjectController.Project.pointSets);
            if (sSD.ShowDialog() == DialogResult.OK && sSD.SelectedSpace != null) {
                if (sSD.SelectedPointSet as ClusterSet == null) {
                    MessageBox.Show("Sorry, Export only for Clusterings!");
                } else {
                    // Show FileDialog
                    SaveFileDialog saveDlg = new SaveFileDialog();
                    saveDlg.Title = "Export to File";
                    saveDlg.Filter = "Text file|*.txt|All files|*.*";
                    saveDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    if (saveDlg.ShowDialog() == DialogResult.OK) {
                        SaveDataToFile(saveDlg.FileName, sSD.SelectedSpace, sSD.SelectedPointSet);
                    }
                }
            }
        }

        private static void SaveDataToFile(String filename, Space space, PointSet selectedPointSet) {
            // Export Data
            StreamWriter output = null;
            try {
                output = new StreamWriter(filename);
                output.WriteLine("#PAVEL by PG500");
                output.WriteLine("#");
                output.WriteLine("#" + space.Label);
                output.WriteLine("#");
                ClusterSet clustering = selectedPointSet as ClusterSet;
                for (int i = 0; i < clustering.Length; i++) {
                    output.Write("# Cluster " + i + ":");
                    foreach (double d in clustering[i].Values) {
                        output.Write(" " + d);
                    }
                    output.WriteLine("");
                }
                output.WriteLine("#");
                int x = 0;
                foreach (Column c in clustering.BasicPointSet.ColumnSet) {
                    output.WriteLine("### Column " + x++ + " " + c.Label);
                }
                output.WriteLine("### Column " + x++ + " Cluster ID");
                output.WriteLine("");
                output.WriteLine("");

                x = 0;
                for (int i = 0; i < clustering.Length; i++) {
                    for (int j = 0; j < (clustering[i] as Cluster).PointSet.Length; j++) {
                        output.WriteLine("# Individual " + x);
                        foreach (double d in (clustering[i] as Cluster).PointSet[j].Values) {
                            output.Write(" " + d);
                        }
                        output.Write(" " + i);
                        x++;
                        output.WriteLine("");
                        output.WriteLine("");
                    }
                }
                output.WriteLine("");
                output.WriteLine("");
                PavelMain.LogBook.Message("Exported file \"" + filename + "\" successful exported!", false);
            } catch (Exception e) {
                PavelMain.LogBook.Error("Error writing file \"" + filename + "\":" + e.Message, true);
            } finally {
                output.Flush();
                output.Close();
            }
        }

        #endregion
    }
}
