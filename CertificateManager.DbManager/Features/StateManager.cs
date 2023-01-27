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
            _functions.RunNonQuery($"drop database {Environment.GetEnvironmentVariable("DB_DATABASE")};",false);
            hasDatabase = false;
        }

        if (!hasDatabase)
        {
            Log.Information($"✨✨  New database will be created for {Environment.GetEnvironmentVariable("DB_DATABASE")} ✨✨  ");
            _functions.RunNonQuery($"create database {Environment.GetEnvironmentVariable("DB_DATABASE")};",false);
        }
        
    }
    
}