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

        // 8. Add To Inventory Quantity
        public static async Task AddToInventoryQuantity(string itemNumber, int additionalQuantity)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();
                string query = "UPDATE inventory SET quantity = quantity + @additionalQuantity WHERE item_number = @itemNumber;";
                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@additionalQuantity", additionalQuantity);
                cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] AddToInventoryQuantity: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }

        // 9. Add Machine
        public static async Task<int> AddMachine(string name)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();
                string query = "INSERT INTO machines (name, created_at) VALUES (@name, NOW()) RETURNING id;";
                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] AddMachine: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }

        // 10. Get Parts With Actual Quantity > 0
        public static async Task<List<Dictionary<string, object>>> GetPartsWithActualQuantity()
        {
            var items = new List<Dictionary<string, object>>();
            await using var conn = DatabaseHelper.GetConnection();

            try
            {
                await conn.OpenAsync();
                string query = "SELECT item_number, actual_quantity FROM inventory WHERE actual_quantity > 0;";
                await using var cmd = new NpgsqlCommand(query, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    items.Add(new Dictionary<string, object>
                    {
                        { "ItemNumber", reader["item_number"].ToString() },
                        { "ActualQuantity", Convert.ToInt32(reader["actual_quantity"]) }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetPartsWithActualQuantity: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }

            return items;
        }

        // 11. Subtract From Inventory
        public static async Task SubtractFromInventory(string itemNumber, int amount)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();
                string query = "UPDATE inventory SET quantity = quantity - @amount WHERE item_number = @itemNumber;";
                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SubtractFromInventory: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }

        // 12. Add Machine Parts
        public static async Task AddMachineParts(int machineId, List<(string itemNumber, int quantity)> parts)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();

                foreach (var (itemNumber, quantity) in parts)
                {
                    string query = @"
                        INSERT INTO machine_parts (machine_id, inventory_item_number, required_quantity)
                        VALUES (@machineId, @itemNumber, @quantity);";

                    await using var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@machineId", machineId);
                    cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                    cmd.Parameters.AddWithValue("@quantity", quantity);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] AddMachineParts: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }

        // 13. Get All Machines
        public static async Task<List<Dictionary<string, object>>> GetAllMachines()
        {
            var machines = new List<Dictionary<string, object>>();
            await using var conn = DatabaseHelper.GetConnection();

            try
            {
                await conn.OpenAsync();
                string query = "SELECT id, name, created_at FROM machines ORDER BY created_at DESC;";
                await using var cmd = new NpgsqlCommand(query, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    machines.Add(new Dictionary<string, object>
                    {
                        { "Id", reader["id"].ToString() ?? "" },
                        { "Name", reader["name"].ToString() ?? "" },
                        { "CreatedAt", Convert.ToDateTime(reader["created_at"]).ToString("yyyy-MM-dd HH:mm:ss") }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetAllMachines: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }

            return machines;
        }

        // 13. Delete Machine and Restore Inventory
        public static async Task DeleteMachineAndRestoreInventory(int machineId)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();

                // 1. Get all inventory parts and their actual quantities
                string selectInventoryQuery = "SELECT item_number, actual_quantity FROM inventory WHERE actual_quantity > 0;";
                List<(string itemNumber, int actualQty)> inventoryParts = new();

                await using (var cmd = new NpgsqlCommand(selectInventoryQuery, conn))
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string itemNumber = reader["item_number"].ToString() ?? "";
                        int actualQty = Convert.ToInt32(reader["actual_quantity"]);
                        if (!string.IsNullOrWhiteSpace(itemNumber) && actualQty > 0)
                        {
                            inventoryParts.Add((itemNumber, actualQty));
                        }
                    }
                }

                // 2. Add back actual quantities to inventory
                foreach (var (itemNumber, actualQty) in inventoryParts)
                {
                    string updateQuery = "UPDATE inventory SET quantity = quantity + @amount WHERE item_number = @itemNumber;";
                    await using var updateCmd = new NpgsqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@amount", actualQty);
                    updateCmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                    await updateCmd.ExecuteNonQueryAsync();
                }

                // 3. Delete the machine entry
                string deleteQuery = "DELETE FROM machines WHERE id = @id;";
                await using var deleteCmd = new NpgsqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@id", machineId);
                await deleteCmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] DeleteMachineAndRestoreInventory: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }

        // 14. Get All Suppliers
        public static async Task<List<Dictionary<string, string>>> GetAllSuppliers()
        {
            var suppliers = new List<Dictionary<string, string>>();
            await using var conn = DatabaseHelper.GetConnection();

            try
            {
                await conn.OpenAsync();
                string query = "SELECT id, name, email, phone, note FROM suppliers ORDER BY name ASC;";
                await using var cmd = new NpgsqlCommand(query, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    suppliers.Add(new Dictionary<string, string>
                    {
                        { "Id", reader["id"].ToString() ?? "" },
                        { "Name", reader["name"].ToString() ?? "" },
                        { "Email", reader["email"].ToString() ?? "" },
                        { "Phone", reader["phone"].ToString() ?? "" },
                        { "Note", reader["note"].ToString() ?? "" }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetAllSuppliers: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }

            return suppliers;
        }

        
        // 15. Insert Entry Log (Inward/Outward) - UPDATED
        public static async Task InsertEntryLog(string itemNumber, int quantity, string entryType, string supplierName, string supplierNote)
        {
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();

                string query = @"
                    INSERT INTO entry_logs 
                    (item_number, quantity, entry_type, supplier_name, supplier_note, created_at)
                    VALUES 
                    (@itemNumber, @quantity, @entryType, @supplierName, @supplierNote, NOW());";

                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@itemNumber", itemNumber);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@entryType", entryType);
                cmd.Parameters.AddWithValue("@supplierName", supplierName);
                cmd.Parameters.AddWithValue("@supplierNote", supplierNote);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] InsertEntryLog: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }

        // 16 Search Suppliers by Partial Name (NEW)
        public static async Task<List<string>> SearchSuppliersAsync(string partialName)
        {
            var supplierNames = new List<string>();
            await using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();
                string query = @"
                    SELECT name
                    FROM suppliers
                    WHERE name ILIKE @partialName
                    ORDER BY name ASC
                    LIMIT 10;";

                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@partialName", $"%{partialName}%");

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    supplierNames.Add(reader["name"].ToString() ?? "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SearchSuppliersAsync: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }

            return supplierNames;
        }        

        // 17. Get Entry Logs (Optional Type Filter: "inward" or "outward")
        public static async Task<List<Dictionary<string, object>>> GetEntryLogs(string? type = null)
        {
            var logs = new List<Dictionary<string, object>>();
            await using var conn = DatabaseHelper.GetConnection();

            try
            {
                await conn.OpenAsync();

                string query = @"
                    SELECT id, item_number, quantity, entry_type, supplier_name, supplier_email, supplier_phone, supplier_note, created_at
                    FROM entry_logs";

                if (!string.IsNullOrWhiteSpace(type))
                {
                    query += " WHERE entry_type = @type";
                }

                query += " ORDER BY created_at DESC;";

                await using var cmd = new NpgsqlCommand(query, conn);
                if (!string.IsNullOrWhiteSpace(type))
                {
                    cmd.Parameters.AddWithValue("@type", type);
                }

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    logs.Add(new Dictionary<string, object>
                    {
                        { "Id", reader["id"] },
                        { "ItemNumber", reader["item_number"] },
                        { "Quantity", reader["quantity"] },
                        { "EntryType", reader["entry_type"] },
                        { "SupplierName", reader["supplier_name"] },
                        { "SupplierEmail", reader["supplier_email"] },
                        { "SupplierPhone", reader["supplier_phone"] },
                        { "SupplierNote", reader["supplier_note"] },
                        { "CreatedAt", Convert.ToDateTime(reader["created_at"]).ToString("yyyy-MM-dd HH:mm:ss") }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetEntryLogs: {ex.Message}");
                throw;
            }
            finally
            {
                await conn.DisposeAsync();
            }

            return logs;
        }
    }
}
