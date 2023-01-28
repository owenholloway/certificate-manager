using Autofac;
using CertificateManager.Features;
using CertificateManager.Features.Modules;
using CertificateManager.Interfaces;
using CertificateManager.Model.Certificates;
using Serilog;

Log.Logger = Logging.CreateLogger();

var builder = new ContainerBuilder();

builder.RegisterModule<CommonStore>();

var container = builder.Build();

// var authority = RootCertificateAuthority.Create("Test Cert");

// container.Resolve<IReadWriteRepository>().Create<RootCertificateAuthority, int>(authority);
// container.Resolve<IReadWriteRepository>().Commit();

var cert = container.Resolve<IReadOnlyRepository>().GetById<RootCertificateAuthority, int>(1).GetCertificate();



Log.Information("Get Cert");