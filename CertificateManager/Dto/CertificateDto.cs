using System.Security.Cryptography.X509Certificates;

namespace CertificateManager.Dto;

public class CertificateDto
{
    public string CertificateName { get; set; }
    
    public byte[] PrivateKey { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] SerialNo { get; set; }
    
    public X509Certificate2 X509Certificate { get; set; }
}