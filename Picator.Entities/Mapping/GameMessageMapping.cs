using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Picator.Entities.Models;

namespace Picator.Entities.Mapping;

/// <summary>
/// Table mapping for game message.
/// </summary>
public class GameMessageMapping : BaseEntityTypeConfiguration<GameMessage>
{
    public override void Configure(EntityTypeBuilder<GameMessage> builder)
    {
        builder.Property(p => p.Content).IsRequired().HasMaxLength(500);

        builder.HasOne(d => d.Game)
            .WithMany(p => p.GameMessage)
            .HasForeignKey(d => d.GameId)
            .HasConstraintName("FK_GameMessage_Game");

        builder.HasOne(d => d.User)
            .WithMany(p => p.GameMessage)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_GameMessage_User");
        builder.HasIndex(i => i.GameId);

        base.Configure(builder);
    }
}