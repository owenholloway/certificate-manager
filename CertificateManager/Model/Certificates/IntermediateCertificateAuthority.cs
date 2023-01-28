using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertificateManager.Dto;
using CertificateManager.Features;

namespace CertificateManager.Model.Certificates;

public class IntermediateCertificateAuthority : Certificate
{
    
    public int RootCaId { get; private set; }
    
    private IntermediateCertificateAuthority()
    {
        
    }

    public static IntermediateCertificateAuthority Create(
        RootCertificateAuthority rootCertificateAuthority, 
        string certificateName)
    {
        var obj = new IntermediateCertificateAuthority();

        var rootCa = rootCertificateAuthority.GetCertificate();

        var certificate = GenerateIntermediate(rootCa, certificateName);

        return obj;

    }
    
    private static CertificateDto GenerateIntermediate(X509Certificate2 ca, string subjectName)
    {
        var keyPair = ECDsa.Create();

        var request = new CertificateRequest($"cn={subjectName}", keyPair, HashAlgorithmName.SHA512);
        
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true,false,0, true));

        var certificate = request.Create(ca, DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1),new byte[]{0,1});

        AsymmetricAlgorithm key = certificate.GetRSAPrivateKey() ?? (AsymmetricAlgorithm) certificate.GetECDsaPrivateKey()!;
        
        var privateKey = key.ExportPkcs8PrivateKey();
        var publicKey = key.ExportSubjectPublicKeyInfo();
        
        return new CertificateDto()
        {
            CertificateName = subjectName,
            PrivateKey = privateKey,
            PublicKey = publicKey,
            X509Certificate = certificate
        };
    }
    
}