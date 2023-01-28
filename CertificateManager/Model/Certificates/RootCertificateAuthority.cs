using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertificateManager.Features;

namespace CertificateManager.Model.Certificates;

public class RootCertificateAuthority : Entity<int>
{
    public string CertificateName { get; private set; }
    
    private RootCertificateAuthority()
    {
        
    }

    public static RootCertificateAuthority Create(string certificateName)
    {
        var obj = new RootCertificateAuthority();


        return obj;

    }
    
    private X509Certificate2 GenerateCA(string subjectName)
    {
        var keyPair = ECDsa.Create();
        var privateKey = keyPair.ExportECPrivateKey();
        var publicKey = keyPair.ExportSubjectPublicKeyInfo();

        var request = new CertificateRequest($"cn={subjectName}", keyPair, HashAlgorithmName.SHA512);
        
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true,false,0, true));

        var certificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));

        return certificate;
    }
    
}