using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rancher.Database;

namespace Rancher
{
    public partial class MainInventoryForm : UserControl
    {
        // The designer file (MainInventoryForm.Designer.cs) declares:
        // - DataGridView inventoryGrid;
        // - Button addButton;
        // - ContextMenuStrip contextMenu;

        public MainInventoryForm()
        {
            InitializeComponent(); // Initializes components from the designer
            this.Padding = new Padding(0, 40, 0, 0);
            this.Resize += MainInventoryForm_Resize;
            this.inventoryGrid.MouseClick += InventoryGrid_MouseClick;
            this.inventoryGrid.CellFormatting += inventoryGrid_CellFormatting; // Ensure this event has a handler

            ApplyUIEnhancements();
            LoadInventoryData();
        }

        private void MainInventoryForm_Resize(object? sender, EventArgs e)
        {
            // Optional: additional resizing logic if needed.
        }

        private void InventoryGrid_MouseClick(object? sender, MouseEventArgs e)
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

        // This method is required by the designer.
        private void inventoryGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            // You can add custom cell formatting logic here if needed.
            // For now, this is a stub to satisfy the designer event hookup.
        }

        private void ApplyUIEnhancements()
        {
            // Set the UserControl background.
            this.BackColor = Color.FromArgb(240, 240, 240);

            // DataGridView overall style.
            inventoryGrid.BorderStyle = BorderStyle.FixedSingle;
            inventoryGrid.BackgroundColor = Color.FromArgb(220, 215, 200);
            inventoryGrid.EnableHeadersVisualStyles = false;
            inventoryGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Header styling.
            inventoryGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200);
            inventoryGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            inventoryGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            inventoryGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            inventoryGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            inventoryGrid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            inventoryGrid.ColumnHeadersHeight = 40;

            // Grid lines / border styling.
            inventoryGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            inventoryGrid.GridColor = Color.FromArgb(180, 180, 180);

            // Default cell style.
            inventoryGrid.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            inventoryGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            inventoryGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            inventoryGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Row settings.
            inventoryGrid.RowTemplate.Height = 35;
            inventoryGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
        }

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

                    // Determine quantity display logic.
                    string green = (quantity > yellowThreshold) ? quantity.ToString() : "";
                    string yellow = (quantity <= yellowThreshold && quantity > redThreshold) ? quantity.ToString() : "";
                    string red = (quantity <= redThreshold) ? quantity.ToString() : "";

                    // Add the row with the new Actual Quantity column (fetched from the database).
                    inventoryGrid.Rows.Add(
                        item.ContainsKey("ItemNumber") ? item["ItemNumber"].ToString() : "N/A",
                        item.ContainsKey("ProductName") ? item["ProductName"].ToString() : "Unknown",
                        item.ContainsKey("ActualQuantity") ? item["ActualQuantity"].ToString() : "0",
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

        private void UpdateRowColors()
        {
            foreach (DataGridViewRow row in inventoryGrid.Rows)
            {
                foreach (string columnName in new string[] { "Green", "Yellow", "Red" })
                {
                    if (row.Cells[columnName].Value != null &&
                        int.TryParse(row.Cells[columnName].Value.ToString(), out int qty))
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

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddItemForm addItemForm = new AddItemForm(inventoryGrid);
            addItemForm.ShowDialog();
            LoadInventoryData();
        }

        private void ModifyItem(object sender, EventArgs e)
        {
            if (inventoryGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = inventoryGrid.SelectedRows[0];
                AddItemForm modifyForm = new AddItemForm(inventoryGrid, selectedRow);
                modifyForm.ShowDialog();

                if (modifyForm.DialogResult == DialogResult.OK)
                {
                    // Refresh inventory data after modification.
                    LoadInventoryData();
                }
            }
        }

        private void DeleteItem(object sender, EventArgs e)
        {
            if (inventoryGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = inventoryGrid.SelectedRows[0];
                string itemNumber = selectedRow.Cells["ItemNumber"].Value?.ToString();

                if (!string.IsNullOrEmpty(itemNumber))
                {
                    try
                    {
                        NeonDbService.DeleteInventoryItem(itemNumber).Wait();
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

        //press ctrl + R for reload function
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.R))
            {
                LoadInventoryData();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
