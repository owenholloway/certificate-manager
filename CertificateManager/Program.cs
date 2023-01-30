using Autofac;
using CertificateManager.Features;
using CertificateManager.Features.Certificate;
using CertificateManager.Features.Modules;
using Serilog;

Log.Logger = Logging.CreateLogger();

var builder = new ContainerBuilder();

builder.RegisterModule<CommonStore>();

builder
    .RegisterType<CertificateController>()
    .InstancePerDependency();

builder
    .RegisterType<CertificateService>()
    .InstancePerDependency();

var container = builder.Build();

await container.Resolve<CertificateService>().Start();
