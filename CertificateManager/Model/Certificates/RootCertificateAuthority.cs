using CertificateManager.Features;

namespace CertificateManager.Model.Certificates;

public class RootCertificateAuthority : Entity<int>
{
    private RootCertificateAuthority()
    {
        
    }

    public static RootCertificateAuthority Create()
    {
        var obj = new RootCertificateAuthority();


        return obj;

    }
    
}