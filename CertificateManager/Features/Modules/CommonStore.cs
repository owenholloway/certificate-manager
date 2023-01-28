using Autofac;
using CertificateManager.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CertificateManager.Features.Modules;

public class CommonStore : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<ReadOnlyRepository>()
            .As<IReadOnlyRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<ReadWriteRepository>()
            .As<IReadWriteRepository>()
            .InstancePerLifetimeScope();

        builder.Register(c =>
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            
            optionsBuilder.LogTo(Log.Information);
            optionsBuilder.EnableSensitiveDataLogging();

            optionsBuilder.UseNpgsql(GenerateConnectionString());
            
            optionsBuilder.UseSnakeCaseNamingConvention();
            
            return optionsBuilder.Options;
        }).As<DbContextOptions>().InstancePerLifetimeScope();

        builder.RegisterType<CommonContext>()
            .As<DbContext>()
            .InstancePerLifetimeScope();

    }
    
    private static string GenerateConnectionString()
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

}