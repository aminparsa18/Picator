using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Picator.Entities.Models;

namespace Picator.Entities.Mapping;

/// <summary>
/// Table mapping for match ticket.
/// </summary>
public class MatchTicketMapping : BaseEntityTypeConfiguration<MatchTicket>
{
    public override void Configure(EntityTypeBuilder<MatchTicket> builder)
    {
        builder.HasOne(d => d.User)
            .WithMany(p => p.MatchTicket)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_MatchTicket_User");

        // Sweep query: oldest queued tickets for a format.
        builder.HasIndex(p => new { p.Status, p.Format, p.CreatedDate });

        // "Does this user already have an active ticket" check.
        builder.HasIndex(p => p.UserId);

        base.Configure(builder);
    }
}
