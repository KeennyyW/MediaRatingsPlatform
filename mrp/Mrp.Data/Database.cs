using System;
using System.Data;
using Microsoft.VisualBasic;
using Npgsql;

namespace Mrp.Data;

public class Database
{

    public string GetConnectionString()
    {
        
        string? db_user = Environment.GetEnvironmentVariable("POSTGRES_USER");
        string? db_name = Environment.GetEnvironmentVariable("POSTGRES_DB");
        string? db_password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

        if (db_name == null || db_password== null  || db_user== null  )
        {
            throw new Exception("failed to recieve Database credentials!");
        }

        var connString = $"Host=localhost;Port=5432;Username={db_user};Password={db_password};Database={db_name}";

        return connString;
        
    }
}
