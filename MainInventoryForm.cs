using System;
using System.Collections.Generic;
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

            ApplyUIEnhancements();
            LoadInventoryData();
        }

        // **Apply UI Enhancements**
        private void ApplyUIEnhancements()
        {
            // Form Background
            this.BackColor = Color.FromArgb(240, 240, 240); // Soft light gray

            // DataGridView overall style
            inventoryGrid.BorderStyle = BorderStyle.FixedSingle;
            inventoryGrid.BackgroundColor = Color.Gray;
            inventoryGrid.EnableHeadersVisualStyles = false;
            
            // Let columns auto-size to fill the grid
            inventoryGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Header styling
            inventoryGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200);
            inventoryGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            inventoryGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            inventoryGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            inventoryGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            inventoryGrid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True; // Allow header text to wrap
            inventoryGrid.ColumnHeadersHeight = 40;

            // Grid lines / border styling
            inventoryGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            inventoryGrid.GridColor = Color.FromArgb(180, 180, 180);

            // Default cell style
            inventoryGrid.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            inventoryGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            inventoryGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            inventoryGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Row settings
            inventoryGrid.RowTemplate.Height = 35;
            inventoryGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);

            // Optionally, enable auto-sizing rows (if needed for wrapped text)
            //inventoryGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        // **Load Inventory Data**
        private async void LoadInventoryData()
        {
            try
            {
                var inventoryData = await NeonDbService.GetInventoryItems();
                inventoryGrid.Rows.Clear();

                foreach (var item in inventoryData)
                {
                    int redThreshold = item.ContainsKey("RedThreshold") ? Convert.ToInt32(item["RedThreshold"]) : 10;
                    int yellowThreshold = item.ContainsKey("YellowThreshold") ? Convert.ToInt32(item["YellowThreshold"]) : 30;
                    int greenThreshold = item.ContainsKey("GreenThreshold") ? Convert.ToInt32(item["GreenThreshold"]) : 31;
                    int quantity = item.ContainsKey("Quantity") ? Convert.ToInt32(item["Quantity"]) : 0;

                    string green = (quantity > yellowThreshold) ? quantity.ToString() : "";
                    string yellow = (quantity <= yellowThreshold && quantity > redThreshold) ? quantity.ToString() : "";
                    string red = (quantity <= redThreshold) ? quantity.ToString() : "";

                    inventoryGrid.Rows.Add(
                        item.ContainsKey("ItemNumber") ? item["ItemNumber"].ToString() : "N/A",
                        item.ContainsKey("ProductName") ? item["ProductName"].ToString() : "Unknown",
                        green,
                        yellow,
                        red,
                        item.ContainsKey("Supplier") ? item["Supplier"].ToString() : "Unknown",
                        redThreshold,
                        yellowThreshold,
                        greenThreshold
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
            inventoryGrid.Width = this.ClientSize.Width - 20;
            inventoryGrid.Height = this.ClientSize.Height - addButton.Height - 20;
        }

        // **Add New Item**
        private void AddButton_Click(object sender, EventArgs e)
        {
            AddItemForm addItemForm = new AddItemForm(inventoryGrid);
            addItemForm.ShowDialog();
            LoadInventoryData();
        }

        // **Modify Item**
        private async void ModifyItem(object sender, EventArgs e)
        {
            if (inventoryGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = inventoryGrid.SelectedRows[0];
                AddItemForm modifyForm = new AddItemForm(inventoryGrid, selectedRow);
                modifyForm.ShowDialog();

                if (modifyForm.DialogResult == DialogResult.OK)
                {
                    string itemNumber = selectedRow.Cells["ItemNumber"].Value?.ToString();
                    if (!string.IsNullOrEmpty(itemNumber))
                    {
                        var updatedItem = await NeonDbService.GetInventoryItemByItemNumber(itemNumber);
                        if (updatedItem != null)
                        {
                            int redThreshold = Convert.ToInt32(updatedItem["RedThreshold"]);
                            int yellowThreshold = Convert.ToInt32(updatedItem["YellowThreshold"]);
                            int greenThreshold = Convert.ToInt32(updatedItem["GreenThreshold"]);
                            int quantity = Convert.ToInt32(updatedItem["Quantity"]);

                            selectedRow.Cells["ProductName"].Value = updatedItem["ProductName"];
                            selectedRow.Cells["Green"].Value = (quantity > yellowThreshold) ? quantity.ToString() : "";
                            selectedRow.Cells["Yellow"].Value = (quantity <= yellowThreshold && quantity > redThreshold) ? quantity.ToString() : "";
                            selectedRow.Cells["Red"].Value = (quantity <= redThreshold) ? quantity.ToString() : "";
                            selectedRow.Cells["Supplier"].Value = updatedItem["Supplier"];

                            if (inventoryGrid.Columns.Contains("RedThreshold"))
                                selectedRow.Cells["RedThreshold"].Value = redThreshold;
                            if (inventoryGrid.Columns.Contains("YellowThreshold"))
                                selectedRow.Cells["YellowThreshold"].Value = yellowThreshold;
                            if (inventoryGrid.Columns.Contains("GreenThreshold"))
                                selectedRow.Cells["GreenThreshold"].Value = greenThreshold;

                            UpdateRowColors();
                        }
                    }
                }
            }
        }

        // **Delete Item**
        private async void DeleteItem(object sender, EventArgs e)
        {
            if (inventoryGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = inventoryGrid.SelectedRows[0];
                string itemNumber = selectedRow.Cells["ItemNumber"].Value?.ToString();

                if (!string.IsNullOrEmpty(itemNumber))
                {
                    try
                    {
                        await NeonDbService.DeleteInventoryItem(itemNumber);
                        inventoryGrid.Rows.Remove(selectedRow);
                        UpdateRowColors();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // **Grid Cell Formatting (Coloring based on Quantity)**
        private void inventoryGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = inventoryGrid.Rows[e.RowIndex];

                int greenQty = int.TryParse(row.Cells["Green"].Value?.ToString(), out int g) ? g : 0;
                int yellowQty = int.TryParse(row.Cells["Yellow"].Value?.ToString(), out int y) ? y : 0;
                int redQty = int.TryParse(row.Cells["Red"].Value?.ToString(), out int r) ? r : 0;

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
                    e.CellStyle.BackColor = Color.LightCoral;
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
                        if (int.TryParse(row.Cells[columnName].Value.ToString(), out int qty))
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