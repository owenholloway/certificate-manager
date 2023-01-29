using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CertificateManager.Dto;
using CertificateManager.Features;

namespace CertificateManager.Model.Certificates;

public class RootCertificateAuthority : Certificate
{
    
    public virtual List<IntermediateCertificateAuthority> IntermediateCertificateAuthorities { get; private set; }
    
    private RootCertificateAuthority()
    {
        
    }

    public static RootCertificateAuthority Create(string certificateName)
    {
        var obj = new RootCertificateAuthority();

        var certificate = GenerateCertificateAuthority(certificateName);

        obj.CertificateName = certificateName;
        
        obj.PrivateKey = certificate.PrivateKey;
        obj.PublicKey = certificate.PublicKey;
        obj.CertificateData = certificate.X509Certificate.RawData;
        obj.SerialNo = Encoding.ASCII.GetBytes(certificate.X509Certificate.SerialNumber);

        obj.ValidFrom = certificate.X509Certificate.NotBefore.ToUniversalTime();
        obj.ValidTill = certificate.X509Certificate.NotAfter.ToUniversalTime();
        
        return obj;

    }

    public X509Certificate2 GetCertificate()
    {
        return X509Certificate2.CreateFromPem(
            PemEncoding.Write("CERTIFICATE", CertificateData),
            PemEncoding.Write("PRIVATE KEY", PrivateKey));

    }
    
    private static CertificateDto GenerateCertificateAuthority(string subjectName)
    {
        var keyPair = ECDsa.Create();

        var request = new CertificateRequest($"cn={subjectName}", keyPair, HashAlgorithmName.SHA512);
        
        request
            .CertificateExtensions
            .Add(new X509BasicConstraintsExtension(
                true,
                true,
                4, 
                true));

        var certificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(6));

        var privateKey = keyPair.ExportPkcs8PrivateKey();
        var publicKey = keyPair.ExportSubjectPublicKeyInfo();
        
        return new CertificateDto()
        {
            CertificateName = subjectName,
            PrivateKey = privateKey,
            PublicKey = publicKey,
            X509Certificate = certificate
        };
    }
    
}