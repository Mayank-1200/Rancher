using System;
using System.Windows.Forms;
using Rancher.Database; // Added for database connection check

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
                // **Optional: Check Database Connection Before Proceeding**
                using (var connection = DatabaseHelper.GetConnection())
                {
                    Console.WriteLine("Database connection established successfully.");
                }

                // **Show Splash Screen**
                using (SplashScreen splash = new SplashScreen())
                {
                    splash.ShowDialog();
                }

                // **Launch Main Inventory Page**
                Application.Run(new MainInventoryForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
