﻿using Npgsql;
using System.Data.Common;

namespace Test.App.Api.Todo.IntegrationTest.Helper
{
    public sealed class DbConnectionFactory(string connectionString, string database)
    {
        public DbConnection TodoDbConnection
        {
            get
            {
                return new NpgsqlConnection(connectionString);
            }
        }
    }
}
