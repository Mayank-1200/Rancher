using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rancher.Database;

namespace Rancher
{
    public partial class MainInventoryForm : Form
    {
        public MainInventoryForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Resize += MainInventoryForm_Resize;
            this.inventoryGrid.MouseClick += InventoryGrid_MouseClick;
            LoadInventoryData(); // Load data on form load
        }

        // **Load Inventory Data Async**
        private async void LoadInventoryData()
        {
            try
            {
                var inventoryData = await NeonDbService.GetInventoryItems();
                inventoryGrid.Rows.Clear();

                foreach (var item in inventoryData)
                {
                    string green = item.Quantity > 100 ? item.Quantity.ToString() : "";
                    string yellow = (item.Quantity <= 100 && item.Quantity >= 50) ? item.Quantity.ToString() : "";
                    string red = (item.Quantity < 50) ? item.Quantity.ToString() : "";

                    inventoryGrid.Rows.Add(
                        item.ItemNumber,
                        item.ProductName,
                        green,
                        yellow,
                        red,
                        item.Supplier
                    );
                }

                UpdateRowColors();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inventory: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // **Grid Mouse Click (Clear selection)**
        private void InventoryGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DataGridView.HitTestInfo hitTestInfo = inventoryGrid.HitTest(e.X, e.Y);
                if (hitTestInfo.RowIndex == -1)
                {
                    inventoryGrid.ClearSelection();
                }
            }
        }

        // **Resize Grid Dynamically**
        private void MainInventoryForm_Resize(object sender, EventArgs e)
        {
            inventoryGrid.Width = this.ClientSize.Width;
            inventoryGrid.Height = this.ClientSize.Height - addButton.Height;
        }

        // **Add New Item**
        private void AddButton_Click(object sender, EventArgs e)
        {
            AddItemForm addItemForm = new AddItemForm(inventoryGrid);
            addItemForm.ShowDialog();
            LoadInventoryData(); // Reload inventory after adding an item
        }

        // **Modify Item**
        private async void ModifyItem(object sender, EventArgs e)
        {
            if (inventoryGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = inventoryGrid.SelectedRows[0];
                AddItemForm modifyForm = new AddItemForm(inventoryGrid, selectedRow);
                modifyForm.ShowDialog();

                // After the form is closed, update the row in the grid with the new data
                if (modifyForm.DialogResult == DialogResult.OK) // Check if changes were saved
                {
                    string itemNumber = selectedRow.Cells["ItemNumber"].Value.ToString();
                    var updatedItem = await NeonDbService.GetInventoryItemByItemNumber(itemNumber); // Fetch updated item from the DB

                    // Update the grid with the new values
                    selectedRow.Cells["ProductName"].Value = updatedItem.ProductName;
                    selectedRow.Cells["Green"].Value = updatedItem.Quantity > 100 ? updatedItem.Quantity.ToString() : "";
                    selectedRow.Cells["Yellow"].Value = (updatedItem.Quantity <= 100 && updatedItem.Quantity >= 50) ? updatedItem.Quantity.ToString() : "";
                    selectedRow.Cells["Red"].Value = updatedItem.Quantity < 50 ? updatedItem.Quantity.ToString() : "";
                    selectedRow.Cells["Supplier"].Value = updatedItem.Supplier;

                    UpdateRowColors(); // Update row colors based on the new quantity
                }
            }
        }

        // **Delete Item Async**
        private async void DeleteItem(object sender, EventArgs e)
        {
            if (inventoryGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = inventoryGrid.SelectedRows[0];
                string itemNumber = selectedRow.Cells["ItemNumber"].Value.ToString();

                try
                {
                    await NeonDbService.DeleteInventoryItem(itemNumber); // Async deletion
                    inventoryGrid.Rows.Remove(selectedRow);
                    UpdateRowColors();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // **Grid Cell Formatting (Coloring based on Quantity)**
        private void inventoryGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = inventoryGrid.Rows[e.RowIndex];

                int greenQty = row.Cells["Green"].Value != null && int.TryParse(row.Cells["Green"].Value.ToString(), out int g) ? g : 0;
                int yellowQty = row.Cells["Yellow"].Value != null && int.TryParse(row.Cells["Yellow"].Value.ToString(), out int y) ? y : 0;
                int redQty = row.Cells["Red"].Value != null && int.TryParse(row.Cells["Red"].Value.ToString(), out int r) ? r : 0;

                e.CellStyle.BackColor = SystemColors.Window;

                if (e.ColumnIndex == row.Cells["Green"].ColumnIndex && greenQty > 0)
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                }
                else if (e.ColumnIndex == row.Cells["Yellow"].ColumnIndex && yellowQty > 0)
                {
                    e.CellStyle.BackColor = Color.Yellow;
                }
                else if (e.ColumnIndex == row.Cells["Red"].ColumnIndex && redQty > 0)
                {
                    e.CellStyle.BackColor = Color.Red;
                }
            }
        }

        // **Update Row Colors Dynamically**
        private void UpdateRowColors()
        {
            foreach (DataGridViewRow row in inventoryGrid.Rows)
            {
                foreach (string columnName in new string[] { "Green", "Yellow", "Red" })
                {
                    if (row.Cells[columnName].Value != null)
                    {
                        int quantity;
                        if (int.TryParse(row.Cells[columnName].Value.ToString(), out quantity))
                        {
                            if (columnName == "Green")
                                row.Cells[columnName].Style.BackColor = Color.LightGreen;
                            else if (columnName == "Yellow")
                                row.Cells[columnName].Style.BackColor = Color.Yellow;
                            else if (columnName == "Red")
                                row.Cells[columnName].Style.BackColor = Color.LightCoral;
                        }
                    }
                }
            }
        }
    }
}
