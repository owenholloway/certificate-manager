using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertificateManager.Dto;
using CertificateManager.Features;
using CertificateManager.Features.Certificate;

namespace CertificateManager.Model.Certificates;

public class IntermediateCertificateAuthority : Certificate
{
    public int RootCaId { get; private set; }
    
    public int IntermediateRequestId { get; private set; }

    public virtual List<IssuedCertificate> IssuedCertificates { get; private set; } = new();

    protected IntermediateCertificateAuthority()
    {
        
    }

    public static IntermediateCertificateAuthority Create(
        int intermediateRequestId,
        RootCertificateAuthority rootCertificateAuthority, 
        string certificateName)
    {
        var obj = new IntermediateCertificateAuthority();

        var rootCa = rootCertificateAuthority.GetCertificate();

        obj.RootCaId = rootCertificateAuthority.Id;
        
        var certificate = GenerateIntermediate(rootCa, certificateName);
        
        obj.CertificateName = certificateName;

        obj.IntermediateRequestId = intermediateRequestId;
        
        obj.PrivateKey = Encryption.Encrypt(certificate.PrivateKey);
        obj.PublicKey = certificate.PublicKey;
        obj.CertificateData = certificate.X509Certificate.RawData;
        obj.SerialNo = certificate.SerialNo;

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
    
    private static CertificateDto GenerateIntermediate(X509Certificate2 ca, string subjectName)
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
                3, 
                true));

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
            .Create(ca, DateTimeOffset.Now, DateTimeOffset.Now.AddYears(3), serialNo);

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