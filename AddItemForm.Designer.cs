using System;
using System.Windows.Forms;

namespace Rancher
{
    partial class AddItemForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtItemNumber;
        private TextBox txtProductName;
        private TextBox txtSupplier; // Changed from ComboBox to TextBox
        private NumericUpDown numQuantity;
        private Button btnSave;
        private Button btnCancel;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtItemNumber = new System.Windows.Forms.TextBox();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.txtSupplier = new System.Windows.Forms.TextBox(); // Supplier input field
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            
            // Form Properties
            this.Text = "Add Item";
            this.ClientSize = new System.Drawing.Size(400, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            // Labels
            Label lblItemNumber = new Label() { Text = "Item Number:", Left = 20, Top = 20, Width = 100 };
            Label lblProductName = new Label() { Text = "Product Name:", Left = 20, Top = 60, Width = 100 };
            Label lblQuantity = new Label() { Text = "Quantity:", Left = 20, Top = 100, Width = 100 };
            Label lblSupplier = new Label() { Text = "Supplier:", Left = 20, Top = 140, Width = 100 };

            // TextBoxes & Inputs
            this.txtItemNumber.Location = new System.Drawing.Point(130, 20);
            this.txtItemNumber.Width = 200;
            
            this.txtProductName.Location = new System.Drawing.Point(130, 60);
            this.txtProductName.Width = 200;
            
            this.numQuantity.Location = new System.Drawing.Point(130, 100);
            this.numQuantity.Width = 200;
            this.numQuantity.Minimum = 0;
            this.numQuantity.Maximum = 1000;
            this.numQuantity.Value = 0; // Default value removed (left blank)
            this.numQuantity.ResetText(); // Ensures blank value on form load

            this.txtSupplier.Location = new System.Drawing.Point(130, 140);
            this.txtSupplier.Width = 200;

            // Buttons
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(80, 180);
            this.btnSave.Width = 100;
            this.btnSave.Click += new System.EventHandler(this.SaveItem);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(200, 180);
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
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
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
