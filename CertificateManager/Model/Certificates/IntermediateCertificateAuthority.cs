using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertificateManager.Features;

namespace CertificateManager.Model.Certificates;

public class IntermediateCertificateAuthority : Entity<int>
{
    public string CertificateName { get; private set; }
    
    private IntermediateCertificateAuthority()
    {
        
    }

    public static IntermediateCertificateAuthority Create(string certificateName)
    {
        var obj = new IntermediateCertificateAuthority();


        return obj;

    }
    
    private X509Certificate2 GenerateIntermediate(X509Certificate2 ca, string subjectName)
    {
        var keyPair = ECDsa.Create();
        var privateKey = keyPair.ExportECPrivateKey();
        var publicKey = keyPair.ExportSubjectPublicKeyInfo();
        
        var request = new CertificateRequest($"cn={subjectName}", keyPair, HashAlgorithmName.SHA512);

        var certificate = request.Create(ca, DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1),new byte[]{0,1});
        
        return certificate;
        
    }
    
}