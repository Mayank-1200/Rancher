using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rancher.Database;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Reflection;

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
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            inventoryGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            inventoryGrid.BackgroundColor = System.Drawing.Color.FromArgb(220, 215, 200);
            inventoryGrid.EnableHeadersVisualStyles = false;
            inventoryGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            inventoryGrid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(200, 200, 200);
            inventoryGrid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            inventoryGrid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            inventoryGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            inventoryGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            inventoryGrid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            inventoryGrid.ColumnHeadersHeight = 40;

            inventoryGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            inventoryGrid.GridColor = System.Drawing.Color.FromArgb(180, 180, 180);

            inventoryGrid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            inventoryGrid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            inventoryGrid.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9);
            inventoryGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            inventoryGrid.RowTemplate.Height = 35;
            inventoryGrid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(230, 230, 230);
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
                        if (columnName == "Green") row.Cells[columnName].Style.BackColor = System.Drawing.Color.LightGreen;
                        else if (columnName == "Yellow") row.Cells[columnName].Style.BackColor = System.Drawing.Color.Yellow;
                        else if (columnName == "Red") row.Cells[columnName].Style.BackColor = System.Drawing.Color.LightCoral;
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
                        LoadInventoryData();
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
            ExportToPdf();
        }

        private void ExportToPdf()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF Files|*.pdf";
                saveFileDialog.Title = "Export Inventory to PDF";
                saveFileDialog.FileName = "Inventory.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        PdfSharpCore.Fonts.GlobalFontSettings.FontResolver ??= new FontResolver();

                        var pdf = new PdfSharpCore.Pdf.PdfDocument();
                        var page = pdf.AddPage();
                        var gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                        var font = new PdfSharpCore.Drawing.XFont("Arial", 10);
                        var boldFont = new PdfSharpCore.Drawing.XFont("Arial", 10, PdfSharpCore.Drawing.XFontStyle.Bold);

                        double margin = 40;
                        double y = margin;
                        double baseRowHeight = 20;

                        // Count visible columns
                        var visibleCols = new List<DataGridViewColumn>();
                        foreach (DataGridViewColumn col in inventoryGrid.Columns)
                        {
                            if (col.Visible) visibleCols.Add(col);
                        }

                        int colCount = visibleCols.Count;
                        double totalWidth = page.Width - 2 * margin;

                        // Assign custom widths
                        Dictionary<string, double> colWidths = new();
                        double partColWidth = totalWidth * 0.3;
                        double otherColWidth = (totalWidth - partColWidth * 2) / (colCount - 2);

                        foreach (var col in visibleCols)
                        {
                            if (col.Name == "ItemNumber" || col.Name == "ProductName")
                                colWidths[col.Name] = partColWidth;
                            else
                                colWidths[col.Name] = otherColWidth;
                        }

                        // Draw header
                        double x = margin;
                        foreach (var col in visibleCols)
                        {
                            double w = colWidths[col.Name];

                            gfx.DrawRectangle(PdfSharpCore.Drawing.XBrushes.LightGray, x, y, w, baseRowHeight);
                            gfx.DrawRectangle(PdfSharpCore.Drawing.XPens.Black, x, y, w, baseRowHeight);
                            gfx.DrawString(col.HeaderText, boldFont, PdfSharpCore.Drawing.XBrushes.Black,
                                new PdfSharpCore.Drawing.XRect(x + 3, y + 3, w - 6, baseRowHeight),
                                PdfSharpCore.Drawing.XStringFormats.TopLeft);

                            x += w;
                        }

                        y += baseRowHeight;

                        // Draw data rows
                        foreach (DataGridViewRow row in inventoryGrid.Rows)
                        {
                            if (y > page.Height - margin)
                            {
                                page = pdf.AddPage();
                                gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                                y = margin;
                            }

                            // Measure tallest cell for row
                            double rowHeight = baseRowHeight;

                            foreach (var col in visibleCols)
                            {
                                string text = row.Cells[col.Index].Value?.ToString() ?? "";
                                double w = colWidths[col.Name];
                                var size = gfx.MeasureString(text, font);

                                int lines = (int)Math.Ceiling(size.Width / (w - 6));
                                double height = lines * baseRowHeight;

                                if (height > rowHeight)
                                    rowHeight = height;
                            }

                            // Draw each cell
                            x = margin;
                            foreach (var col in visibleCols)
                            {
                                string text = row.Cells[col.Index].Value?.ToString() ?? "";
                                double w = colWidths[col.Name];

                                var background = PdfSharpCore.Drawing.XBrushes.White;
                                if (col.Name == "Green" && !string.IsNullOrEmpty(text))
                                    background = PdfSharpCore.Drawing.XBrushes.LightGreen;
                                else if (col.Name == "Yellow" && !string.IsNullOrEmpty(text))
                                    background = PdfSharpCore.Drawing.XBrushes.Yellow;
                                else if (col.Name == "Red" && !string.IsNullOrEmpty(text))
                                    background = PdfSharpCore.Drawing.XBrushes.LightCoral;

                                gfx.DrawRectangle(background, x, y, w, rowHeight);
                                gfx.DrawRectangle(PdfSharpCore.Drawing.XPens.Black, x, y, w, rowHeight);

                                gfx.DrawString(text, font, PdfSharpCore.Drawing.XBrushes.Black,
                                    new PdfSharpCore.Drawing.XRect(x + 3, y + 3, w - 6, rowHeight),
                                    PdfSharpCore.Drawing.XStringFormats.TopLeft);

                                x += w;
                            }

                            y += rowHeight;
                        }

                        pdf.Save(saveFileDialog.FileName);
                        MessageBox.Show("Inventory exported successfully to PDF!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Export to PDF failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
