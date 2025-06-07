using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Rancher
{
    public class SplashScreen : Form
    {
        private Label welcomeLabel;
        private System.Windows.Forms.Timer glowTimer; // ✅ Explicitly defined
        private int glowIntensity = 0;
        private bool increasing = true;

        public SplashScreen()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(500, 300);
            this.BackColor = Color.FromArgb(30, 144, 255);

            welcomeLabel = new Label()
            {
                Text = "WELCOME",
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            this.Controls.Add(welcomeLabel);
            welcomeLabel.Location = new Point((this.ClientSize.Width - welcomeLabel.Width) / 2, (this.ClientSize.Height - welcomeLabel.Height) / 2);

            this.Opacity = 0;

            glowTimer = new System.Windows.Forms.Timer(); // ✅ Fix: Explicitly use System.Windows.Forms.Timer
            glowTimer.Interval = 50;
            glowTimer.Tick += GlowEffect;
            glowTimer.Start();
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await FadeIn();
            await Task.Delay(2000);
            await FadeOut();
            this.Close();
        }

        private async Task FadeIn()
        {
            for (double i = 0; i <= 1; i += 0.05)
            {
                this.Opacity = i;
                await Task.Delay(30);
            }
        }

        private async Task FadeOut()
        {
            for (double i = 1; i >= 0; i -= 0.05)
            {
                this.Opacity = i;
                await Task.Delay(30);
            }
        }

        private void GlowEffect(object? sender, EventArgs e)
        {
            // Ensure alpha remains within the valid range (0-255)
            glowIntensity = Math.Max(0, Math.Min(glowIntensity, 255));

            welcomeLabel.ForeColor = Color.FromArgb(glowIntensity, 255, 255, 255);

            if (increasing)
            {
                glowIntensity += 10;
                if (glowIntensity >= 255) increasing = false;
            }
            else
            {
                glowIntensity -= 10;
                if (glowIntensity <= 100) increasing = true;
            }
        }

    }
}
