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
            this.inventoryGrid.EnableHeadersVisualStyles = false;
            this.inventoryGrid.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight; // Keeps selection color normal
            this.inventoryGrid.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;

            this.SuspendLayout();

            // 
            // inventoryGrid
            // 
            this.inventoryGrid.Dock = DockStyle.Fill;
            this.inventoryGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.inventoryGrid.ReadOnly = true; // Disables manual edits
            this.inventoryGrid.AllowUserToAddRows = false;
            this.inventoryGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.inventoryGrid.Columns.Add("ItemNumber", "Item Number");
            this.inventoryGrid.Columns.Add("ProductName", "Product Name");
            this.inventoryGrid.Columns.Add("Green", "Green (>100)");
            this.inventoryGrid.Columns.Add("Yellow", "Yellow (50-100)");
            this.inventoryGrid.Columns.Add("Red", "Red (0-50)");
            this.inventoryGrid.Columns.Add("Supplier", "Supplier");
            this.inventoryGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.inventoryGrid_CellFormatting); // Hooking the event

            // 
            // addButton
            // 
            this.addButton.Text = "Add Item";
            this.addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.addButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.addButton.Height = 50; // Increased height
            this.addButton.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);

            // 
            // Context Menu (Right Click)
            // 
            this.contextMenu.Items.Add("Modify", null, this.ModifyItem);
            this.contextMenu.Items.Add("Delete", null, this.DeleteItem);
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
        }
    }
}