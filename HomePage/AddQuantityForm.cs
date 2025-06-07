using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rancher.Database;

namespace Rancher
{
    public class AddQuantityForm : Form
    {
        private ComboBox entryTypeCombo;
        private TextBox txtItemNumber, txtNote;
        private ComboBox supplierComboBox;
        private NumericUpDown numQuantity;
        private Button btnSubmit, btnCancel;

        public AddQuantityForm()
        {
            InitializeComponent();
            LoadSuppliers();
        }

        private void InitializeComponent()
        {
            this.Text = "Add / Subtract Quantity";
            this.Size = new Size(400, 380);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblType = new Label() { Text = "Entry Type:", AutoSize = true, Location = new Point(20, 20) };
            entryTypeCombo = new ComboBox()
            {
                Location = new Point(150, 18),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            entryTypeCombo.Items.AddRange(new string[] { "Inward", "Outward" });
            entryTypeCombo.SelectedIndex = 0;

            Label lblItemNumber = new Label() { Text = "Part Number:", AutoSize = true, Location = new Point(20, 60) };
            txtItemNumber = new TextBox() { Location = new Point(150, 58), Width = 200 };

            Label lblQuantity = new Label() { Text = "Quantity:", AutoSize = true, Location = new Point(20, 100) };
            numQuantity = new NumericUpDown()
            {
                Location = new Point(150, 98),
                Width = 200,
                Minimum = 1,
                Maximum = 100000
            };

            Label lblSupplierName = new Label() { Text = "Supplier Name:", AutoSize = true, Location = new Point(20, 140) };
            supplierComboBox = new ComboBox()
            {
                Location = new Point(150, 138),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDown, // Typing allowed
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };

            Label lblNote = new Label() { Text = "Note:", AutoSize = true, Location = new Point(20, 180) };
            txtNote = new TextBox() { Location = new Point(150, 178), Width = 200, Height = 60, Multiline = true };

            btnSubmit = new Button()
            {
                Text = "Submit",
                Location = new Point(80, 280),
                Width = 100,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSubmit.Click += BtnSubmit_Click;

            btnCancel = new Button()
            {
                Text = "Cancel",
                Location = new Point(200, 280),
                Width = 100
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[]
            {
                lblType, entryTypeCombo,
                lblItemNumber, txtItemNumber,
                lblQuantity, numQuantity,
                lblSupplierName, supplierComboBox,
                lblNote, txtNote,
                btnSubmit, btnCancel
            });
        }

        private async void LoadSuppliers()
        {
            try
            {
                var suppliers = await NeonDbService.GetAllSuppliers();
                supplierComboBox.Items.Clear();
                supplierComboBox.Items.AddRange(suppliers.Select(s => s["Name"]).ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load suppliers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            string type = entryTypeCombo.SelectedItem.ToString()?.ToLower() ?? "inward";
            string itemNumber = txtItemNumber.Text.Trim();
            int quantity = (int)numQuantity.Value;
            string supplierName = supplierComboBox.Text.Trim(); // take from text
            string note = txtNote.Text.Trim();

            if (string.IsNullOrWhiteSpace(itemNumber))
            {
                MessageBox.Show("Part number cannot be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(supplierName))
            {
                MessageBox.Show("Supplier name cannot be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🚨 Strict supplier validation (new logic)
            bool supplierExists = supplierComboBox.Items.Cast<string>()
                .Any(item => item.Equals(supplierName, StringComparison.OrdinalIgnoreCase));

            if (!supplierExists)
            {
                MessageBox.Show("Please select a valid supplier from the list.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (type == "inward")
                {
                    await NeonDbService.AddToInventoryQuantity(itemNumber, quantity);
                }
                else if (type == "outward")
                {
                    await NeonDbService.SubtractFromInventory(itemNumber, quantity);
                }

                await NeonDbService.InsertEntryLog(itemNumber, quantity, type, supplierName, note);

                MessageBox.Show($"{(type == "inward" ? "Added" : "Subtracted")} {quantity} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
