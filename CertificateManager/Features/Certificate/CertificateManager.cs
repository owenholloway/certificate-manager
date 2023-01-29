using System.Data.Entity;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertificateManager.Features.Stores;
using CertificateManager.Interfaces;
using CertificateManager.Interfaces.Stores;
using CertificateManager.Model.Certificates;

namespace CertificateManager.Features.Certificate;

public class CertificateController
{
    private IReadOnlyRepository ReadOnlyRepository { get; init; }
    private IReadWriteRepository ReadWriteRepository { get; init; }
    
    private static DateTimeOffset Now() => DateTimeOffset.Now.ToUniversalTime();
    
    public CertificateController(IReadOnlyRepository readOnlyRepository, IReadWriteRepository readWriteRepository)
    {
        ReadOnlyRepository = readOnlyRepository;
        ReadWriteRepository = readWriteRepository;
    }

    public void ResolveChainConfiguration()
    {
        var currentCertificates = ReadOnlyRepository
            .Table<RootCertificateAuthority, int>()
            .Count(ca => ca.ValidTill > Now());

        if (currentCertificates == 0)
        {
            GenerateNewCa();
        }
        
        var currentIntermediates = ReadOnlyRepository
            .Table<IntermediateCertificateAuthority, int>()
            .Count(ca => ca.ValidTill > Now());

        if (currentIntermediates == 0)
        {
            GenerateIntermediateCa();
        }
        
    }


    public void OutputCertificateChains()
    {
        var now = DateTimeOffset.Now.ToUniversalTime();
        var caChains = ReadOnlyRepository
            .Table<RootCertificateAuthority, int>()
            .Where(ca => ca.ValidTill > Now())
            .IncludeRelationships();
            
        var privateOutput 
            = Environment.GetEnvironmentVariable("CERT_PRIV_DIR");
        var publicOutput 
            = Environment.GetEnvironmentVariable("CERT_PUB_DIR");

        if (privateOutput == null) return;
        if (publicOutput == null) return;
        Directory.CreateDirectory(privateOutput);
        Directory.CreateDirectory(publicOutput);

        foreach (var certificateAuthority in caChains)
        {
           
            var caPublicFileData = new string(PemEncoding.Write("CERTIFICATE", certificateAuthority.CertificateData));
            File.WriteAllText($"{publicOutput}{certificateAuthority.CertificateName}.pem",caPublicFileData);
            
            var caPrivateFileData = new string(PemEncoding.Write("PRIVATE KEY", certificateAuthority.PrivateKey));
            File.WriteAllText($"{privateOutput}{certificateAuthority.CertificateName}.key.pem",caPrivateFileData);
            
            foreach (var intermediateCertificateAuthority in certificateAuthority.IntermediateCertificateAuthorities)
            {
                var intermediatePublicFileData = new string(PemEncoding.Write("CERTIFICATE", intermediateCertificateAuthority.CertificateData));
                File.WriteAllText($"{publicOutput}{intermediateCertificateAuthority.CertificateName}.pem",intermediatePublicFileData);

                var intermediateFullChainData =  intermediatePublicFileData  + "\n" + caPublicFileData;
                File.WriteAllText($"{publicOutput}{intermediateCertificateAuthority.CertificateName}.fullchain.pem",intermediateFullChainData);
            
                var intermediatePrivateFileData = new string(PemEncoding.Write("PRIVATE KEY", intermediateCertificateAuthority.PrivateKey));
                File.WriteAllText($"{privateOutput}{intermediateCertificateAuthority.CertificateName}.key.pem",intermediatePrivateFileData);
            }
            
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
        
        var ca = ReadOnlyRepository
            .Table<RootCertificateAuthority, int>()
            .IncludeRelationships()
            .FirstOrDefault(ca => ca.ValidTill > Now());

        if (ca == null) return;
        
        var certificate = IntermediateCertificateAuthority.Create(ca, "temp");

        ReadWriteRepository.Create<IntermediateCertificateAuthority, int>(certificate);
        ReadWriteRepository.Commit();
        
    }
}