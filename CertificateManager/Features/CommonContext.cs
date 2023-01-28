using Microsoft.EntityFrameworkCore;

namespace CertificateManager.Features;

public class CommonContext : DbContext
{
    
    public CommonContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
    }
    
}