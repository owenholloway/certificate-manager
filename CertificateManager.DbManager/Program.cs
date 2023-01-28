using Autofac;
using CertificateManager.DbManager.Features;
using Serilog;

Log.Logger = Logging.CreateLogger();

var builder = new ContainerBuilder();

Log.Debug("RegisterType SqlConnection as SingleInstance");
builder
    .RegisterType<SqlConnection>()
    .SingleInstance();

Log.Debug("RegisterType SqlFunctions as SingleInstance");
builder
    .RegisterType<SqlFunctions>()
    .SingleInstance();

Log.Debug("RegisterType StateManager as SingleInstance");
builder
    .RegisterType<StateManager>()
    .SingleInstance();

var container = builder.Build();

Log.Information("🎩 Starting schema update, hold onto your hats 🎩");

container.Resolve<StateManager>().ResolveDatabaseState();
container.Resolve<StateManager>().ResolveSchemaState();
