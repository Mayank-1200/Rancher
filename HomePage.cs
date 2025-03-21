using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rancher.Database;

namespace Rancher
{
    // A simple professional header control with subtle drop shadow.
    public class ProfessionalHeaderLabel : Control
    {
        public ProfessionalHeaderLabel()
        {
            this.DoubleBuffered = true;
            this.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            this.ForeColor = Color.FromArgb(50, 50, 50);
            this.Height = 90;
            this.Text = "Inventory Dashboard";
            this.TextAlign = ContentAlignment.MiddleCenter;
        }

        public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleCenter;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = this.ClientRectangle;

            // Measure text size.
            SizeF textSize = e.Graphics.MeasureString(this.Text, this.Font);

            // Calculate centered position.
            float x = (rect.Width - textSize.Width) / 2;
            float y = (rect.Height - textSize.Height) / 2;

            // Draw a very subtle drop shadow.
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(80, Color.Black)))
            {
                e.Graphics.DrawString(this.Text, this.Font, shadowBrush, x + 2, y + 2);
            }

            // Draw the text.
            using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
            {
                e.Graphics.DrawString(this.Text, this.Font, textBrush, x, y);
            }
        }
    }

    public class HomePage : UserControl
    {
        private FlowLayoutPanel cardsPanel = new FlowLayoutPanel();
        private ProfessionalHeaderLabel headerLabel = new ProfessionalHeaderLabel();

        public HomePage()
        {
            InitializeComponent();
            LoadInventoryData();
        }

        private void InitializeComponent()
        {
            // Set a soft, warm background that's easy on the eyes.
            this.BackColor = Color.FromArgb(220, 215, 200);
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            // Configure the custom header.
            headerLabel.Dock = DockStyle.Top;
            headerLabel.Margin = new Padding(0, 0, 0, 15);

            // Configure the FlowLayoutPanel for cards.
            cardsPanel.Dock = DockStyle.Fill;
            cardsPanel.AutoScroll = true;
            cardsPanel.FlowDirection = FlowDirection.LeftToRight;
            cardsPanel.WrapContents = true;
            cardsPanel.Padding = new Padding(10);
            cardsPanel.BackColor = Color.Transparent;

            // Add controls.
            this.Controls.Add(cardsPanel);
            this.Controls.Add(headerLabel);
        }

        /// <summary>
        /// Retrieves inventory items, sorts them (red priority first), and displays each as a card.
        /// </summary>
        private async void LoadInventoryData()
        {
            try
            {
                var inventoryData = await NeonDbService.GetInventoryItems();
                cardsPanel.Controls.Clear();

                var sortedData = inventoryData.OrderBy(item =>
                {
                    int redThreshold = item.ContainsKey("RedThreshold") ? Convert.ToInt32(item["RedThreshold"]) : 10;
                    int yellowThreshold = item.ContainsKey("YellowThreshold") ? Convert.ToInt32(item["YellowThreshold"]) : 30;
                    int quantity = item.ContainsKey("Quantity") ? Convert.ToInt32(item["Quantity"]) : 0;
                    if (quantity <= redThreshold)
                        return 0;
                    else if (quantity <= yellowThreshold)
                        return 1;
                    else
                        return 2;
                }).ToList();

                foreach (var item in sortedData)
                {
                    int redThreshold = item.ContainsKey("RedThreshold") ? Convert.ToInt32(item["RedThreshold"]) : 10;
                    int yellowThreshold = item.ContainsKey("YellowThreshold") ? Convert.ToInt32(item["YellowThreshold"]) : 30;
                    int quantity = item.ContainsKey("Quantity") ? Convert.ToInt32(item["Quantity"]) : 0;

                    Color quantityColor;
                    if (quantity > yellowThreshold)
                    {
                        quantityColor = Color.LightGreen;
                    }
                    else if (quantity > redThreshold)
                    {
                        quantityColor = Color.Goldenrod;
                    }
                    else
                    {
                        quantityColor = Color.IndianRed;
                    }

                    Panel card = CreateItemCard(
                        item.ContainsKey("ItemNumber") ? item["ItemNumber"].ToString() : "N/A",
                        item.ContainsKey("ProductName") ? item["ProductName"].ToString() : "Unknown",
                        quantity,
                        item.ContainsKey("Supplier") ? item["Supplier"].ToString() : "Unknown",
                        quantityColor
                    );

                    cardsPanel.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inventory: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creates a refined, modern card panel with rounded corners and a subtle drop shadow.
        /// </summary>
        private Panel CreateItemCard(string itemNumber, string productName, int quantity, string supplier, Color quantityColor)
        {
            Panel card = new Panel
            {
                Width = 280,
                Height = 180,
                BackColor = Color.White,
                Margin = new Padding(15),
                Padding = new Padding(10)
            };

            int radius = 15;
            Rectangle bounds = new Rectangle(0, 0, card.Width, card.Height);
            card.Region = new Region(RoundedRect(bounds, radius));

            // Add a subtle drop shadow by drawing in the Paint event.
            card.Paint += (s, e) =>
            {
                int shadowOffset = 4;
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(60, Color.Black)))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    // Draw shadow on bottom and right.
                    e.Graphics.FillRectangle(shadowBrush, shadowOffset, card.Height - shadowOffset, card.Width - shadowOffset, shadowOffset);
                    e.Graphics.FillRectangle(shadowBrush, card.Width - shadowOffset, shadowOffset, shadowOffset, card.Height - shadowOffset);
                }
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };
            for (int i = 0; i < 5; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            Label lblItemNumber = new Label
            {
                Text = "Item: " + itemNumber,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI Semibold", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            Label lblProductName = new Label
            {
                Text = "Product: " + productName,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            Label lblSupplier = new Label
            {
                Text = "Supplier: " + supplier,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            Label lblQuantity = new Label
            {
                Text = "Quantity: " + quantity,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI Semibold", 10),
                BackColor = quantityColor,
                ForeColor = Color.White,
                Padding = new Padding(5)
            };

            Label lblExtra = new Label
            {
                Text = "", // Optionally include extra info here.
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray
            };

            layout.Controls.Add(lblItemNumber, 0, 0);
            layout.Controls.Add(lblProductName, 0, 1);
            layout.Controls.Add(lblSupplier, 0, 2);
            layout.Controls.Add(lblQuantity, 0, 3);
            layout.Controls.Add(lblExtra, 0, 4);

            card.Controls.Add(layout);
            return card;
        }

        /// <summary>
        /// Returns a GraphicsPath for a rectangle with rounded corners.
        /// </summary>
        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
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
