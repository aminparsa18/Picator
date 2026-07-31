using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Picator.Entities.Models;

namespace Picator.Entities.Mapping;

/// <summary>
/// Table mapping for round.
/// </summary>
public class RoundMapping : BaseEntityTypeConfiguration<Round>
{
    public override void Configure(EntityTypeBuilder<Round> builder)
    {
        builder.Property(p => p.Word).IsRequired().HasMaxLength(100);

        builder.HasOne(d => d.Game)
            .WithMany(p => p.Round)
            .HasForeignKey(d => d.GameId)
            .HasConstraintName("FK_Round_Game");

        builder.HasOne(d => d.DrawerGameMember)
            .WithMany()
            .HasForeignKey(d => d.DrawerGameMemberId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Round_GameMember");

        builder.HasIndex(i => i.GameId);

        base.Configure(builder);
    }
}
