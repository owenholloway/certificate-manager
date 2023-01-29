using CertificateManager.Model.Certificates;
using CertificateManager.Model.Certificates.Requests;
using Microsoft.EntityFrameworkCore;

namespace CertificateManager.Features.Stores;

public static class CertificateMappings
{
    public static IQueryable<RootCertificateAuthority> IncludeRelationships(this IQueryable<RootCertificateAuthority> obj)
    {
        return obj
            .Include(ca => ca.IntermediateCertificateAuthorities)
            .ThenInclude(ica => ica.IssuedCertificates);
    }
    
    public static IQueryable<RootRequest> IncludeRelationships(this IQueryable<RootRequest> obj)
    {
        return obj
        .Include(ca => ca.RootCertificateAuthorities)
        .ThenInclude(rca => rca.IntermediateCertificateAuthorities);
    }
    
    public static IQueryable<IntermediateRequest> IncludeRelationships(this IQueryable<IntermediateRequest> obj)
    {
        return obj
            .Include(ir => ir.IntermediateCertificateAuthorities)
            .Include(ir => ir.RootCertificateAuthority)
            .ThenInclude(rca => rca.IntermediateCertificateAuthorities);
    }
    
    public static IQueryable<IssueRequest> IncludeRelationships(this IQueryable<IssueRequest> obj)
    {
        return obj
            .Include(ir => ir.IssuedCertificates)
            .Include(ir => ir.IntermediateCertificateAuthority);
    }
    
}