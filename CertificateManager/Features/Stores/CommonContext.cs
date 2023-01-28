using CertificateManager.Model.Certificates;
using Microsoft.EntityFrameworkCore;

namespace CertificateManager.Features.Stores;

public class CommonContext : DbContext
{
    
    public CommonContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<RootCertificateAuthority> RootCertificateAuthorities { get; set; }
    public DbSet<IntermediateCertificateAuthority> IntermediateCertificateAuthorities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RootCertificateAuthority>()
            .HasMany<IntermediateCertificateAuthority>(rca => rca.IntermediateCertificateAuthorities)
            .WithOne()
            .HasForeignKey(ica => ica.RootCaId);

        base.OnModelCreating(modelBuilder);
    }

}