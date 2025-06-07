using System;
using System.Windows.Forms;

namespace Rancher
{
    partial class AddItemForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtItemNumber;
        private TextBox txtProductName;
        private TextBox txtSupplierName;
        public TextBox txtSupplierEmail;
        public TextBox txtSupplierPhone;
        public TextBox txtSupplierNote;
        private NumericUpDown numQuantity;
        private NumericUpDown numRedThreshold;
        private NumericUpDown numYellowThreshold;
        private NumericUpDown numGreenThreshold;
        private Button btnSave;
        private Button btnCancel;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtItemNumber = new TextBox();
            this.txtProductName = new TextBox();
            this.txtSupplierName = new TextBox();
            this.txtSupplierEmail = new TextBox();
            this.txtSupplierPhone = new TextBox();
            this.txtSupplierNote = new TextBox();
            this.numQuantity = new NumericUpDown();
            this.numRedThreshold = new NumericUpDown();
            this.numYellowThreshold = new NumericUpDown();
            this.numGreenThreshold = new NumericUpDown();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRedThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYellowThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGreenThreshold)).BeginInit();

            // Form Properties
            this.Text = "Add Item";
            this.ClientSize = new System.Drawing.Size(480, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            int labelWidth = 140;
            int inputLeft = 170;
            int inputWidth = 270;
            int topStart = 20;
            int gap = 35;

            // Labels
            Label lblItemNumber = new Label() { Text = "Item Number:", Left = 20, Top = topStart + 0 * gap, Width = labelWidth };
            Label lblProductName = new Label() { Text = "Product Name:", Left = 20, Top = topStart + 1 * gap, Width = labelWidth };
            Label lblQuantity = new Label() { Text = "Quantity:", Left = 20, Top = topStart + 2 * gap, Width = labelWidth };
            Label lblSupplierName = new Label() { Text = "Supplier Name:", Left = 20, Top = topStart + 3 * gap, Width = labelWidth };
            Label lblSupplierEmail = new Label() { Text = "Supplier Email:", Left = 20, Top = topStart + 4 * gap, Width = labelWidth };
            Label lblSupplierPhone = new Label() { Text = "Supplier Phone:", Left = 20, Top = topStart + 5 * gap, Width = labelWidth };
            Label lblSupplierNote = new Label() { Text = "Supplier Note:", Left = 20, Top = topStart + 6 * gap, Width = labelWidth };
            Label lblRedThreshold = new Label() { Text = "Red Threshold:", Left = 20, Top = topStart + 7 * gap, Width = labelWidth };
            Label lblYellowThreshold = new Label() { Text = "Yellow Threshold:", Left = 20, Top = topStart + 8 * gap, Width = labelWidth };
            Label lblGreenThreshold = new Label() { Text = "Green Threshold:", Left = 20, Top = topStart + 9 * gap, Width = labelWidth };

            // TextBoxes & Inputs
            this.txtItemNumber.SetBounds(inputLeft, topStart + 0 * gap, inputWidth, 25);
            this.txtProductName.SetBounds(inputLeft, topStart + 1 * gap, inputWidth, 25);
            this.numQuantity.SetBounds(inputLeft, topStart + 2 * gap, inputWidth, 25);
            this.numQuantity.Minimum = 0;
            this.numQuantity.Maximum = 100000;

            this.txtSupplierName.SetBounds(inputLeft, topStart + 3 * gap, inputWidth, 25);
            this.txtSupplierEmail.SetBounds(inputLeft, topStart + 4 * gap, inputWidth, 25);
            this.txtSupplierPhone.SetBounds(inputLeft, topStart + 5 * gap, inputWidth, 25);
            this.txtSupplierNote.SetBounds(inputLeft, topStart + 6 * gap, inputWidth, 25);

            this.numRedThreshold.SetBounds(inputLeft, topStart + 7 * gap, inputWidth, 25);
            this.numRedThreshold.Minimum = 0;
            this.numRedThreshold.Maximum = 100000;

            this.numYellowThreshold.SetBounds(inputLeft, topStart + 8 * gap, inputWidth, 25);
            this.numYellowThreshold.Minimum = 0;
            this.numYellowThreshold.Maximum = 100000;

            this.numGreenThreshold.SetBounds(inputLeft, topStart + 9 * gap, inputWidth, 25);
            this.numGreenThreshold.Minimum = 0;
            this.numGreenThreshold.Maximum = 100000;

            // Buttons
            this.btnSave.Text = "Save";
            this.btnSave.SetBounds(100, topStart + 11 * gap, 100, 35);
            this.btnSave.Click += new EventHandler(this.SaveItem);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.SetBounds(250, topStart + 11 * gap, 100, 35);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Add Controls
            this.Controls.AddRange(new Control[]
            {
                lblItemNumber, txtItemNumber,
                lblProductName, txtProductName,
                lblQuantity, numQuantity,
                lblSupplierName, txtSupplierName,
                lblSupplierEmail, txtSupplierEmail,
                lblSupplierPhone, txtSupplierPhone,
                lblSupplierNote, txtSupplierNote,
                lblRedThreshold, numRedThreshold,
                lblYellowThreshold, numYellowThreshold,
                lblGreenThreshold, numGreenThreshold,
                btnSave, btnCancel
            });

            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRedThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYellowThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGreenThreshold)).EndInit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }
    }
}
