using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rancher.Database
{
    public static class NeonDbService
    {
        // 1. Add Inventory Item
        public static async Task AddInventoryItem(string itemNumber, string productName, int quantity, int supplierId, int redThreshold, int yellowThreshold, int greenThreshold)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();
                string query = @"
                    INSERT INTO inventory (item_number, product_name, quantity, supplier_id, red_threshold, yellow_threshold, green_threshold)
                    VALUES (@itemNumber, @productName, @quantity, @supplierId, @redThreshold, @yellowThreshold, @greenThreshold);";

                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                cmd.Parameters.AddWithValue("@productName", productName);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@supplierId", supplierId);
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

        // 2. Get All Inventory Items
        public static async Task<List<Dictionary<string, object>>> GetInventoryItems()
        {
            var items = new List<Dictionary<string, object>>();
            await using var conn = DatabaseHelper.GetConnection();

            try
            {
                await conn.OpenAsync();
                string query = @"
                    SELECT i.*, s.name AS supplier_name
                    FROM inventory i
                    LEFT JOIN suppliers s ON i.supplier_id = s.id;";

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
                        { "Supplier", reader["supplier_name"]?.ToString() ?? "Unknown" },
                        { "RedThreshold", reader.IsDBNull(reader.GetOrdinal("red_threshold")) ? 0 : reader.GetInt32(reader.GetOrdinal("red_threshold")) },
                        { "YellowThreshold", reader.IsDBNull(reader.GetOrdinal("yellow_threshold")) ? 0 : reader.GetInt32(reader.GetOrdinal("yellow_threshold")) },
                        { "GreenThreshold", reader.IsDBNull(reader.GetOrdinal("green_threshold")) ? 0 : reader.GetInt32(reader.GetOrdinal("green_threshold")) },
                        { "SupplierId", reader.IsDBNull(reader.GetOrdinal("supplier_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("supplier_id")) }
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

        // 3. Update Inventory Item
        public static async Task UpdateInventoryItem(string itemNumber, string productName, int quantity, int supplierId, int redThreshold, int yellowThreshold, int greenThreshold)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();

                string query = @"
                    UPDATE inventory
                    SET product_name = @productName, quantity = @quantity, supplier_id = @supplierId, 
                        red_threshold = @redThreshold, yellow_threshold = @yellowThreshold, green_threshold = @greenThreshold
                    WHERE item_number = @itemNumber;";

                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                cmd.Parameters.AddWithValue("@productName", productName);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@supplierId", supplierId);
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

        // 4. Delete Inventory Item
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

        // 5. Get Supplier ID by Name (or insert if not exists)
        public static async Task<int> GetOrInsertSupplierId(string name, string email, string phone, string note)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();

                string selectQuery = "SELECT id FROM suppliers WHERE name = @name LIMIT 1;";
                await using var selectCmd = new NpgsqlCommand(selectQuery, conn);
                selectCmd.Parameters.AddWithValue("@name", name);
                var result = await selectCmd.ExecuteScalarAsync();

                if (result != null)
                    return Convert.ToInt32(result);

                string insertQuery = "INSERT INTO suppliers (name, email, phone, note) VALUES (@name, @email, @phone, @note) RETURNING id;";
                await using var insertCmd = new NpgsqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@name", name);
                insertCmd.Parameters.AddWithValue("@email", email);
                insertCmd.Parameters.AddWithValue("@phone", phone);
                insertCmd.Parameters.AddWithValue("@note", note);

                return Convert.ToInt32(await insertCmd.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetOrInsertSupplierId: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }

        // 6. Get Supplier Info by Name
        public static async Task<Dictionary<string, string>?> GetSupplierByName(string name)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM suppliers WHERE name = @name LIMIT 1;";

                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Dictionary<string, string>
                    {
                        { "Name", reader["name"].ToString() ?? "" },
                        { "Email", reader["email"].ToString() ?? "" },
                        { "Phone", reader["phone"].ToString() ?? "" },
                        { "Note", reader["note"].ToString() ?? "" }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetSupplierByName: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }

            return null;
        }

        // 7. Update Supplier Info
        public static async Task UpdateSupplier(int id, string name, string email, string phone, string note)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();

                string query = @"
                    UPDATE suppliers
                    SET name = @name, email = @email, phone = @phone, note = @note
                    WHERE id = @id;";

                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@note", note);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] UpdateSupplier: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }
    }
}
