using System;
using System.Windows.Forms;
using Rancher.Database;

namespace Rancher
{
    public partial class AddItemForm : Form
    {
        private DataGridView inventoryGrid;
        private DataGridViewRow? selectedRow;

        // Constructor for Adding a New Item
        public AddItemForm(DataGridView grid)
        {
            InitializeComponent();
            inventoryGrid = grid;
        }

        // Constructor for Modifying an Existing Item
        public AddItemForm(DataGridView grid, DataGridViewRow row)
        {
            InitializeComponent();
            inventoryGrid = grid;
            selectedRow = row;

            // Populate existing data
            txtItemNumber.Text = row.Cells["ItemNumber"].Value.ToString();
            txtItemNumber.Enabled = false; // Prevent changing item number
            txtProductName.Text = row.Cells["ProductName"].Value.ToString();
            txtSupplier.Text = row.Cells["Supplier"].Value.ToString();

            if (!string.IsNullOrEmpty(row.Cells["Green"].Value?.ToString()))
                numQuantity.Value = Convert.ToInt32(row.Cells["Green"].Value);
            else if (!string.IsNullOrEmpty(row.Cells["Yellow"].Value?.ToString()))
                numQuantity.Value = Convert.ToInt32(row.Cells["Yellow"].Value);
            else if (!string.IsNullOrEmpty(row.Cells["Red"].Value?.ToString()))
                numQuantity.Value = Convert.ToInt32(row.Cells["Red"].Value);
        }

        private async void SaveItem(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtItemNumber.Text) ||
                string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtSupplier.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string itemNumber = txtItemNumber.Text;
            string productName = txtProductName.Text;
            string supplier = txtSupplier.Text;
            int quantity = (int)numQuantity.Value;

            // Determine the stock category
            string green = quantity > 100 ? quantity.ToString() : "";
            string yellow = (quantity <= 100 && quantity >= 50) ? quantity.ToString() : "";
            string red = (quantity < 50) ? quantity.ToString() : "";

            if (selectedRow != null) // Modify Existing Item
            {
                // Update the item in the database
                await NeonDbService.UpdateInventoryItem(itemNumber, productName, quantity, supplier);

                // Update UI Table
                int rowIndex = selectedRow.Index; // Get the index of the selected row
                inventoryGrid.Rows[rowIndex].Cells["ProductName"].Value = productName;
                inventoryGrid.Rows[rowIndex].Cells["Green"].Value = green;
                inventoryGrid.Rows[rowIndex].Cells["Yellow"].Value = yellow;
                inventoryGrid.Rows[rowIndex].Cells["Red"].Value = red;
                inventoryGrid.Rows[rowIndex].Cells["Supplier"].Value = supplier;

                this.DialogResult = DialogResult.OK; // Indicate that the item was modified
            }
            else // Add New Item
            {
                // Check for duplicate item number
                foreach (DataGridViewRow row in inventoryGrid.Rows)
                {
                    if (row.Cells["ItemNumber"].Value?.ToString() == itemNumber)
                    {
                        MessageBox.Show("Item number already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Insert into database
                await NeonDbService.AddInventoryItem(itemNumber, productName, quantity, supplier);

                // Add to UI Table
                inventoryGrid.Rows.Add(itemNumber, productName, green, yellow, red, supplier);
                inventoryGrid.ClearSelection();

                this.DialogResult = DialogResult.OK; // Indicate that a new item was added
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
