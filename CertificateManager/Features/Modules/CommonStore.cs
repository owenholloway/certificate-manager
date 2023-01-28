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
            
            
            return optionsBuilder.Options;
        }).As<DbContextOptions>().InstancePerLifetimeScope();

        builder.RegisterType<CommonContext>()
            .As<DbContext>()
            .InstancePerLifetimeScope();

    }

}