using Npgsql;
using System;

namespace Rancher.Database
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = "your_password_string";

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString); // Do NOT open the connection here
        }
    }
}
