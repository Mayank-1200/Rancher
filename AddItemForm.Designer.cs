using System;
using System.Windows.Forms;

namespace Rancher
{
    partial class AddItemForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtItemNumber;
        private TextBox txtProductName;
        private TextBox txtSupplier;
        private NumericUpDown numQuantity;
        private NumericUpDown numRedThreshold;
        private NumericUpDown numYellowThreshold;
        private NumericUpDown numGreenThreshold;
        private Button btnSave;
        private Button btnCancel;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtItemNumber = new System.Windows.Forms.TextBox();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.txtSupplier = new System.Windows.Forms.TextBox();
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.numRedThreshold = new System.Windows.Forms.NumericUpDown();
            this.numYellowThreshold = new System.Windows.Forms.NumericUpDown();
            this.numGreenThreshold = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRedThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYellowThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGreenThreshold)).BeginInit();

            // Form Properties
            this.Text = "Add Item";
            this.ClientSize = new System.Drawing.Size(420, 360);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            // Labels
            Label lblItemNumber = new Label() { Text = "Item Number:", Left = 20, Top = 20, Width = 120 };
            Label lblProductName = new Label() { Text = "Product Name:", Left = 20, Top = 60, Width = 120 };
            Label lblQuantity = new Label() { Text = "Quantity:", Left = 20, Top = 100, Width = 120 };
            Label lblSupplier = new Label() { Text = "Supplier:", Left = 20, Top = 140, Width = 120 };
            Label lblRedThreshold = new Label() { Text = "Red Threshold:", Left = 20, Top = 180, Width = 120 };
            Label lblYellowThreshold = new Label() { Text = "Yellow Threshold:", Left = 20, Top = 220, Width = 120 };
            Label lblGreenThreshold = new Label() { Text = "Green Threshold:", Left = 20, Top = 260, Width = 120 };

            // TextBoxes & Inputs
            this.txtItemNumber.Location = new System.Drawing.Point(150, 20);
            this.txtItemNumber.Width = 230;

            this.txtProductName.Location = new System.Drawing.Point(150, 60);
            this.txtProductName.Width = 230;

            this.numQuantity.Location = new System.Drawing.Point(150, 100);
            this.numQuantity.Width = 230;
            this.numQuantity.Minimum = 0;
            this.numQuantity.Maximum = 100000; // Increased for larger stock numbers

            this.txtSupplier.Location = new System.Drawing.Point(150, 140);
            this.txtSupplier.Width = 230;

            // Threshold Numeric Fields
            this.numRedThreshold.Location = new System.Drawing.Point(150, 180);
            this.numRedThreshold.Width = 230;
            this.numRedThreshold.Minimum = 0;
            this.numRedThreshold.Maximum = 100000;

            this.numYellowThreshold.Location = new System.Drawing.Point(150, 220);
            this.numYellowThreshold.Width = 230;
            this.numYellowThreshold.Minimum = 0;
            this.numYellowThreshold.Maximum = 100000;

            this.numGreenThreshold.Location = new System.Drawing.Point(150, 260);
            this.numGreenThreshold.Width = 230;
            this.numGreenThreshold.Minimum = 0;
            this.numGreenThreshold.Maximum = 100000;

            // Buttons
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(100, 300);
            this.btnSave.Width = 100;
            this.btnSave.Click += new System.EventHandler(this.SaveItem);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(220, 300);
            this.btnCancel.Width = 100;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // Add Controls
            this.Controls.Add(lblItemNumber);
            this.Controls.Add(this.txtItemNumber);
            this.Controls.Add(lblProductName);
            this.Controls.Add(this.txtProductName);
            this.Controls.Add(lblQuantity);
            this.Controls.Add(this.numQuantity);
            this.Controls.Add(lblSupplier);
            this.Controls.Add(this.txtSupplier);
            this.Controls.Add(lblRedThreshold);
            this.Controls.Add(this.numRedThreshold);
            this.Controls.Add(lblYellowThreshold);
            this.Controls.Add(this.numYellowThreshold);
            this.Controls.Add(lblGreenThreshold);
            this.Controls.Add(this.numGreenThreshold);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);

            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRedThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYellowThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGreenThreshold)).EndInit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
