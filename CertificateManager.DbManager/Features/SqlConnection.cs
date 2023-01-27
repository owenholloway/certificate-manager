using Npgsql;

namespace CertificateManager.DbManager.Features;

public class SqlConnection
{
    
    public NpgsqlConnection DatabaseScopedConnection { get;  }
    public NpgsqlConnection DatabaseConnection { get; }

    public SqlConnection()
    {
        DatabaseScopedConnection = new NpgsqlConnection(GenerateScopedConnectionValue());
        DatabaseConnection = new NpgsqlConnection(GenerateConnectionValue());
    }

    private static string GenerateScopedConnectionValue()
    {
        var dbHost 
            = Environment.GetEnvironmentVariable("DB_HOST");
        var dbPort 
            = Environment.GetEnvironmentVariable("DB_PORT");
        var dbUser 
            = Environment.GetEnvironmentVariable("DB_USER");
        var dbPass 
            = Environment.GetEnvironmentVariable("DB_PASS");
        var dbDatabase 
            = Environment.GetEnvironmentVariable("DB_DATABASE");
        
        return $"Host={dbHost}:{dbPort};"
               +$"Username={dbUser};"
               +$"Password={dbPass};"
               +$"Database={dbDatabase};";
    }
    
    private static string GenerateConnectionValue()
    {
        var dbHost 
            = Environment.GetEnvironmentVariable("DB_HOST");
        var dbPort 
            = Environment.GetEnvironmentVariable("DB_PORT");
        var dbUser 
            = Environment.GetEnvironmentVariable("DB_USER");
        var dbPass 
            = Environment.GetEnvironmentVariable("DB_PASS");

        return $"Host={dbHost}:{dbPort};"
               +$"Username={dbUser};"
               +$"Password={dbPass};"
               +$"Database=postgres;";
    }
}