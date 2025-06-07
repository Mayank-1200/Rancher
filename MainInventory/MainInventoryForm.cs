using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel; // ✅ ClosedXML instead of EPPlus
using Rancher.Database;

namespace Rancher
{
    public partial class MainInventoryForm : UserControl
    {
        public MainInventoryForm()
        {
            InitializeComponent();
            this.Padding = new Padding(0, 40, 0, 0);
            this.Resize += MainInventoryForm_Resize;
            this.inventoryGrid.MouseClick += InventoryGrid_MouseClick;
            this.inventoryGrid.CellFormatting += inventoryGrid_CellFormatting;

            ApplyUIEnhancements();
            LoadInventoryData();
        }

        private void MainInventoryForm_Resize(object? sender, EventArgs e) { }

        private void InventoryGrid_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DataGridView.HitTestInfo hitTestInfo = inventoryGrid.HitTest(e.X, e.Y);
                if (hitTestInfo.RowIndex == -1)
                    inventoryGrid.ClearSelection();
            }
        }

        private void inventoryGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e) { }

        private void ApplyUIEnhancements()
        {
            this.BackColor = Color.FromArgb(240, 240, 240);
            inventoryGrid.BorderStyle = BorderStyle.FixedSingle;
            inventoryGrid.BackgroundColor = Color.FromArgb(220, 215, 200);
            inventoryGrid.EnableHeadersVisualStyles = false;
            inventoryGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            inventoryGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200);
            inventoryGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            inventoryGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            inventoryGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            inventoryGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            inventoryGrid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            inventoryGrid.ColumnHeadersHeight = 40;

            inventoryGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            inventoryGrid.GridColor = Color.FromArgb(180, 180, 180);

            inventoryGrid.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            inventoryGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            inventoryGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            inventoryGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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

                    string green = (quantity > yellowThreshold) ? quantity.ToString() : "";
                    string yellow = (quantity <= yellowThreshold && quantity > redThreshold) ? quantity.ToString() : "";
                    string red = (quantity <= redThreshold) ? quantity.ToString() : "";

                    string supplierName = item.ContainsKey("Supplier") ? item["Supplier"]?.ToString() ?? "Unknown" : "Unknown";

                    inventoryGrid.Rows.Add(
                        item["ItemNumber"].ToString() ?? "N/A",
                        item["ProductName"].ToString() ?? "Unknown",
                        item["ActualQuantity"].ToString() ?? "0",
                        green,
                        yellow,
                        red,
                        supplierName,
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
                foreach (string columnName in new[] { "Green", "Yellow", "Red" })
                {
                    if (row.Cells[columnName].Value != null && int.TryParse(row.Cells[columnName].Value.ToString(), out int qty))
                    {
                        if (columnName == "Green") row.Cells[columnName].Style.BackColor = Color.LightGreen;
                        else if (columnName == "Yellow") row.Cells[columnName].Style.BackColor = Color.Yellow;
                        else if (columnName == "Red") row.Cells[columnName].Style.BackColor = Color.LightCoral;
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
                    LoadInventoryData();
            }
        }

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
                        LoadInventoryData(); // Reload the grid fresh
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ViewSupplier(object sender, EventArgs e)
        {
            if (inventoryGrid.SelectedRows.Count == 0)
                return;

            DataGridViewRow selectedRow = inventoryGrid.SelectedRows[0];
            string supplierName = selectedRow.Cells["Supplier"].Value?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(supplierName))
            {
                MessageBox.Show("No supplier associated with this item.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ShowSupplierDetails(supplierName);
        }

        private async void ShowSupplierDetails(string supplierName)
        {
            try
            {
                var supplier = await NeonDbService.GetSupplierByName(supplierName);

                if (supplier == null)
                {
                    MessageBox.Show("Supplier not found in database.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SupplierInfoForm infoForm = new SupplierInfoForm(supplier);
                infoForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching supplier: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Export Inventory to Excel";
                saveFileDialog.FileName = "Inventory.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Inventory");

                            // Write headers
                            int colIndex = 1;
                            foreach (DataGridViewColumn column in inventoryGrid.Columns)
                            {
                                if (column.Visible)
                                {
                                    worksheet.Cell(1, colIndex).Value = column.HeaderText;
                                    colIndex++;
                                }
                            }

                            // Write data
                            for (int i = 0; i < inventoryGrid.Rows.Count; i++)
                            {
                                int visibleColIndex = 1;
                                for (int j = 0; j < inventoryGrid.Columns.Count; j++)
                                {
                                    if (inventoryGrid.Columns[j].Visible)
                                    {
                                        var value = inventoryGrid.Rows[i].Cells[j].Value?.ToString() ?? "";
                                        worksheet.Cell(i + 2, visibleColIndex).Value = value;
                                        visibleColIndex++;
                                    }
                                }
                            }

                            workbook.SaveAs(saveFileDialog.FileName);
                        }

                        MessageBox.Show("Inventory exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Export failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

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
