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

    public void CheckIfScriptRun(SqlScript script)
    {
        var query = $"select exists ( select from schema_versions where name = '{script.Filename}' )";
        
        Log.Information($"🟦 Checking SQL File -- {script.Filename} -- ");
        
        script.HasBeenRun = RunBooleanQuery(query);

        if (script.HasBeenRun) Log.Information($"🟧 Already Run -- {script.Filename} -- ");

    }

    public void RunScript(SqlScript script)
    {
        if (script.HasBeenRun) return;

        GetConnection().Open();

        try
        {
            var begin = new NpgsqlCommand("begin;", GetConnection());
            begin.ExecuteNonQuery();

            var scriptContent = script.GetScriptContent();
            
            var scriptFunction = new NpgsqlCommand(scriptContent, GetConnection());
            scriptFunction.ExecuteNonQuery();
            
            var commit = new NpgsqlCommand("commit;", GetConnection());
            commit.ExecuteNonQuery();
            
            var recordCommandString = $"insert into schema_versions values ('{script.Filename}',now())";
            var recordCommand = new NpgsqlCommand(recordCommandString, GetConnection());
            recordCommand.ExecuteNonQuery();
            
            Log.Information($"🟩 Applied to Schema -- {script.Filename} -- ");
            
        }
        catch (Exception e)
        {
            //Log.Error(e.StackTrace);
            
            var rollback = new NpgsqlCommand("rollback;", GetConnection());
            rollback.ExecuteNonQuery();
            
            
            Log.Information($"🟥 Failed Script     -- {script.Filename} -- ");
        }
        
        GetConnection().Close();
        
        script.HasBeenRun = true;
        
        
    }
    
    private NpgsqlConnection GetConnection(bool usedScoped = true)
    {
        return usedScoped ? _connection.DatabaseScopedConnection : _connection.DatabaseConnection;
    }
    
}