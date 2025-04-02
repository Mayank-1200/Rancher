using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rancher.Database
{
    public static class NeonDbService
    {
        // **1. Insert New Item into Inventory**
        public static async Task AddInventoryItem(string itemNumber, string productName, int quantity, string supplier, int redThreshold, int yellowThreshold, int greenThreshold)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();

                // Insert without modifying actual_quantity (it should be handled separately if needed)
                string query = @"
                    INSERT INTO inventory (item_number, product_name, quantity, supplier, red_threshold, yellow_threshold, green_threshold)
                    VALUES (@itemNumber, @productName, @quantity, @supplier, @redThreshold, @yellowThreshold, @greenThreshold);";

                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                cmd.Parameters.AddWithValue("@productName", productName);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@supplier", supplier);
                cmd.Parameters.AddWithValue("@redThreshold", redThreshold);
                cmd.Parameters.AddWithValue("@yellowThreshold", yellowThreshold);
                cmd.Parameters.AddWithValue("@greenThreshold", greenThreshold);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] AddInventoryItem: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }

        // **2. Fetch All Items from Inventory**
        public static async Task<List<Dictionary<string, object>>> GetInventoryItems()
        {
            var items = new List<Dictionary<string, object>>();
            await using var conn = DatabaseHelper.GetConnection();

            try
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM inventory;";
                
                await using var cmd = new NpgsqlCommand(query, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var item = new Dictionary<string, object>
                    {
                        { "ItemNumber", reader["item_number"].ToString() },
                        { "ProductName", reader["product_name"].ToString() },
                        { "Quantity", reader.IsDBNull(reader.GetOrdinal("quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("quantity")) },
                        { "ActualQuantity", reader.IsDBNull(reader.GetOrdinal("actual_quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("actual_quantity")) },
                        { "Supplier", reader["supplier"].ToString() },
                        { "RedThreshold", reader.IsDBNull(reader.GetOrdinal("red_threshold")) ? 0 : reader.GetInt32(reader.GetOrdinal("red_threshold")) },
                        { "YellowThreshold", reader.IsDBNull(reader.GetOrdinal("yellow_threshold")) ? 0 : reader.GetInt32(reader.GetOrdinal("yellow_threshold")) },
                        { "GreenThreshold", reader.IsDBNull(reader.GetOrdinal("green_threshold")) ? 0 : reader.GetInt32(reader.GetOrdinal("green_threshold")) }
                    };
                    items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetInventoryItems: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }

            return items;
        }

        // **3. Modify an Existing Item (Does NOT update actual_quantity)**
        public static async Task UpdateInventoryItem(string itemNumber, string productName, int quantity, string supplier, int redThreshold, int yellowThreshold, int greenThreshold)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();

                string query = @"
                    UPDATE inventory
                    SET product_name = @productName, quantity = @quantity, supplier = @supplier, 
                        red_threshold = @redThreshold, yellow_threshold = @yellowThreshold, green_threshold = @greenThreshold
                    WHERE item_number = @itemNumber;";

                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                cmd.Parameters.AddWithValue("@productName", productName);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@supplier", supplier);
                cmd.Parameters.AddWithValue("@redThreshold", redThreshold);
                cmd.Parameters.AddWithValue("@yellowThreshold", yellowThreshold);
                cmd.Parameters.AddWithValue("@greenThreshold", greenThreshold);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] UpdateInventoryItem: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }

        // **4. Delete an Item**
        public static async Task DeleteInventoryItem(string itemNumber)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();
                string query = "DELETE FROM inventory WHERE item_number = @itemNumber;";
                
                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] DeleteInventoryItem: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }

        // **5. Fetch Single Item by Item Number**
        public static async Task<Dictionary<string, object>?> GetInventoryItemByItemNumber(string itemNumber)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM inventory WHERE item_number = @itemNumber;";
                
                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@itemNumber", itemNumber);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Dictionary<string, object>
                    {
                        { "ItemNumber", reader["item_number"].ToString() },
                        { "ProductName", reader["product_name"].ToString() },
                        { "Quantity", reader.IsDBNull(reader.GetOrdinal("quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("quantity")) },
                        { "ActualQuantity", reader.IsDBNull(reader.GetOrdinal("actual_quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("actual_quantity")) },
                        { "Supplier", reader["supplier"].ToString() },
                        { "RedThreshold", reader.IsDBNull(reader.GetOrdinal("red_threshold")) ? 0 : reader.GetInt32(reader.GetOrdinal("red_threshold")) },
                        { "YellowThreshold", reader.IsDBNull(reader.GetOrdinal("yellow_threshold")) ? 0 : reader.GetInt32(reader.GetOrdinal("yellow_threshold")) },
                        { "GreenThreshold", reader.IsDBNull(reader.GetOrdinal("green_threshold")) ? 0 : reader.GetInt32(reader.GetOrdinal("green_threshold")) }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetInventoryItemByItemNumber: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }

            return null;
        }
    }
}
