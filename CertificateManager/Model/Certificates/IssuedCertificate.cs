using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CertificateManager.Dto;
using CertificateManager.Features.Certificate;

namespace CertificateManager.Model.Certificates;

public class IssuedCertificate : Certificate
{
    
    public int IssueRequestId { get; private set; }
    public int IntermediateCaId { get; private set; }


    public static IssuedCertificate Create(
        int issueRequestId,
        IntermediateCertificateAuthority intermediateCertificateAuthority, 
        string certificateSigningRequest)
    {
        var obj = new IssuedCertificate();

        var certificate = GenerateCertificate(
            intermediateCertificateAuthority.GetCertificate(), 
            certificateSigningRequest);

        obj.IssueRequestId = issueRequestId;
        obj.IntermediateCaId = intermediateCertificateAuthority.Id;
        
        
        obj.CertificateName = certificate.CertificateName;
        
        obj.PrivateKey = Encryption.Encrypt(certificate.PrivateKey);
        obj.PublicKey = certificate.PublicKey;
        obj.CertificateData = certificate.X509Certificate.RawData;
        obj.SerialNo = Encoding.ASCII.GetBytes(certificate.X509Certificate.SerialNumber);

        obj.ValidFrom = certificate.X509Certificate.NotBefore.ToUniversalTime();
        obj.ValidTill = certificate.X509Certificate.NotAfter.ToUniversalTime();
        
        return obj;
    }
    
    
    private static CertificateDto GenerateCertificate(X509Certificate2 ca, string certificateSigningRequest)
    {
        var keyPair = ECDsa.Create();

        var request = CertificateRequest.LoadSigningRequestPem(
            certificateSigningRequest, 
            HashAlgorithmName.SHA512,
            CertificateRequestLoadOptions.Default,
            RSASignaturePadding.Pkcs1);

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
            CertificateName = request.SubjectName.Name,
            PrivateKey = privateKey,
            PublicKey = publicKey,
            SerialNo = serialNo,
            X509Certificate = certificate
        };
    }

}