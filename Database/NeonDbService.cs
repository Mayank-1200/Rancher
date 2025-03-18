using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rancher.Database
{
    public static class NeonDbService
    {
        // **1. Insert New Item into Inventory**
        public static async Task AddInventoryItem(string itemNumber, string productName, int quantity, string supplier)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = @"
                        INSERT INTO inventory (item_number, product_name, quantity, supplier)
                        VALUES (@itemNumber, @productName, @quantity, @supplier);";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                        cmd.Parameters.AddWithValue("@productName", productName);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@supplier", supplier);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting inventory item: " + ex.Message);
                throw;
            }
        }

        // **2. Fetch All Items from Inventory**
        public static async Task<List<InventoryItem>> GetInventoryItems()
        {
            List<InventoryItem> items = new List<InventoryItem>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM inventory;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(new InventoryItem
                            {
                                ItemNumber = reader["item_number"].ToString(),
                                ProductName = reader["product_name"].ToString(),
                                Quantity = Convert.ToInt32(reader["quantity"]),
                                Supplier = reader["supplier"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching inventory items: " + ex.Message);
                throw;
            }

            return items;
        }

        // **3. Modify an Existing Item**
        public static async Task UpdateInventoryItem(string itemNumber, string productName, int quantity, string supplier)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = @"
                        UPDATE inventory
                        SET product_name = @productName, quantity = @quantity, supplier = @supplier
                        WHERE item_number = @itemNumber;";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                        cmd.Parameters.AddWithValue("@productName", productName);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@supplier", supplier);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating inventory item: " + ex.Message);
                throw;
            }
        }

        // **4. Delete an Item**
        public static async Task DeleteInventoryItem(string itemNumber)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = "DELETE FROM inventory WHERE item_number = @itemNumber;";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting inventory item: " + ex.Message);
                throw;
            }
        }

                // **5. Fetch Single Item by Item Number**
        public static async Task<InventoryItem> GetInventoryItemByItemNumber(string itemNumber)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM inventory WHERE item_number = @itemNumber;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@itemNumber", itemNumber);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new InventoryItem
                                {
                                    ItemNumber = reader["item_number"].ToString(),
                                    ProductName = reader["product_name"].ToString(),
                                    Quantity = Convert.ToInt32(reader["quantity"]),
                                    Supplier = reader["supplier"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching inventory item: " + ex.Message);
                throw;
            }

            return null;
        }

    }

    // **Helper Model for Data Handling**
    public class InventoryItem
    {
        public string ItemNumber { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Supplier { get; set; }
    }
}
