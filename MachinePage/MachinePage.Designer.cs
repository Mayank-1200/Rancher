namespace Rancher
{
    partial class MachinePage
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblMachineName;
        private System.Windows.Forms.TextBox txtMachineName;
        private System.Windows.Forms.Button btnAddMachine;

        private System.Windows.Forms.Panel inputPanel;
        private System.Windows.Forms.Panel scrollPanel;
        private System.Windows.Forms.Panel cardsHost;
        private System.Windows.Forms.Panel containerPanel;

        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.PictureBox logoBox;

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
            this.lblMachineName = new System.Windows.Forms.Label();
            this.txtMachineName = new System.Windows.Forms.TextBox();
            this.btnAddMachine = new System.Windows.Forms.Button();
            this.inputPanel = new System.Windows.Forms.Panel();
            this.scrollPanel = new System.Windows.Forms.Panel();
            this.cardsHost = new System.Windows.Forms.Panel();
            this.containerPanel = new System.Windows.Forms.Panel();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.logoBox = new System.Windows.Forms.PictureBox();

            this.SuspendLayout();

            // 
            // inputPanel
            // 
            this.inputPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.inputPanel.Height = 60;
            this.inputPanel.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.inputPanel.BackColor = System.Drawing.Color.FromArgb(245, 240, 230);
            this.inputPanel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.inputPanel.Controls.Add(this.lblMachineName);
            this.inputPanel.Controls.Add(this.txtMachineName);
            this.inputPanel.Controls.Add(this.btnAddMachine);

            // 
            // lblMachineName
            // 
            this.lblMachineName.AutoSize = true;
            this.lblMachineName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMachineName.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.lblMachineName.Location = new System.Drawing.Point(20, 18);
            this.lblMachineName.Name = "lblMachineName";
            this.lblMachineName.Size = new System.Drawing.Size(112, 19);
            this.lblMachineName.TabIndex = 0;
            this.lblMachineName.Text = "Machine Name:";

            // 
            // txtMachineName
            // 
            this.txtMachineName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtMachineName.Location = new System.Drawing.Point(160, 15);
            this.txtMachineName.Name = "txtMachineName";
            this.txtMachineName.Size = new System.Drawing.Size(280, 25);
            this.txtMachineName.TabIndex = 1;

            // 
            // btnAddMachine
            // 
            this.btnAddMachine.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAddMachine.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnAddMachine.ForeColor = System.Drawing.Color.White;
            this.btnAddMachine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddMachine.FlatAppearance.BorderSize = 0;
            this.btnAddMachine.Location = new System.Drawing.Point(460, 13);
            this.btnAddMachine.Name = "btnAddMachine";
            this.btnAddMachine.Size = new System.Drawing.Size(130, 30);
            this.btnAddMachine.TabIndex = 2;
            this.btnAddMachine.Text = "Add Machine";
            this.btnAddMachine.UseVisualStyleBackColor = false;
            this.btnAddMachine.Click += new System.EventHandler(this.btnAddMachine_Click);

            // 
            // scrollPanel
            // 
            this.scrollPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.scrollPanel.BackColor = System.Drawing.Color.FromArgb(220, 215, 200);
            this.scrollPanel.AutoScroll = true;
            this.scrollPanel.Padding = new System.Windows.Forms.Padding(10);
            this.scrollPanel.Width = 340;
            this.scrollPanel.Controls.Add(this.cardsHost);

            // 
            // cardsHost
            // 
            this.cardsHost.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardsHost.AutoSize = false;
            this.cardsHost.Width = 320;
            this.cardsHost.Location = new System.Drawing.Point(10, 10);
            this.cardsHost.Name = "cardsHost";
            this.cardsHost.TabIndex = 0;

            // 
            // rightPanel
            // 
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.BackColor = System.Drawing.Color.Transparent;
            this.rightPanel.Padding = new System.Windows.Forms.Padding(0);
            this.rightPanel.Controls.Add(this.logoBox);

            // 
            // logoBox
            // 
            this.logoBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoBox.BackColor = System.Drawing.Color.Transparent;

            // Load logo from file if it exists
            string logoPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "logo_rancher.png");
            if (System.IO.File.Exists(logoPath))
            {
                this.logoBox.Image = System.Drawing.Image.FromFile(logoPath);
            }

            // 
            // containerPanel
            // 
            this.containerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerPanel.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0); // Padding under nav bar
            this.containerPanel.BackColor = System.Drawing.Color.Transparent;
            this.containerPanel.Controls.Add(this.rightPanel);
            this.containerPanel.Controls.Add(this.scrollPanel);
            this.containerPanel.Controls.Add(this.inputPanel);

            // 
            // MachinePage
            // 
            this.BackColor = System.Drawing.Color.FromArgb(220, 215, 200);
            this.Controls.Add(this.containerPanel);
            this.Name = "MachinePage";
            this.Size = new System.Drawing.Size(800, 500);
            this.ResumeLayout(false);
        }
    }
}
