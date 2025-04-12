using System;
using System.Windows.Forms;
using System.Drawing; // Required for Icon

namespace Rancher
{
    public partial class MainForm : Form
    {
        private Panel contentPanel;

        public MainForm()
        {
            InitializeComponent();

            // Set application icon
            this.Icon = new Icon("Resources/rancher_icon.ico");

            this.WindowState = FormWindowState.Maximized;
            LoadHomePage(); // Load Home page initially
        }

        private void InitializeComponent()
        {
            this.Text = "Rancher - Inventory Management";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Create MenuStrip (Navigation Bar)
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem homeMenu = new ToolStripMenuItem("Home");
            ToolStripMenuItem inventoryMenu = new ToolStripMenuItem("Main Inventory");
            ToolStripMenuItem machineMenu = new ToolStripMenuItem("Machine Page");
            // ToolStripMenuItem placeholderMenu = new ToolStripMenuItem("Placeholder");

            homeMenu.Click += (s, e) => LoadHomePage();
            inventoryMenu.Click += (s, e) => LoadInventoryPage();
            machineMenu.Click += (s, e) => LoadMachinePage();

            menuStrip.Items.Add(homeMenu);
            menuStrip.Items.Add(inventoryMenu);
            menuStrip.Items.Add(machineMenu);
            // menuStrip.Items.Add(placeholderMenu);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Create Panel for Loading Pages
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(contentPanel);
        }

        private void LoadHomePage()
        {
            LoadPage(new HomePage());
        }

        private void LoadInventoryPage()
        {
            LoadPage(new MainInventoryForm());
        }

        private void LoadMachinePage()
        {
            LoadPage(new MachinePage());
        }

        private void LoadPage(Control page)
        {
            contentPanel.Controls.Clear();
            page.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(page);
        }
    }
}
