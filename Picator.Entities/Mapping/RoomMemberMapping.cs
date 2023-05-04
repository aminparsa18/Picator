using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Picator.Entities.Models;

namespace Picator.Entities.Mapping;

/// <summary>
/// Table mapping for room member.
/// </summary>
public class RoomMemberMapping : BaseEntityTypeConfiguration<RoomMember>
{
    public override void Configure(EntityTypeBuilder<RoomMember> builder)
    {
        builder.HasOne(d => d.Room)
            .WithMany(p => p.RoomMember)
            .HasForeignKey(d => d.RoomId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RoomMember_Room");

        builder.HasOne(d => d.User)
            .WithMany(p => p.RoomMember)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_RoomMember_User");
        builder.HasIndex(i => i.RoomId);

        base.Configure(builder);
    }
}