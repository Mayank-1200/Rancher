using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rancher.Database;

namespace Rancher
{
    public partial class MachinePage : UserControl
    {
        public MachinePage()
        {
            InitializeComponent();
            _ = LoadMachinesAsync(); // Load machine list on page load
        }

        private async void btnAddMachine_Click(object sender, EventArgs e)
        {
            string machineName = txtMachineName.Text.Trim();

            if (string.IsNullOrEmpty(machineName))
            {
                MessageBox.Show("Please enter a machine name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int machineId = await NeonDbService.AddMachine(machineName);
                var parts = await NeonDbService.GetPartsWithActualQuantity();

                List<(string itemNumber, int quantity)> usedParts = new();

                foreach (var part in parts)
                {
                    string itemNumber = part["ItemNumber"].ToString() ?? "";
                    int actualQty = Convert.ToInt32(part["ActualQuantity"]);

                    if (!string.IsNullOrWhiteSpace(itemNumber) && actualQty > 0)
                    {
                        await NeonDbService.SubtractFromInventory(itemNumber, actualQty);
                        usedParts.Add((itemNumber, actualQty));
                    }
                }

                if (usedParts.Count > 0)
                {
                    await NeonDbService.AddMachineParts(machineId, usedParts);
                }

                MessageBox.Show("Machine and part consumption recorded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMachineName.Clear();
                await LoadMachinesAsync(); // Refresh machine list
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding the machine:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadMachinesAsync()
        {
            try
            {
                var machines = await NeonDbService.GetAllMachines();

                machineGrid.Rows.Clear();

                foreach (var machine in machines)
                {
                    machineGrid.Rows.Add(
                        machine["Id"]?.ToString(),
                        machine["Name"]?.ToString(),
                        machine["CreatedAt"]?.ToString()
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load machines: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
