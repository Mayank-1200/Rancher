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
        private TextBox txtProductSearch;
        private ListBox lstProductResults;
        private TextBox txtNote;
        private ComboBox supplierComboBox;
        private NumericUpDown numQuantity;
        private Button btnSubmit, btnCancel;

        private Dictionary<string, string> productDisplayToItemNumber = new();
        private string selectedItemNumber = "";

        public AddQuantityForm()
        {
            InitializeComponent();
            LoadSuppliers();
        }

        private void InitializeComponent()
        {
            this.Text = "Add / Subtract Quantity";
            this.Size = new Size(540, 480);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int labelX = 20;
            int controlX = 180;
            int controlWidth = 300;

            Label lblType = new Label() { Text = "Entry Type:", AutoSize = true, Location = new Point(labelX, 20) };
            entryTypeCombo = new ComboBox()
            {
                Location = new Point(controlX, 18),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            entryTypeCombo.Items.AddRange(new string[] { "Inward", "Outward" });
            entryTypeCombo.SelectedIndex = 0;

            Label lblProduct = new Label() { Text = "Part (Name - Number):", AutoSize = true, Location = new Point(labelX, 60) };
            txtProductSearch = new TextBox()
            {
                Location = new Point(controlX, 58),
                Width = controlWidth
            };

            lstProductResults = new ListBox()
            {
                Location = new Point(controlX, 82),
                Width = controlWidth,
                Height = 100,
                Visible = false
            };

            void HandleProductSelection()
            {
                var selected = lstProductResults.SelectedItem?.ToString();
                if (!string.IsNullOrWhiteSpace(selected) && productDisplayToItemNumber.ContainsKey(selected))
                {
                    txtProductSearch.Text = selected;
                    selectedItemNumber = productDisplayToItemNumber[selected];
                    lstProductResults.Visible = false;
                }
                else
                {
                    selectedItemNumber = "";
                    lstProductResults.Visible = false;
                }
            }

            lstProductResults.Click += (s, e) => HandleProductSelection();
            lstProductResults.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    HandleProductSelection();
                    txtProductSearch.Focus();
                    e.Handled = true;
                }
            };

            txtProductSearch.TextChanged += async (s, e) =>
            {
                string query = txtProductSearch.Text.Trim();
                selectedItemNumber = ""; // Reset when typing
                await UpdateProductSearch(query);
            };

            txtProductSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Down && lstProductResults.Visible && lstProductResults.Items.Count > 0)
                {
                    lstProductResults.Focus();
                    lstProductResults.SelectedIndex = 0;
                    e.Handled = true;
                }
            };

            Label lblQuantity = new Label() { Text = "Quantity:", AutoSize = true, Location = new Point(labelX, 200) };
            numQuantity = new NumericUpDown()
            {
                Location = new Point(controlX, 198),
                Width = controlWidth,
                Minimum = 1,
                Maximum = 100000
            };

            Label lblSupplierName = new Label() { Text = "Supplier Name:", AutoSize = true, Location = new Point(labelX, 240) };
            supplierComboBox = new ComboBox()
            {
                Location = new Point(controlX, 238),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };

            Label lblNote = new Label() { Text = "Note:", AutoSize = true, Location = new Point(labelX, 280) };
            txtNote = new TextBox()
            {
                Location = new Point(controlX, 278),
                Width = controlWidth,
                Height = 60,
                Multiline = true
            };

            btnSubmit = new Button()
            {
                Text = "Submit",
                Location = new Point(140, 360),
                Width = 100,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSubmit.Click += BtnSubmit_Click;

            btnCancel = new Button()
            {
                Text = "Cancel",
                Location = new Point(260, 360),
                Width = 100
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[]
            {
                lblType, entryTypeCombo,
                lblProduct, txtProductSearch, lstProductResults,
                lblQuantity, numQuantity,
                lblSupplierName, supplierComboBox,
                lblNote, txtNote,
                btnSubmit, btnCancel
            });
        }

        private async Task UpdateProductSearch(string input)
        {
            if (input.Length < 2)
            {
                lstProductResults.Visible = false;
                return;
            }

            try
            {
                var results = await NeonDbService.SearchInventoryByProductName(input);
                lstProductResults.Items.Clear();
                productDisplayToItemNumber.Clear();

                foreach (var item in results)
                {
                    // Combined display format remains unchanged
                    string display = $"{item["ProductName"]} - {item["ItemNumber"]}";
                    if (!productDisplayToItemNumber.ContainsKey(display))
                    {
                        productDisplayToItemNumber[display] = item["ItemNumber"];
                        lstProductResults.Items.Add(display);
                    }
                }

                lstProductResults.Visible = lstProductResults.Items.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search error: " + ex.Message);
            }
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
            string type = entryTypeCombo.SelectedItem?.ToString()?.ToLower() ?? "inward";
            int quantity = (int)numQuantity.Value;
            string supplierName = supplierComboBox.Text.Trim();
            string note = txtNote.Text.Trim();

            if (string.IsNullOrWhiteSpace(selectedItemNumber))
            {
                MessageBox.Show("Please select a valid part from the list.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(supplierName))
            {
                MessageBox.Show("Supplier name cannot be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                    await NeonDbService.AddToInventoryQuantity(selectedItemNumber, quantity);
                else
                    await NeonDbService.SubtractFromInventory(selectedItemNumber, quantity);

                await NeonDbService.InsertEntryLog(selectedItemNumber, quantity, type, supplierName, note);

                MessageBox.Show($"{(type == "inward" ? "Added" : "Subtracted")} {quantity} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                selectedItemNumber = ""; // Reset
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
