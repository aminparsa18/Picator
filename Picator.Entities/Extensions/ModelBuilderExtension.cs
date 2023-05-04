using Microsoft.EntityFrameworkCore;
using Picator.Entities.Identity;

namespace Picator.Entities.Extensions;

public static class ModelBuilderExtension
{
    public static void AddCustomIdentityMappings(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<User>().HasIndex(e => e.Code).IsUnique();
        modelBuilder.Entity<User>().Property(e => e.Code).IsRequired();

        modelBuilder.Entity<Role>().ToTable("Roles");

        modelBuilder.Entity<RoleClaim>().ToTable("RoleClaim");

        modelBuilder.Entity<UserClaim>().ToTable("UserClaim");

        modelBuilder.Entity<UserRole>().ToTable("UserRole");
        modelBuilder.Entity<UserRole>()
            .HasOne(userRole => userRole.Role)
            .WithMany(role => role.Users).HasForeignKey(r => r.RoleId);

        modelBuilder.Entity<UserRole>()
            .HasOne(userRole => userRole.User)
            .WithMany(role => role.Roles).HasForeignKey(r => r.UserId);

        modelBuilder.Entity<RoleClaim>()
                .HasOne(roleClaim => roleClaim.Role)
                .WithMany(claim => claim.Claims).HasForeignKey(c => c.RoleId);

        modelBuilder.Entity<UserClaim>()
            .HasOne(userClaim => userClaim.User)
            .WithMany(claim => claim.Claims).HasForeignKey(c => c.UserId);
    }
}