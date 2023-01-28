using Serilog;

namespace CertificateManager.DbManager.Features;

public class StateManager
{

    private SqlFunctions _functions;
    private bool _resetDatabase = false;
    
    public StateManager(SqlFunctions functions)
    {
        _functions = functions;
        
        var resetDb 
            = Environment.GetEnvironmentVariable("DB_CLEAN");
        if (resetDb != null) _resetDatabase = bool.Parse(resetDb);
    }

    public void ResolveDatabaseState()
    {
        var checkDbQuery =
            $"select exists(" +
            $"select datname from pg_catalog.pg_database " +
            $"where lower(datname) = lower('{Environment.GetEnvironmentVariable("DB_DATABASE")}'));";
        
        var hasDatabase = _functions
            .RunBooleanQuery(checkDbQuery, false);

        if (hasDatabase && !_resetDatabase)
        {
            Log.Information($"Database {Environment.GetEnvironmentVariable("DB_DATABASE")} exists, ready to run schema update");
            return;
        }

        if (hasDatabase && _resetDatabase)
        {
            Log.Information($"Found database {Environment.GetEnvironmentVariable("DB_DATABASE")} and reset requested");
            Log.Warning("💣💣 Database reset has been requested, will destroy everything 💣💣");
            _functions.RunNonQuery($"drop database {Environment.GetEnvironmentVariable("DB_DATABASE")} with (FORCE);",false);
            hasDatabase = false;
        }

        if (!hasDatabase)
        {
            Log.Information($"✨✨  New {Environment.GetEnvironmentVariable("DB_DATABASE")} database will be created  ✨✨  ");
            _functions
                .RunNonQuery($"create database {Environment.GetEnvironmentVariable("DB_DATABASE")};",false);
            _functions
                .RunNonQuery("create table " +
                             "schema_versions(" +
                             "name varchar(128) not null," +
                             "executed timestamp with time zone default current_timestamp)");
        }
        
    }

    public void ResolveSchemaState()
    {
        const string structurePath = "./SQL/01_structure/";
        
        var files = Directory.GetFiles(structurePath);

        var scripts = 
            files
                .Select(file => 
                    new SqlScript(structurePath, 
                        file.Replace(structurePath, "")))
                .ToList();

        foreach (var script in scripts)
        {
            _functions.CheckIfScriptRun(script);
            _functions.RunScript(script);
        }
        
    }
    
}