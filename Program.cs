using System;
using System.Windows.Forms;
using Rancher.Database; // For database connection check

namespace Rancher
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                // **Check Database Connection Before Proceeding**
                using (var connection = DatabaseHelper.GetConnection())
                {
                    Console.WriteLine("Database connection established successfully.");
                }

                // **Show Splash Screen**
                using (SplashScreen splash = new SplashScreen())
                {
                    splash.ShowDialog();
                }

                // **Launch MainForm Instead of MainInventoryForm**
                Application.Run(new MainForm());  
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
