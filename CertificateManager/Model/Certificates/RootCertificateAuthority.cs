using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CertificateManager.Dto;
using CertificateManager.Features.Certificate;

namespace CertificateManager.Model.Certificates;

public class RootCertificateAuthority : Certificate
{
    public int RootRequestId { get; private set; }
    
    public virtual List<IntermediateCertificateAuthority> IntermediateCertificateAuthorities { get; private set; } = new();
    

    protected RootCertificateAuthority()
    {
        
    }

    public static RootCertificateAuthority Create(int rootRequestId, string certificateName)
    {
        var obj = new RootCertificateAuthority();

        var certificate = GenerateCertificateAuthority(certificateName);

        obj.CertificateName = certificateName;

        obj.RootRequestId = rootRequestId;
        obj.PrivateKey = Encryption.Encrypt(certificate.PrivateKey);
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
            PemEncoding.Write("PRIVATE KEY", Encryption.Decrypt(PrivateKey)));

    }
    
    private static CertificateDto GenerateCertificateAuthority(string subjectName)
    {
        var keyPair = RSA.Create(2048);

        var request = new CertificateRequest(
            $"cn={subjectName}", 
            keyPair, 
            HashAlgorithmName.SHA512,
            RSASignaturePadding.Pkcs1);
        
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