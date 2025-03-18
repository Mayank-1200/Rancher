using System;
using System.Windows.Forms;

namespace Rancher
{
    public partial class AddItemForm : Form
    {
        private DataGridView inventoryGrid;
        private DataGridViewRow selectedRow;

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
            txtSupplier.Text = row.Cells["Supplier"].Value.ToString(); // Changed from dropdown to text box

            if (!string.IsNullOrEmpty(row.Cells["Green"].Value?.ToString()))
                numQuantity.Value = Convert.ToInt32(row.Cells["Green"].Value);
            else if (!string.IsNullOrEmpty(row.Cells["Yellow"].Value?.ToString()))
                numQuantity.Value = Convert.ToInt32(row.Cells["Yellow"].Value);
            else if (!string.IsNullOrEmpty(row.Cells["Red"].Value?.ToString()))
                numQuantity.Value = Convert.ToInt32(row.Cells["Red"].Value);
        }

        private void SaveItem(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtItemNumber.Text) ||
                string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtSupplier.Text)) // Changed from dropdown check
            {
                MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int quantity = (int)numQuantity.Value;

            // Ensure only one quantity field is populated
            string green = quantity > 100 ? quantity.ToString() : "";
            string yellow = (quantity <= 100 && quantity >= 50) ? quantity.ToString() : "";
            string red = (quantity < 50) ? quantity.ToString() : "";

            if (selectedRow != null) // Modify Existing Item
            {
                selectedRow.Cells["ProductName"].Value = txtProductName.Text;
                selectedRow.Cells["Green"].Value = green;
                selectedRow.Cells["Yellow"].Value = yellow;
                selectedRow.Cells["Red"].Value = red;
                selectedRow.Cells["Supplier"].Value = txtSupplier.Text; // Changed from dropdown to text box
            }
            else // Add New Item
            {
                // Check for duplicate item numbers
                foreach (DataGridViewRow row in inventoryGrid.Rows)
                {
                    if (row.Cells["ItemNumber"].Value?.ToString() == txtItemNumber.Text)
                    {
                        MessageBox.Show("Item number already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                inventoryGrid.Rows.Add(
                    txtItemNumber.Text,
                    txtProductName.Text,
                    green,
                    yellow,
                    red,
                    txtSupplier.Text // Changed from dropdown to text box
                );
                inventoryGrid.ClearSelection();
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
