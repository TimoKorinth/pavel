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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

// Minimum und Maximum der NumericUpDowns richtig setzen, in Abhaengigkeit voneinander
//
// Nicht unbedingt noetig, im Moment verhaelt sich das Control bei zusammenlaufen
// von InPoint und OutPoint mathematisch korrekt. Um Irritationen zu vermeiden
// koennte man die beschraenkung setzen, das ist aber luxus.
// Das ganze waere sehr kompliziert umzusetzen weil an allen moeglichen Stellen
// Checks eingefuehrt werden muessten, idealerweise im Profile und dann ueber Events
// und Callbacks hierhin zurueckgeben. Also nee, bei aller Liebe...

namespace Pavel.Plugins {
    /// <summary>
    /// Controls inPoint and outPoint in X-Direction for the MillingsimulationSolution
    /// </summary>
    public partial class ProfilePanel : UserControl {
        private MillingSimulationSolution.Profile profile;
        internal ProfilePanel(MillingSimulationSolution.Profile profile) {
            InitializeComponent();
            this.profile = profile;

            this.heightLabel.Text = profile.pointSet.Label;

            this.inPoint.Minimum   = 0; 
            this.inPoint.Maximum   = profile.pointSet.Length;
            this.inPoint.Increment = 1000;

            this.outPoint.Minimum   = 0;
            this.outPoint.Maximum   = profile.pointSet.Length;
            this.outPoint.Increment = 1000;

            UpdatePanel();
            RegisterEvents();
        }

        private void RegisterEvents() {
            profile.Changed       += profileChanged;
            inPoint.ValueChanged  += inPointValueChanged;
            outPoint.ValueChanged += outPointValueChanged;
        }

        private void UnregisterEvents() {
            outPoint.ValueChanged += outPointValueChanged;
            inPoint.ValueChanged  += inPointValueChanged;
            profile.Changed       -= profileChanged;
        }

        protected override void Dispose(bool disposing) {
            UnregisterEvents();
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// React to changes in the Profile, e.g. when
        /// setting in/outPoint through another Method (using Mouse for example)
        /// </summary>
        void profileChanged(object sender, EventArgs e) {
            UpdatePanel();
        }

        void inPointValueChanged(object sender, EventArgs e) {
            profile.InPoint = (int)this.inPoint.Value;
        }

        void outPointValueChanged(object sender, EventArgs e) {
            profile.OutPoint = (int)this.outPoint.Value;
        }

        private void UpdatePanel() {
            this.inPoint.Value  = profile.InPoint;
            this.outPoint.Value = profile.OutPoint;
        }
    }
}
