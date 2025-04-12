namespace Rancher
{
    partial class MachinePage
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblMachineName;
        private System.Windows.Forms.TextBox txtMachineName;
        private System.Windows.Forms.Button btnAddMachine;
        private System.Windows.Forms.DataGridView machineGrid;

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
            this.lblMachineName = new System.Windows.Forms.Label();
            this.txtMachineName = new System.Windows.Forms.TextBox();
            this.btnAddMachine = new System.Windows.Forms.Button();
            this.machineGrid = new System.Windows.Forms.DataGridView();

            ((System.ComponentModel.ISupportInitialize)(this.machineGrid)).BeginInit();
            this.SuspendLayout();

            // 
            // lblMachineName
            // 
            this.lblMachineName.AutoSize = true;
            this.lblMachineName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblMachineName.Location = new System.Drawing.Point(30, 30);
            this.lblMachineName.Name = "lblMachineName";
            this.lblMachineName.Size = new System.Drawing.Size(106, 19);
            this.lblMachineName.TabIndex = 0;
            this.lblMachineName.Text = "Machine Name:";

            // 
            // txtMachineName
            // 
            this.txtMachineName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtMachineName.Location = new System.Drawing.Point(150, 27);
            this.txtMachineName.Name = "txtMachineName";
            this.txtMachineName.Size = new System.Drawing.Size(300, 25);
            this.txtMachineName.TabIndex = 1;

            // 
            // btnAddMachine
            // 
            this.btnAddMachine.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAddMachine.Location = new System.Drawing.Point(150, 70);
            this.btnAddMachine.Name = "btnAddMachine";
            this.btnAddMachine.Size = new System.Drawing.Size(150, 35);
            this.btnAddMachine.TabIndex = 2;
            this.btnAddMachine.Text = "Add Machine";
            this.btnAddMachine.UseVisualStyleBackColor = true;
            this.btnAddMachine.Click += new System.EventHandler(this.btnAddMachine_Click);

            // 
            // machineGrid
            // 
            this.machineGrid.AllowUserToAddRows = false;
            this.machineGrid.AllowUserToDeleteRows = false;
            this.machineGrid.AllowUserToResizeRows = false;
            this.machineGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.machineGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.machineGrid.Location = new System.Drawing.Point(30, 130);
            this.machineGrid.Name = "machineGrid";
            this.machineGrid.ReadOnly = true;
            this.machineGrid.RowTemplate.Height = 30;
            this.machineGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.machineGrid.Size = new System.Drawing.Size(540, 300);
            this.machineGrid.TabIndex = 3;

            // Define visible columns only
            this.machineGrid.Columns.Add("Name", "Name");
            this.machineGrid.Columns.Add("CreatedAt", "Created At");

            // 
            // MachinePage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblMachineName);
            this.Controls.Add(this.txtMachineName);
            this.Controls.Add(this.btnAddMachine);
            this.Controls.Add(this.machineGrid);
            this.Name = "MachinePage";
            this.Size = new System.Drawing.Size(600, 450);

            ((System.ComponentModel.ISupportInitialize)(this.machineGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
