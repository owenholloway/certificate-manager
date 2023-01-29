using CertificateManager.Model.Certificates;
using CertificateManager.Model.Certificates.Requests;
using Microsoft.EntityFrameworkCore;

namespace CertificateManager.Features.Stores;

public class CommonContext : DbContext
{
    
    public CommonContext(DbContextOptions options) : base(options)
    {
    }
    
    
    public DbSet<RootRequest> RootRequests { get; set; }
    public DbSet<RootCertificateAuthority> RootCertificateAuthorities { get; set; }
    
    public DbSet<IntermediateRequest> IntermediateRequests { get; set; }
    public DbSet<IntermediateCertificateAuthority> IntermediateCertificateAuthorities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RootRequest>()
            .HasMany<RootCertificateAuthority>(rca => rca.RootCertificateAuthorities)
            .WithOne()
            .HasForeignKey(rca => rca.RootRequestId);
        
        modelBuilder.Entity<RootCertificateAuthority>()
            .HasMany<IntermediateCertificateAuthority>(rca => rca.IntermediateCertificateAuthorities)
            .WithOne()
            .HasForeignKey(ica => ica.RootCaId);

        modelBuilder.Entity<IntermediateRequest>()
            .HasMany<IntermediateCertificateAuthority>(ir => ir.IntermediateCertificateAuthorities)
            .WithOne()
            .HasForeignKey(ica => ica.IntermediateRequestId);

        modelBuilder.Entity<IntermediateRequest>()  
            .HasOne<RootCertificateAuthority>(ir => ir.RootCertificateAuthority)
            .WithOne()
            .HasForeignKey<IntermediateRequest>(ir => ir.RootCaId);
        
        base.OnModelCreating(modelBuilder);
    }

}