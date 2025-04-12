namespace Rancher
{
    partial class MainInventoryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView inventoryGrid;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.Panel bottomPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.inventoryGrid = new System.Windows.Forms.DataGridView();
            this.addButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bottomPanel = new System.Windows.Forms.Panel();
            System.Windows.Forms.TableLayoutPanel bottomTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();

            ((System.ComponentModel.ISupportInitialize)(this.inventoryGrid)).BeginInit();

            // Configure inventoryGrid
            this.inventoryGrid.EnableHeadersVisualStyles = false;
            this.inventoryGrid.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.inventoryGrid.DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.inventoryGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inventoryGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.inventoryGrid.ReadOnly = true;
            this.inventoryGrid.AllowUserToAddRows = false;
            this.inventoryGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            // Add columns to inventoryGrid
            this.inventoryGrid.Columns.Add("ItemNumber", "Part Number");
            this.inventoryGrid.Columns.Add("ProductName", "Part Name");
            this.inventoryGrid.Columns.Add("ActualQuantity", "Actual Quantity");
            this.inventoryGrid.Columns.Add("Green", "Green (>100)");
            this.inventoryGrid.Columns.Add("Yellow", "Yellow (50-100)");
            this.inventoryGrid.Columns.Add("Red", "Red (0-50)");
            this.inventoryGrid.Columns.Add("Supplier", "Supplier");

            // Add hidden threshold columns for internal logic and hide them from UI
            this.inventoryGrid.Columns.Add("RedThreshold", "Red Threshold");
            this.inventoryGrid.Columns.Add("YellowThreshold", "Yellow Threshold");
            this.inventoryGrid.Columns.Add("GreenThreshold", "Green Threshold");
            this.inventoryGrid.Columns["RedThreshold"].Visible = false;
            this.inventoryGrid.Columns["YellowThreshold"].Visible = false;
            this.inventoryGrid.Columns["GreenThreshold"].Visible = false;

            this.inventoryGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.inventoryGrid_CellFormatting);

            // Configure addButton
            this.addButton.Text = "Add Item";
            this.addButton.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);
            // Remove previous docking since layout is handled by TableLayoutPanel
            this.addButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addButton.Height = 50; // retained from original

            // Configure exportButton
            this.exportButton.Text = "Export to Excel";
            this.exportButton.Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // Remove previous docking since layout is handled by TableLayoutPanel
            this.exportButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exportButton.Height = 45; // retained from original

            // Configure contextMenu
            this.contextMenu.Items.Add("Modify", null, this.ModifyItem);
            this.contextMenu.Items.Add("Delete", null, this.DeleteItem);
            this.contextMenu.Items.Add("View Supplier", null, this.ViewSupplier);
            this.inventoryGrid.ContextMenuStrip = this.contextMenu;

            // Configure bottomPanel to hold the two buttons side by side
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            // Set bottomPanel height to the max of the two buttons' heights 
            // (actual height will be managed by the TableLayoutPanel layout)
            this.bottomPanel.Height = 50;

            // Configure TableLayoutPanel for side-by-side layout
            bottomTableLayoutPanel.ColumnCount = 2;
            bottomTableLayoutPanel.RowCount = 1;
            bottomTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            bottomTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            bottomTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));

            // Add buttons to the TableLayoutPanel
            bottomTableLayoutPanel.Controls.Add(this.addButton, 0, 0);
            bottomTableLayoutPanel.Controls.Add(this.exportButton, 1, 0);

            // Add the TableLayoutPanel to bottomPanel
            this.bottomPanel.Controls.Add(bottomTableLayoutPanel);

            // Configure MainInventoryForm
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.inventoryGrid);
            this.Controls.Add(this.bottomPanel);
            this.Text = "Main Inventory";
            this.ResumeLayout(false);

            ((System.ComponentModel.ISupportInitialize)(this.inventoryGrid)).EndInit();
        }
    }
}
