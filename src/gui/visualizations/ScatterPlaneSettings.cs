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
        private static decimal median    = 0.91M;
        private static decimal threshold = 60;

        public ScatterPlaneSettings(ScatterPlot scatterplot) {
            this.scatterplot = scatterplot;
            InitializeComponent();

            slicesUpDown.Minimum   = 0;
            slicesUpDown.Maximum   = 10000;
            slicesUpDown.Value     = slices;
            slicesUpDown.Increment = 10;

            medianUpDown.Minimum       = 0;
            medianUpDown.Maximum       = 1;
            medianUpDown.DecimalPlaces = 3;
            medianUpDown.Value         = median;
            medianUpDown.Increment     = 0.01M;

            thresholdUpDown.Minimum   = 0;
            thresholdUpDown.Maximum   = 10000;
            thresholdUpDown.Value     = threshold;
            thresholdUpDown.Increment = 10;
        }

        private void okButton_Click(object sender, EventArgs e) {
            slices    = slicesUpDown.Value;
            median    = medianUpDown.Value;
            threshold = thresholdUpDown.Value;
            scatterplot.PlaceScatterPlanes((int)slicesUpDown.Value, (double)medianUpDown.Value, (int)thresholdUpDown.Value);
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}