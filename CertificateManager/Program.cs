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

var authority = RootCertificateAuthority.Create("Test Cert");

container.Resolve<IReadWriteRepository>().Create<RootCertificateAuthority, int>(authority);
container.Resolve<IReadWriteRepository>().Commit();

var ca = container.Resolve<IReadOnlyRepository>().GetById<RootCertificateAuthority, int>(1);

var intermediate = IntermediateCertificateAuthority.Create(ca, "Test Intermediate");

container.Resolve<IReadWriteRepository>().Create<IntermediateCertificateAuthority, int>(intermediate);
container.Resolve<IReadWriteRepository>().Commit();

