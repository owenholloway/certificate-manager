using Serilog;

namespace CertificateManager.Features.Certificate;

public class CertificateService
{
    private CertificateController CertificateController;
    
    public CertificateService(CertificateController certificateController)
    {
        CertificateController = certificateController;
    }

    public async Task Start()
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (await periodicTimer.WaitForNextTickAsync())
        {
            Log.Information("ResolveChainConfiguration");
            CertificateController.ResolveChainConfiguration();
            Log.Information("OutputCertificateChains");
            CertificateController.OutputCertificateChains();
        }
        
    }
    
    
}