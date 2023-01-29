using CertificateManager.Features.Stores;

namespace CertificateManager.Model.Certificates.Requests;

public class IntermediateRequest : Entity<int>
{
    public string CertificateName { get; private set; } = string.Empty;

    public virtual List<IntermediateCertificateAuthority> 
        IntermediateCertificateAuthorities { get; private set; } = new();
    
    public int RootCaId { get; private set; }
    
    public virtual RootCertificateAuthority RootCertificateAuthority { get; set; }
    
    public bool KeepActive { get; private set; } = false;
}