using Auth.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace TesterLab.Data
{
  /// <summary>
  /// Context de base de données pour l'application.
  /// </summary>
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // ═══════════════════════════════════════════════════════
      // Configuration ApplicationUser
      // ═══════════════════════════════════════════════════════
      modelBuilder.Entity<ApplicationUser>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Email).IsUnique();
        entity.HasIndex(e => e.Username).IsUnique();
        entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
        entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
        entity.Property(e => e.PasswordHash).IsRequired();
      });

      // ═══════════════════════════════════════════════════════
      // Configuration Role
      // ═══════════════════════════════════════════════════════
      modelBuilder.Entity<Role>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Name).IsUnique();
        entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Description).HasMaxLength(500);
      });

      // ═══════════════════════════════════════════════════════
      // Configuration UserRole (Many-to-Many)
      // ═══════════════════════════════════════════════════════
      modelBuilder.Entity<UserRole>(entity =>
      {
        // Clé composite
        entity.HasKey(ur => new { ur.UserId, ur.RoleId });

        // Relation User
        entity.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation Role
        entity.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Property(ur => ur.AssignedAt).IsRequired();
      });

      // ═══════════════════════════════════════════════════════
      // Configuration RefreshToken
      // ═══════════════════════════════════════════════════════
      modelBuilder.Entity<RefreshToken>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Token).IsUnique();
        entity.HasIndex(e => e.UserId);
        entity.Property(e => e.Token).IsRequired();
        entity.Property(e => e.UserId).IsRequired();
      });

      // ═══════════════════════════════════════════════════════
      // Seed des rôles par défaut
      // ═══════════════════════════════════════════════════════
      modelBuilder.Entity<Role>().HasData(
          new Role
          {
            Id = "1",
            Name = "Admin",
            Description = "Administrateur avec tous les droits",
            CreatedAt = DateTime.UtcNow
          },
          new Role
          {
            Id = "2",
            Name = "User",
            Description = "Utilisateur standard",
            CreatedAt = DateTime.UtcNow
          },
          new Role
          {
            Id = "3",
            Name = "Moderator",
            Description = "Modérateur du contenu",
            CreatedAt = DateTime.UtcNow
          }
      );
    }
  }
}
