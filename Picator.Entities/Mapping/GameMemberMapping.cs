using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Picator.Entities.Models;

namespace Picator.Entities.Mapping;

/// <summary>
/// Table mapping for game member.
/// </summary>
public class GameMemberMapping : BaseEntityTypeConfiguration<GameMember>
{
    public override void Configure(EntityTypeBuilder<GameMember> builder)
    {
        builder.HasOne(d => d.Game)
            .WithMany(p => p.GameMember)
            .HasForeignKey(d => d.GameId)
            .HasConstraintName("FK_GameMember_Game");

        builder.HasOne(d => d.User)
            .WithMany(p => p.GameMember)
            .IsRequired(false)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_GameMember_User");

        builder.HasIndex(p => p.GameId);
        builder.HasIndex(p => p.UserId);

        base.Configure(builder);
    }
}