using CertificateManager.Model.Certificates;
using Microsoft.EntityFrameworkCore;

namespace CertificateManager.Features.Stores;

public static class CertificateMappings
{
    public static IQueryable<RootCertificateAuthority> IncludeRelationships(this IQueryable<RootCertificateAuthority> obj)
    {
        return obj.Include(ca => ca.IntermediateCertificateAuthorities);
    }
}