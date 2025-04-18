namespace Rancher
{
    partial class MainInventoryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView inventoryGrid;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ContextMenuStrip contextMenu;

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
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);

            ((System.ComponentModel.ISupportInitialize)(this.inventoryGrid)).BeginInit();

            this.inventoryGrid.EnableHeadersVisualStyles = false;
            this.inventoryGrid.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.inventoryGrid.DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;

            this.SuspendLayout();

            // 
            // inventoryGrid
            // 
            this.inventoryGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inventoryGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.inventoryGrid.ReadOnly = true;
            this.inventoryGrid.AllowUserToAddRows = false;
            this.inventoryGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            this.inventoryGrid.Columns.Add("ItemNumber", "Part Number");
            this.inventoryGrid.Columns.Add("ProductName", "Part Name");
            this.inventoryGrid.Columns.Add("ActualQuantity", "Actual Quantity");
            this.inventoryGrid.Columns.Add("Green", "Green (>100)");
            this.inventoryGrid.Columns.Add("Yellow", "Yellow (50-100)");
            this.inventoryGrid.Columns.Add("Red", "Red (0-50)");
            this.inventoryGrid.Columns.Add("Supplier", "Supplier");

            // ✅ Add hidden threshold columns for internal logic
            this.inventoryGrid.Columns.Add("RedThreshold", "Red Threshold");
            this.inventoryGrid.Columns.Add("YellowThreshold", "Yellow Threshold");
            this.inventoryGrid.Columns.Add("GreenThreshold", "Green Threshold");

            // ✅ Hide them from UI
            this.inventoryGrid.Columns["RedThreshold"].Visible = false;
            this.inventoryGrid.Columns["YellowThreshold"].Visible = false;
            this.inventoryGrid.Columns["GreenThreshold"].Visible = false;

            this.inventoryGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.inventoryGrid_CellFormatting);

            // 
            // addButton
            // 
            this.addButton.Text = "Add Item";
            this.addButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.addButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.addButton.Height = 50;
            this.addButton.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);

            // 
            // contextMenu
            // 
            this.contextMenu.Items.Add("Modify", null, this.ModifyItem);
            this.contextMenu.Items.Add("Delete", null, this.DeleteItem);
            this.contextMenu.Items.Add("View Supplier", null, this.ViewSupplier);
            this.inventoryGrid.ContextMenuStrip = this.contextMenu;

            // 
            // MainInventoryForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.inventoryGrid);
            this.Controls.Add(this.addButton);
            this.Text = "Main Inventory";
            this.ResumeLayout(false);

            ((System.ComponentModel.ISupportInitialize)(this.inventoryGrid)).EndInit();
        }
    }
}
