using System.Data.Entity;
using CertificateManager.Features.Stores;
using CertificateManager.Interfaces;
using CertificateManager.Interfaces.Stores;
using CertificateManager.Model.Certificates;

namespace CertificateManager.Features.Certificate;

public class CertificateController
{
    private IReadOnlyRepository ReadOnlyRepository { get; init; }
    private IReadWriteRepository ReadWriteRepository { get; init; }
    
    public CertificateController(IReadOnlyRepository readOnlyRepository, IReadWriteRepository readWriteRepository)
    {
        ReadOnlyRepository = readOnlyRepository;
        ReadWriteRepository = readWriteRepository;
    }

    public void ResolveChainConfiguration()
    {
        var now = DateTimeOffset.Now.ToUniversalTime();
        var currentCertificates = ReadOnlyRepository
            .Table<RootCertificateAuthority, int>()
            .Count(ca => ca.ValidTill > now);

        if (currentCertificates == 0)
        {
            GenerateNewCa();
        }
        
        var currentIntermediates = ReadOnlyRepository
            .Table<IntermediateCertificateAuthority, int>()
            .Count(ca => ca.ValidTill > now);

        if (currentIntermediates == 0)
        {
            GenerateIntermediateCa();
        }
        
    }

    private void GenerateNewCa()
    {
        var caName = Environment.GetEnvironmentVariable("ROOT_CA_NAME");
        
        if (caName == null) return;
        
        var certificate = RootCertificateAuthority.Create(caName);
        
        ReadWriteRepository.Create<RootCertificateAuthority, int>(certificate);
        ReadWriteRepository.Commit();

    }

    private void GenerateIntermediateCa()
    {
        var now = DateTimeOffset.Now.ToUniversalTime();
        
        var ca = ReadOnlyRepository
            .Table<RootCertificateAuthority, int>()
            .IncludeRelationships()
            .FirstOrDefault(ca => ca.ValidTill > now);

        if (ca == null) return;
        
        var certificate = IntermediateCertificateAuthority.Create(ca, "temp");

        ReadWriteRepository.Create<IntermediateCertificateAuthority, int>(certificate);
        ReadWriteRepository.Commit();
        
    }
    
}