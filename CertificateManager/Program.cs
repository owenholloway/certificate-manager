using CertificateManager.Features;
using Microsoft.Extensions.Hosting;
using Serilog;

Log.Logger = Logging.CreateLogger();

Host.CreateDefaultBuilder(args)
    .UseSerilog(Log.Logger)
    .ConfigureServices(ServiceManager.Configure)
    .Build()
    .Run();
    
// var builder = new ContainerBuilder();
//
// builder.RegisterModule<CommonStore>();
//
// builder
//     .RegisterType<CertificateController>()
//     .InstancePerDependency();
//
// builder
//     .RegisterType<CertificateService>()
//     .InstancePerDependency();
//
// var container = builder.Build();

//container.Resolve<CertificateController>().OutputCertificateChains();

//await container.Resolve<CertificateService>().Start();


