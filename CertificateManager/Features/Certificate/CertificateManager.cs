using System.Security.Cryptography;
using CertificateManager.Features.Stores;
using CertificateManager.Interfaces.Stores;
using CertificateManager.Model.Certificates;
using CertificateManager.Model.Certificates.Requests;
using Serilog;

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
        var openRootRequests = ReadWriteRepository
            .Table<RootRequest, int>()
            .IncludeRelationships()
            .Where(rr => 
                rr.RootCertificateAuthorities
                    .Count(rca => rca.ValidTill > Now()) == 0);
        
        foreach (var openRequest in openRootRequests)
        {       
            var certificate = RootCertificateAuthority.Create(openRequest.Id, openRequest.CertificateName);
        
            ReadWriteRepository.Create<RootCertificateAuthority, int>(certificate);

            Thread.Sleep(200);
            
        }
        
        ReadWriteRepository.Commit();
        
        Thread.Sleep(200);
        
        var openIntermediateRequests = ReadWriteRepository
            .Table<IntermediateRequest, int>()
            .IncludeRelationships()
            .Where(rr => 
                rr.IntermediateCertificateAuthorities
                    .Count(rca => rca.ValidTill > Now()) == 0);
        
        foreach (var openRequest in openIntermediateRequests)
        {
            var certificate = IntermediateCertificateAuthority
                .Create(
                    openRequest.Id, 
                    openRequest.RootCertificateAuthority, 
                    openRequest.CertificateName);
        
            ReadWriteRepository.Create<IntermediateCertificateAuthority, int>(certificate);

            Thread.Sleep(200);
        }
        
        ReadWriteRepository.Commit();
        
    }

    
    public void OutputCertificateChains()
    {   
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
    
}