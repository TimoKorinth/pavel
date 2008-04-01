using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Pavel.GUI.Visualizations {
    public partial class ScatterPlaneSettings : Form {
        private ScatterPlot scatterplot;
        private static decimal slices    = 250;
        private static decimal quantile  = 0.91M;
        private static decimal threshold = 60;

        public ScatterPlaneSettings(ScatterPlot scatterplot) {
            this.scatterplot = scatterplot;
            InitializeComponent();

            slicesUpDown.Minimum   = 0;
            slicesUpDown.Maximum   = 10000;
            slicesUpDown.Value     = slices;
            slicesUpDown.Increment = 10;

            quantileUpDown.Minimum       = 0;
            quantileUpDown.Maximum       = 1;
            quantileUpDown.DecimalPlaces = 3;
            quantileUpDown.Value         = quantile;
            quantileUpDown.Increment     = 0.01M;

            thresholdUpDown.Minimum   = 0;
            thresholdUpDown.Maximum   = 10000;
            thresholdUpDown.Value     = threshold;
            thresholdUpDown.Increment = 10;
        }

        private void okButton_Click(object sender, EventArgs e) {
            slices    = slicesUpDown.Value;
            quantile  = quantileUpDown.Value;
            threshold = thresholdUpDown.Value;
            scatterplot.PlaceScatterPlanes((int)slicesUpDown.Value, (double)quantileUpDown.Value, (int)thresholdUpDown.Value);
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}