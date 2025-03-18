using Npgsql;
using System;

namespace Rancher.Database
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = "Host=ep-round-fire-a1zbr4vw-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_IzLhFaDg3u8S;SslMode=Require";

        public static NpgsqlConnection GetConnection()
        {
            try
            {
                var connection = new NpgsqlConnection(ConnectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database connection failed: " + ex.Message);
                throw;
            }
        }
    }
}
