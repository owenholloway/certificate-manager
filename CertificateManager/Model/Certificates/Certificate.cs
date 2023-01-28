using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
using CertificateManager.Features;

namespace CertificateManager.Model.Certificates;

public class Certificate : Entity<int>
{
    public string CertificateName { get; protected set; }
    
    public byte[] PrivateKey { get; protected set; }
    public byte[] PublicKey { get; protected set; }
    public byte[] CertificateData { get; protected set; }
    public byte[] SerialNo { get; protected set; }
    
    public DateTimeOffset ValidFrom { get; protected set; }
    public DateTimeOffset ValidTill { get; protected set; }
    
}