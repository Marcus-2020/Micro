using Micro.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Micro.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure o schema padrão para o Identity
        builder.HasDefaultSchema("identity");

        // Configura o relacionamento entre o IdentityUser e a tabela User personalizada.
        builder.Entity<IdentityUser>()
            .HasOne<User>()
            .WithOne(user => user.IdentityUser)
            .HasForeignKey<User>(user => user.IdentityUserId);
    }
}