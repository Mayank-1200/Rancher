using System;
using System.Drawing;
using System.Windows.Forms;

namespace Rancher
{
    public partial class MainInventoryForm : Form
    {
        public MainInventoryForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Resize += MainInventoryForm_Resize;
            this.inventoryGrid.MouseClick += InventoryGrid_MouseClick; // Detect clicks inside the table
        }

        private void InventoryGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) 
            {
                // Get the row index where the click happened
                DataGridView.HitTestInfo hitTestInfo = inventoryGrid.HitTest(e.X, e.Y);

                // If click happened below the last row, clear selection
                if (hitTestInfo.RowIndex == -1)
                {
                    inventoryGrid.ClearSelection();
                }
            }
        }

        private void MainInventoryForm_Resize(object sender, EventArgs e)
        {
            inventoryGrid.Width = this.ClientSize.Width;
            inventoryGrid.Height = this.ClientSize.Height - addButton.Height;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddItemForm addItemForm = new AddItemForm(inventoryGrid);
            addItemForm.ShowDialog();
            UpdateRowColors();
        }

        private void ModifyItem(object sender, EventArgs e)
        {
            if (inventoryGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = inventoryGrid.SelectedRows[0];
                AddItemForm modifyForm = new AddItemForm(inventoryGrid, selectedRow);
                modifyForm.ShowDialog();
                UpdateRowColors();
            }
        }

        private void DeleteItem(object sender, EventArgs e)
        {
            if (inventoryGrid.SelectedRows.Count > 0)
            {
                inventoryGrid.Rows.Remove(inventoryGrid.SelectedRows[0]);
                UpdateRowColors();
            }
        }

        private void inventoryGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure it's not the header row
            {
                DataGridViewRow row = inventoryGrid.Rows[e.RowIndex];

                // Get the quantity for each column
                int greenQty = row.Cells["Green"].Value != null && int.TryParse(row.Cells["Green"].Value.ToString(), out int g) ? g : 0;
                int yellowQty = row.Cells["Yellow"].Value != null && int.TryParse(row.Cells["Yellow"].Value.ToString(), out int y) ? y : 0;
                int redQty = row.Cells["Red"].Value != null && int.TryParse(row.Cells["Red"].Value.ToString(), out int r) ? r : 0;

                // Reset background color first to avoid leftover formatting
                e.CellStyle.BackColor = SystemColors.Window; // Default background

                // Highlight only the column where the quantity is set
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
