using CertificateManager.Features.Stores;

namespace CertificateManager.Model.Certificates.Requests;

public class IssueRequest : Entity<int>
{
    
    public int IntermediateCaId { get; private set; }
    
    public virtual IntermediateCertificateAuthority 
        IntermediateCertificateAuthority { get; private set; }
    
    public string CertificateSigningRequest { get; private set; }

    public virtual List<IssuedCertificate> IssuedCertificates { get; private set; } = new();

}