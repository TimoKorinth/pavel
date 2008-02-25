namespace Pavel.Plugins {
    partial class ProfilePanel {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.heightLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.outPoint = new System.Windows.Forms.NumericUpDown();
            this.inPoint = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inPoint)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.heightLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.inPoint, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.outPoint, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(157, 142);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // heightLabel
            // 
            this.heightLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.heightLabel, 2);
            this.heightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.heightLabel.Location = new System.Drawing.Point(3, 0);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(41, 13);
            this.heightLabel.TabIndex = 0;
            this.heightLabel.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "X-Range";
            // 
            // outPoint
            // 
            this.outPoint.Location = new System.Drawing.Point(75, 43);
            this.outPoint.Name = "outPoint";
            this.outPoint.Size = new System.Drawing.Size(66, 20);
            this.outPoint.TabIndex = 5;
            // 
            // inPoint
            // 
            this.inPoint.Location = new System.Drawing.Point(3, 43);
            this.inPoint.Name = "inPoint";
            this.inPoint.Size = new System.Drawing.Size(66, 20);
            this.inPoint.TabIndex = 4;
            // 
            // ProfilePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ProfilePanel";
            this.Size = new System.Drawing.Size(157, 142);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inPoint)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label heightLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown inPoint;
        private System.Windows.Forms.NumericUpDown outPoint;
    }
}
