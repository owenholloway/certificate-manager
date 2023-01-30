using Autofac;
using CertificateManager.Features.Stores;
using CertificateManager.Interfaces;
using CertificateManager.Interfaces.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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

            var factory = new LoggerFactory().AddSerilog(Log.Logger);
            
            optionsBuilder.UseLoggerFactory(factory);
            //optionsBuilder.EnableSensitiveDataLogging();
            //optionsBuilder.EnableDetailedErrors();

            optionsBuilder.UseNpgsql(GenerateConnectionString());
            
            optionsBuilder.UseSnakeCaseNamingConvention();
            
            var options = new MemoryCacheOptions
            {
                ExpirationScanFrequency = TimeSpan.FromMilliseconds(100)
            };

            var cache = new MemoryCache(options);

            optionsBuilder.UseMemoryCache(cache);
            
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