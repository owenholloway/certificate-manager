using CertificateManager.Model.Certificates;
using Microsoft.EntityFrameworkCore;

namespace CertificateManager.Features;

public class CommonContext : DbContext
{
    
    public CommonContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<RootCertificateAuthority> RootCertificateAuthorities { get; set; }
    public DbSet<IntermediateCertificateAuthority> IntermediateCertificateAuthorities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
    }

}