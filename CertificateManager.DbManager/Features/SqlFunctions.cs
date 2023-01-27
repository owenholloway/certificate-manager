using Npgsql;
using Serilog;

namespace CertificateManager.DbManager.Features;

public class SqlFunctions
{
    private SqlConnection _connection { get; set; }
    
    public SqlFunctions(SqlConnection connection)
    {
        _connection = connection;
    }

    public bool RunBooleanQuery(string query, bool usedScoped = true)
    {
        Log.Verbose($"Running RunBooleanQuery: {query}");
        
        var result = false;
        
        GetConnection(usedScoped).Open();
        
        var command = new NpgsqlCommand(query, GetConnection(usedScoped));

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            result = reader.GetBoolean(0);
        }
        
        GetConnection(usedScoped).Close();

        return result;
    }


    public void RunNonQuery(string query, bool usedScoped = true)
    {        
        Log.Verbose($"Running RunNonQuery: {query}");

        GetConnection(usedScoped).Open();
        
        var command = new NpgsqlCommand(query, GetConnection(usedScoped));

        var result = command.ExecuteNonQuery();

        GetConnection(usedScoped).Close();
    }
    
    private NpgsqlConnection GetConnection(bool usedScoped = true)
    {
        return usedScoped ? _connection.DatabaseScopedConnection : _connection.DatabaseConnection;
    }
    
}