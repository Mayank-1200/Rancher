using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rancher.Database;

namespace Rancher
{
    public class ProfessionalHeaderLabel : Control
    {
        public ProfessionalHeaderLabel()
        {
            this.DoubleBuffered = true;
            this.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            this.ForeColor = Color.FromArgb(50, 50, 50);
            this.Height = 90;
            this.Text = "Inventory Dashboard";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = this.ClientRectangle;
            SizeF textSize = e.Graphics.MeasureString(this.Text, this.Font);
            float x = (rect.Width - textSize.Width) / 2;
            float y = (rect.Height - textSize.Height) / 2;

            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(80, Color.Black)))
                e.Graphics.DrawString(this.Text, this.Font, shadowBrush, x + 2, y + 2);

            using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
                e.Graphics.DrawString(this.Text, this.Font, textBrush, x, y);
        }
    }

    public class HomePage : UserControl
    {
        private Panel containerPanel = new Panel();
        private Panel scrollPanel = new Panel();
        private Panel cardsHost = new Panel();
        private Panel rightPanel = new Panel();
        private FlowLayoutPanel filterPanel = new FlowLayoutPanel();
        private Label filterLabel = new Label();
        private ComboBox filterComboBox = new ComboBox();
        private ProfessionalHeaderLabel headerLabel = new ProfessionalHeaderLabel();
        private PictureBox logoBox = new PictureBox();
        private Button addQuantityButton = new Button();

        private const int CardHeight = 180;
        private const int CardMargin = 20;
        private const int CardWidth = 280;

        private List<Dictionary<string, object>> allInventoryItems = new();

        public HomePage()
        {
            InitializeComponent();
            LoadInventoryData();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(220, 215, 200);
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            headerLabel.Dock = DockStyle.Top;
            headerLabel.Margin = new Padding(0, 0, 0, 10);

            // Add Quantity Button
            addQuantityButton.Text = "Inward-Outward";
            addQuantityButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            addQuantityButton.BackColor = Color.DarkRed;
            addQuantityButton.ForeColor = Color.White;
            addQuantityButton.FlatStyle = FlatStyle.Flat;
            addQuantityButton.FlatAppearance.BorderSize = 0;
            addQuantityButton.Size = new Size(155, 35);
            addQuantityButton.Location = new Point(this.Width - 180, headerLabel.Bottom + 5);
            addQuantityButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            addQuantityButton.Click += AddQuantityButton_Click;
            this.Controls.Add(addQuantityButton);

            // Filter panel
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 40;
            filterPanel.FlowDirection = FlowDirection.LeftToRight;
            filterPanel.Padding = new Padding(10, 0, 0, 10);
            filterPanel.AutoSize = true;
            filterPanel.WrapContents = false;
            filterPanel.BackColor = Color.Transparent;

            filterLabel.Text = "Filter by Threshold:";
            filterLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            filterLabel.ForeColor = Color.FromArgb(60, 60, 60);
            filterLabel.TextAlign = ContentAlignment.MiddleLeft;
            filterLabel.Margin = new Padding(0, 6, 10, 0);
            filterLabel.AutoSize = true;

            filterComboBox.Width = 180;
            filterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            filterComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            filterComboBox.Font = new Font("Segoe UI", 9);
            filterComboBox.Items.AddRange(new object[]
            {
                "Red Threshold",
                "Yellow Threshold",
                "Green Threshold",
                "All"
            });
            filterComboBox.SelectedIndex = 0;
            filterComboBox.DrawItem += FilterComboBox_DrawItem;
            filterComboBox.SelectedIndexChanged += (s, e) => ApplyThresholdFilter();

            filterPanel.Controls.Add(filterLabel);
            filterPanel.Controls.Add(filterComboBox);

            // Scroll panel for cards
            scrollPanel.Dock = DockStyle.Left;
            scrollPanel.Width = 340;
            scrollPanel.AutoScroll = true;
            scrollPanel.Padding = new Padding(10);
            scrollPanel.BackColor = Color.Transparent;

            cardsHost.Dock = DockStyle.Top;
            cardsHost.AutoSize = false;
            cardsHost.Width = CardWidth + CardMargin * 2;
            scrollPanel.Controls.Add(cardsHost);

            // Right panel with logo
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.Transparent;
            rightPanel.Padding = new Padding(0);

            logoBox.Dock = DockStyle.Fill;
            logoBox.SizeMode = PictureBoxSizeMode.Zoom;
            logoBox.BackColor = Color.Transparent;

            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo_rancher.png");
            if (File.Exists(logoPath))
            {
                logoBox.Image = Image.FromFile(logoPath);
            }

            rightPanel.Controls.Add(logoBox);
            rightPanel.Resize += (s, e) =>
            {
                logoBox.Location = new Point(
                    (rightPanel.Width - logoBox.Width) / 2,
                    (rightPanel.Height - logoBox.Height) / 2
                );
            };

            containerPanel.Dock = DockStyle.Fill;
            containerPanel.Controls.Add(rightPanel);
            containerPanel.Controls.Add(scrollPanel);

            this.Controls.Add(containerPanel);
            this.Controls.Add(filterPanel);
            this.Controls.Add(headerLabel);
        }

        private void FilterComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            string itemText = filterComboBox.Items[e.Index].ToString();
            Color bgColor = itemText switch
            {
                string s when s.Contains("Red") => Color.IndianRed,
                string s when s.Contains("Yellow") => Color.Goldenrod,
                string s when s.Contains("Green") => Color.LightGreen,
                _ => Color.LightGray
            };

            e.DrawBackground();
            using (SolidBrush brush = new SolidBrush(bgColor))
                e.Graphics.FillRectangle(brush, e.Bounds);

            TextRenderer.DrawText(e.Graphics, itemText, e.Font, e.Bounds, Color.White, TextFormatFlags.Left);
            e.DrawFocusRectangle();
        }

        private async void LoadInventoryData()
        {
            try
            {
                allInventoryItems = await NeonDbService.GetInventoryItems();
                ApplyThresholdFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inventory: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyThresholdFilter()
        {
            string selected = filterComboBox.SelectedItem?.ToString() ?? "Red Threshold";

            var filtered = allInventoryItems.Where(item =>
            {
                int red = item.ContainsKey("RedThreshold") ? Convert.ToInt32(item["RedThreshold"]) : 10;
                int yellow = item.ContainsKey("YellowThreshold") ? Convert.ToInt32(item["YellowThreshold"]) : 30;
                int qty = item.ContainsKey("Quantity") ? Convert.ToInt32(item["Quantity"]) : 0;

                return selected switch
                {
                    string s when s.Contains("Red") => qty <= red,
                    string s when s.Contains("Yellow") => qty > red && qty <= yellow,
                    string s when s.Contains("Green") => qty > yellow,
                    _ => true
                };
            }).ToList();

            RenderCards(filtered);
        }

        private void RenderCards(List<Dictionary<string, object>> items)
        {
            cardsHost.Controls.Clear();
            int y = 0;

            foreach (var item in items)
            {
                int red = item.ContainsKey("RedThreshold") ? Convert.ToInt32(item["RedThreshold"]) : 10;
                int yellow = item.ContainsKey("YellowThreshold") ? Convert.ToInt32(item["YellowThreshold"]) : 30;
                int qty = item.ContainsKey("Quantity") ? Convert.ToInt32(item["Quantity"]) : 0;

                Color color = qty > yellow ? Color.LightGreen :
                              qty > red ? Color.Goldenrod :
                              Color.IndianRed;

                string itemNumber = item.GetValueOrDefault("ItemNumber", "N/A").ToString();
                string productName = item.GetValueOrDefault("ProductName", "Unknown").ToString();
                string supplier = item.GetValueOrDefault("Supplier", "Unknown").ToString();

                Panel card = CreateItemCard(itemNumber, productName, qty, supplier, color);

                card.Top = y;
                card.Left = CardMargin;
                cardsHost.Controls.Add(card);
                y += CardHeight + CardMargin;
            }

            cardsHost.Height = y;
        }

        private Panel CreateItemCard(string itemNumber, string productName, int quantity, string supplier, Color quantityColor)
        {
            Panel card = new Panel
            {
                Width = CardWidth,
                Height = CardHeight,
                BackColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(10)
            };

            card.Region = new Region(RoundedRect(new Rectangle(0, 0, card.Width, card.Height), 15));

            TableLayoutPanel layout = new()
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 1,
                BackColor = Color.Transparent
            };

            for (int i = 0; i < 5; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            layout.Controls.Add(CreateLabel("Item: " + itemNumber, bold: true), 0, 0);
            layout.Controls.Add(CreateLabel("Product: " + productName), 0, 1);
            layout.Controls.Add(CreateLabel("Supplier: " + supplier), 0, 2);
            layout.Controls.Add(new Label
            {
                Text = "Quantity: " + quantity,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI Semibold", 10),
                BackColor = quantityColor,
                ForeColor = Color.White,
                Padding = new Padding(5)
            }, 0, 3);
            layout.Controls.Add(CreateLabel("", italic: true, gray: true), 0, 4);

            card.Controls.Add(layout);
            return card;
        }

        private Label CreateLabel(string text, bool bold = false, bool italic = false, bool gray = false)
        {
            FontStyle style = FontStyle.Regular;
            if (bold) style |= FontStyle.Bold;
            if (italic) style |= FontStyle.Italic;

            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, style),
                ForeColor = gray ? Color.Gray : Color.FromArgb(60, 60, 60)
            };
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

        private void AddQuantityButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new AddQuantityForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadInventoryData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
