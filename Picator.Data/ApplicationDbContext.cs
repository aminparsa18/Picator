using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Picator.Entities.Extensions;
using Picator.Entities.Identity;
using Picator.Entities.Mapping;
using Picator.Entities.Models;

namespace Picator.Data;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, IdentityUserLogin<Guid>, RoleClaim, IdentityUserToken<Guid>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Avatar> Avatar { get; set; }
    public virtual DbSet<Game> Game { get; set; }
    public virtual DbSet<GameMember> GameMember { get; set; }
    public virtual DbSet<GameMessage> GameMessage { get; set; }
    public virtual DbSet<GameWord> GameWord { get; set; }
    public virtual DbSet<RefreshToken> RefreshToken { get; set; }
    public virtual DbSet<Room> Room { get; set; }
    public virtual DbSet<RoomMember> RoomMember { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("dbo");
        builder.ApplyConfigurationsFromAssembly(typeof(AvatarMapping).Assembly);
        builder.AddCustomIdentityMappings();
    }
}