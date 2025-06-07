using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rancher.Database;

namespace Rancher
{
    public class SuppliersPage : UserControl
    {
        private DataGridView supplierGrid;

        public SuppliersPage()
        {
            InitializeComponent();
            ApplyUIEnhancements();
            _ = LoadSuppliersAsync();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(0, 40, 0, 0); // Match menu spacing

            supplierGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                RowHeadersVisible = false
            };

            this.Controls.Add(supplierGrid);
        }

        private void ApplyUIEnhancements()
        {
            this.BackColor = Color.FromArgb(240, 240, 240);
            supplierGrid.BorderStyle = BorderStyle.FixedSingle;
            supplierGrid.BackgroundColor = Color.FromArgb(220, 215, 200);
            supplierGrid.EnableHeadersVisualStyles = false;
            supplierGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            supplierGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200);
            supplierGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            supplierGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            supplierGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            supplierGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            supplierGrid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            supplierGrid.ColumnHeadersHeight = 40;

            supplierGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            supplierGrid.GridColor = Color.FromArgb(180, 180, 180);

            supplierGrid.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            supplierGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            supplierGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            supplierGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            supplierGrid.RowTemplate.Height = 35;
            supplierGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
        }

        private async Task LoadSuppliersAsync()
        {
            try
            {
                var suppliers = await NeonDbService.GetAllSuppliers();

                supplierGrid.Columns.Clear();
                supplierGrid.Rows.Clear();

                supplierGrid.Columns.Add("Name", "Name");
                supplierGrid.Columns.Add("Email", "Email");
                supplierGrid.Columns.Add("Phone", "Phone");
                supplierGrid.Columns.Add("Note", "Note");

                foreach (var supplier in suppliers)
                {
                    supplierGrid.Rows.Add(
                        supplier["Name"],
                        supplier["Email"],
                        supplier["Phone"],
                        supplier["Note"]
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load suppliers:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
