using System;
using System.Drawing;
using System.Windows.Forms;

namespace Rancher
{
    public class UpdateQuantityForm : Form
    {
        private Label lblPrompt;
        private NumericUpDown numAdditionalQuantity;
        private Button btnSave;
        private Button btnCancel;
        private string itemNumber;

        // Exposes the additional quantity entered by the user.
        public int AdditionalQuantity { get; private set; }

        public UpdateQuantityForm(string itemNumber)
        {
            this.itemNumber = itemNumber;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Update Quantity";
            this.ClientSize = new Size(300, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            lblPrompt = new Label()
            {
                Text = $"Enter additional quantity for item: {itemNumber}",
                AutoSize = true,
                Location = new Point(10, 20)
            };

            numAdditionalQuantity = new NumericUpDown()
            {
                Location = new Point(10, 50),
                Width = 260,
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 0
            };

            btnSave = new Button()
            {
                Text = "Save",
                Location = new Point(50, 90),
                Size = new Size(80, 30)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button()
            {
                Text = "Cancel",
                Location = new Point(170, 90),
                Size = new Size(80, 30)
            };
            btnCancel.Click += BtnCancel_Click;

            this.Controls.Add(lblPrompt);
            this.Controls.Add(numAdditionalQuantity);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            AdditionalQuantity = (int)numAdditionalQuantity.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
