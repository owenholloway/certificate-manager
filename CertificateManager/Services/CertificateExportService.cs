using CertificateManager.Features.Certificate;
using CertificateManager.Interfaces.Stores;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CertificateManager.Services;

public class CertificateExportService : IHostedService
{
    
    private IReadOnlyRepository ReadOnlyRepository { get; init; }
    private IReadWriteRepository ReadWriteRepository { get; init; }
    private CertificateController CertificateController { get; init; }

    private readonly PeriodicTimer _timer;
    
    public CertificateExportService(
        IReadOnlyRepository readOnlyRepository, 
        IReadWriteRepository readWriteRepository, 
        CertificateController certificateController)
    {
        ReadOnlyRepository = readOnlyRepository;
        ReadWriteRepository = readWriteRepository;
        CertificateController = certificateController;

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (
            await _timer.WaitForNextTickAsync(cancellationToken) 
            && !cancellationToken.IsCancellationRequested)
        {
            Log.Information("Running new resolver");
            CertificateController.ResolveChainConfiguration();
            CertificateController.OutputCertificateChains();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}