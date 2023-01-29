using CertificateManager.Features.Stores;

namespace CertificateManager.Model.Certificates.Requests;

public class RootRequest : Entity<int>
{
    public string CertificateName { get; private set; } = string.Empty;

    public virtual List<RootCertificateAuthority> 
        RootCertificateAuthorities { get; private set; } = new();

    public bool KeepActive { get; private set; } = false;

}