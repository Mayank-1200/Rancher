using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rancher.Database;

namespace Rancher
{
    public class InwardOutwardPage : UserControl
    {
        private ComboBox filterComboBox = new ComboBox();
        private DataGridView dataGrid = new DataGridView();
        private Label headerLabel = new Label();

        public InwardOutwardPage()
        {
            InitializeComponent();
            ApplyUIEnhancements();
            LoadLogData(); // Load all initially
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20, 60, 20, 20); // padding for nav bar
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Header Label
            headerLabel.Text = "Inward/Outward Logs";
            headerLabel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            headerLabel.ForeColor = Color.FromArgb(50, 50, 50);
            headerLabel.Dock = DockStyle.Top;
            headerLabel.Height = 50;
            headerLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Filter ComboBox
            filterComboBox.Items.AddRange(new string[] { "All", "Inward", "Outward" });
            filterComboBox.SelectedIndex = 0;
            filterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            filterComboBox.Font = new Font("Segoe UI", 10);
            filterComboBox.Dock = DockStyle.Top;
            filterComboBox.Margin = new Padding(0, 5, 0, 5);
            filterComboBox.SelectedIndexChanged += (s, e) => LoadLogData();

            // DataGridView
            dataGrid.Dock = DockStyle.Fill;
            dataGrid.ReadOnly = true;
            dataGrid.AllowUserToAddRows = false;
            dataGrid.AllowUserToDeleteRows = false;
            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Add controls in correct order
            this.Controls.Add(dataGrid);
            this.Controls.Add(filterComboBox);
            this.Controls.Add(headerLabel);
        }

        private void ApplyUIEnhancements()
        {
            dataGrid.BorderStyle = BorderStyle.FixedSingle;
            dataGrid.BackgroundColor = Color.FromArgb(220, 215, 200);
            dataGrid.EnableHeadersVisualStyles = false;

            dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200);
            dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dataGrid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGrid.ColumnHeadersHeight = 40;

            dataGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGrid.GridColor = Color.FromArgb(180, 180, 180);

            dataGrid.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            dataGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid.RowTemplate.Height = 35;
            dataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
        }

        private async void LoadLogData()
        {
            try
            {
                string selected = filterComboBox.SelectedItem?.ToString()?.ToLower() ?? "all";
                string? type = selected == "all" ? null : selected;

                var logs = await NeonDbService.GetEntryLogs(type);

                dataGrid.DataSource = logs.Select(log => new
                {
                    Part = log["ItemNumber"],
                    Quantity = log["Quantity"],
                    Type = log["EntryType"],
                    Supplier = log["SupplierName"],
                    Note = log["SupplierNote"],
                    Date = log["CreatedAt"]
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load logs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
