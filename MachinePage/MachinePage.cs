using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rancher.Database;

namespace Rancher
{
    public partial class MachinePage : UserControl
    {
        private readonly string placeholderText = "Enter machine name...";

        public MachinePage()
        {
            InitializeComponent();
            SetupInputSection();
            _ = LoadMachinesAsync();
        }

        private void SetupInputSection()
        {
            txtMachineName.ForeColor = Color.Gray;
            txtMachineName.Text = placeholderText;
            txtMachineName.GotFocus += RemovePlaceholder;
            txtMachineName.LostFocus += SetPlaceholder;
        }

        private void RemovePlaceholder(object sender, EventArgs e)
        {
            if (txtMachineName.Text == placeholderText)
            {
                txtMachineName.Text = "";
                txtMachineName.ForeColor = Color.Black;
            }
        }

        private void SetPlaceholder(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMachineName.Text))
            {
                txtMachineName.Text = placeholderText;
                txtMachineName.ForeColor = Color.Gray;
            }
        }

        private async void btnAddMachine_Click(object sender, EventArgs e)
        {
            string machineName = txtMachineName.Text.Trim();
            if (string.IsNullOrEmpty(machineName) || machineName == placeholderText)
            {
                MessageBox.Show("Please enter a machine name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                int machineId = await NeonDbService.AddMachine(machineName);
                var parts = await NeonDbService.GetPartsWithActualQuantity();
                foreach (var part in parts)
                {
                    string itemNumber = part["ItemNumber"].ToString() ?? "";
                    int actualQty = Convert.ToInt32(part["ActualQuantity"]);
                    if (!string.IsNullOrWhiteSpace(itemNumber) && actualQty > 0)
                    {
                        await NeonDbService.SubtractFromInventory(itemNumber, actualQty);
                    }
                }
                MessageBox.Show("Machine added and part consumption recorded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMachineName.Clear();
                SetPlaceholder(txtMachineName, EventArgs.Empty);
                await LoadMachinesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding the machine:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadMachinesAsync()
        {
            try
            {
                var machines = await NeonDbService.GetAllMachines();
                cardsHost.Controls.Clear();

                int yOffset = 0;
                foreach (var machine in machines)
                {
                    int id = int.Parse(machine["Id"]?.ToString() ?? "0");
                    string name = machine["Name"]?.ToString() ?? "Unnamed";
                    string createdAt = machine["CreatedAt"]?.ToString() ?? "Unknown";
                    Panel card = CreateStyledMachineCard(id, name, createdAt);
                    card.Top = yOffset;
                    cardsHost.Controls.Add(card);
                    yOffset += card.Height + 20;
                }

                cardsHost.Height = yOffset;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load machines: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateStyledMachineCard(int machineId, string name, string createdAt)
        {
            int cardWidth = 280;
            int cardHeight = 120;
            Panel card = new Panel
            {
                Width = cardWidth,
                Height = cardHeight,
                BackColor = Color.White,
                Margin = new Padding(10),
                Padding = new Padding(10)
            };

            card.Region = new Region(RoundedRect(new Rectangle(0, 0, card.Width, card.Height), 15));

            card.Paint += (s, e) =>
            {
                using SolidBrush shadow = new(Color.FromArgb(60, Color.Black));
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillRectangle(shadow, 4, card.Height - 4, card.Width - 4, 4);
                e.Graphics.FillRectangle(shadow, card.Width - 4, 4, 4, card.Height - 4);
            };

            TableLayoutPanel layout = new()
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                BackColor = Color.Transparent
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            Label nameLabel = new Label
            {
                Text = "Name: " + name,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label dateLabel = new Label
            {
                Text = "Created At: " + createdAt,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            layout.Controls.Add(nameLabel);
            layout.Controls.Add(dateLabel);

            card.Controls.Add(layout);

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete Machine");
            deleteItem.Click += async (s, e) =>
            {
                var confirm = MessageBox.Show(
                    "Are you sure you want to delete this machine?\nThis will also restore inventory quantities.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        await NeonDbService.DeleteMachineAndRestoreInventory(machineId);
                        await LoadMachinesAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting machine:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            contextMenu.Items.Add(deleteItem);
            card.ContextMenuStrip = contextMenu;

            return card;
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            GraphicsPath path = new();
            int diameter = radius * 2;
            Rectangle arc = new(bounds.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
