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
            txtItemNumber.Text = row.Cells["ItemNumber"].Value?.ToString() ?? "";
            txtItemNumber.Enabled = false; // Prevent changing item number
            txtProductName.Text = row.Cells["ProductName"].Value?.ToString() ?? "";
            txtSupplier.Text = row.Cells["Supplier"].Value?.ToString() ?? "";

            // Set default threshold values
            int redThreshold = 10, yellowThreshold = 30, greenThreshold = 31;

            // Only try to read threshold values if the columns exist in the DataGridView
            if (row.DataGridView.Columns.Contains("RedThreshold"))
            {
                int.TryParse(row.Cells["RedThreshold"].Value?.ToString(), out redThreshold);
            }
            if (row.DataGridView.Columns.Contains("YellowThreshold"))
            {
                int.TryParse(row.Cells["YellowThreshold"].Value?.ToString(), out yellowThreshold);
            }
            if (row.DataGridView.Columns.Contains("GreenThreshold"))
            {
                int.TryParse(row.Cells["GreenThreshold"].Value?.ToString(), out greenThreshold);
            }

            // Retrieve quantity from one of the color columns
            int quantity = 0;
            if (int.TryParse(row.Cells["Green"].Value?.ToString(), out int greenQty))
                quantity = greenQty;
            else if (int.TryParse(row.Cells["Yellow"].Value?.ToString(), out int yellowQty))
                quantity = yellowQty;
            else if (int.TryParse(row.Cells["Red"].Value?.ToString(), out int redQty))
                quantity = redQty;

            numQuantity.Value = quantity;
            numRedThreshold.Value = redThreshold;
            numYellowThreshold.Value = yellowThreshold;
            numGreenThreshold.Value = greenThreshold;
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
            int redThreshold = (int)numRedThreshold.Value;
            int yellowThreshold = (int)numYellowThreshold.Value;
            int greenThreshold = (int)numGreenThreshold.Value;

            // Determine which column should hold the quantity based on threshold
            string green = (quantity > yellowThreshold) ? quantity.ToString() : "";
            string yellow = (quantity <= yellowThreshold && quantity > redThreshold) ? quantity.ToString() : "";
            string red = (quantity <= redThreshold) ? quantity.ToString() : "";

            if (selectedRow != null) // Modify Existing Item
            {
                // Update the item in the database
                await NeonDbService.UpdateInventoryItem(itemNumber, productName, quantity, supplier, redThreshold, yellowThreshold, greenThreshold);

                // Update UI Table
                int rowIndex = selectedRow.Index;
                inventoryGrid.Rows[rowIndex].Cells["ProductName"].Value = productName;
                inventoryGrid.Rows[rowIndex].Cells["Green"].Value = green;
                inventoryGrid.Rows[rowIndex].Cells["Yellow"].Value = yellow;
                inventoryGrid.Rows[rowIndex].Cells["Red"].Value = red;
                inventoryGrid.Rows[rowIndex].Cells["Supplier"].Value = supplier;
                
                // Update threshold columns only if they exist
                if (inventoryGrid.Columns.Contains("RedThreshold"))
                    inventoryGrid.Rows[rowIndex].Cells["RedThreshold"].Value = redThreshold;
                if (inventoryGrid.Columns.Contains("YellowThreshold"))
                    inventoryGrid.Rows[rowIndex].Cells["YellowThreshold"].Value = yellowThreshold;
                if (inventoryGrid.Columns.Contains("GreenThreshold"))
                    inventoryGrid.Rows[rowIndex].Cells["GreenThreshold"].Value = greenThreshold;

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
                await NeonDbService.AddInventoryItem(itemNumber, productName, quantity, supplier, redThreshold, yellowThreshold, greenThreshold);

                // Add to UI Table (assumes grid has columns for thresholds)
                inventoryGrid.Rows.Add(itemNumber, productName, green, yellow, red, supplier, redThreshold, yellowThreshold, greenThreshold);
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
