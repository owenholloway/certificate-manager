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

        obj.RootCaId = rootCertificateAuthority.Id;
        
        var certificate = GenerateIntermediate(rootCa, certificateName);
        
        obj.CertificateName = certificateName;
        
        obj.PrivateKey = certificate.PrivateKey;
        obj.PublicKey = certificate.PublicKey;
        obj.CertificateData = certificate.X509Certificate.RawData;
        obj.SerialNo = certificate.SerialNo;

        obj.ValidFrom = certificate.X509Certificate.NotBefore.ToUniversalTime();
        obj.ValidTill = certificate.X509Certificate.NotAfter.ToUniversalTime();

        return obj;

    }
    
    private static CertificateDto GenerateIntermediate(X509Certificate2 ca, string subjectName)
    {
        var keyPair = ECDsa.Create();

        var request = new CertificateRequest($"cn={subjectName}", keyPair, HashAlgorithmName.SHA512);
        
        request
            .CertificateExtensions
            .Add(new X509BasicConstraintsExtension(
                true,
                false,
                0, 
                false));

        var timestamp = DateTime.Now;

        var serialNo = new[]
        {
            (byte)timestamp.Year,
            (byte)timestamp.Month,
            (byte)timestamp.Day,
            (byte)timestamp.Hour,
            (byte)timestamp.Minute,
            (byte)timestamp.Second,
            (byte)timestamp.Millisecond
        };
        
        var certificate = request
            .Create(ca, DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), serialNo);

        var privateKey = keyPair.ExportPkcs8PrivateKey();
        var publicKey = keyPair.ExportSubjectPublicKeyInfo();
        
        return new CertificateDto()
        {
            CertificateName = subjectName,
            PrivateKey = privateKey,
            PublicKey = publicKey,
            SerialNo = serialNo,
            X509Certificate = certificate
        };
    }
    
}