using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rancher
{
    public class SupplierInfoForm : Form
    {
        private Label lblName;
        private Label lblEmail;
        private Label lblPhone;
        private TextBox txtNote;

        public SupplierInfoForm(Dictionary<string, string> supplier)
        {
            InitializeComponent();

            lblName.Text = $"Name: {supplier["Name"]}";
            lblEmail.Text = $"Email: {supplier["Email"]}";
            lblPhone.Text = $"Phone: {supplier["Phone"]}";
            txtNote.Text = supplier["Note"];
        }

        private void InitializeComponent()
        {
            this.Text = "Supplier Details";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(400, 320);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblName = new Label() { Left = 20, Top = 20, Width = 340, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            lblEmail = new Label() { Left = 20, Top = 50, Width = 340 };
            lblPhone = new Label() { Left = 20, Top = 80, Width = 340 };
            Label lblNote = new Label() { Left = 20, Top = 110, Text = "Note:", Width = 340 };

            txtNote = new TextBox()
            {
                Left = 20,
                Top = 135,
                Width = 340,
                Height = 90,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            Button btnClose = new Button()
            {
                Text = "Close",
                Left = 150,
                Width = 100,
                Height = 30,
                Top = 230,
                DialogResult = DialogResult.OK
            };
            btnClose.Click += (s, e) => { this.Close(); };

            this.Controls.Add(lblName);
            this.Controls.Add(lblEmail);
            this.Controls.Add(lblPhone);
            this.Controls.Add(lblNote);
            this.Controls.Add(txtNote);
            this.Controls.Add(btnClose);
        }
    }
}
