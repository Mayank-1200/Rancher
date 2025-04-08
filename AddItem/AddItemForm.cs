using System;
using System.Windows.Forms;
using Rancher.Database;

namespace Rancher
{
    public partial class AddItemForm : Form
    {
        private DataGridView inventoryGrid;
        private DataGridViewRow? selectedRow;
        private int actualQuantity = 0;
        private int supplierId = 0;

        public AddItemForm(DataGridView grid)
        {
            InitializeComponent();
            inventoryGrid = grid;
        }

        public AddItemForm(DataGridView grid, DataGridViewRow row)
        {
            InitializeComponent();
            inventoryGrid = grid;
            selectedRow = row;

            txtItemNumber.Text = row.Cells["ItemNumber"].Value?.ToString() ?? "";
            txtItemNumber.Enabled = false;
            txtProductName.Text = row.Cells["ProductName"].Value?.ToString() ?? "";
            txtSupplierName.Text = row.Cells["Supplier"].Value?.ToString() ?? "";

            int redThreshold = 10, yellowThreshold = 30, greenThreshold = 31;

            if (row.DataGridView.Columns.Contains("RedThreshold"))
                int.TryParse(row.Cells["RedThreshold"].Value?.ToString(), out redThreshold);
            if (row.DataGridView.Columns.Contains("YellowThreshold"))
                int.TryParse(row.Cells["YellowThreshold"].Value?.ToString(), out yellowThreshold);
            if (row.DataGridView.Columns.Contains("GreenThreshold"))
                int.TryParse(row.Cells["GreenThreshold"].Value?.ToString(), out greenThreshold);

            int.TryParse(row.Cells["ActualQuantity"].Value?.ToString(), out actualQuantity);

            int quantity = 0;
            if (int.TryParse(row.Cells["Green"].Value?.ToString(), out int greenQty)) quantity = greenQty;
            else if (int.TryParse(row.Cells["Yellow"].Value?.ToString(), out int yellowQty)) quantity = yellowQty;
            else if (int.TryParse(row.Cells["Red"].Value?.ToString(), out int redQty)) quantity = redQty;

            numQuantity.Value = quantity;
            numRedThreshold.Value = redThreshold;
            numYellowThreshold.Value = yellowThreshold;
            numGreenThreshold.Value = greenThreshold;

            _ = LoadSupplierDetailsAsync(txtSupplierName.Text);
        }

        private async Task LoadSupplierDetailsAsync(string supplierName)
        {
            try
            {
                var supplier = await NeonDbService.GetSupplierByName(supplierName);
                if (supplier != null)
                {
                    txtSupplierEmail.Text = supplier["Email"];
                    txtSupplierPhone.Text = supplier["Phone"];
                    txtSupplierNote.Text = supplier["Note"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load supplier info: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SaveItem(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtItemNumber.Text) ||
                string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtSupplierName.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string itemNumber = txtItemNumber.Text.Trim();
            string productName = txtProductName.Text.Trim();
            string supplierName = txtSupplierName.Text.Trim();
            string supplierEmail = txtSupplierEmail.Text.Trim();
            string supplierPhone = txtSupplierPhone.Text.Trim();
            string supplierNote = txtSupplierNote.Text.Trim();

            int quantity = (int)numQuantity.Value;
            int redThreshold = (int)numRedThreshold.Value;
            int yellowThreshold = (int)numYellowThreshold.Value;
            int greenThreshold = (int)numGreenThreshold.Value;

            string green = (quantity > yellowThreshold) ? quantity.ToString() : "";
            string yellow = (quantity <= yellowThreshold && quantity > redThreshold) ? quantity.ToString() : "";
            string red = (quantity <= redThreshold) ? quantity.ToString() : "";

            try
            {
                int supplierId = await NeonDbService.GetOrInsertSupplierId(supplierName, supplierEmail, supplierPhone, supplierNote);

                if (selectedRow != null)
                {
                    // Also update supplier info
                    await NeonDbService.UpdateSupplier(supplierId, supplierName, supplierEmail, supplierPhone, supplierNote);

                    await NeonDbService.UpdateInventoryItem(itemNumber, productName, quantity, supplierId, redThreshold, yellowThreshold, greenThreshold);

                    int rowIndex = selectedRow.Index;
                    inventoryGrid.Rows[rowIndex].Cells["ProductName"].Value = productName;
                    inventoryGrid.Rows[rowIndex].Cells["Green"].Value = green;
                    inventoryGrid.Rows[rowIndex].Cells["Yellow"].Value = yellow;
                    inventoryGrid.Rows[rowIndex].Cells["Red"].Value = red;
                    inventoryGrid.Rows[rowIndex].Cells["Supplier"].Value = supplierName;
                    inventoryGrid.Rows[rowIndex].Cells["RedThreshold"].Value = redThreshold;
                    inventoryGrid.Rows[rowIndex].Cells["YellowThreshold"].Value = yellowThreshold;
                    inventoryGrid.Rows[rowIndex].Cells["GreenThreshold"].Value = greenThreshold;

                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    foreach (DataGridViewRow row in inventoryGrid.Rows)
                    {
                        if (row.Cells["ItemNumber"].Value?.ToString() == itemNumber)
                        {
                            MessageBox.Show("Item number already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    await NeonDbService.AddInventoryItem(itemNumber, productName, quantity, supplierId, redThreshold, yellowThreshold, greenThreshold);

                    inventoryGrid.Rows.Add(itemNumber, productName, actualQuantity.ToString(), green, yellow, red, supplierName, redThreshold, yellowThreshold, greenThreshold);
                    inventoryGrid.ClearSelection();

                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
