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
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Pavel.Framework;

namespace Pavel.Plugins.ProjectStarterPages {
    public partial class ProfileOpener : Pavel.GUI.ProjectStarterPage {

        public ProfileOpener() {
            InitializeComponent();
        }

        public override bool Execute() {
            List<PointSet> profilePointSets = new List<PointSet>();

            int height = 0;
            foreach (object filename in this.profileList.Items) {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(filename as string)) {
                    PointSet ps = new MillingSimulationParser().ParseProfile(reader);
                    ps.Label = "Test Profile " + height.ToString();
                    profilePointSets.Add(ps);
                    height++;
                }
            }
            try {
                foreach (Point p in ProjectController.Project.pointSets[0]) {
                    HeightPoint hp = p as HeightPoint;
                    hp.profile = profilePointSets[hp.height];
                }
                MillingSimulationUseCase.openingHistogramPointSets = null;
            } catch (IndexOutOfRangeException e) {
                throw new ApplicationException("The number of loaded simulation profiles must match the number of histograms in the simulation file.", e);
            }
            return true;
        }

        public override void Reset() {
            this.profileList.Items.Clear();
        }

        public override void Undo() {
            base.Undo();
            foreach (Point p in ProjectController.Project.pointSets[0]) {
                HeightPoint hp = p as HeightPoint;
                hp.profile = null;
            }
        }

        public override bool HasCorrectInput() {
            return true;
        }

        private void selectProfilesButton_Click(object sender, EventArgs e) {
            OpenFileDialog fD = new OpenFileDialog();
            fD.Filter = "All Files|*.*;";
            fD.CheckFileExists = true;
            fD.Multiselect = true;
            fD.Title = "Choose Displacement Profiles";
            if (fD.ShowDialog() == DialogResult.OK) {
                this.profileList.Items.Clear();
                Array.Sort(fD.FileNames);
                foreach (string filename in fD.FileNames){
                    this.profileList.Items.Add(filename);		 
            	}
            }
        }
        
    }
}