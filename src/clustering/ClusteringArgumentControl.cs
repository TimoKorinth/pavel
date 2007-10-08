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
using System.Reflection;
using System.Drawing;

namespace Pavel.Clustering {
    /// <summary>
    /// Control that displays information relevant for the active ClusteringAlgorithm.
    /// </summary>
    public class ClusteringArgumentControl : UserControl {

        #region Fields

        private ToolTip helpToolTip;
        private System.ComponentModel.IContainer components;
        private TableLayoutPanel argsTable;

        #endregion

        #region Constructors

        /// <summary>
        /// Standard constructor that initializes the Control
        /// </summary>
        public ClusteringArgumentControl() : base() {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor used for Data-Binding.
        /// Use this constructor for standard Control by use of ArgsDescription-Attribute.
        /// </summary>
        /// <param name="ca">A ClusteringAlgorithm-Instance for Data-Binding</param>
        public ClusteringArgumentControl(ClusteringAlgorithm ca) : base() {
            InitializeComponent();
            Type clusterType = ca.GetType();
            PropertyInfo[] pInfo = clusterType.GetProperties();
            argsTable.RowCount = 0;
            argsTable.SuspendLayout();

            for (int i = 0; i < pInfo.Length; i++) {
                foreach (Attribute attribute in pInfo[i].GetCustomAttributes(false)) {
                    if (attribute is ArgsDescriptionAttribute) {
                        ArgsDescriptionAttribute argAttribute = (ArgsDescriptionAttribute)attribute;

                        #region Create label
                        Label label = new Label();
                        label.Dock = DockStyle.Fill;
                        if (argAttribute.Name != "") {
                            label.Text = argAttribute.Name + ":";
                        } else {
                            label.Text = pInfo[i].Name + ":";
                        }
                        label.TextAlign = ContentAlignment.MiddleRight;
                        this.helpToolTip.SetToolTip(label, argAttribute.Description);

                        //this.argsTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
                        //Add label
                        argsTable.Controls.Add(label, 0, argsTable.RowCount);
                        #endregion

                        //Create Control
                        if (argAttribute is CheckBoxAttribute) {
                            CheckBox checkBox = new CheckBox();
                            checkBox.Dock = DockStyle.Fill;
                            checkBox.DataBindings.Add(new Binding("Checked", ca, pInfo[i].Name));
                            
                            argsTable.Controls.Add(checkBox, 1, argsTable.RowCount);
                        } else if (argAttribute is SpinnerAttribute) {
                            NumericUpDown spinner = new NumericUpDown();
                            spinner.Dock = DockStyle.Fill;
                            spinner.DecimalPlaces = (argAttribute as SpinnerAttribute).DecimalPlaces;
                            spinner.Increment = (argAttribute as SpinnerAttribute).Increment;
                            spinner.Minimum = (decimal)(argAttribute as SpinnerAttribute).Minimum;
                            spinner.Maximum = (decimal)(argAttribute as SpinnerAttribute).Maximum;
                            spinner.DataBindings.Add(new Binding("Value", ca, pInfo[i].Name));
                            
                            argsTable.Controls.Add(spinner, 1, argsTable.RowCount);
                        } else if (argAttribute is ComboBoxAttribute) {
                            ComboBox box = new ComboBox();
                            box.Dock = DockStyle.Fill;
                            box.Items.AddRange((argAttribute as ComboBoxAttribute).Items);
                            box.DataBindings.Add(new Binding("SelectedIndex", ca, pInfo[i].Name));
                            
                            argsTable.Controls.Add(box, 1, argsTable.RowCount);
                        }

                        argsTable.RowCount++;

                        // Only one ArgsDescriptionAttribute
                        break;
                    } // if attribute is ArgsDescriptionAttribute
                } // foreach attribute
            } //"foreach" field

            argsTable.ResumeLayout();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the Control.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.argsTable = new System.Windows.Forms.TableLayoutPanel();
            this.helpToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // argsTable
            // 
            this.argsTable.AutoSize = true;
            this.argsTable.BackColor = System.Drawing.SystemColors.Control;
            this.argsTable.ColumnCount = 2;
            this.argsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 177F));
            this.argsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.argsTable.Location = new System.Drawing.Point(3, 3);
            this.argsTable.Name = "argsTable";
            this.argsTable.RowCount = 2;
            this.argsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.argsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.argsTable.Size = new System.Drawing.Size(338, 30);
            this.argsTable.TabIndex = 0;
            // 
            // ClusteringArgumentControl
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.argsTable);
            this.Name = "ClusteringArgumentControl";
            this.Size = new System.Drawing.Size(344, 36);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
